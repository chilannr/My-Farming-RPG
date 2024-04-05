[System.Serializable]
public class GridPropertyDetails
{
    public int gridX; // 网格的X坐标
    public int gridY; // 网格的Y坐标
    public bool isDiggable = false; // 是否可挖掘
    public bool canDropItem = false; // 是否可以放置物品
    public bool canPlaceFurniture = false; // 是否可以放置家具
    public bool isPath = false; // 是否为路径
    public bool isNPCObstacle = false; // 是否为NPC障碍物
    public int daysSinceDug = -1; // 挖掘后的天数
    public int daysSinceWatered = -1; // 浇水后的天数
    public int seedItemCode = -1; // 种子物品的代码
    public int growthDays = -1; // 生长需要的天数
    public int daysSinceLastHarvest = -1; // 上次收获后的天数

    public GridPropertyDetails()
    {
    }
}