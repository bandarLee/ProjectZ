using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

public class BoxInventory : MonoBehaviourPunCallbacks
{
    public BoxType boxType;
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public BoxInventoryUI boxInventoryUI;

    private void Start()
    {
        PhotonView photonView = GetComponent<PhotonView>();
        boxInventoryUI = GetComponentInChildren<BoxInventoryUI>();
        if (boxInventoryUI != null)
        {
            boxInventoryUI.SetBoxInventory(this);
        }
        UpdateInventoryUI();
    }

    [PunRPC]
    public void AddItemRPC(string itemName, string itemType, string uniqueId, string iconPath, string itemEffect, string itemDescription)
    {
        Item newItem = new Item
        {
            itemName = itemName,
            itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemType),
            uniqueId = uniqueId,
            icon = Resources.Load<Sprite>(iconPath),
            itemEffect = itemEffect,
            itemDescription = itemDescription
        };
        AddItem(newItem, false);
    }

    public void AddItem(Item newItem, bool sync = true)
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

        if (sync)
        {
            photonView.RPC("AddItemRPC", RpcTarget.OthersBuffered, newItem.itemName, newItem.itemType.ToString(), newItem.uniqueId, newItem.icon.name, newItem.itemEffect, newItem.itemDescription);
        }
    }

    [PunRPC]
    public void RemoveItemRPC(string itemName)
    {
        RemoveItem(itemName, false);
    }

    public void RemoveItem(string itemName, bool sync = true)
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

            if (sync)
            {
                photonView.RPC("RemoveItemRPC", RpcTarget.OthersBuffered, itemName);
            }
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
