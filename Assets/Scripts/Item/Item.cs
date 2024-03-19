using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [ItemCodeDescription] // 自定义属性，用于显示物品代码的描述
    [SerializeField]
    private int _itemCode; // 物品代码

    private SpriteRenderer spriteRenderer; // Sprite渲染器

    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } } // 物品代码的属性

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // 获取子物体中的Sprite渲染器组件
    }

    private void Start()
    {
        if (ItemCode != 0) // 如果物品代码不为0
        {
            Init(ItemCode); // 初始化物品
        }
    }

    private void Init(int itemCodeParam)
    {
        if (itemCodeParam != 0) // 如果传入的物品代码不为0
        {
            ItemCode = itemCodeParam; // 设置物品代码
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode); // 获取物品详细信息
            spriteRenderer.sprite = itemDetails.itemSprite; // 设置Sprite渲染器的精灵为物品的精灵
            if (itemDetails.itemType == ItemType.Reapable_scenary) // 如果物品类型是可收获的场景物品
            {
                gameObject.AddComponent<ItemNudge>(); // 添加ItemNudge组件，用于处理物品的旋转动画和碰撞事件
            }
        }
    }
}