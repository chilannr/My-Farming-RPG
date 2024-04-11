using UnityEngine;

/// <summary>
/// NPC的日程事件类，包含了日程事件的各种属性。
/// </summary>
[System.Serializable]
public class NPCScheduleEvent
{
    /// <summary>
    /// 小时部分。
    /// </summary>
    public int hour;

    /// <summary>
    /// 分钟部分。
    /// </summary>
    public int minute;

    /// <summary>
    /// 优先级。
    /// </summary>
    public int priority;

    /// <summary>
    /// 日。
    /// </summary>
    public int day;

    /// <summary>
    /// 天气。
    /// </summary>
    public Weather weather;

    /// <summary>
    /// 季节。
    /// </summary>
    public Season season;

    /// <summary>
    /// 目标场景名称。
    /// </summary>
    public SceneName toSceneName;

    /// <summary>
    /// 目标格子坐标。
    /// </summary>
    public GridCoordinate toGridCoordinate;

    /// <summary>
    /// 到达目的地时NPC的朝向。
    /// </summary>
    public Direction npcFacingDirectionAtDestination = Direction.none;

    /// <summary>
    /// 到达目的地时的动画。
    /// </summary>
    public AnimationClip animationAtDestination;

    /// <summary>
    /// 获取事件的时间，以小时和分钟的形式表示。
    /// </summary>
    public int Time
    {
        get
        {
            return (hour * 100) + minute;
        }
    }

    /// <summary>
    /// 构造函数，用于初始化NPC日程事件的各个属性。
    /// </summary>
    /// <param name="hour">小时部分</param>
    /// <param name="minute">分钟部分</param>
    /// <param name="priority">优先级</param>
    /// <param name="day">日</param>
    /// <param name="weather">天气</param>
    /// <param name="season">季节</param>
    /// <param name="toSceneName">目标场景名称</param>
    /// <param name="toGridCoordinate">目标格子坐标</param>
    /// <param name="animationAtDestination">到达目的地时的动画</param>
    public NPCScheduleEvent(int hour, int minute, int priority, int day, Weather weather, Season season, SceneName toSceneName, GridCoordinate toGridCoordinate, AnimationClip animationAtDestination)
    {
        this.hour = hour;
        this.minute = minute;
        this.priority = priority;
        this.day = day;
        this.weather = weather;
        this.season = season;
        this.toSceneName = toSceneName;
        this.toGridCoordinate = toGridCoordinate;
        this.animationAtDestination = animationAtDestination;
    }

    /// <summary>
    /// 默认构造函数。
    /// </summary>
    public NPCScheduleEvent()
    {

    }

    /// <summary>
    /// 返回事件的字符串表示形式，包括时间、优先级、日、天气和季节。
    /// </summary>
    public override string ToString()
    {
        return $"Time: {Time}, Priority: {priority}, Day: {day} Weather: {weather}, Season: {season}";
    }
}
