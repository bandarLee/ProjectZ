using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class BoxInventory : MonoBehaviourPunCallbacks
{
    public BoxType boxType;
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public BoxInventoryUI boxInventoryUI;
    private HashSet<string> processedItems = new HashSet<string>();


    private void Start()
    {
        boxInventoryUI = GetComponentInChildren<BoxInventoryUI>();
        if (boxInventoryUI != null)
        {
            boxInventoryUI.SetBoxInventory(this);
        }
        UpdateInventoryUI();
    }

    [PunRPC]
    public void BoxAddItemRPC(string itemName, string itemType, string uniqueId, string itemEffect, string itemDescription)
    {
        if (processedItems.Contains(uniqueId)) return;

        var icon = FindObjectOfType<ItemPresets>().GetIconByName(itemName);
        Item newItem = new Item
        {
            itemName = itemName,
            itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemType),
            uniqueId = uniqueId,
            icon = icon,
            itemEffect = itemEffect,
            itemDescription = itemDescription
        };
        BoxAddItem(newItem, false);
        processedItems.Add(uniqueId);

    }

    public void BoxAddItem(Item newItem, bool synchronize = true)
    {
        string itemName = newItem.uniqueId;
        items[itemName] = newItem;
        if (newItem.itemType == ItemType.Weapon || newItem.itemType == ItemType.ETC)
        {
            string uniqueItemName = newItem.itemName + "_" + System.Guid.NewGuid().ToString();
            newItem.uniqueId = uniqueItemName;
            items[uniqueItemName] = newItem;
            itemQuantities[uniqueItemName] = 1;
        }
        else
        {
            if (itemQuantities.ContainsKey(itemName))
            {
                itemQuantities[itemName]++;
            }
            else
            {
                itemQuantities[itemName] = 1;
            }
        }
        if (synchronize && photonView.IsMine)
        {
            photonView.RPC("BoxAddItemRPC", RpcTarget.OthersBuffered, newItem.itemName, newItem.itemType.ToString(), newItem.uniqueId, newItem.itemEffect, newItem.itemDescription);
        }

        UpdateInventoryUI();
    }

    [PunRPC]
    public void BoxRemoveItemRPC(string itemName)
    {
        BoxRemoveItem(itemName, false);
    }

    public void BoxRemoveItem(string itemName, bool synchronize = true)
    {
        if (items.ContainsKey(itemName))
        {
            itemQuantities[itemName]--;
            if (itemQuantities[itemName] <= 0)
            {
                items.Remove(itemName);
                itemQuantities.Remove(itemName);
            }

            if (synchronize)
            {
                photonView.RPC("BoxRemoveItemRPC", RpcTarget.OthersBuffered, itemName);
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