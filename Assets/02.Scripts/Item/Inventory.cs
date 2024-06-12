using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Inventory : MonoBehaviourPunCallbacks
{
    public static Inventory Instance { get; private set; }
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public InventoryUI inventoryUI;
    private HashSet<string> processedItems = new HashSet<string>();



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.TagObject = this;
    }

    public void AddItem(Item newItem, bool synchronize = true)
    { 
        if (processedItems.Contains(newItem.uniqueId)) return;
        if (newItem == null || string.IsNullOrEmpty(newItem.itemName) || string.IsNullOrEmpty(newItem.uniqueId))
        {
            Debug.LogWarning("AddItem: null or invalid item");
            return;
        }
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
        processedItems.Add(newItem.uniqueId);

        inventoryUI.UpdateInventoryUI();
    }



    public void RemoveItem(string itemName, bool synchronize = true)
    {
      

        if (!items.ContainsKey(itemName)) return;

        if (itemQuantities.ContainsKey(itemName))
        {
            itemQuantities[itemName]--;
            if (itemQuantities[itemName] <= 0)
            {
                items.Remove(itemName);
                itemQuantities.Remove(itemName);
            }
        }
        else
        {
            items.Remove(itemName);
        }


        inventoryUI.UpdateInventoryUI();
        FindObjectOfType<QuickSlotManager>().UpdateQuickSlotUI();
    }
}
