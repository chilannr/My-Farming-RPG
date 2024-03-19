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
    [System.ComponentModel.Description("��")]
    Spring,

    [System.ComponentModel.Description("��")]
    Summer,

    [System.ComponentModel.Description("��")]
    Autumn,

    [System.ComponentModel.Description("��")]
    Winter,

    none,
    count
}
public enum ItemType
{
    [System.ComponentModel.Description("����")]
    Seed,               // ����

    [System.ComponentModel.Description("��Ʒ")]
    Commodity,          // ��Ʒ

    [System.ComponentModel.Description("��ˮ����")]
    Watering_tool,      // ��ˮ����

    [System.ComponentModel.Description("���ع���")]
    Hoeing_tool,        // ���ع���

    [System.ComponentModel.Description("��������")]
    Chopping_tool,      // ��������

    [System.ComponentModel.Description("�ƻ�����")]
    Breaking_tool,      // �ƻ�����

    [System.ComponentModel.Description("�ո��")]
    Reaping_tool,       // �ո��

    [System.ComponentModel.Description("�ռ�����")]
    Collecting_tool,    // �ռ�����

    [System.ComponentModel.Description("���ջ�ĳ�����Ʒ")]
    Reapable_scenary,   // ���ջ�ĳ�����Ʒ

    [System.ComponentModel.Description("������")]
    none,               // ������

    [System.ComponentModel.Description("����")]
    Count               // ����
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