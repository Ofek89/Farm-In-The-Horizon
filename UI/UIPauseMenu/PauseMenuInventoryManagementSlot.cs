using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuInventoryManagementSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image inventoryManagementSlotImage; //populated by itm image
    public TextMeshProUGUI textMeshProUGUI; //for item quantity
    public GameObject greyedOutImageGO;
    [SerializeField] private PauseMenuInventoryManagement inventoryManagement = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null; //for text tooltip item description

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    // private Vector3 startingPosition;
    public GameObject draggedItem;
    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemQuantity != 0)

        {
            // Instatiate gameobject as dragged item
            draggedItem = Instantiate(inventoryManagement.inventoryManagementDraggedItemPrefab, inventoryManagement.transform);

            // Get image for dragged item
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventoryManagementSlotImage.sprite;
            draggedItemImage.color = new Color(1f, 1f, 1f, 0.8f); //change the opacity for dragged image to 80%

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // move game object as dragged item
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;

            // Destroy inventory text box
            inventoryManagement.DestroyInventoryTextBoxGameobject();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Destroy game object as dragged item
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            // Get object drag is over, check if item drag ended at top of an inventory slot in the pause menu
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagementSlot>() != null)
            {
                // get the slot number where the drag ended
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagementSlot>().slotNumber;

                // Swap inventory items in inventory list (between the pausemenu and the hotbar)
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                // Destroy inventory text box
                inventoryManagement.DestroyInventoryTextBoxGameobject();
            }
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

            // Instantiate inventory text box
            inventoryManagement.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryManagement.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBoxComp = inventoryManagement.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            // Set item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            // Populate text box
            //inventoryTextBox.SetTextboxText(itemDetails.itemName, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");
            inventoryTextBoxComp.SetTextboxText(itemDetails.itemName, itemTypeDescription, "Value " + itemDetails.itemValue.ToString(),
            itemDetails.canBeEaten ? "Energy " + itemDetails.itemRestoreEnergyAmount.ToString() : "", itemDetails.itemLongDescription, "", "");

            // Set text box position
            if (slotNumber > 23)
            {
                inventoryManagement.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryManagement.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryManagement.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryManagement.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManagement.DestroyInventoryTextBoxGameobject();

        //Decrease the size of UI content of the Slot back to the normal size of Width and Height (16,16)
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(16, 16);
    }

}
