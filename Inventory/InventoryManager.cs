using System.Collections.Generic;
using UnityEngine;

public class InventoryManager :SingletonMonobehaviour<InventoryManager>, ISaveable
{
    private UIInventoryBar inventoryBar;

    //Key: int | Value: ItemDetails . This Dictionary will hold all the items
    private Dictionary<int, ItemDetails> itemDetailsDictionary; 

    private int[] selectedInventoryItem; //the index of the array is the inventory list, and the value is the item code

    //an array of different inventory lists
    public List<InventoryItem>[] inventoryLists; 

    //array of integers. the index of the array is the inventory list (from the InventoryLocation enum), and the values is the capacity of
    //that inventory list. (for example, index 0 represents the "Player" inventory capacity, because the "Player" is the enum value at index 0, and the int number in the aray at index 0
    //is the capacity of that inventory)
    [HideInInspector] public int[] inventoryListCapacityIntArray;

    //Our scriptable objects items list, each element in the list is of type itemDetails
    [SerializeField] private SO_ItemList itemList = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake() {
        base.Awake();

        //Create Inventory Lists
        CreateInventoryLists();

        //create item details dictionary
        CreateItemDetailsDictionary();

        //Initailise selected inventory item array wih the amount of inventory arrays by the inventory enum
        selectedInventoryItem = new int[(int)InventoryLocation.count];
        //for each element in the created array, store value of -1 which means we dont have an item selected
        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }

