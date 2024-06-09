using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Inventory : MonoBehaviourPunCallbacks
{
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();
    public InventoryUI inventoryUI;
    private HashSet<string> processedItems = new HashSet<string>(); // 추가된 아이템의 고유 ID를 저장하는 HashSet
    PhotonView pv;
    private void Awake()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();

    }

    private void Start()
    {
        if (inventoryUI == null)
        {
            inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI == null)
            {
            }
        }
        PhotonNetwork.LocalPlayer.TagObject = this;
        pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void AddItemRPC(string itemName, string itemType, string uniqueId, string itemEffect, string itemDescription, PhotonMessageInfo info)
    {

        if (processedItems.Contains(uniqueId)) return;

        Item newItem = new Item
        {
            itemName = itemName,
            itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemType),
            uniqueId = uniqueId,
            icon = FindObjectOfType<ItemPresets>().GetIconByName(itemName), // 아이콘 가져오기
            itemEffect = itemEffect,
            itemDescription = itemDescription
        };

        AddItem(newItem, false); 
        processedItems.Add(uniqueId);
    }

    public void AddItem(Item newItem, bool synchronize = true)
    {
        if (!pv.IsMine) return;

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

        if (synchronize)
        {
            pv.RPC("AddItemRPC", RpcTarget.OthersBuffered, newItem.itemName, newItem.itemType.ToString(), newItem.uniqueId, newItem.itemEffect, newItem.itemDescription);
        }

        inventoryUI.UpdateInventoryUI();
    }

    [PunRPC]
    public void RemoveItemRPC(string itemName, PhotonMessageInfo info)
    {
        if (!pv.IsMine) return;

        RemoveItem(itemName, false);
    }

    public void RemoveItem(string itemName, bool synchronize = true)
    {
        if (!pv.IsMine) return;

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

        if (synchronize)
        {
            pv.RPC("RemoveItemRPC", RpcTarget.OthersBuffered, itemName);
        }

        inventoryUI.UpdateInventoryUI();
        FindObjectOfType<QuickSlotManager>().UpdateQuickSlotUI();
    }
}
