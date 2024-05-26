using System.Collections.Generic;
using UnityEngine;

public class BoxInventory : MonoBehaviour
{
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public BoxInventoryUI boxInventoryUI;

    private void Start()
    {
        boxInventoryUI = FindObjectOfType<BoxInventoryUI>();
    }

    public void AddItem(Item newItem)
    {
        if (newItem.itemType == ItemType.Weapon || newItem.itemType == ItemType.ETC)
        {
            string uniqueItemName = newItem.itemName + "_" + System.Guid.NewGuid().ToString();
            newItem.uniqueId = uniqueItemName;
            items[uniqueItemName] = newItem;
            itemQuantities[uniqueItemName] = 1;
        }
        else
        {
            if (items.ContainsKey(newItem.itemName))
            {
                itemQuantities[newItem.itemName]++;
            }
            else
            {
                items[newItem.itemName] = newItem;
                itemQuantities[newItem.itemName] = 1;
            }
        }

        boxInventoryUI.UpdateInventoryUI();
    }

    public void RemoveItem(string itemName)
    {
        if (!items.ContainsKey(itemName)) return;

        itemQuantities[itemName]--;
        if (itemQuantities[itemName] <= 0)
        {
            items.Remove(itemName);
            itemQuantities.Remove(itemName);
        }

        boxInventoryUI.UpdateInventoryUI();
    }
}
