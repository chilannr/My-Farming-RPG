using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas; // 画布
    private Grid grid; // 网格
    private Camera mainCamera; // 主摄像机
    [SerializeField] private Image cursorImage = null; // 光标图像
    [SerializeField] private RectTransform cursorRectTransform = null; // 光标矩形变换
    [SerializeField] private Sprite greenCursorSprite = null; // 绿色光标精灵
    [SerializeField] private Sprite redCursorSprite = null; // 红色光标精灵

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; } // 光标位置是否有效

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; } // 物品使用网格半径

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; } // 选择的物品类型

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; } // 光标是否启用

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    // 在第一帧之前调用
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    // 每一帧调用一次
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            // 获取光标的网格位置
            Vector3Int gridPosition = GetGridPositionForCursor();

            // 获取玩家的网格位置
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            // 设置光标精灵
            SetCursorValidity(gridPosition, playerGridPosition);

            // 获取光标的矩形变换位置
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        // 检查物品使用半径是否有效
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // 获取选择的物品详细信息
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // 获取光标位置的网格属性详细信息
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            // 根据选择的物品和网格属性详细信息确定光标有效性
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Commodity:

                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Watering_tool:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    /// <summary>
    /// 设置光标为无效
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    /// <summary>
    /// 设置光标为有效
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    /// <summary>
    /// 测试光标对于目标gridPropertyDetails的商品的有效性。如果有效返回true,否则返回false
    /// </summary>
    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;

    }
    /// <summary>
    /// 测试光标对于目标gridPropertyDetails的工具的有效性。如果有效返回true,否则返回false
    /// </summary>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // 判断工具类型
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                // 如果是铲子工具，并且当前位置可以挖掘且未被挖掘过，则判断是否有可收获的物品
                if (gridPropertyDetails.isDiggable && gridPropertyDetails.daysSinceDug == -1)
                {
                    // 获取光标位置上的物品列表
                    //List<ItemDetails> itemsAtCursor = GetItemsAtCursor();

                    // 判断是否有可收获的物品
                    //foreach (ItemDetails item in itemsAtCursor)
                    //{
                    //    if (item.canBeHarvested)
                    //    {
                    //        return false;
                    //    }
                    //}

                    return true;
                }
                else
                {
                    return false;
                }

            case ItemType.Watering_tool:
                // 如果是浇水工具，并且当前位置已经挖掘过但未浇水，则返回true
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ItemType.Chopping_tool:
            case ItemType.Collecting_tool:
            case ItemType.Breaking_tool:
                // 如果是砍伐、收集或破坏工具
                // 检查是否种植了种子
                //if (HasSeeds())
                //{
                //    // 获取种子的详细信息
                //    SeedDetails seedDetails = GetSeedDetails();

                //    // 检查作物是否已经完全成长
                //    if (gridPropertyDetails.growthStage == seedDetails.growthStages - 1)
                //    {
                //        // 检查工具是否可以用来收获作物
                //        if (itemDetails.canHarvest)
                //        {
                //            return true;
                //        }
                //        else
                //        {
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}
                //else
                //{
                //    return false;
                //}

            default:
                return false;
        }
    }
    /// <summary>
    /// 设置光标对于目标gridPropertyDetails的种子的有效性。如果有效返回true,否则返回false
    /// </summary>
    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;

    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }
    public Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }
    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));  // z是相对于摄像机的物体的远近距离 - 摄像机在-10,所以物体在(-)-10前面 = 10
        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

}