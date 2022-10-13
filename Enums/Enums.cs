//all animation type/states : idle | walk | run | useTool | swingTool | liftTool | holdTool | pick
//each with 4 variations: up, down, right, left
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
    useToolUp, //tools animations start
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
    holdToolLeft, //tools animations end
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count
}

public enum CharacterPartAnimator //all the body parts with an animator component as child of the player gameobject
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

public enum PartVariantType //defines the variant type we switch the animation to
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

public enum GridBoolProperty //used to identify which boolean propert each grid relates to
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle,
    canFish
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
    fishing, //added
    none
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

public enum ItemType 
{
    Seed,
    Commodity, //wood, stone etc
    Fishing_tool, //fishing pole
    Watering_tool, //watercan
    Hoeing_tool,
    Chopping_tool, //axe
    Breaking_tool, //pickaxe
    Reaping_tool,
    Collecting_tool,
    Reapable_scenary, //grass that we can cut
    Furniture,
    none,
    count //will give us the number items in out itemtype enums list
}

public enum InventoryLocation 
{
    player,
    chest,
    count //will help us track how many total inventories we have 
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin,
    Scene4_MainMenu,
}

public enum Facing //used for changing sprites for players items
{
    none,
    front,
    back,
    right
}

public enum Season 
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count
}