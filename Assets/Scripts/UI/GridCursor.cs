using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas; // 画布
    private Grid grid; // 网格
    private Camera mainCamera; // 主摄像机
    [SerializeField] private Image cursorImage = null; // 光标图像
    [SerializeField] private Image[] cursorImagePro = null; // 光标图像
    [SerializeField] private Image[] cursorImageUltra = null; // 光标图像
    [SerializeField] private RectTransform cursorRectTransform = null; // 光标矩形变换
    [SerializeField] private RectTransform cursorRectTransformPro = null; // 光标矩形变换
    [SerializeField] private RectTransform cursorRectTransformUltra = null; // 光标矩形变换
    [SerializeField] private Sprite greenCursorSprite = null; // 绿色光标精灵
    [SerializeField] private Sprite redCursorSprite = null; // 红色光标精灵
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null; // 作物详细信息列表
    private GridCursorState gridCursorState; // 网格光标状态
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
        gridCursorState=GridCursorState.normal;
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
            cursorRectTransformPro.position = cursorRectTransform.position;
            cursorRectTransformUltra.position = cursorRectTransform.position;
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

        // 获取选择的物品详细信息
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool_Pro:
            case ItemType.Watering_tool_Pro:
                gridCursorState = GridCursorState.pro;
                break;
            case ItemType.Hoeing_tool_Ultra:
            case ItemType.Watering_tool_Ultra:
                gridCursorState = GridCursorState.ultra;
                break;
            default:
                gridCursorState = GridCursorState.normal;
                break;
        }
        SetCursorToValid();

        // 检查物品使用半径是否有效
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
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
                case ItemType.Watering_tool_Pro:
                case ItemType.Watering_tool_Ultra:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                case ItemType.Hoeing_tool_Pro:
                case ItemType.Hoeing_tool_Ultra:
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
        DisableCursor();
        EnableCursor();
        if (gridCursorState == GridCursorState.pro)
        {
            for (int i = 0; i < cursorImagePro.Length; i++)
            {
                cursorImagePro[i].sprite = redCursorSprite;
            }
        }
        else if (gridCursorState == GridCursorState.ultra)
        {
            for (int i = 0; i < cursorImageUltra.Length; i++)
            {
                cursorImageUltra[i].sprite = redCursorSprite;
            }
        }
        else
        {
            cursorImage.sprite = redCursorSprite;
        }
        CursorPositionIsValid = false;
    }
    /// <summary>
    /// 设置光标为有效
    /// </summary>
    private void SetCursorToValid()
    {
        DisableCursor();
        
        EnableCursor();
        if (gridCursorState == GridCursorState.pro)
        {
            for (int i = 0; i < cursorImagePro.Length; i++)
            {
                cursorImagePro[i].sprite = greenCursorSprite;
            }
        }
        else if (gridCursorState == GridCursorState.ultra)
        {
            for (int i = 0; i < cursorImageUltra.Length; i++)
            {
                cursorImageUltra[i].sprite = greenCursorSprite;
            }
        }
        else
        {
            cursorImage.sprite = greenCursorSprite;
        }
        CursorPositionIsValid = false;
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
    /// 根据目标的gridPropertyDetails设置光标是否对工具有效。如果有效则返回true，否则返回false。
    /// </summary>
    /// <param name="gridPropertyDetails">目标的GridPropertyDetails</param>
    /// <param name="itemDetails">工具的ItemDetails</param>
    /// <returns>如果光标有效则返回true，否则返回false</returns>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // 根据工具类型进行判断
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    #region 需要获取光标位置上的物品列表，以便检查是否可收割

                    // 获取光标的世界坐标
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // 获取光标位置上的物品列表
                    List<Item> itemList = new List<Item>();
                    List<Crop> cropList = new List<Crop>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);

                    #endregion 需要获取光标位置上的物品列表，以便检查是否可收割

                    // 遍历找到的物品，检查是否可收割 - 不允许玩家在可收割的场景物品上挖掘
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }
                    //检查网格上是否可收获有作物，不允许玩家在可收获的场景作物上挖掘
                    if (gridPropertyDetails.seedItemCode != -1) 
                    { 
                        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                    if (cropDetails != null)
                    {
                        // 检查作物是否已经完全成长
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                            return false;
                    }
                    }
                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            case ItemType.Hoeing_tool_Pro:
                if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    #region 需要获取光标位置上的物品列表，以便检查是否可收割

                    // 获取光标的世界坐标
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // 获取光标位置上的物品列表
                    List<Item> itemList = new List<Item>();
                    List<Crop> cropList = new List<Crop>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSizePro, 0f);

                    #endregion 需要获取光标位置上的物品列表，以便检查是否可收割

                    // 遍历找到的物品，检查是否可收割 - 不允许玩家在可收割的场景物品上挖掘
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }
                    //检查网格上是否可收获有作物，不允许玩家在可收获的场景作物上挖掘
                    if (gridPropertyDetails.seedItemCode != -1)
                    {
                        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                        if (cropDetails != null)
                        {
                            // 检查作物是否已经完全成长
                            if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                                return false;
                        }
                    }
                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            case ItemType.Hoeing_tool_Ultra:
                if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    #region 需要获取光标位置上的物品列表，以便检查是否可收割

                    // 获取光标的世界坐标
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // 获取光标位置上的物品列表
                    List<Item> itemList = new List<Item>();
                    List<Crop> cropList = new List<Crop>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSizeUltra, 0f);

                    #endregion 需要获取光标位置上的物品列表，以便检查是否可收割

                    // 遍历找到的物品，检查是否可收割 - 不允许玩家在可收割的场景物品上挖掘
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }
                    //检查网格上是否可收获有作物，不允许玩家在可收获的场景作物上挖掘
                    if (gridPropertyDetails.seedItemCode != -1)
                    {
                        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                        if (cropDetails != null)
                        {
                            // 检查作物是否已经完全成长
                            if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                                return false;
                        }
                    }
                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            case ItemType.Watering_tool:
            case ItemType.Watering_tool_Pro:
            case ItemType.Watering_tool_Ultra:
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

                // 检查是否可以用选中的工具收获物品，检查物品是否已经完全成长

                // 检查是否种植了作物
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    // 获取种子的作物详情
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    // 如果找到了作物详情
                    if (cropDetails != null)
                    {
                        // 检查作物是否已经完全成长
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            // 检查是否可以用选中的工具收获作物
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;

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
        for (int i = 0; i < cursorImagePro.Length; i++)
        {
            cursorImagePro[i].color = Color.clear;
        }
        for (int i = 0; i < cursorImageUltra.Length; i++)
        {
            cursorImageUltra[i].color = Color.clear;
        }
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        if (gridCursorState == GridCursorState.pro)
        {
            for (int i = 0; i < cursorImagePro.Length; i++)
            {
                cursorImagePro[i].color = Color.white;
            }
        }
        else if (gridCursorState == GridCursorState.ultra)
        {
            for (int i = 0; i < cursorImageUltra.Length; i++)
            {
                cursorImageUltra[i].color = Color.white;
            }
        }
        else
        {
            cursorImage.color = Color.white;
        }

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

    enum GridCursorState
    {
        normal,
        pro,
        ultra
    }
}