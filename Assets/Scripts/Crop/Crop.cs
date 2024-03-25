using System.Collections; // ����ʹ��Э��
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0; // �ջ�������

    [Tooltip("��Ӧ�ô��Ӷ�������ʾ�ջ�Ч�����ɵ��Transform���")]
    [SerializeField] private Transform harvestActionEffectTransform = null; // �ջ�Ч������λ��

    [Tooltip("��Ӧ�ô��Ӷ��������")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null; // �ջ������ﾫ����Ⱦ��

    [HideInInspector]
    public Vector2Int cropGridPosition; // �����������е�λ��


    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp)
    {
        // ��ȡ������������
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
            return;

        // ��ȡ������Ʒ����
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
            return;

        // ��ȡ��������
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
            return;

        // ��ȡ����Ķ�����
        Animator animator = GetComponentInChildren<Animator>();

        // �������߶���
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

        // �������ϴ�����������Ч��
        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }


        // ��ȡ����������ջ�������
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestActions == -1)
            return; // ��������޷������ջ���������


        // �����ջ�������
        harvestActionCount += 1;

        // ����Ƿ����������ջ�������
        if (harvestActionCount >= requiredHarvestActions)
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {

        // �Ƿ����ջ񶯻�
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            // ������ջ���,����ӵ�������Ⱦ��
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

        // �Ƿ����ջ���Ч
        if (cropDetails.harvestSound != SoundName.none)
        {
            //AudioManager.Instance.PlaySound(cropDetails.harvestSound);
        }


        // ������������ɾ������
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        // �Ƿ����ջ񶯻�֮ǰ��������
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // �Ƿ����ջ�֮ǰ���ú�����ײ��
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            // �������к�����ײ��
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // �Ƿ����ջ񶯻� - �ڶ�����ɺ��������������Ϸ����
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
        SpawnHarvestedItems(cropDetails); // �����ջ���Ʒ

        // ���������Ƿ��ת��Ϊ��һ������
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }


        Destroy(gameObject); // ���ٵ�ǰ��Ϸ����
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        // ���ɽ�Ҫ��������Ʒ,����i�������������Ʒ������
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            // ����Ҫ�������ٸ�����
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
                    //  ����Ʒ��ӵ���ұ���
                    InventoryManager.Instance.AddItem(InventoryLocation.player,cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    // ���λ��
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // �����µ�����
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // ��ʾ������Ʒ
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }


}
