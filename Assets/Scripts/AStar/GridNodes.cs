using UnityEngine;

/// <summary>
/// 表示网格中所有节点的类。
/// </summary>
public class GridNodes
{
    private int width;
    private int height;
    private Node[,] gridNode;

    /// <summary>
    /// 创建一个网格节点的实例。
    /// </summary>
    /// <param name="width">网格的宽度</param>
    /// <param name="height">网格的高度</param>
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridNode = new Node[width, height];

        // 初始化网格节点
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// 获取指定位置的网格节点。
    /// </summary>
    /// <param name="xPosition">节点的X坐标</param>
    /// <param name="yPosition">节点的Y坐标</param>
    /// <returns>指定位置的网格节点</returns>
    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }
}
