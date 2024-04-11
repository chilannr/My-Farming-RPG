using System;
using UnityEngine;

/// <summary>
/// 表示路径搜索中的节点的类。
/// </summary>
public class Node : IComparable<Node>
{
    /// <summary>
    /// 格子位置。
    /// </summary>
    public Vector2Int gridPosition;

    /// <summary>
    /// 从起始节点到此节点的实际成本（路径长度）。
    /// </summary>
    public int gCost = 0;

    /// <summary>
    /// 从此节点到目标节点的预估成本。
    /// </summary>
    public int hCost = 0;

    /// <summary>
    /// 表示节点是否为障碍物。
    /// </summary>
    public bool isObstacle = false;

    /// <summary>
    /// 移动惩罚，用于考虑节点的移动难度。
    /// </summary>
    public int movementPenalty;

    /// <summary>
    /// 节点的父节点，用于构建路径。
    /// </summary>
    public Node parentNode;

    /// <summary>
    /// 创建一个节点实例。
    /// </summary>
    /// <param name="gridPosition">节点在网格中的位置</param>
    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    /// <summary>
    /// 获取节点的总成本（实际成本和预估成本之和）。
    /// </summary>
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    /// <summary>
    /// 比较当前节点和另一个节点的总成本。
    /// </summary>
    /// <param name="nodeToCompare">要比较的节点</param>
    /// <returns>
    /// 返回负值，如果当前节点的总成本小于要比较节点的总成本；
    /// 返回正值，如果当前节点的总成本大于要比较节点的总成本；
    /// 返回零，如果两个节点的总成本相等。
    /// </returns>
    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}
