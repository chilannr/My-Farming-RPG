using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GridCursor gridCursor;
    private Cursor cursor;
    public GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        parentCanvas= GetComponentInParent<Canvas>();
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition ;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
        cursor= FindObjectOfType<Cursor>();
    }
    private void ClearCursors()
    {
        // Disable cursor
        gridCursor.DisableCursor();
        cursor.DisableCursor();

        // Set item type to none
        gridCursor.SelectedItemType = ItemType.none;
        cursor.SelectedItemType = ItemType.none;
    }


    #region 鼠标拖拽

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            // 禁用键盘输入
            Player.Instance.DisablePlayerInputAndResetMovement();

            // 实例化被拖拽的物品游戏对象
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            // 获取被拖拽物品的图像
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 移动被拖拽的物品游戏对象
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 销毁被拖拽的物品游戏对象
        if (draggedItem != null)
        {
            Destroy(draggedItem);
            
            // 如果拖拽结束在物品栏上方，则获取拖拽结束的物品槽并交换物品
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                
                // 获取拖拽结束的物品槽编号
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                // 在物品清单中交换物品
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                // 销毁物品文本框
                DestroyInventoryTextBox();

                // 清除选中的物品
                ClearSelectedItem();
            }
            // 否则尝试放置物品（如果可以放置的话）
            else
            {
                
                if (itemDetails.canBeDropped)
                {
                    
                    DropSelectedItemAtMousePosition();
                }
            }

            // 启用玩家输入
            Player.Instance.EnablePlayerInput();
        }
    }

    #endregion


    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {
            // 如果物品详情不为空且未被选中
            if (gridCursor.CursorPositionIsValid)
            {
                // 获取鼠标位置的世界坐标
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
               
                // 在鼠标位置创建物品的预制体
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y - Settings.gridCellSize / 2f, worldPosition.z), Quaternion.identity, parentItem);
                 Item item = itemGameObject.GetComponent<Item>();
                 item.ItemCode = itemDetails.itemCode;

                    // 从玩家的物品清单中移除物品
                    InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                    // 如果物品清单中没有该物品了，则取消选中
                    if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                    {
                        ClearSelectedItem();
                    }
                
            }
        }
    }
    /// <summary>
    /// 将该物品槽设置为选中状态
    /// </summary>
    private void SetSelectedItem()
    {
        // 清除当前高亮的物品
        inventoryBar.ClearHighlightOnInventorySlots();

        // 在物品栏上高亮显示物品
        isSelected = true;

        // 设置高亮的物品槽
        inventoryBar.SetHighlightedInventorySlots();

        // Set use radius for cursors
        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;
        cursor.ItemUseRadius = itemDetails.itemUseRadius;

        // If item requires a grid cursor then enable cursor
        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }
        if (itemDetails.itemUseRadius > 0f)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }

        // Set item type
        gridCursor.SelectedItemType = itemDetails.itemType;
        cursor.SelectedItemType = itemDetails.itemType;

        // 在物品清单中设置选中的物品
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        if (itemDetails.canBeCarried == true)
        {
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
    }

    public void ClearSelectedItem()
    {
        ClearCursors();

        // 清除当前高亮的物品
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        // 在物品清单中清除选中的物品
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        // 清除玩家携带的物品
        Player.Instance.ClearCarriedItem();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果是左键点击
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 如果物品槽当前被选中，则取消选中
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                // 如果物品数量大于0，则选中物品
                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    #region  鼠标悬浮
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 填充文本框的物品详情
        if (itemQuantity != 0)
        {
            // 实例化物品详情文本框
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            // 设置物品类型描述
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);
            string chineseName = EnumExtensions.GetEnumDescription(itemDetails.itemType);

            // 填充文本框
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, chineseName, "", itemDetails.itemLongDescription, "", "");
            
            // 根据物品栏位置设置文本框位置
            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }
    #endregion

    public void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }
    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }
}