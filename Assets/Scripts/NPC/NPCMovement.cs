using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 要求该脚本附加到的游戏对象上也有 Rigidbody2D、Animator、NPCPath、SpriteRenderer 和 BoxCollider2D 组件
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    public SceneName npcCurrentScene; // NPC 当前所在场景
    [HideInInspector] public SceneName npcTargetScene; // NPC 目标场景
    [HideInInspector] public Vector3Int npcCurrentGridPosition; // NPC 当前网格位置
    [HideInInspector] public Vector3Int npcTargetGridPosition; // NPC 目标网格位置
    [HideInInspector] public Vector3 npcTargetWorldPosition; // NPC 目标世界位置
    public Direction npcFacingDirectionAtDestination; // NPC 在目标位置时朝向

    private SceneName npcPreviousMovementStepScene; // 上一个移动步骤的场景
    private Vector3Int npcNextGridPosition; // NPC 下一个网格位置
    private Vector3 npcNextWorldPosition; // NPC 下一个世界位置

    [Header("NPC Movement")]
    public float npcNormalSpeed = 2f; // NPC 正常移动速度

    [SerializeField] private float npcMinSpeed = 1f; // NPC 最小移动速度
    [SerializeField] private float npcMaxSpeed = 3f; // NPC 最大移动速度
    private bool npcIsMoving = false; // NPC 是否正在移动的标志

    [HideInInspector] public AnimationClip npcTargetAnimationClip; // NPC 目标动画片段

    [Header("NPC Animation")]
    [SerializeField] private AnimationClip blankAnimation = null; // 空白动画片段

    private Grid grid; // 网格组件
    private Rigidbody2D rigidBody2D; // Rigidbody2D 组件
    private BoxCollider2D boxCollider2D; // BoxCollider2D 组件
    private WaitForFixedUpdate waitForFixedUpdate; // 等待固定更新的实例
    private Animator animator; // Animator 组件
    private AnimatorOverrideController animatorOverrideController; // AnimatorOverrideController 实例
    private int lastMoveAnimationParameter; // 上一个移动动画参数
    private NPCPath npcPath; // NPCPath 组件
    private bool npcInitialised = false; // NPC 是否已初始化的标志
    private SpriteRenderer spriteRenderer; // SpriteRenderer 组件
    [HideInInspector] public bool npcActiveInScene = false; // NPC 在当前场景是否活动的标志

    private bool sceneLoaded = false; // 场景是否已加载的标志

    private Coroutine moveToGridPositionRoutine; // 移动到网格位置的协程

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad; // 订阅场景加载完成事件
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded; // 订阅场景卸载前事件
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad; // 取消订阅场景加载完成事件
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded; // 取消订阅场景卸载前事件
    }

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>(); // 获取 Rigidbody2D 组件
        boxCollider2D = GetComponent<BoxCollider2D>(); // 获取 BoxCollider2D 组件
        animator = GetComponent<Animator>(); // 获取 Animator 组件
        npcPath = GetComponent<NPCPath>(); // 获取 NPCPath 组件
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取 SpriteRenderer 组件

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController); // 创建 AnimatorOverrideController 实例
        animator.runtimeAnimatorController = animatorOverrideController; // 设置 Animator 使用 AnimatorOverrideController

        // 初始化目标世界位置、目标网格位置和目标场景为当前值
        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }

    // 在第一帧之前调用
    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate(); // 创建 WaitForFixedUpdate 实例

        SetIdleAnimation(); // 设置空闲动画
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            if (npcIsMoving == false)
            {
                // 设置 NPC 当前和下一个网格位置 - 考虑 NPC 可能正在播放动画
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;

                if (npcPath.npcMovementStepStack.Count > 0)
                {
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek(); // 获取移动步骤堆栈顶部的步骤

                    npcCurrentScene = npcMovementStep.sceneName; // 设置当前场景

                    // 如果 NPC 即将移动到新场景,重置位置到新场景的起点并更新路径时间
                    if (npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimesOnPath();
                    }

                    // 如果 NPC 在当前场景中,则将 NPC 设置为活动状态以使其可见,从堆栈中弹出移动步骤,然后调用方法移动 NPC
                    if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();

                        npcMovementStep = npcPath.npcMovementStepStack.Pop();

                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }

                    // 否则,如果 NPC 不在当前场景
                    else
                    {
                        SetNPCInactiveInScene();

                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        if (npcMovementStepTime < gameTime)
                        {
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();

                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcCurrentGridPosition);
                        }
                    }

                }
                // else if no more NPC movement steps
                else
                {
                    ResetMoveAnimation();

                    SetNPCFacingDirection();

                    SetNPCEventAnimation();
                }
            }
        }
    }

    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        // 将NPC目标场景设置为计划事件中的目标场景
        npcTargetScene = npcScheduleEvent.toSceneName;

        // 将NPC目标网格位置设置为计划事件中的目标网格坐标
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;

        // 获取并设置NPC目标世界位置
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);

        // 将NPC在目的地的面向方向设置为计划事件中的面向方向
        npcFacingDirectionAtDestination = npcScheduleEvent.npcFacingDirectionAtDestination;

        // 将NPC目标动画剪辑设置为计划事件中的目标动画
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;

        // 清除NPC事件动画
        ClearNPCEventAnimation();
    }

    private void SetNPCEventAnimation()
    {
        // 如果NPC目标动画剪辑不为空
        if (npcTargetAnimationClip != null)
        {
            // 重置空闲动画
            ResetIdleAnimation();

            // 将动画重写控制器的空动画设置为NPC目标动画剪辑
            animatorOverrideController[blankAnimation] = npcTargetAnimationClip;

            // 设置动画参数，开始播放事件动画
            animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            // 如果NPC目标动画剪辑为空，则将动画重写控制器的空动画设置为空动画
            animatorOverrideController[blankAnimation] = blankAnimation;

            // 设置动画参数，停止播放事件动画
            animator.SetBool(Settings.eventAnimation, false);
        }
    }

    public void ClearNPCEventAnimation()
    {
        // 将动画重写控制器的空动画设置为空动画
        animatorOverrideController[blankAnimation] = blankAnimation;

        // 设置动画参数，停止播放事件动画
        animator.SetBool(Settings.eventAnimation, false);

        // 清除NPC的任何旋转
        transform.rotation = Quaternion.identity;
    }
    /// <summary>
    /// 根据NPC在目标位置的面向方向设置空闲动画
    /// </summary>
    private void SetNPCFacingDirection()
    {
        // 重置空闲动画
        ResetIdleAnimation();

        // 根据面向方向设置对应的空闲动画
        switch (npcFacingDirectionAtDestination)
        {
            case Direction.up:
                animator.SetBool(Settings.idleUp, true);
                break;

            case Direction.down:
                animator.SetBool(Settings.idleDown, true);
                break;

            case Direction.left:
                animator.SetBool(Settings.idleLeft, true);
                break;

            case Direction.right:
                animator.SetBool(Settings.idleRight, true);
                break;

            case Direction.none:
                // 不设置任何空闲动画
                break;

            default:
                // 默认情况下不做任何操作
                break;
        }
    }

    /// <summary>
    /// 设置NPC在场景中为激活状态
    /// </summary>
    public void SetNPCActiveInScene()
    {
        // 启用精灵渲染器和2D碰撞体
        spriteRenderer.enabled = true;
        boxCollider2D.enabled = true;
        // 设置NPC为在场景中激活状态
        npcActiveInScene = true;
    }

    /// <summary>
    /// 设置NPC在场景中为非激活状态
    /// </summary>
    public void SetNPCInactiveInScene()
    {
        // 禁用精灵渲染器和2D碰撞体
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        // 设置NPC为在场景中非激活状态
        npcActiveInScene = false;
    }

    /// <summary>
    /// 在场景加载完成后执行的操作
    /// </summary>
    private void AfterSceneLoad()
    {
        // 获取场景中的网格对象
        grid = GameObject.FindObjectOfType<Grid>();

        // 如果NPC未初始化
        if (!npcInitialised)
        {
            // 初始化NPC
            InitialiseNPC();
            npcInitialised = true;
        }

        // 设置场景已加载标志为true
        sceneLoaded = true;
    }

    /// <summary>
    /// 在场景卸载前执行的操作
    /// </summary>
    private void BeforeSceneUnloaded()
    {
        // 设置场景已加载标志为false
        sceneLoaded = false;
    }

    /// <summary>
    /// 根据给定的世界坐标返回网格坐标
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <returns>网格坐标</returns>
    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (grid != null)
        {
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    /// <summary>
    /// 根据给定的网格坐标返回世界坐标(网格正中心)
    /// </summary>
    /// <param name="gridPosition">网格坐标</param>
    /// <returns>世界坐标</returns>
    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        // 获取网格正中心坐标
        return new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y + Settings.gridCellSize / 2f, worldPosition.z);
    }

    /// <summary>
    /// 取消NPC移动
    /// </summary>
    public void CancelNPCMovement()
    {
        // 清除NPC路径
        npcPath.ClearPath();
        // 重置下一个网格坐标和世界坐标
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3.zero;
        npcIsMoving = false;

        // 如果正在执行移动到网格位置的协程,则停止该协程
        if (moveToGridPositionRoutine != null)
        {
            StopCoroutine(moveToGridPositionRoutine);
        }

        // 重置移动动画
        ResetMoveAnimation();

        // 清除事件动画
        ClearNPCEventAnimation();
        npcTargetAnimationClip = null;

        // 重置空闲动画
        ResetIdleAnimation();

        // 设置空闲动画
        SetIdleAnimation();
    }

    /// <summary>
    /// 初始化NPC
    /// </summary>
    private void InitialiseNPC()
    {
        // 如果当前场景与NPC所在场景相同,则设置NPC在场景中为激活状态
        if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInactiveInScene();
        }

        // 记录上一次移动步骤所在的场景
        npcPreviousMovementStepScene = npcCurrentScene;

        // 获取NPC当前网格坐标
        npcCurrentGridPosition = GetGridPosition(transform.position);

        // 设置下一个网格坐标和目标网格坐标为当前网格坐标
        npcNextGridPosition = npcCurrentGridPosition;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);

        // 获取NPC世界坐标
        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition);
    }

    /// <summary>
    /// 将NPC移动到指定的网格位置
    /// </summary>
    /// <param name="gridPosition">目标网格位置</param>
    /// <param name="npcMovementStepTime">NPC移动步骤时间</param>
    /// <param name="gameTime">游戏时间</param>
    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }

    /// <summary>
    /// 移动到网格位置的协程
    /// </summary>
    /// <param name="gridPosition">目标网格位置</param>
    /// <param name="npcMovementStepTime">NPC移动步骤时间</param>
    /// <param name="gameTime">游戏时间</param>
    /// <returns></returns>
    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        npcIsMoving = true;

        // 设置移动动画
        SetMoveAnimation(gridPosition);

        // 获取目标世界坐标
        npcNextWorldPosition = GetWorldPosition(gridPosition);

        // 如果移动步骤时间在未来,则计算移动所需时间
        if (npcMovementStepTime > gameTime)
        {
            //计算时间差(秒)
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            // 计算速度
            float npcCalculatedSpeed = Mathf.Max(npcMinSpeed, Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondsPerGameSecond);

            // 如果速度在最小和最大速度范围内,则执行移动
            if (npcCalculatedSpeed <= npcMaxSpeed)
            {
                while (Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize)
                {
                    // 计算单位向量
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    // 计算移动向量
                    Vector2 move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime, unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);

                    // 移动刚体位置
                    rigidBody2D.MovePosition(rigidBody2D.position + move);

                    yield return waitForFixedUpdate;
                }
            }
        }

        // 设置NPC位置为目标世界坐标
        rigidBody2D.position = npcNextWorldPosition;
        // 更新当前网格坐标和下一个网格坐标
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }
    /// <summary>
    /// 设置移动动画，根据目标格子位置确定移动方向。
    /// </summary>
    /// <param name="gridPosition">目标格子位置</param>
    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        // 重置空闲动画
        ResetIdleAnimation();

        // 重置移动动画
        ResetMoveAnimation();

        // 获取世界坐标
        Vector3 toWorldPosition = GetWorldPosition(gridPosition);

        // 获取方向向量
        Vector3 directionVector = toWorldPosition - transform.position;

        if (Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            // 使用左右动画
            if (directionVector.x > 0)
            {
                animator.SetBool(Settings.walkRight, true);
            }
            else
            {
                animator.SetBool(Settings.walkLeft, true);
            }
        }
        else
        {
            // 使用上下动画
            if (directionVector.y > 0)
            {
                animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                animator.SetBool(Settings.walkDown, true);
            }
        }
    }

    /// <summary>
    /// 设置空闲动画
    /// </summary>
    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }

    /// <summary>
    /// 重置移动动画
    /// </summary>
    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.walkRight, false);
        animator.SetBool(Settings.walkLeft, false);
        animator.SetBool(Settings.walkUp, false);
        animator.SetBool(Settings.walkDown, false);
    }

    /// <summary>
    /// 重置空闲动画
    /// </summary>
    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleRight, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleDown, false);
    }
}