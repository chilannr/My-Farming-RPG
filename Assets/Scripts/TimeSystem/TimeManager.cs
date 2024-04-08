using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>, ISaveable
{
    private int gameYear = 1; // 游戏年份
    private Season gameSeason = Season.Spring; // 游戏季节
    private int gameDay = 1; // 游戏日期
    private int gameHour = 6; // 游戏小时
    private int gameMinute = 30; // 游戏分钟
    private int gameSecond = 0; // 游戏秒数
    private string gameDayOfWeek = "Mon"; // 游戏星期几

    private bool gameClockPaused = false; // 游戏时钟是否暂停

    private float gameTick = 0f; // 游戏滴答时间

    private string _iSaveableUniqueID; // ISaveable接口的唯一标识
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave; // 游戏对象的保存数据
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID; // 生成唯一标识
        GameObjectSave = new GameObjectSave(); // 创建游戏对象的保存数据
    }

    private void OnEnable()
    {
        ISaveableRegister(); // 注册ISaveable接口

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut; // 订阅场景卸载前淡出事件
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn; // 订阅场景加载后淡入事件
    }

    private void OnDisable()
    {
        ISaveableDeregister(); // 注销ISaveable接口

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut; // 取消订阅场景卸载前淡出事件
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn; // 取消订阅场景加载后淡入事件
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true; // 暂停游戏时钟
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false; // 恢复游戏时钟
    }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); // 触发游戏分钟推进事件
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick(); // 游戏滴答
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime ; // 累加游戏滴答时间

        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond; // 重置游戏滴答时间

            UpdateGameSecond(); // 更新游戏秒数
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++; // 游戏秒数加一

        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++; // 游戏分钟加一

            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++; // 游戏小时加一

                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++; // 游戏日期加一

                    if (gameDay > 30)
                    {
                        gameDay = 1;
                        int gs = (int)gameSeason;
                        gs++; // 游戏季节加一

                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs; // 重置游戏季节

                            gameYear++; // 游戏年份加一

                            if (gameYear > 9999)
                                gameYear = 1;

                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); // 触发游戏年份推进事件
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); // 触发游戏季节推进事件
                    }

                    gameDayOfWeek = GetDayOfWeek(); // 获取当前游戏日期的星期几
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); // 触发游戏日期推进事件
                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); // 触发游戏小时推进事件
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); // 触发游戏分钟推进事件
            //Debug.Log("游戏年份: " + gameYear + "，游戏季节: " + gameSeason + "，游戏日期: " + gameDay + "，星期几: " + gameDayOfWeek + "，游戏小时: " + gameHour + "，游戏分钟: " + gameMinute + "，游戏秒数: " + gameSecond);
        }
    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)gameSeason) * 30) + gameDay; // 计算总天数
        int dayOfWeek = totalDays % 7; // 计算星期几

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";

            case 2:
                return "Tue";

            case 3:
                return "Wed";

            case 4:
                return "Thu";

            case 5:
                return "Fri";

            case 6:
                return "Sat";

            case 0:
                return "Sun";

            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond); // 获取当前游戏时间

        return gameTime;
    }

    public Season GetGameSeason()
    {
        return gameSeason;
    }


    //TODO:Remove
    /// <summary>
    /// Advance 1 game minute
    /// </summary>
    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond(); // 快速推进游戏时间
        }
    }

    //TODO:Remove
    /// <summary>
    /// Advance 1 day
    /// </summary>
    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond(); // 快速推进游戏时间
        }
    }

    public void ISaveableRegister()
    {
        // 将当前对象添加到游戏保存管理器的可保存对象列表中
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        // 从游戏保存管理器的可保存对象列表中移除当前对象
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // 删除持久场景中已存在的场景保存数据(如果有)
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // 创建新的场景保存对象
        SceneSave sceneSave = new SceneSave();

        // 创建新的整数字典
        sceneSave.intDictionary = new Dictionary<string, int>();

        // 创建新的字符串字典
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // 将游戏年、日、时、分、秒添加到整数字典中
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        // 将游戏星期和季节添加到字符串字典中
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        // 将场景保存数据添加到游戏对象的持久场景保存中
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        // 尝试从游戏保存数据中获取该游戏对象的保存数据
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 尝试从游戏对象保存数据中获取持久场景保存数据
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // 如果存在整数字典和字符串字典
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    // 从整数字典中获取并设置游戏年、日、时、分、秒
                    if (sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear))
                        gameYear = savedGameYear;

                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                        gameDay = savedGameDay;

                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour))
                        gameHour = savedGameHour;

                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                        gameMinute = savedGameMinute;

                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                        gameSecond = savedGameSecond;

                    // 从字符串字典中获取并设置游戏星期和季节
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                        gameDayOfWeek = savedGameDayOfWeek;

                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if (Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    // 将游戏时间重置为0
                    gameTick = 0f;

                    // 触发游戏分钟进度事件
                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    // 刷新游戏时钟
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // 由于时间管理器运行在持久场景中,因此无需进行任何操作
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // 由于时间管理器运行在持久场景中,因此无需进行任何操作
    }

}
