using System.Collections.Generic;
using UnityEngine;

// 该类用于管理网格属性
[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    public Grid grid; // 网格对象
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary; // 网格属性字典
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null; // 网格属性数组

    private string _iSaveableUniqueID; // ISaveable的唯一标识符
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave; // 游戏对象的保存数据
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID; // 获取唯一标识符
        GameObjectSave = new GameObjectSave(); // 创建游戏对象的保存数据
    }

    private void OnEnable()
    {
        ISaveableRegister(); // 注册ISaveable

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded; // 注册场景加载完成事件
    }

    private void OnDisable()
    {
        ISaveableDeregister(); // 注销ISaveable

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded; // 注销场景加载完成事件
    }

    private void Start()
    {
        InitialiseGridProperties(); // 初始化网格属性
    }

    /// <summary>
    /// 初始化网格属性字典，并将其值从SO_GridProperties资源中存储到每个场景的GameObjectSave中
    /// </summary>
    private void InitialiseGridProperties()
    {
        // 遍历所有网格属性数组中的网格属性
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // 创建网格属性字典
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            // 填充网格属性字典 - 遍历SO网格属性列表中的所有网格属性
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                // 获取网格属性的详细信息
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                // 根据网格属性的类型设置网格属性的值
                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;

                    default:
                        break;
                }

                // 设置网格属性的详细信息
                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            // 创建场景保存数据
            SceneSave sceneSave = new SceneSave();

            // 将网格属性字典添加到场景保存数据中
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            // 如果是起始场景，将网格属性字典设置为当前迭代的值
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // 将场景保存数据添加到游戏对象的场景数据中
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }

    private void AfterSceneLoaded()
    {
        // 获取网格对象
        grid = GameObject.FindObjectOfType<Grid>();
    }

    /// <summary>
    /// Returns the gridPropertyDetails at the gridlocation for the supplied dictionary, or null if no properties exist at that location.
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        // Check if grid property details exist forcoordinate and retrieve
        if (!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            // if not found
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    /// <summary>
    /// Get the grid property details for the tile at (gridX,gridY).  If no grid property details exist null is returned and can assume that all grid property details values are null or false
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Get sceneSave for scene - it exists since we created it in initialise
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            // get grid property details dictionary - it exists since we created it in initialise
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Remove sceneSave for scene
        GameObjectSave.sceneData.Remove(sceneName);

        // Create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        // create & add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        // Add scene save to game object scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    /// <summary>
    /// Set the grid property details to gridPropertyDetails for the tile at (gridX,gridY) for current scene
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    /// <summary>
    /// Set the grid property details to gridPropertyDetails for the tile at (gridX,gridY) for the gridpropertyDictionary.
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        // Set value
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    public GameObjectSave ISaveableSave()
    {
        throw new System.NotImplementedException();
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        throw new System.NotImplementedException();
    }
}