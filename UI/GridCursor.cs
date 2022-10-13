using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this script is attached to the UIPanel game object, in our main game canvas
public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private Sprite FishingCursorSprite = null;
    [SerializeField] private Sprite WateringCursorSprite = null;
    [SerializeField] private Sprite HandCollectCursorSprite = null;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    //added property 
    private ItemDetails _selectedItemDetails;
    public ItemDetails SelectedItemDetails { get => _selectedItemDetails; set => _selectedItemDetails = value; }

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            // Get grid position for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            // Get grid position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            // Set cursor sprite, based on cursor grid position and the player grid position
            SetCursorValidity(gridPosition, playerGridPosition);

            // Get rect transform position for cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    //getting position for the cursor
    public Vector3Int GetGridPositionForCursor()
    {
        //getting world position for the cursor by converting the mouse position from a screen point to a world point using the below method on the main camera
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));  // z is how far the objects are in front of the camera - camera is at -10 so objects are (-)-10 in front = 10
        return grid.WorldToCell(worldPosition); //take the world position to a method on the grid object, and converting a wold position to a cell position
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        //the transofrm.position is the world position for the player, we convert it to a grid position
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    // Set the cursor to be invalid
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    // * check if we can use the specific tool according the the bool grid tilemap which fits to that tool
    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid(); //we check first if we have a valid situation to change the cursor, else its an invalid cursor

        // Check item use radius is valid (by the item's grid use radius declared on the item's details)
        //if the distance on either x or y is greater then radius declared on the item's details, cursor is invalid
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // Get selected item details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // Get grid property details at cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            // Determine cursor validity based on inventory item selected and grid property details
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Commodity:

                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Fishing_tool:

                    if (!IsCursorValidForFishing(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    } else
                        //Debug.Log("You can fish here!");
                    break;

                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                case ItemType.Watering_tool:
                case ItemType.Hoeing_tool:

                    if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    // ** Cursor is valid - adjust the cursor sprite according to the item type
    private void SetCursorToValid()
    {
        //Fishing_tool
        if (SelectedItemType == ItemType.Fishing_tool)
        { 
            cursorImage.sprite = FishingCursorSprite;
            CursorPositionIsValid = true;
        } 
        //Hoeing_tool / Seed / Breaking_tool
        else if (SelectedItemType == ItemType.Hoeing_tool || SelectedItemType == ItemType.Seed ||
            SelectedItemType == ItemType.Breaking_tool) 
        {   
            cursorImage.sprite = greenCursorSprite;
            CursorPositionIsValid = true;
        }
        //Commodity
        else if (SelectedItemType == ItemType.Commodity)
        { 
            //set the cursor to the item sprite 
            cursorImage.sprite = SelectedItemDetails.itemSprite;
            cursorImage.color = new Color(0.7f, 0.7f, 0.7f, 0.7f); //change opacity to 70%

            CursorPositionIsValid = true;
        }
        //Watering_tool
        else if (SelectedItemType == ItemType.Watering_tool)
        {
            cursorImage.sprite = WateringCursorSprite;
            CursorPositionIsValid = true;
        }
        //Collecting_tool
        else if (SelectedItemType == ItemType.Collecting_tool)
        {
            cursorImage.sprite = HandCollectCursorSprite;
            cursorImage.color = new Color(0.8f, 0.8f, 0.8f, 0.8f); //change opacity to 80%

            CursorPositionIsValid = true;
        } 
        //Chopping_tool
        else if (SelectedItemType == ItemType.Chopping_tool)
        {
            cursorImage.sprite = HandCollectCursorSprite;
            CursorPositionIsValid = true;
        }
    }

    // *** Check if tilemap bool grid is valid aka tilemap is painted for the appropriate item tool selected
    #region valid cursors

    // Test cursor validity for a commodity for the target gridPropertyDetails. Returns true if valid, false if invalid
    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    // Set cursor validity for a seed for the target gridPropertyDetails. Returns true if valid, false if invalid
    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    // Set cursor validity for fishing for the target gridPropertyDetails. Returns true if valid, false if invalid
    private bool IsCursorValidForFishing(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canFish;
    }

    // Sets the cursor as either valid or invalid for the tool for the target gridPropertyDetails. Returns true if valid or false if invalid
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // Switch on item type
        switch (itemDetails.itemType)
        {
            //Hoeing_tool
            case ItemType.Hoeing_tool:
                if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1) //-1 means it hasnt been dugged
                {
                    #region Need to get any items at location so we can check if they are reapable
                    // Get world position for cursor
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // Get list of items at cursor location
                    List<Item> itemList = new List<Item>();

                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);

                    #endregion Need to get any items at location so we can check if they are reapable

                    // Loop through items found to see if any are reapable type - we are not going to let the player dig where there are reapable scenary items
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }

                    if (foundReapable)
                    {
                        return false; //means we found reapable item on the grid means we cant dig it 
                    }
                    else
                    {
                        return true; //means there is no reapabl item on the grid, means we can dig it
                    }
                }
                else
                {
                    return false; //means the grid spot has been dugged so we cant dig again
                }

            //Watering_tool
            case ItemType.Watering_tool:
                //checks to see if the ground has been dugged and the ground hasnt been watered yet
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
            case ItemType.Collecting_tool:

                // Check if item can be harvested with item selected, check item is fully grown

                // Check if seed planted
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    // Get crop details for seed
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    // if crop details found
                    if (cropDetails != null)
                    {
                        // Check if crop fully grown
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            // Check if crop can be harvested with tool selected
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;

            default:
                return false; //if it isnt diggable or has been dugged, we cant dig here
        }
    }
    #endregion valid cursors

    // **** Trigger click actions for specific tool type selected - Happens on Player script @ ProcessPlayerClickInput() method
    // (which triggers events from the UIInventorySlot which declared on the EventHandler script)

    //called in the UIInventorySlot.CleaCursors() method when an inventory slot item is no longer selected
    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnabled = false;
    }

    //called in the UIInventorySlot.SetSelectedItem() method when an inventory slot item is selected and its itemUseGrid radius > 0
    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f); //change opacity to 255
        CursorIsEnabled = true;
    }

    //getting rect transform position (pixel position) for UI items locating on a canvas
    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }
}