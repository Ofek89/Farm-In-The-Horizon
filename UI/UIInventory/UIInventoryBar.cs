using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null; //for the blank sprite reference
    [SerializeField] private UIInventorySlot[] inventorySlotArr = null; //array for all the inventorySlots in the inventory (total of 12)
    public GameObject inventoryBarDraggedItem; 
    [HideInInspector] public GameObject inventoryTextBoxGameobject; //the description textbox that will be instantiated from the UIInventorySlot script, when we mouseover an item, will be instantiated at this field
    
    //changes according to the players position, which makes the UIInventorybar move
    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;
    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    private void OnDisable() {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    private void Update()
    {
        //Switch inventory bar positin depanding on player's position
        SwitchInventoryBarPosition();
    }

    //this method is executed when the InventoryUpdatedEvent is called (when we add new item to the inventory)
    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        //if the inventory type from the inventory enum, is the player's inventory
        //THIS HELPS US SEPERATE BETWEEN DIFFERENT INVENTORIES IN THE GAME 
        if (inventoryLocation == InventoryLocation.player) 
        {
            ClearInventorySlots();

            //after clearing all the slots above -> rebuildings all the slots with the items currently in the player's inventory

            //checks if the inventory slots array contains at least one inventory slot, and that there is at least one item in the inventory list
            //(because if there isnt any items in the inventory, there is nothing to update)
            if (inventorySlotArr.Length > 0 && inventoryList.Count > 0)
            {
                // loop through inventory slots and update with corresponding inventory list item
                for (int i = 0; i < inventorySlotArr.Length; i++)
                {   //checks we havnt gone beyond the current amount of item in the inventory, if thats the case, we break out of the loop
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode; //gets the item code from the inventory list and stores it on a temp variable called itemCode

                        // ItemDetails itemDetails = InventoryManager.Instance.itemList.itemDetails.Find(x => x.itemCode == itemCode);
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            // add images and details to inventory item slot
                            inventorySlotArr[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlotArr[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlotArr[i].itemDetails = itemDetails;
                            inventorySlotArr[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInventorySlots(i); //will check on the slot that we update if that slot is being selected
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    //clear the current inventory slots of information
    private void ClearInventorySlots()
    {
        if (inventorySlotArr.Length > 0)
        {
            // loop through inventory slots and update with blank sprite
            for (int i = 0; i < inventorySlotArr.Length; i++)

            {
                inventorySlotArr[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlotArr[i].textMeshProUGUI.text = "";
                inventorySlotArr[i].itemDetails = null;
                inventorySlotArr[i].itemQuantity = 0;
                SetHighlightedInventorySlots(i);
            }
        }
    }

    //Overloaded method: 1 takes no parameter, the other takes int itemPosition as parameter
    //Set the selected highlight if set on all inventory item positions
    public void SetHighlightedInventorySlots()
    {
        if (inventorySlotArr.Length > 0)
        {
            // loop through inventory slots and clear highlight sprites
            for (int i = 0; i < inventorySlotArr.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    //Set the selected highlight if set on an inventory item for a given slot item position
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        //checks there is at least 1 item at the inventory, and that for this item we have itemDetails exist -> just double checking we have an item in the inventory
        if (inventorySlotArr.Length > 0 && inventorySlotArr[itemPosition].itemDetails != null)
        {
            if (inventorySlotArr[itemPosition].isSelected) //if that item is selected
            {
                inventorySlotArr[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f); //enable highlight color, changed its alpha to 1

                // Update inventory to show item as selected
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlotArr[itemPosition].itemDetails.itemCode);
            }
        }
    }



    //we call this function from the UIInventorySlot script
    //Clear all highlights from the inventory bar
    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlotArr.Length > 0) //inventorySlotArr contains all the inventorySlots in the inventoryBar
        {
            // loop through inventory slots and check if an inventory slot is selected, if it does, clear the selection
            for (int i = 0; i < inventorySlotArr.Length; i++)
            {
                if (inventorySlotArr[i].isSelected)
                {
                    inventorySlotArr[i].isSelected = false;
                    inventorySlotArr[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f); //changes alpha to 0
                    // Update inventory to show item as not selected
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    //switches inventory bar position according to the players position
    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            // transform.position = new Vector3(transform.position.x, 7.5f, 0f); // this was changed to control the recttransform see below
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            //transform.position = new Vector3(transform.position.x, mainCamera.pixelHeight - 120f, 0f);// this was changed to control the recttransform see below
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < inventorySlotArr.Length; i++)
        {
            if (inventorySlotArr[i].draggedItem != null)
            {
                Destroy(inventorySlotArr[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for (int i = 0; i < inventorySlotArr.Length; i++)
        {
            inventorySlotArr[i].ClearSelectedItem();
        }
    }

}
