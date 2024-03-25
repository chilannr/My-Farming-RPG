using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// �������ڹ�����������
[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform;// ���︸����
    private Tilemap groundDecoration1;// ����װ��1
    private Tilemap groundDecoration2;// ����װ��2
    private Grid grid; // �������
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary; // ���������ֵ�
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;// ���������б�
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null; // ������������

    [SerializeField] private RuleTile dugGroundRuleTile = null;// �ھ��������ש
    [SerializeField] private RuleTile wateredGroundRuleTile = null;// �ѽ�ˮ��������ש

    private string _iSaveableUniqueID; // ISaveable��Ψһ��ʶ��
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave; // ��Ϸ����ı�������
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID; // ��ȡΨһ��ʶ��
        GameObjectSave = new GameObjectSave(); // ������Ϸ����ı�������
    }

    private void OnEnable()
    {
        ISaveableRegister(); // ע��ISaveable

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded; // ע�᳡����������¼�
        EventHandler.AdvanceGameDayEvent += AdvanceDay; // ע����Ϸ�ս����¼�
    }

    private void OnDisable()
    {
        ISaveableDeregister(); // ע��ISaveable

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded; // ע��������������¼�
        EventHandler.AdvanceGameDayEvent -= AdvanceDay; // ע����Ϸ�ս����¼�
    }

    private void Start()
    {
        InitialiseGridProperties(); // ��ʼ����������
    }

    private void ClearDisplayGroundDecorations()
    {
        //�������װ��
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }
    private void ClearDisplayAllPlantedCrops()
    {
        //���������ֲ������

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
    /// ��ʾ��ֲ������
    /// </summary>
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        // ��������ӵ���Ʒ����
        if (gridPropertyDetails.seedItemCode > -1)
        {
            // ��ȡ��������
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if (cropDetails != null)
            {
                // Ҫʹ�õ�Ԥ����
                GameObject cropPrefab;

                // ������λ��ʵ��������Ԥ����
                int growthStages = cropDetails.growthDays.Length;

                int currentGrowthStage = 0;

                // ������������ȷ����ǰ�����׶�
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

                // ������λ��ת��Ϊ��������
                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

                // ʵ��������Ԥ����
                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                // �����������������
                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;

                // ��������ĸ�����
                cropInstance.transform.SetParent(cropParentTransform);

                // �������������λ��
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
        }
    }


    /// <summary>
    /// ��ʼ�����������ֵ䣬������ֵ��SO_GridProperties��Դ�д洢��ÿ��������GameObjectSave��
    /// </summary>
    private void InitialiseGridProperties()
    {
        // ���������������������е���������
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // �������������ֵ�
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            // ������������ֵ� - ����SO���������б��е�������������
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                // ��ȡ�������Ե���ϸ��Ϣ
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                // �����������Ե����������������Ե�ֵ
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

                // �����������Ե���ϸ��Ϣ
                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            // ����������������
            SceneSave sceneSave = new SceneSave();

            // �����������ֵ���ӵ���������������
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            // �������ʼ�����������������ֵ�����Ϊ��ǰ������ֵ
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // ����������������ӵ���Ϸ����ĳ���������
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }

    private void AfterSceneLoaded()
    {
        // ��ȡ���︸����transform
        if (GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform= null;
        }

        // ��ȡ�������
        grid = GameObject.FindObjectOfType<Grid>();

        // ��ȡ����װ��1��2��Tilemap
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
    /// ���������������顣
    /// ���������������鵽���������ֵ��С�
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
    /// �ƽ���Ϸ�е�һ�졣
    /// ��������������Ե���ʾ��ͨ���������е���������������ѭ�����еĳ�����
    /// ��ȡ�������������������ֵ䣬���һ�����ﱻ��ֲ����������������
    /// ������ر���ˮ�����ˮ�֡�
    /// ���������������飬�Է�ӳ���ĵ�ֵ��
    /// ��ʾ�����������顣
    /// </summary>
    /// <param name="gameYear">��Ϸ���</param>
    /// <param name="gameSeason">��Ϸ����</param>
    /// <param name="gameDay">��Ϸ����</param>
    /// <param name="gameDayOfWeek">��Ϸ����</param>
    /// <param name="gameHour">��ϷСʱ</param>
    /// <param name="gameMinute">��Ϸ����</param>
    /// <param name="gameSecond">��Ϸ����</param>
    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // ��������������Ե���ʾ
        ClearDisplayGridPropertyDetails();

        // ͨ���������е���������������ѭ�����еĳ���
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // ��ȡ�������������������ֵ�
            if (GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        // �����������������Է�ӳ����������
                        // ���һ�����ﱻ��ֲ
                        if (gridPropertyDetails.growthDays > -1)
                        {   
                            gridPropertyDetails.growthDays += 1;
                        }

                        // ������ر���ˮ�����ˮ��
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        // ����������������
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);
                    }
                }
            }
        }

        // ��ʾ�������������Է�ӳ���ĵ�ֵ
        DisplayGridPropertyDetails();
    }
}