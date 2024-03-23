using System.Collections.Generic;
using UnityEngine;

// 通过 CreateAssetMenu 特性，可以在 Unity 编辑器的 Assets/Create 菜单下创建该类型的实例
[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName sceneName; // 场景名称
    public int gridWidth; // 网格宽度
    public int gridHeight; // 网格高度
    public int originX; // 原点的 x 坐标
    public int originY; // 原点的 y 坐标

    [SerializeField]
    public List<GridProperty> gridPropertyList; // 网格属性列表
}