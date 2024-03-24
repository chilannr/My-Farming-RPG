using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas; // ����
    private Grid grid; // ����
    private Camera mainCamera; // �������
    [SerializeField] private Image cursorImage = null; // ���ͼ��
    [SerializeField] private RectTransform cursorRectTransform = null; // �����α任
    [SerializeField] private Sprite greenCursorSprite = null; // ��ɫ��꾫��
    [SerializeField] private Sprite redCursorSprite = null; // ��ɫ��꾫��

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

        // �����Ʒʹ�ð뾶�Ƿ���Ч
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // ��ȡѡ�����Ʒ��ϸ��Ϣ
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
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
    /// ���ù��Ϊ��Ч
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    /// <summary>
    /// ���ù��Ϊ��Ч
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
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
    /// ���Թ�����Ŀ��gridPropertyDetails�Ĺ��ߵ���Ч�ԡ������Ч����true,���򷵻�false
    /// </summary>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // �жϹ�������
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                // ����ǲ��ӹ��ߣ����ҵ�ǰλ�ÿ����ھ���δ���ھ�������ж��Ƿ��п��ջ����Ʒ
                if (gridPropertyDetails.isDiggable && gridPropertyDetails.daysSinceDug == -1)
                {
                    // ��ȡ���λ���ϵ���Ʒ�б�
                    //List<ItemDetails> itemsAtCursor = GetItemsAtCursor();

                    // �ж��Ƿ��п��ջ����Ʒ
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
                // ����ǽ�ˮ���ߣ����ҵ�ǰλ���Ѿ��ھ����δ��ˮ���򷵻�true
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
                // ����ǿ������ռ����ƻ�����
                // ����Ƿ���ֲ������
                //if (HasSeeds())
                //{
                //    // ��ȡ���ӵ���ϸ��Ϣ
                //    SeedDetails seedDetails = GetSeedDetails();

                //    // ��������Ƿ��Ѿ���ȫ�ɳ�
                //    if (gridPropertyDetails.growthStage == seedDetails.growthStages - 1)
                //    {
                //        // ��鹤���Ƿ���������ջ�����
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
    /// ���ù�����Ŀ��gridPropertyDetails�����ӵ���Ч�ԡ������Ч����true,���򷵻�false
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

}