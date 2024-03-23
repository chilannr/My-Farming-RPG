using UnityEngine;

// Serializable 属性使得该类可以在 Unity 编辑器中进行序列化和显示
[System.Serializable]
public class GridCoordinate
{
    public int x; // x 坐标
    public int y; // y 坐标

    // 构造函数，用于初始化坐标
    public GridCoordinate(int p1, int p2)
    {
        x = p1;
        y = p2;
    }

    // 将 GridCoordinate 类型转换为 Vector2 类型
    public static explicit operator Vector2(GridCoordinate gridCoordinate)
    {
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }

    // 将 GridCoordinate 类型转换为 Vector2Int 类型
    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    // 将 GridCoordinate 类型转换为 Vector3 类型
    public static explicit operator Vector3(GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }

    // 将 GridCoordinate 类型转换为 Vector3Int 类型
    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}