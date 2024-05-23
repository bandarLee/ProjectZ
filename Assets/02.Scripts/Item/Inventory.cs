using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public InventoryUI inventoryUI; 



    public void AddItem(Item newItem)
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

        inventoryUI.UpdateInventoryUI();
    }
}