        // Get unique ID for gameobject and create save data object
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }


    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }

    // (1) creates inventory lists 
    private void CreateInventoryLists() {
        //init the total array of inventory lists
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        //loops through that array, and for each element it creates an inventory list o inventory items
        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>(); 
        }

        // initialise inventory list capacity array
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        // initialise player inventory list capacity, by the value we wrote in the Settings script
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    // (1) populates the itemDetailsDictionary from the scriptable object items list
    private void CreateItemDetailsDictionary() {
        //creates new empty dictionary in the dictionary field we declared above
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        //loop through our scriptable objects itemDetails and populates them inside our new Dictionary
        foreach (ItemDetails itemDetails in itemList.itemDetails) {
            //adds new item to the dictionary, with itemDetails.itemCode (holds int number) as the key and itemDetails as the value (holds itemDetails value)
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    // (2) Get itemDetails for any item code passed in as a parameter
    //Returns the itemDetails (from the SO_ItemsList) for the ItemCode, or null if the item code doesn't exist
    //"public ItemDetails" -> means this method return type will be of type ItemDetails
    public ItemDetails GetItemDetails(int itemCode) {
        
        ItemDetails itemDetails; //local temporary var
        //try to get info from the dictionary we created in "CreateItemDetailsDictionary" method, out of the itemCode parameter we got
        //"TryGetValue" is a saved Dictionary method that checks if we have an item code as the parameter, if we do, it returns the value of itemDetails as we wrote below
        //which will be the value of the Key we entered as itemCode
        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails)) {
            return itemDetails;
        } else {
            return null; //we havnt found a value
        }
    }

    // Returns the itemDetails (from the SO_ItemList) for the currently selected item in the inventoryLocation , or null if an item isn't selected
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    // Get the selected item for inventoryLocation - returns itemCode or -1 if nothing is selected
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    // *** ADD ITEM METODS (overload methods) ***

    //the first AddItem method, makes use of the gameObject prefab of the item to destroy it,
    //and then call he overload AddItem method, to add the item to the inventory

    // Add an item to the inventory list for the inventoryLocation and then destroy the gameObjectToDelete
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    // Add an item to the inventory list for the inventoryLocation (which is our inventory list enum), and an item, which is the script attached to the item prefab
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode; //stores the item code, at a local variable called itemCode
        //create a local field for the inventory list, and we set it equal to the inventory list for the player
        //that we priviously created
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if inventory already contains the item, if it exists it returns the itemPosition, else it returns -1
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            //items doesnt exist in the inventory, the position gives to us by the above method FindItemInInventory()
            AddItemAtPosition(inventoryList, itemCode, itemPosition); 
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode); //item already exists in the inventory, and we add addition item
        }

        //  Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    //Swap item at fromItem index with item at toItem index in inventoryLocation inventory list
    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        // if fromItem index and toItemIndex are within the bounds of the list, not the same, and greater than or equal to zero
        if (fromItem < inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count
             && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            //grabs both items by using items indexes
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            //swap the items
            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            //Send event that inventory has been updated (to cause an update at the inventory UI)
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }


    // Clear the selected inventory item for inventoryLocation, called from UIInventoryBar script, on ClearHighlightOnInventorySlots() method
    //and from UIInvenorySlot script, at ClearSelectedItem() method
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1; // -1 means no item is selected
    }

    // Find if an itemCode is already in the inventory. Returns the item position in the inventory list, or -1 if the item is not in the inventory
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode) 
    {
        //local inventory list field, of type list of inventory items and we store in it our inventory location (means the inventory we search for item at)
        //the inventory location is the enum specifing our different inventory
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //loop through all the inventory list
        for (int i = 0; i < inventoryList.Count; i++)
        {
            // we check for each item if its position is equal to the item code parameter we passed in
            // if it is, we return its position, else we return -1
            if (inventoryList[i].itemCode == itemCode) 
            {
                return i; 
            }
        }

        return -1; 
    }

    // *** ADD ITEM POSITION METHODS (overload methods) ***

    // Add item to the end of the inventory (Item doesnt exist yet in our inventory)
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem(); //create new inventoryItem struct

        inventoryItem.itemCode = itemCode; //adds the item code to the itemcode of the item we want to add
        inventoryItem.itemQuantity = 1; //change quantity value to 1
        inventoryList.Add(inventoryItem); //adds new item to the inventory

        //DebugPrintInventoryList(inventoryList);
    }

    // Add item to position in the inventory (Item already exists yet in our inventory)
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem(); //create new inventoryItem struct

        int quantity = inventoryList[position].itemQuantity + 1; //accessing the item that already exists in the inventorylist and add 1 quantity to it
        inventoryItem.itemQuantity = quantity; //updates the new quantity value
        inventoryItem.itemCode = itemCode; //give it the same itemCode as the item which already exists in the inventory
        inventoryList[position] = inventoryItem; //adds this new item to the position in the inventory which had the item already exist (but now it is updated with new quantity)

        //DebugPrintInventoryList(inventoryList);
    }

    //  Add an item of type itemCode to the inventory list for the inventoryLocation
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        //  Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    // Get the item type description for an item type - returns the item type description as a string for a given ItemType
    //we get itemType from itemDetails.itemType | we ues this method from the UIInventorySlot to display the itemType at the toolTip text UI when mouseover on an item
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;

            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;

            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;

            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;

            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;

            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            
            case ItemType.Fishing_tool:
                itemTypeDescription = Settings.FishingTool;
                break;

            default:
                itemTypeDescription = itemType.ToString();
                break;
        }

        return itemTypeDescription;
    }

    //Remove an item from the inventory, and creates a game object at the position it was dropped
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode) 
    {
        //select the inventory list out of the inventoryLists array
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //checks if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if(itemPosition != -1) {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        //Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if (quantity > 0) //checks if after we removed one item, there is at least 1 quantity left of that item
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else //no more quantity left from that item
        {
            inventoryList.RemoveAt(position);
        }
    }

    // Set the selected inventory item (the element inside the selectedInventoryItem array) for inventoryLocation(inventory enum) to itemCode
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode; //item code parameter will be stored at the index of the inventorylocation
    }

    private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
       foreach (InventoryItem inventoryItem in inventoryList)
       {
           Debug.Log("Item Name:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemName + "    Item Quantity: " + inventoryItem.itemQuantity);
       }

       Debug.Log("*** End of our inventoryList ***");
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // Create new scene save
        SceneSave sceneSave = new SceneSave();

        // Remove any existing scene save for persistent scene for this gameobject
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Add inventory lists array to persistent scene save
        sceneSave.listInvItemArray = inventoryLists;

        // Add  inventory list capacity array to persistent scene save
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityIntArray);

        // Add scene save for gameobject
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }


    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Need to find inventory lists - start by trying to locate saveScene for game object
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // list inv items array exists for persistent scene
                if (sceneSave.listInvItemArray != null)
                {
                    inventoryLists = sceneSave.listInvItemArray;

                    //  Send events that inventory has been updated
                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    // Clear any items player was carrying
                    Player.Instance.ClearCarriedItem();

                    // Clear any highlights on inventory bar
                    inventoryBar.ClearHighlightOnInventorySlots();
                }

                // int array dictionary exists for scene
                if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryCapacityArray))
                {
                    inventoryListCapacityIntArray = inventoryCapacityArray;
                }
            }

        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required her since the inventory manager is on a persistent scene;
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since the inventory manager is on a persistent scene;
    }
}