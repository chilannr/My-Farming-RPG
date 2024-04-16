using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas; // ����
    private Grid grid; // ����
    private Camera mainCamera; // �������
    [SerializeField] private Image cursorImage = null; // ���ͼ��
    [SerializeField] private Image[] cursorImagePro = null; // ���ͼ��
    [SerializeField] private Image[] cursorImageUltra = null; // ���ͼ��
    [SerializeField] private RectTransform cursorRectTransform = null; // �����α任
    [SerializeField] private RectTransform cursorRectTransformPro = null; // �����α任
    [SerializeField] private RectTransform cursorRectTransformUltra = null; // �����α任
    [SerializeField] private Sprite greenCursorSprite = null; // ��ɫ��꾫��
    [SerializeField] private Sprite redCursorSprite = null; // ��ɫ��꾫��
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null; // ������ϸ��Ϣ�б�
    private GridCursorState gridCursorState; // ������״̬
    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; } // ���λ���Ƿ���Ч

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; } // ��Ʒʹ������뾶

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; } // ѡ�����Ʒ����

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; } // ����Ƿ�����

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    // �ڵ�һ֮֡ǰ����
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        gridCursorState=GridCursorState.normal;
    }

    // ÿһ֡����һ��
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
            // ��ȡ��������λ��
            Vector3Int gridPosition = GetGridPositionForCursor();

            // ��ȡ��ҵ�����λ��
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            // ���ù�꾫��
            SetCursorValidity(gridPosition, playerGridPosition);

            // ��ȡ���ľ��α任λ��
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

        // ��ȡѡ�����Ʒ��ϸ��Ϣ
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

        // �����Ʒʹ�ð뾶�Ƿ���Ч
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }
        // ��ȡ���λ�õ�����������ϸ��Ϣ
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            // ����ѡ�����Ʒ������������ϸ��Ϣȷ�������Ч��
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
    /// ���ù��Ϊ��Ч
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
    /// ���ù��Ϊ��Ч
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
    /// ���Թ�����Ŀ��gridPropertyDetails����Ʒ����Ч�ԡ������Ч����true,���򷵻�false
    /// </summary>
    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;

    }
    /// <summary>
    /// ����Ŀ���gridPropertyDetails���ù���Ƿ�Թ�����Ч�������Ч�򷵻�true�����򷵻�false��
    /// </summary>
    /// <param name="gridPropertyDetails">Ŀ���GridPropertyDetails</param>
    /// <param name="itemDetails">���ߵ�ItemDetails</param>
    /// <returns>��������Ч�򷵻�true�����򷵻�false</returns>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // ���ݹ������ͽ����ж�
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    #region ��Ҫ��ȡ���λ���ϵ���Ʒ�б��Ա����Ƿ���ո�

                    // ��ȡ������������
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // ��ȡ���λ���ϵ���Ʒ�б�
                    List<Item> itemList = new List<Item>();
                    List<Crop> cropList = new List<Crop>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);

                    #endregion ��Ҫ��ȡ���λ���ϵ���Ʒ�б��Ա����Ƿ���ո�

                    // �����ҵ�����Ʒ������Ƿ���ո� - ����������ڿ��ո�ĳ�����Ʒ���ھ�
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }
                    //����������Ƿ���ջ����������������ڿ��ջ�ĳ����������ھ�
                    if (gridPropertyDetails.seedItemCode != -1) 
                    { 
                        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                    if (cropDetails != null)
                    {
                        // ��������Ƿ��Ѿ���ȫ�ɳ�
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
                    #region ��Ҫ��ȡ���λ���ϵ���Ʒ�б��Ա����Ƿ���ո�

                    // ��ȡ������������
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // ��ȡ���λ���ϵ���Ʒ�б�
                    List<Item> itemList = new List<Item>();
                    List<Crop> cropList = new List<Crop>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSizePro, 0f);

                    #endregion ��Ҫ��ȡ���λ���ϵ���Ʒ�б��Ա����Ƿ���ո�

                    // �����ҵ�����Ʒ������Ƿ���ո� - ����������ڿ��ո�ĳ�����Ʒ���ھ�
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }
                    //����������Ƿ���ջ����������������ڿ��ջ�ĳ����������ھ�
                    if (gridPropertyDetails.seedItemCode != -1)
                    {
                        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                        if (cropDetails != null)
                        {
                            // ��������Ƿ��Ѿ���ȫ�ɳ�
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
                    #region ��Ҫ��ȡ���λ���ϵ���Ʒ�б��Ա����Ƿ���ո�

                    // ��ȡ������������
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // ��ȡ���λ���ϵ���Ʒ�б�
                    List<Item> itemList = new List<Item>();
                    List<Crop> cropList = new List<Crop>();
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSizeUltra, 0f);

                    #endregion ��Ҫ��ȡ���λ���ϵ���Ʒ�б��Ա����Ƿ���ո�

                    // �����ҵ�����Ʒ������Ƿ���ո� - ����������ڿ��ո�ĳ�����Ʒ���ھ�
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }
                    //����������Ƿ���ջ����������������ڿ��ջ�ĳ����������ھ�
                    if (gridPropertyDetails.seedItemCode != -1)
                    {
                        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                        if (cropDetails != null)
                        {
                            // ��������Ƿ��Ѿ���ȫ�ɳ�
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

                // ����Ƿ������ѡ�еĹ����ջ���Ʒ�������Ʒ�Ƿ��Ѿ���ȫ�ɳ�

                // ����Ƿ���ֲ������
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    // ��ȡ���ӵ���������
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    // ����ҵ�����������
                    if (cropDetails != null)
                    {
                        // ��������Ƿ��Ѿ���ȫ�ɳ�
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            // ����Ƿ������ѡ�еĹ����ջ�����
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
    /// ���ù�����Ŀ��gridPropertyDetails�����ӵ���Ч�ԡ������Ч����true,���򷵻�false
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
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));  // z�������������������Զ������ - �������-10,����������(-)-10ǰ�� = 10
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