using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetilsDictionary; // ��Ʒ�����ֵ䣬���ڴ洢��Ʒ����ϸ��Ϣ

    public List<InventoryItem>[] inventoryLists; // ��Ʒ�嵥�б����飬���ڴ洢��ͬλ�õ���Ʒ�嵥

    private int[] selectedInventoryItem; // �������������Ʒ�嵥��������ֵ����Ʒ�Ĵ���
    [HideInInspector] public int[] inventoryListCapacityIntArray; // ��Ʒ�嵥�������飬���ڴ洢��ͬλ�õ���Ʒ�嵥����

    [SerializeField] private SO_ItemList itemList = null; // ��Ʒ�б��ScriptableObject

    protected override void Awake()
    {
        base.Awake();

        // ������Ʒ�嵥�б�
        CreateInventoryLists();

        // ������Ʒ�����ֵ�
        CreateItemDetailsDictionary();

        // ��ʼ��ѡ������Ʒ����
        selectedInventoryItem = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }

        // ��ȡ��Ϸ�����ΨһID�������������ݶ���
        //ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        //GameObjectSave = new GameObjectSave();
    }

    private void CreateInventoryLists()
    {
        // ������ͬλ�õ���Ʒ�嵥�б�
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];
        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity; // ������ҳ�ʼ��Ʒ�嵥����
    }

    /// <summary>
    /// ����Ʒ��ӵ�ָ��λ�õ���Ʒ�嵥�������ٴ�ɾ������Ϸ����
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// ����Ʒ��ӵ�ָ��λ�õ���Ʒ�嵥
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // �����Ʒ�嵥���Ƿ��Ѿ���������Ʒ
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        // ������Ʒ�����¼�
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    private void CreateItemDetailsDictionary()
    {
        // ������Ʒ�����ֵ�
        itemDetilsDictionary = new Dictionary<int, ItemDetails>();
        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetilsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    /// <summary>
    /// ��ȡָ����Ʒ�������Ʒ����
    /// </summary>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;
        if (itemDetilsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    ///<summary>
    /// ����Ʒ�嵥�У��� fromItem ����������Ʒ�� toItem ����������Ʒ���н���
    ///</summary>

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        // ��� fromItem ������ toItem �������б�Χ�ڣ��Ҳ���ͬ�Ҵ��ڵ�����
        if (fromItem < inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count
             && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            // ������Ʒ�嵥�Ѹ��µ��¼�
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }
    /// <summary>
    /// ��ָ��λ�õ���Ʒ�嵥�в�����Ʒ��������Ʒ��λ�������������Ʒ�����嵥���򷵻�-1
    /// </summary>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// ��ָ��λ�õ���Ʒ�嵥�е�ָ��λ�������Ʒ
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;

        DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// ��ָ��λ�õ���Ʒ�嵥ĩβ�����Ʒ
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        DebugPrintInventoryList(inventoryList);
    }
    

    /// <summary>
    /// Get the item type description for an item type - returns the item type description as a string for a given ItemType
    /// </summary>
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;

            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;

            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;

            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;

            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;

            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;

            default:
                itemTypeDescription = itemType.ToString();
                break;
        }

        return itemTypeDescription;
    }

    /// <summary>
    /// ����Ʒ�嵥���Ƴ���Ʒ�������䱻������λ�ô���һ����Ϸ����
    /// </summary>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // �����Ʒ�嵥���Ƿ��Ѿ���������Ʒ
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        // ������Ʒ�嵥�Ѹ��µ��¼�
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    /// <summary>
    /// ����ָ��λ�õ�ѡ����Ʒ
    /// </summary>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    /// <summary>
    /// ���ص�ǰ��ѡ��Ʒ�ڸ����Ŀ��λ�ã�inventoryLocation���е���Ʒ��ϸ��Ϣ���� SO_ItemList �л�ȡ�������û��ѡ����Ʒ���򷵻� null��
    /// </summary>
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    /// <summary>
    /// ��ȡ�������λ�ã�inventoryLocation���е���ѡ��Ʒ - ������Ʒ���루itemCode�������û��ѡ���κ���Ʒ���򷵻� -1��
    /// </summary>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    /// <summary>
    /// ���ָ����Ʒ�嵥λ�õ�ѡ����Ʒ
    /// </summary>
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
        foreach (InventoryItem inventoryItem in inventoryList)
        {
            Debug.Log("��Ʒ������" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "    ��Ʒ������" + inventoryItem.itemQuantity);
        }
        Debug.Log("******************************************************************************");
    }
}