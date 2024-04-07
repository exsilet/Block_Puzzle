using JetBrains.Annotations;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Object/Level")]
public class LevelSO : ScriptableObject
{
    public Level[] Levels;
}

[System.Serializable]
public class Level
{
    [Space(5)]
    public Mode Mode;

    [Space(5)]
    public SpecialBlock[] SpecialBlockShape;

    [Space(5)]
    public ConveyorBelts ConveyorBelts;

    public SpriteInfo[] spriteInfo; 

    [Space(5)]
    public BlockShapeInfo[] standardBlockShapesInfo;

    [Space(5)]
    public BlockShapeInfo[] advancedBlockShapesInfo;

    [Space(5)]
    public BlockShapeInfo[] FixedBlockShape;

    [Space(5)]
    public LevelGoal[] Goal;

    [Space(5)]
    public JewelMachines JewelMachine;

    [Space(5)]
    public BlockerSticks BlockerStick;

    #region BlastMode Additional Settings
    public bool allowBombs;

    public BombType BombType;

    [Space(5)]
    [Header("Balloon Bomb and IceMachine settings")]
    public int remainingcounter;

    [Space(5)]
    [Header("Balloon Bomb place after how many moves")]
    public int addBombAfterMoves;
    #endregion

    [Space(5)]
    public Guide guide;

    [Space(5)]
    public Row[] rows;
}

[System.Serializable]
public class SpriteInfo
{
    public string spriteTag;
    public int probability;
}

[System.Serializable]
public class Guide
{
    public bool enabled;
    public SpriteType guideSprite;
    public string guideMessage;
}

[System.Serializable]
public class SpecialBlock
{
    public bool allowSpecialBlockShape;
    public int probability;
    public SpriteType spriteType;
}

[System.Serializable]
public class ConveyorBelts
{
    public bool enabled;
    public CircularConveyor CircularConveyor;
    public LinearConveyor LinearConveyor;
    public SingleBlockConveyor SingleBlockConveyor;
}


[System.Serializable]
public class LinearConveyor
{
    public bool enabled;
    public ConveyorLocation[] Locations;
}

[System.Serializable]
public class CircularConveyor
{
    public bool enabled;
    [Range(1, 10)]
    public int length;
    [Range(1, 10)]
    public int height;

    public Vector2 startPosition;
}

[System.Serializable]
public class SingleBlockConveyor
{
    public bool enabled;
    public BlockConveyors[] blockConveyors;
}

[System.Serializable]
public class BlockConveyors
{
    public Vector2 position;
    public ConveyorType conveyorType;
}

[System.Serializable]
public class ConveyorLocation
{
    public bool onRow;
    public bool onColoum;
    public bool reverse;
    [Range(1, 2)]
    public int conveyorWidth;
    [Range(0, 8)]
    public int PositionOnGrid;
}

[System.Serializable]
public class Mode
{
    public GameMode GameMode = GameMode.Level;
    public GameModeSettings GameModeSettings;
}

[System.Serializable]
public class Row
{
    [Space(10)]
    public BlockSprite[] coloum;
}

[System.Serializable]
public class BlockSprite
{
    [Space(10)]
    public SpriteType spriteType;
    public SpriteType secondarySpriteType;
    public bool hasStages;
    public int stage;
    public int remainingCounter;
}

[System.Serializable]
public enum SpriteType
{
    Empty,
    MilkBottle,
    MilkShop,
    Ice,
    //RedWithIce,
    Red,
    Yellow,
    Blue,
    Green,
    Cyan,
    Purple,
    Pink,
    Orange,
    Hat,
    Bird,
    MagnetWithBubble,
    Magnet,
    Bubble,
    Kite,
    MusicalNode,
    MusicalPlayer,
    BalloonBomb,
    IceMachine,
    ColouredJewel,
    UnColouredJewel,
    JewelMachine,
    Diamond,
    Panda,
    MagnetWithIce,
    OpenShell,
    CloseShell,
    ShellWithPearl,
    Spell,
    HorizontalSpell,
    VerticalSpell,
    IceBomb,
    Star,
    RedGiftBox,
    YellowGiftBox,
    BlueGiftBox,
    GreenGiftBox,
    CyanGiftBox,
    PurpleGiftBox,
    PinkGiftBox,
    StarWithIce,
    AllColourBlock,
    PandaStage1,
    PandaStage2,
    PandaStage3,
    PandaStage4,
    PandaStage5,
    MusicalNodeGoal,
    ConveyorRight,
    WhiteSprite,
    BubbleWithIce,
    HatWithIce,
}

[System.Serializable]
public enum Guides
{
    GuidePanda
}

public enum BombType
{
    BalloonBomb,
    IceBomb
}

[System.Serializable]
public class BlockShape
{
    public BlockShapeInfo[] StandardBlockShape;
    public BlockShapeInfo[] AdvanceBlockShape;
}

[System.Serializable]
public class LevelGoal
{
    public SpriteType spriteType;
    public int target;
}

[System.Serializable]
public class JewelMachines
{
    public bool enabled;
    public int counter;
    public int GemsToSpwan;
}

[System.Serializable]
public class BlockerSticks
{
    public bool enabled;
    public Stick[] Stick;
}

[System.Serializable]
public class Stick
{
    public bool onVertical;
    public bool onHorrizontal;
    public int length;
    public Vector2 startingPosition;
}

[System.Serializable]
public enum ConveyorType
{
    Right,
    Left,
    Up,
    Down
}


