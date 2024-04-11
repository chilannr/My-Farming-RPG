using UnityEngine;

/// <summary>
/// 表示NPC移动中的一步，包含场景、时间和格子位置等详细信息。
/// </summary>
public class NPCMovementStep
{
    /// <summary>
    /// 与移动步骤关联的场景名称。
    /// </summary>
    public SceneName sceneName;

    /// <summary>
    /// 移动步骤发生的小时部分。
    /// </summary>
    public int hour;

    /// <summary>
    /// 移动步骤发生的分钟部分。
    /// </summary>
    public int minute;

    /// <summary>
    /// 移动步骤发生的秒部分。
    /// </summary>
    public int second;

    /// <summary>
    /// 表示NPC目标位置的格子坐标。
    /// </summary>
    public Vector2Int gridCoordinate;
}
