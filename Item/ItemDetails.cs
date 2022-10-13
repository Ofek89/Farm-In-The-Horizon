using UnityEngine;

[System.Serializable] //let us include it as scriptable objects
public class ItemDetails
{
    [Header("General Item Properties")]
    public int itemCode; //unique code to identify the item
    public ItemType itemType; //itemType by the enum we declared for item types
    public int itemValue; //the amount of coins the item is worth in coins amount
    public int itemRestoreEnergyAmount; //if its is eatable, this will be the amount of energy it restores
    public string itemName;
    public Sprite itemSprite;
    [TextArea]
    public string itemLongDescription;

    [Header("Item usage distance values")]
    public short itemUseGridRadius; //the distance in grid squares from the player which we can use the item
    public float itemUseRadius; //if we dont want to use distance by grid units, we can use distance radius

    [Header("Item Options")]
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;
}
