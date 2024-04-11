using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    // NPC移动步骤栈
    public Stack<NPCMovementStep> npcMovementStepStack;

    // NPCMovement组件引用
    private NPCMovement npcMovement;

    private void Awake()
    {
        // 获取NPCMovement组件
        npcMovement = GetComponent<NPCMovement>();
        // 初始化移动步骤栈
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }


    public void ClearPath()
    {
        npcMovementStepStack.Clear();
    }

    /// <summary>
    /// 根据NPC日程事件构建路径
    /// </summary>
    /// <param name="npcScheduleEvent">NPC日程事件</param>
    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        // 如果日程事件目标场景与NPC当前场景相同
        if (npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            Vector2Int npcCurrentGridPosition = (Vector2Int)npcMovement.npcCurrentGridPosition;
            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.toGridCoordinate;

            // 构建路径并添加移动步骤到移动步骤栈
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPosition, npcTargetGridPosition, npcMovementStepStack);
        }
        // 如果日程事件目标场景与NPC当前场景不同
        else if (npcScheduleEvent.toSceneName != npcMovement.npcCurrentScene)
        {
            SceneRoute sceneRoute;

            // 获取与日程事件匹配的场景路线
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.npcCurrentScene.ToString(), npcScheduleEvent.toSceneName.ToString());

            // 如果找到有效的场景路线
            if (sceneRoute != null)
            {
                // 按照场景路径列表的逆序遍历
                for (int i = sceneRoute.scenePathList.Count - 1; i >= 0; i--)
                {
                    int toGridX, toGridY, fromGridX, fromGridY;
                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    // 检查是否为最终目的地
                    if (scenePath.toGridCell.x >= Settings.maxGridWidth || scenePath.toGridCell.y >= Settings.maxGridHeight)
                    {
                        // 如果是,使用最终目的地网格坐标
                        toGridX = npcScheduleEvent.toGridCoordinate.x;
                        toGridY = npcScheduleEvent.toGridCoordinate.y;
                    }
                    else
                    {
                        // 否则使用场景路径的目标网格坐标
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }

                    // 检查是否为起始位置
                    if (scenePath.fromGridCell.x >= Settings.maxGridWidth || scenePath.fromGridCell.y >= Settings.maxGridHeight)
                    {
                        // 如果是,使用NPC当前网格坐标
                        fromGridX = npcMovement.npcCurrentGridPosition.x;
                        fromGridY = npcMovement.npcCurrentGridPosition.y;
                    }
                    else
                    {
                        // 否则使用场景路径的起始网格坐标
                        fromGridX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }

                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);
                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);

                    // 构建路径并添加移动步骤到移动步骤栈
                    NPCManager.Instance.BuildPath(scenePath.sceneName, fromGridPosition, toGridPosition, npcMovementStepStack);
                }
            }
        }

        // 如果移动步骤栈大小大于1,则更新时间并弹出第一个起始位置步骤
        if (npcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            npcMovementStepStack.Pop(); // 丢弃起始步骤

            // 设置NPC移动的日程事件详情
            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }

    /// <summary>
    /// 更新路径上移动步骤的预期游戏时间
    /// </summary>
    public void UpdateTimesOnPath()
    {
        // 获取当前游戏时间
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNPCMovementStep = null;

        foreach (NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            if (previousNPCMovementStep == null)
                previousNPCMovementStep = npcMovementStep;

            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

            // 如果是对角线移动
            if (MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);

            previousNPCMovementStep = npcMovementStep;
        }
    }

    /// <summary>
    /// 判断previousNPCMovementStep是否在npcMovementStep的对角线方向,返回true或false
    /// </summary>
    /// <param name="npcMovementStep">当前移动步骤</param>
    /// <param name="previousNPCMovementStep">上一个移动步骤</param>
    /// <returns></returns>
    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if ((npcMovementStep.gridCoordinate.x != previousNPCMovementStep.gridCoordinate.x) && (npcMovementStep.gridCoordinate.y != previousNPCMovementStep.gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}