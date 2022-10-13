using UnityEngine;

//item script attached to the item prefab gameobject item
public class Item : MonoBehaviour
{
    //includes the item name drawer property for the inspector, drawn by the "ItemCodeNameDrawer" script and declared at the "ItemCodeNameAttribute" script
    //the name here is of the name of "ItemCodeNameAttribute" script, without the "Attribute" word
    [ItemCodeName]
    
    [SerializeField]
    private int _itemCode;

    private SpriteRenderer spriteRenderer;

    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } }

    //Start of Item Magnet variables
    private Rigidbody2D rb;
    private bool hasTarget;
    private Vector3 targetPosition;
    private float itemMagetSpeed = 35f; 
    //End of Item Magnet Variables

    private void Awake()
    {
        //get the sr component from the item prefab child that holds that sr
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    private void FixedUpdate()
    {
        ItemMagnetMovement();
    }

    #region ItemMagnet Methods
    private void ItemMagnetMovement()
    {
        if (hasTarget)
        {
            Vector2 targetDirection = (targetPosition - transform.position).normalized;

            //I multiplied the speed here by 5 because the movement is too slow, to speed the values up
            rb.velocity = new Vector2(targetDirection.x, targetDirection.y) * (itemMagetSpeed * 5) * Time.deltaTime;
        }
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }
    #endregion ItemMagnet Methods

    //init the item with the nudge effect, depanding on the item type
    public void Init (int itemCodeParam)
    {
        if (itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;

            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            spriteRenderer.sprite = itemDetails.itemSprite;

            // If item type is reapable then add nudgeable component
            if (itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}

