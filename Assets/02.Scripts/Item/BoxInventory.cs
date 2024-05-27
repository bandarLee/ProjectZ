using System.Collections.Generic;
using UnityEngine;

public enum BoxType
{
    Diner,
    PoliceLarge,
    PoliceSmall,
    PostOfficeLarge,
    PostOfficeSmall,
    WarehouseLarge,
    WarehouseSmall
}
[System.Serializable]
public struct BoxTypeConfig
{
    public BoxType boxType;
    public int itemCount;
    public float foodProbability;
    public float weaponProbability;
    public float healProbability;
    public float mentalProbability;
    public float etcProbability;
}


public class BoxInventory : MonoBehaviour
{
    public BoxType boxType;
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public BoxInventoryUI boxInventoryUI;

    private void Start()
    {
        boxInventoryUI = GetComponentInChildren<BoxInventoryUI>();
        if (boxInventoryUI != null)
        {
            boxInventoryUI.SetBoxInventory(this);
        }
        UpdateInventoryUI();
    }

    public void AddItem(Item newItem)
    {
        string itemName = newItem.uniqueId;
        items[itemName] = newItem;

        if (itemQuantities.ContainsKey(itemName))
        {
            itemQuantities[itemName]++;
        }
        else
        {
            itemQuantities[itemName] = 1;
        }

        UpdateInventoryUI();
    }

    public void RemoveItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            if (itemQuantities[itemName] > 1)
            {
                itemQuantities[itemName]--;
            }
            else
            {
                items.Remove(itemName);
                itemQuantities.Remove(itemName);
            }
            UpdateInventoryUI();
        }
    }

    public void UpdateInventoryUI()
    {
        if (boxInventoryUI != null)
        {
            boxInventoryUI.UpdateInventoryUI();
        }
    }
}
