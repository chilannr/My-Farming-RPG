using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ��Ҫ GenerateGUID ���
[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    private Transform parentItem; // ��Ʒ�ĸ�����
    [SerializeField] private GameObject itemPrefab = null; // ��ƷԤ����

    // ISaveable�ӿڵ�ΨһID�ͱ������Ϸ����
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    // �������غ��ȡ��Ʒ�ĸ�����
    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    // ��ʼ��ISaveableUniqueID��GameObjectSave
    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    // ���ٳ����е���Ʒ
    private void DestroySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();
        for (int i = itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    // ʵ���������е���Ʒ
    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    // ʵ����һ�鳡����Ʒ
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

    // ȡ��ISaveable��ע����¼��Ľ��
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    // ע��ISaveable�Ͱ�AfterSceneLoad�¼�
    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    // ȡ��ISaveable��ע��
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    // ע��ISaveable
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }
    // ����Ϸ���������м�����Ʒ����
    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // �ָ���ǰ����������
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    // �ָ�ָ����������Ʒ����
    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItem != null)
            {
                // ���ֳ�����Ʒ�б� - ���ٳ����е�������Ʒ
                DestroySceneItems();

                // ʵ����������Ʒ�б�
                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    // ������Ʒ���ݲ�����GameObjectSave
    public GameObjectSave ISaveableSave()
    {
        // �洢��ǰ��������
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    // �洢ָ����������Ʒ����
    public void ISaveableStoreScene(string sceneName)
    {
        // �Ƴ��ó���������
        GameObjectSave.sceneData.Remove(sceneName);

        // ��ȡ�����е�������Ʒ
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        // ���������е���Ʒ
        foreach (Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            // ��������Ʒ��ӵ��б���
            sceneItemList.Add(sceneItem);
        }

        // ���������������ݣ�������Ϊ������Ʒ�б�
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemList;

        // ����������������ӵ�GameObjectSave��
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}