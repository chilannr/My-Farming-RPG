using UnityEngine;

public static class Settings
{

    // Scenes
    public const string PersistentScene = "PersistentScene";

    // Obscuring Item Fading - ObscuringItemFader
    public const float fadeInSeconds = 0.25f; // 渐入时间
    public const float fadeOutSeconds = 0.35f; // 渐出时间
    public const float targetAlpha = 0.45f; // 目标透明度

    //Tilemap
    public const float gridCellSize = 1f; // 瓦片地图块大小
    public static Vector2 cursorSize = Vector2.one; // 游戏光标大小

    //Player
    public const float playerCentreYOffset = 0.875f; // 玩家中心Y偏移量

    // Player Movement
    public const float runningSpeed = 5.333f; // 跑步速度
    public const float walkingSpeed = 2.666f; // 行走速度
    public static float useToolAnimationPause = 0.25f; // 使用工具动画暂停时间
    public static float liftToolAnimationPause = 0.4f; // 提起工具动画暂停时间
    public static float pickAnimationPause = 1f; // 拾取动画暂停时间
    public static float afterUseToolAnimationPause = 0.2f; // 使用工具后动画暂停时间
    public static float afterLiftToolAnimationPause = 0.4f; // 提起工具后动画暂停时间
    public static float afterPickAnimationPause = 0.2f; // 拾取后动画暂停时间

    //Inventory
    public static int playerInitialInventoryCapacity = 24; // 玩家初始背包容量
    public static int playerMaximumInventoryCapacity = 48; // 玩家最大背包容量

    // Player Animation Parameters
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;
    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;

    // Shared Animation Parameters
    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;

    //Tools
    public const string HoeingTool = "Hoe"; // 锄头工具
    public const string ChoppingTool = "Axe"; // 斧头工具
    public const string BreakingTool = "Pickaxe"; // 镐头工具
    public const string ReapingTool = "Scythe"; // 镰刀工具
    public const string WateringTool = "Watering Can"; // 浇水工具
    public const string CollectingTool = "Basket"; // 收集工具

    //Reaping
    public const int maxCollidersToTestPerReapSwing = 15; // 最大测试每根刀刃的碰撞器数量
    public const int maxTargetComponentsToDestroyPerReapSwing = 2; // 最大摧毁每根刀刃的目标组件数量
    

    //Time System
    public const float secondsPerGameSecond = 0.012f; // 游戏时间每秒对应的实际时间

    //static constructor
    static Settings()
    {
        // Player Animation Parameters
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");

        // Shared Animation parameters
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");
    }

}