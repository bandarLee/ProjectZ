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
        if (photonView == null)
        {
            Debug.LogError($"PhotonView가 {gameObject.name}에 없습니다.");
        }

        boxInventoryUI = GetComponentInChildren<BoxInventoryUI>();
        if (boxInventoryUI != null)
        {
            boxInventoryUI.SetBoxInventory(this);
        }
        UpdateInventoryUI();
    }

    [PunRPC]
    public void AddItemRPC(string itemName, string itemType, string uniqueId)
    {
        Item newItem = new Item { itemName = itemName, itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemType), uniqueId = uniqueId };
        AddItem(newItem);
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

    [PunRPC]
    public void SyncGeneratedItems(ItemData[] itemsData)
    {
        foreach (var itemData in itemsData)
        {
            ItemType itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemData.ItemTypeString);
            Item item = new Item
            {
                itemName = itemData.ItemName,
                itemType = itemType,
                uniqueId = itemData.UniqueId
            };
            AddItem(item);
        }
    }
}

[System.Serializable]
public class ItemData
{
    public string ItemName;
    public string ItemTypeString;
    public string UniqueId;
}
