using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 需要 GenerateGUID 组件
[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    private Transform parentItem; // 物品的父对象
    [SerializeField] private GameObject itemPrefab = null; // 物品预制体

    // ISaveable接口的唯一ID和保存的游戏对象
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    // 场景加载后获取物品的父对象
    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    // 初始化ISaveableUniqueID和GameObjectSave
    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    // 销毁场景中的物品
    private void DestroySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();
        for (int i = itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    // 实例化场景中的物品
    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    // 实例化一组场景物品
    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;
        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    // 取消ISaveable的注册和事件的解绑
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    // 注册ISaveable和绑定AfterSceneLoad事件
    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    // 取消ISaveable的注册
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    // 注册ISaveable
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }
    // 从游戏保存数据中加载物品数据
    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 恢复当前场景的数据
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    // 恢复指定场景的物品数据
    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItem != null)
            {
                // 发现场景物品列表 - 销毁场景中的现有物品
                DestroySceneItems();

                // 实例化场景物品列表
                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    // 保存物品数据并返回GameObjectSave
    public GameObjectSave ISaveableSave()
    {
        // 存储当前场景数据
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    // 存储指定场景的物品数据
    public void ISaveableStoreScene(string sceneName)
    {
        // 移除该场景的数据
        GameObjectSave.sceneData.Remove(sceneName);

        // 获取场景中的所有物品
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        // 遍历场景中的物品
        foreach (Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            // 将场景物品添加到列表中
            sceneItemList.Add(sceneItem);
        }

        // 创建场景保存数据，并设置为场景物品列表
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemList;

        // 将场景保存数据添加到GameObjectSave中
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}