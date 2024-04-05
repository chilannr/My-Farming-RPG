using System.ComponentModel;
using System.Reflection;
using System;

public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkUp,
    walkDown,
    walkRight,
    walkLeft,
    runUp,
    runDown,
    runRight,
    runLeft,
    useToolUp,
    useToolDown,
    useToolRight,
    useToolLeft,
    swingToolUp,
    swingToolDown,
    swingToolRight,
    swingToolLeft,
    liftToolUp,
    liftToolDown,
    liftToolRight,
    liftToolLeft,
    holdToolUp,
    holdToolDown,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count
}

public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}
public enum PartVariantColour
{
    none,
    count
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}
public enum GridBoolProperty
{
    // 表示网格是否可挖掘
    diggable,

    // 表示网格是否可以放置物品
    canDropItem,

    // 表示网格是否可以放置家具
    canPlaceFurniture,

    // 表示网格是否是路径
    isPath,

    // 表示网格是否是NPC的障碍物
    isNPCObstacle
}


public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

public enum ToolEffect
{
    none,
    watering
}


public enum HarvestActionEffect
{
    // 落叶飘落效果
    deciduousLeavesFalling,

    // 松果掉落效果
    pineConesFalling,

    // 砍伐树木的效果
    choppingTreeTrunk,

    // 石头破碎效果
    breakingStone,

    // 收割效果
    reaping,

    // 没有效果
    none
}

public enum Weather
{
    dry,
    raining,
    snowing,
    none,
    count
}


public enum Direction
{
    up,
    down,
    left,
    right,
    none
}

// 定义声音名称的枚举类型
public enum SoundName
{
    none = 0,                       // 无声音
    effectFootstepSoftGround = 10,  // 轻脚步声
    effectFootstepHardGround = 20,  // 重脚步声
    effectAxe = 30,                 // 斧头声音
    effectPickaxe = 40,             // 镐声音
    effectScythe = 50,              // 镰刀声音
    effectHoe = 60,                 // 锄头声音
    effectWateringCan = 70,         // 浇水罐声音
    effectBasket = 80,              // 篮子声音
    effectPickupSound = 90,         // 捡起声音
    effectRustle = 100,             // 摩擦声音
    effectTreeFalling = 110,        // 树木倒下声音
    effectPlantingSound = 120,      // 种植声音
    effectPluck = 130,              // 拔取声音
    effectStoneShatter = 140,       // 石头碎裂声音
    effectWoodSplinters = 150,      // 木头碎裂声音
    ambientCountryside1 = 1000,     // 乡村环境音1
    ambientCountryside2 = 1010,     // 乡村环境音2
    ambientIndoors1 = 1020,         // 室内环境音1
    musicCalm3 = 2000,              // 平静音乐3
    musicCalm1 = 2010               // 平静音乐1
}


public enum Season
{
    [System.ComponentModel.Description("春")]
    Spring,

    [System.ComponentModel.Description("夏")]
    Summer,

    [System.ComponentModel.Description("秋")]
    Autumn,

    [System.ComponentModel.Description("冬")]
    Winter,

    none,
    count
}
public enum ItemType
{
    [System.ComponentModel.Description("种子")]
    Seed,               // 种子

    [System.ComponentModel.Description("商品")]
    Commodity,          // 商品

    [System.ComponentModel.Description("浇水工具")]
    Watering_tool,      // 浇水工具

    [System.ComponentModel.Description("耕地工具")]
    Hoeing_tool,        // 耕地工具

    [System.ComponentModel.Description("砍伐工具")]
    Chopping_tool,      // 砍伐工具

    [System.ComponentModel.Description("破坏工具")]
    Breaking_tool,      // 破坏工具

    [System.ComponentModel.Description("收割工具")]
    Reaping_tool,       // 收割工具

    [System.ComponentModel.Description("收集工具")]
    Collecting_tool,    // 收集工具

    [System.ComponentModel.Description("可收获的场景物品")]
    Reapable_scenary,   // 可收获的场景物品

    [System.ComponentModel.Description("无类型")]
    none,               // 无类型

    [System.ComponentModel.Description("计数")]
    count               // 计数
}

public static class EnumExtensions
{
    public static string GetEnumDescription(Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute != null ? attribute.Description : value.ToString();
    }
}

public enum Facing
{
    none,
    front,
    back,
    right
}