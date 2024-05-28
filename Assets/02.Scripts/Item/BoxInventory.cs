using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class BoxInventory : MonoBehaviourPunCallbacks
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

    [PunRPC]
    public void AddItemRPC(string itemName, string itemType, string uniqueId, string itemEffect, string itemDescription)
    {
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
        AddItem(newItem, false);
    }

    public void AddItem(Item newItem, bool synchronize = true)
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

        if (synchronize)
        {
            photonView.RPC("AddItemRPC", RpcTarget.OthersBuffered, newItem.itemName, newItem.itemType.ToString(), newItem.uniqueId, newItem.itemEffect, newItem.itemDescription);
        }

        UpdateInventoryUI();
    }

    [PunRPC]
    public void RemoveItemRPC(string itemName)
    {
        RemoveItem(itemName, false);
    }

    public void RemoveItem(string itemName, bool synchronize = true)
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

            if (synchronize)
            {
                photonView.RPC("RemoveItemRPC", RpcTarget.OthersBuffered, itemName);
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