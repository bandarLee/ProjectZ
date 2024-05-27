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
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
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

    public void TransferToPlayerInventory(string itemName)
    {
        var item = items[itemName];
        RemoveItem(itemName);
        photonView.RPC("RPC_TransferToPlayerInventory", RpcTarget.AllBuffered, itemName, item.itemType.ToString(), item.icon.name, item.itemEffect, item.itemDescription, item.uniqueId);
    }

    [PunRPC]
    public void RPC_TransferToPlayerInventory(string itemName, string itemType, string iconName, string itemEffect, string itemDescription, string uniqueId)
    {
        var itemTypeEnum = (ItemType)System.Enum.Parse(typeof(ItemType), itemType);
        var icon = Resources.Load<Sprite>("Icons/" + iconName);

        var item = new Item
        {
            itemName = itemName,
            itemType = itemTypeEnum,
            icon = icon,
            itemEffect = itemEffect,
            itemDescription = itemDescription,
            uniqueId = uniqueId
        };

        FindObjectOfType<Inventory>().AddItem(item);
    }

    public void UpdateInventoryUI()
    {
        if (boxInventoryUI != null)
        {
            boxInventoryUI.UpdateInventoryUI();
        }
    }
}
