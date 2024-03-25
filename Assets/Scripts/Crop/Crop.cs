using System.Collections; // 用于使用协程
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0; // 收获动作计数

    [Tooltip("这应该从子对象中显示收获效果生成点的Transform填充")]
    [SerializeField] private Transform harvestActionEffectTransform = null; // 收获效果生成位置

    [Tooltip("这应该从子对象中填充")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null; // 收获后的作物精灵渲染器

    [HideInInspector]
    public Vector2Int cropGridPosition; // 作物在网格中的位置


    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp)
    {
        // 获取网格属性详情
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
            return;

        // 获取种子物品详情
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
            return;

        // 获取作物详情
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
            return;

        // 获取作物的动画器
        Animator animator = GetComponentInChildren<Animator>();

        // 触发工具动画
        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        // 在作物上触发工具粒子效果
        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }


        // 获取工具所需的收获动作次数
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestActions == -1)
            return; // 这个工具无法用于收获这种作物


        // 增加收获动作计数
        harvestActionCount += 1;

        // 检查是否满足所需收获动作次数
        if (harvestActionCount >= requiredHarvestActions)
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {

        // 是否有收获动画
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            // 如果有收获精灵,则添加到精灵渲染器
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }

        // 是否有收获音效
        if (cropDetails.harvestSound != SoundName.none)
        {
            //AudioManager.Instance.PlaySound(cropDetails.harvestSound);
        }


        // 从网格属性中删除作物
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        // 是否在收获动画之前隐藏作物
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // 是否在收获之前禁用盒型碰撞体
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            // 禁用所有盒型碰撞体
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // 是否有收获动画 - 在动画完成后销毁这个作物游戏对象
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {

            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails); // 生成收获物品

        // 这种作物是否会转化为另一种作物
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }


        Destroy(gameObject); // 销毁当前游戏对象
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        // 生成将要产出的物品,参数i代表作物产出物品的种类
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            // 计算要产出多少个作物
            if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
            }

            for (int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    //  将物品添加到玩家背包
                    InventoryManager.Instance.AddItem(InventoryLocation.player,cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    // 随机位置
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // 创建新的作物
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // 显示种子物品
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }


}
