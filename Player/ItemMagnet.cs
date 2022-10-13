using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Item>(out Item item))
        {
            // Get item details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            //makes sure we magnet only item that we can pick up and drop 
            if (itemDetails.canBePickedUp == true && itemDetails.canBeDropped)
            {
                item.SetTarget(transform.parent.position);
            }
        }
    }
}
