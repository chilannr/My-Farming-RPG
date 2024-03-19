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
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
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
    deciduousLeavesFalling,
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
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

public enum SoundName
{
    none = 0,
    effectFootstepSoftGround = 10,
    effectFootstepHardGround = 20,
    effectAxe = 30,
    effectPickaxe = 40,
    effectScythe = 50,
    effectHoe = 60,
    effectWateringCan = 70,
    effectBasket = 80,
    effectPickupSound = 90,
    effectRustle = 100,
    effectTreeFalling = 110,
    effectPlantingSound = 120,
    effectPluck = 130,
    effectStoneShatter = 140,
    effectWoodSplinters = 150,
    ambientCountryside1 = 1000,
    ambientCountryside2 = 1010,
    ambientIndoors1 = 1020,
    musicCalm3 = 2000,
    musicCalm1 = 2010
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
    Count               // 计数
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