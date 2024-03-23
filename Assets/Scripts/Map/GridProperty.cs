[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate; // 网格坐标
    public GridBoolProperty gridBoolProperty; // 网格布尔属性
    public bool gridBoolValue = false; // 网格布尔值，默认为 false

    // 构造函数，用于初始化网格属性
    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
}