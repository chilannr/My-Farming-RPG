using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode; // 对应种子的物品代码
    public int[] growthDays; // 每个生长阶段所需的天数
    public GameObject[] growthPrefab;// 实例化生长阶段时使用的预制体
    public Sprite[] growthSprite; // 生长精灵
    public Season[] seasons; // 生长季节
    public Sprite harvestedSprite; // 收获后使用的精灵

    [ItemCodeDescription]
    public int harvestedTransformItemCode; // 如果作物在收获后转化为另一种物品,这个代码将被填充
    public bool hideCropBeforeHarvestedAnimation; // 是否在收获动画之前隐藏作物
    public bool disableCropCollidersBeforeHarvestedAnimation;// 是否在收获动画之前禁用作物的碰撞体,以避免收获动画影响其他游戏对象
    public bool isHarvestedAnimation; // 如果最后一个生长阶段的预制体需要播放收获动画
    public bool isHarvestActionEffect = false; // 确定是否有收获效果
    public bool spawnCropProducedAtPlayerPosition; // 是否在玩家位置生成收获物品
    public HarvestActionEffect harvestActionEffect; // 作物的收获效果
    public SoundName harvestSound; // 作物的收获音效

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // 可以收获该作物的工具物品代码数组,如果不需要工具则为0
    public int[] requiredHarvestActions; // 对应工具所需的收获动作次数

    [ItemCodeDescription]
    public int[] cropProducedItemCode; // 收获后产生的物品代码数组
    public int[] cropProducedMinQuantity; // 收获后产生的物品最小数量
    public int[] cropProducedMaxQuantity; // 如果最大数量大于最小数量,则会在最小和最大数量之间随机产生
    public int daysToRegrow; // 重新生长所需的天数,-1表示只有一次收获

    /// <summary>
    /// 返回该工具是否可以用于收获该作物,可以返回true,否则返回false
    /// </summary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }

    }


    /// <summary>
    /// 如果该工具不能用于收获该作物,返回-1,否则返回该工具所需的收获动作次数
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}