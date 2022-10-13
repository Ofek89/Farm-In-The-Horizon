using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//IBeginDragHandler, IDragHandler, IEndDragHandler -> three drag events, that are implemented in x3 methods below OnBeginDrag, OnDrag, OnEndDrag
//IPointerEnterHandler, IPointerExitHandler -> two events for enter or exit mouseover, implemented by interfaces OnPointerEnter and OnPointerExit
//IPointerClickHandler -> implemented by OnPointerClick
public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    private Camera mainCamera; //we use a reference to camera because we use a method which req is thats called screenToWorldPoint
    private Canvas parentCanvas; //the item description gameobject will be as a child of that parentCanvas
    private Transform parentItem;
    public GameObject draggedItem;

    private GridCursor gridCursor; //grid cursor script
    private Cursor cursor; //non grid cursor script

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [HideInInspector] public bool isSelected = false; //tracks if the slotUI is selected or not

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject itemPrefab = null;

    [SerializeField] private GameObject inventoryTextBoxPrefab = null; //reference for the description gameobject for show description on items 

    [HideInInspector] public ItemDetails itemDetails; //holds all the details for the item
    [HideInInspector] public int itemQuantity;

    [SerializeField] private int slotNumber = 0; //to track the slot number when we swap items

    private void Awake() {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start() {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventory;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
        EventHandler.CastFishingEvent -= CastFishing; //ADDED
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
        EventHandler.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventory;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition;
        EventHandler.CastFishingEvent += CastFishing; //ADDED
    }

    private void ClearCursors()
    {
        // Disable cursor
        gridCursor.DisableCursor();
        cursor.DisableCursor();

        // Set item type to none
        gridCursor.SelectedItemType = ItemType.none;
        cursor.SelectedItemType = ItemType.none;
    }

    //Sets this inventory slot item to be selected
    private void SetSelectedItem()
    {
        //Clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        //Highlight item on inventory bar
        isSelected = true;

        //Set highlighted inventory slots
        inventoryBar.SetHighlightedInventorySlots();

        // Set use radius for cursors
        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;
        cursor.ItemUseRadius = itemDetails.itemUseRadius;

        // if item require a grid cursor then enable cursor
        if (itemDetails.itemUseGridRadius > 0) 
        {
            gridCursor.EnableCursor();
        } 
        else 
        {
            gridCursor.DisableCursor();
        }

        //If item requires a cursor then enable cursor
        if (itemDetails.itemUseRadius > 0)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }

        // Set selected item type
        gridCursor.SelectedItemType = itemDetails.itemType;
        cursor.SelectedItemType = itemDetails.itemType; ;

        // set selected item details
        gridCursor.SelectedItemDetails = itemDetails;
        cursor.SelectedItemDetails = itemDetails;

        //Set item selected in inventory at the InventoryManager
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        if (itemDetails.canBeCarried == true)
        {
            // Show player carrying item
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else // show player carrying nothing
        {
            Player.Instance.ClearCarriedItem();
        }
    }

    public void ClearSelectedItem()
    {
        ClearCursors();

        // Clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        // set no item selected in inventory 
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        //Clear player carrying item
        Player.Instance.ClearCarriedItem();
    }

    private void RemoveSelectedItemFromInventory()
    {
        if (itemDetails != null && isSelected)
        { 
            int itemCode = itemDetails.itemCode;

            // Remove item from players inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemCode);

            // If no more of item then clear selected
            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, itemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    // Drops the item (if selected) at the current mouse position.  Called by the OnEndDrag event.
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {
            // Vector3Int gridPosition = GridPropertiesManager.Instance.grid.WorldToCell(worldPosition);
            // GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);

            // if (gridPropertyDetails != null && gridPropertyDetails.canDropItem) 
            // {

            // if can drop item here
            if (gridCursor.CursorPositionIsValid)
            {
                //calculated where the item (that we want to drop) should be added in the scene
                //ScreenToWorldPoint is a static saved method used on the main camera, it converts screen point to a world point
                //we calculate it by the mouse x and y positions, and the z position is the negative value of the maincamera Z position
                //because in our game, the camera is at -10, so we want to create the item just negative the current camera's exact position
                //which will be Z of 0 because -10 and then the oposite which is +10 wil result in a Z of 0
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
                
                // Create item from prefab at mouse position, in world position, as  child of the Items parent node in the Hierarchy
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3 (worldPosition.x, worldPosition.y - Settings.gridCellSize / 2f, worldPosition.z), 
                    Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                // Remove item from players inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                // If no more of item then clear selected
                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }

            } 
            else 
            {
                Debug.Log("Cant Drop Item Here! TODO: Add UI Alert");
            }

        }
    }

    //ADDED
    //this method is subscribed to CallCastFishingEvent which evokes fishing event from the EventHandler script
    private void CastFishing()
    {
        Debug.Log("TODO: add fishing logic. CastFishingEvent Has Been Called! All Subscribers Gets Executed now!");
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData) {
        //itemDetails gains its value each time an item is added to the inventory slot, meaning if an inventory slot as an item which we can drag,
        //it means that this inventory slot that contains that item, has its itemDetails value populated with that item
        //if itemDetails isnt null, meaning we have an item in the inventory slot
        if (itemDetails != null) 
        {
            // Disable keyboard input
            Player.Instance.DisablePlayerInputAndResetMovement();

            // Instatiate gameobject as dragged item (instantiate the draggedItem prefab, and te inventoryBar transform, which is the prefab parent)
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            // Get image for dragged item
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>(); //local image field
            draggedItemImage.sprite = inventorySlotImage.sprite;
            draggedItemImage.color = new Color(1f, 1f, 1f, 0.8f); //change the opacity for dragged image to 80%

            SetSelectedItem();
        }
    }

    //IDragHandler
    public void OnDrag(PointerEventData eventData) {
        // move game object as dragged item
        if (draggedItem != null)
        {
            //set the dragged item position at the mouse position
            //we could also use eventData parameter for the mouse position, instead of Input.mousePosition
            draggedItem.transform.position = Input.mousePosition; 
        }
    }

    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData) {
        // Destroy game object as dragged item
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            // If drag ends over inventory bar, get item drag is over and swap them
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                // get the slot number where the drag ended and stores it in a local field
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                // Swap inventory items in inventory list
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                // Destroy inventory text box
                DestroyInventoryTextBox();

                //Clear selected item
                ClearSelectedItem();
            }
            // else attempt to drop the item if it can be dropped
            else
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }

            // Enable player input
            Player.Instance.EnablePlayerInput();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Populate text box with item details
        if (itemQuantity != 0)
        {
            //increase the UI content of the Slot when we mouse over it (the normal size is Vector2(16, 16); which represents (Width,Height)
            RectTransform rt = this.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(17, 17);

            //Instantiate inventory text box prefab so we can display item description info, stores it on a field at the inventoryBar
            //instantiating at the UIInventorySlot transform position, without rotating it 
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            //set the parent of the tetbox prefab as a child of the parentCanvas
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBoxComp = inventoryBar.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            // Set item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            // Populate text box
            //inventoryTextBox.SetTextboxText(itemDetails.itemName, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");
            inventoryTextBoxComp.SetTextboxText(itemDetails.itemName, itemTypeDescription, "Value " + itemDetails.itemValue.ToString(), 
            itemDetails.canBeEaten ? "Energy " + itemDetails.itemRestoreEnergyAmount.ToString() : "", itemDetails.itemLongDescription, "", "");

            // Set text box position according to inventory bar position
            if (inventoryBar.IsInventoryBarPositionBottom)

            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();

        //Decrease the size of UI content of the Slot back to the normal size of Width and Height (16,16)
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(16, 16);
    }

    public void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        // if left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("Left Mouse Button Clicked!");
            // if inventory slot currently selected then deselect
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }

        // if right click
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right Mouse Button Clicked!");
        }
    }

    public void SceneLoaded()
    {
        //here we make sure we reference the parent item object only after the scene has been loaded
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

}


