using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// 该类用于管理网格属性
[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform;// 作物父物体
    private Tilemap groundDecoration1;// 地面装饰1
    private Tilemap groundDecoration2;// 地面装饰2
    private Grid grid; // 网格对象
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary; // 网格属性字典
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;// 作物详情列表
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null; // 网格属性数组

    [SerializeField] private RuleTile dugGroundRuleTile = null;// 挖掘地面规则瓷砖
    [SerializeField] private RuleTile wateredGroundRuleTile = null;// 已浇水地面规则瓷砖

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
        EventHandler.AdvanceGameDayEvent += AdvanceDay; // 注册游戏日进度事件
    }

    private void OnDisable()
    {
        ISaveableDeregister(); // 注销ISaveable

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded; // 注销场景加载完成事件
        EventHandler.AdvanceGameDayEvent -= AdvanceDay; // 注销游戏日进度事件
    }

    private void Start()
    {
        InitialiseGridProperties(); // 初始化网格属性
    }

    private void ClearDisplayGroundDecorations()
    {
        //清除地面装饰
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }
    private void ClearDisplayAllPlantedCrops()
    {
        //清除所有种植的作物

        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();

        foreach (Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Dug
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGroundRuleTile(gridPropertyDetails);
        }
    }
    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Watered
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGroundRuleTile(gridPropertyDetails);
        }
    }
    private void ConnectWateredGroundRuleTile(GridPropertyDetails gridPropertyDetails)
    {
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 1), wateredGroundRuleTile);
    }
    private void ConnectDugGroundRuleTile(GridPropertyDetails gridPropertyDetails)
    {
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 1), dugGroundRuleTile);
    }


    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisplayGridPropertyDetails()
    {
        
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);
            DisplayPlantedCrop(gridPropertyDetails);
        }
    }
    /// <summary>
    /// Returns Crop Details for the provided seedItemCode
    /// </summary>
    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
    }
    /// <summary>
    /// 显示种植的作物
    /// </summary>
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        // 如果有种子的物品代码
        if (gridPropertyDetails.seedItemCode > -1)
        {
            // 获取作物详情
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if (cropDetails != null)
            {
                // 要使用的预制体
                GameObject cropPrefab;

                // 在网格位置实例化作物预制体
                int growthStages = cropDetails.growthDays.Length;

                int currentGrowthStage = 0;

                // 根据生长天数确定当前生长阶段
                for (int i = growthStages - 1; i >= 0; i--)
                {
                    if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                    {
                        currentGrowthStage = i;
                        Debug.Log("Current growth stage: " + currentGrowthStage);
                        break;
                    }
                }

                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                // 将网格位置转换为世界坐标
                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

                // 实例化作物预制体
                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                // 设置作物的生长精灵
                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;

                // 设置作物的父物体
                cropInstance.transform.SetParent(cropParentTransform);

                // 设置作物的网格位置
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
        }
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
        // 获取作物父物体transform
        if (GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform= null;
        }

        // 获取网格对象
        grid = GameObject.FindObjectOfType<Grid>();

        // 获取地面装饰1和2的Tilemap
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();

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
            // If grid properties exist
            if (gridPropertyDictionary.Count > 0)
            {
                // grid property details found for the current scene destroy existing ground decoration
                ClearDisplayGridPropertyDetails();

                // Instantiate grid property details for current scene
                DisplayGridPropertyDetails();
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
    /// 设置网格属性详情。
    /// 设置网格属性详情到网格属性字典中。
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
    /// <summary>
    /// 推进游戏中的一天。
    /// 清除所有网格属性的显示，通过遍历所有的网格属性数组来循环所有的场景。
    /// 获取场景的网格属性详情字典，如果一个作物被种植，增加生长天数。
    /// 如果土地被浇水，清除水分。
    /// 设置网格属性详情，以反映更改的值。
    /// 显示网格属性详情。
    /// </summary>
    /// <param name="gameYear">游戏年份</param>
    /// <param name="gameSeason">游戏季节</param>
    /// <param name="gameDay">游戏天数</param>
    /// <param name="gameDayOfWeek">游戏星期</param>
    /// <param name="gameHour">游戏小时</param>
    /// <param name="gameMinute">游戏分钟</param>
    /// <param name="gameSecond">游戏秒数</param>
    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // 清除所有网格属性的显示
        ClearDisplayGridPropertyDetails();

        // 通过遍历所有的网格属性数组来循环所有的场景
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // 获取场景的网格属性详情字典
            if (GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        // 更新所有网格属性以反映天数的增加
                        // 如果一个作物被种植
                        if (gridPropertyDetails.growthDays > -1)
                        {   
                            gridPropertyDetails.growthDays += 1;
                        }

                        // 如果土地被浇水，清除水分
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        // 设置网格属性详情
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);
                    }
                }
            }
        }

        // 显示网格属性详情以反映更改的值
        DisplayGridPropertyDetails();
    }
}