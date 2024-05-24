using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public InventoryUI inventoryUI;

    private void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void AddItem(Item newItem)
    {
        // 무기나 기타 아이템은 수량을 늘리지 않고 별도로 추가
        if (newItem.itemType == ItemType.Weapon || newItem.itemType == ItemType.ETC)
        {
            string uniqueItemName = newItem.itemName + "_" + System.Guid.NewGuid().ToString();
            items[uniqueItemName] = newItem;
            itemQuantities[uniqueItemName] = 1;
        }
        else
        {
            // 그 외 아이템은 수량을 증가시킴
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

        inventoryUI.UpdateInventoryUI();
    }
}
