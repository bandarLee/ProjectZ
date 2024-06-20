using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;
using System;

public class BoxInventoryUI : MonoBehaviour
{
    public Item currentSelectedItem;
    public GameObject[] inventorySlots;
    private BoxInventory currentBoxInventory;

    public GameObject ItemInfo;
    public TMP_Text itemNameText;
    public TMP_Text itemTypeText;
    public TMP_Text itemEffectText;
    public TMP_Text itemDescriptionText;
    public Image itemIconImage;
    public GameObject inventoryObject;
    public GameObject boxinventoryUIobject;
    private Inventory playerInventory; 

    private void Start()
    {
        ItemInfo.SetActive(false);
        playerInventory = Inventory.Instance;

    }

    public void SetBoxInventory(BoxInventory boxInventory)
    {
        currentBoxInventory = boxInventory;
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        if (currentBoxInventory == null) return;

        var itemList = currentBoxInventory.items.Keys.ToList();
        var itemQuantities = currentBoxInventory.itemQuantities;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Slot slot = inventorySlots[i].GetComponent<Slot>();
            if (i < itemList.Count)
            {
                var itemName = itemList[i];
                var currentItem = currentBoxInventory.items[itemName];
                int currentQuantity = itemQuantities.ContainsKey(itemName) ? itemQuantities[itemName] : 0;

                if (slot != null)
                {
                    slot.normalIcon.sprite = currentItem.icon;
                    slot.highlightedIcon.sprite = currentItem.icon;
                    slot.pressedIcon.sprite = currentItem.icon;

                    slot.normalIcon.transform.localScale = new Vector3(2, 2, 2);
                    slot.highlightedIcon.transform.localScale = new Vector3(2, 2, 2);
                    slot.pressedIcon.transform.localScale = new Vector3(2, 2, 2);

                    slot.quantityText.text = currentQuantity.ToString();
                }

                inventorySlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                int capturedIndex = i;

                inventorySlots[i].GetComponent<Button>().onClick.AddListener(() => ShowItemInfo(capturedIndex));
                var slotsitemList = currentBoxInventory.items.Values.ToList();
                slot.slotitem = slotsitemList[capturedIndex];

            }
            else
            {
                if (slot != null)
                {
                    slot.normalIcon.sprite = null;
                    slot.highlightedIcon.sprite = null;
                    slot.pressedIcon.sprite = null;

                    slot.normalIcon.transform.localScale = new Vector3(0, 0, 0);
                    slot.highlightedIcon.transform.localScale = new Vector3(0, 0, 0);
                    slot.pressedIcon.transform.localScale = new Vector3(0, 0, 0);

                    slot.quantityText.text = "";
                }

                inventorySlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }

        }
    }

    public void ShowItemInfo(int index)
    {
        ItemInfo.SetActive(true);

        var itemList = currentBoxInventory.items.Values.ToList();
        if (index < itemList.Count)
        {
            Item item = itemList[index];
            currentSelectedItem = item;
            itemNameText.text = item.itemName;
            itemTypeText.text = GetItemType(item.itemType);
            itemEffectText.text = item.itemEffect;
            itemDescriptionText.text = item.itemDescription;
            itemIconImage.sprite = item.icon;
        }
    }

    public void TransferToPlayerInventory()
    {
        if (currentSelectedItem == null) {return;}

        currentBoxInventory.BoxRemoveItem(currentSelectedItem);

        if (playerInventory == null)
        {
            playerInventory = Inventory.Instance;
        }

        if (playerInventory != null)
        {
            playerInventory.AddItem(currentSelectedItem);

            if (currentBoxInventory.photonView != null)
            {
                currentBoxInventory.photonView.RPC("BoxRemoveItemRPC", RpcTarget.OthersBuffered, currentSelectedItem.itemName, currentSelectedItem.itemType.ToString(), currentSelectedItem.uniqueId, currentSelectedItem.itemEffect, currentSelectedItem.itemDescription);
            }

            currentSelectedItem = null;
            ItemInfo.SetActive(false);

            UpdateInventoryUI();
            playerInventory.inventoryUI.UpdateInventoryUI();
        }
 
    }
    public void TransferToPlayerInventorySlot(Item slotitem)
    {
        if (slotitem == null) { return; }

        currentBoxInventory.BoxRemoveItem(slotitem);

        if (playerInventory == null)
        {
            playerInventory = Inventory.Instance;
        }

        if (playerInventory != null)
        {
            playerInventory.AddItem(slotitem);

            if (currentBoxInventory.photonView != null)
            {
                currentBoxInventory.photonView.RPC("BoxRemoveItemRPC", RpcTarget.OthersBuffered, currentSelectedItem.itemName, currentSelectedItem.itemType.ToString(), currentSelectedItem.uniqueId, currentSelectedItem.itemEffect, currentSelectedItem.itemDescription);
            }

            slotitem = null;
            ItemInfo.SetActive(false);

            UpdateInventoryUI();
            playerInventory.inventoryUI.UpdateInventoryUI();
        }

    }
    public void TransferToBoxInventory()
    {
        Item selectedItem = playerInventory.inventoryUI.currentSelectedItem;


            currentBoxInventory.BoxAddItem(selectedItem);

            if (currentBoxInventory.photonView != null)
            {
                currentBoxInventory.photonView.RPC("BoxAddItemRPC", RpcTarget.OthersBuffered, selectedItem.itemName, selectedItem.itemType.ToString(), selectedItem.uniqueId, selectedItem.itemEffect, selectedItem.itemDescription);
            }

        if (playerInventory != null)
        {
            string itemName = (selectedItem.itemType == ItemType.Weapon || selectedItem.itemType == ItemType.ETC || selectedItem.itemType == ItemType.Gun)
                              ? selectedItem.uniqueId : selectedItem.itemName;
            if (playerInventory.itemQuantities.ContainsKey(itemName))
            {

                playerInventory.itemQuantities[itemName]--;
                if (playerInventory.itemQuantities[itemName] <= 0)
                {
                    playerInventory.items.Remove(itemName);
                    playerInventory.itemQuantities.Remove(itemName);
                    playerInventory.inventoryUI.quickSlotManager.RemoveItemFromQuickSlots(selectedItem);


                }

            }
            else
            {
                Debug.Log("아이템 보관 실행1");
                playerInventory.RemoveItem(selectedItem.uniqueId);

                playerInventory.items.Remove(selectedItem.uniqueId);
                playerInventory.itemQuantities.Remove(selectedItem.uniqueId);
                playerInventory.inventoryUI.quickSlotManager.RemoveItemFromQuickSlots(selectedItem);

            }
            playerInventory.inventoryUI.currentSelectedItem = null;
            playerInventory.inventoryUI.ItemInfo.SetActive(false);

            UpdateInventoryUI();
            playerInventory.inventoryUI.UpdateInventoryUI();
        }
   
    }

    public string GetItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Food:
                return "음식";
            case ItemType.Weapon:
                return "무기";
            case ItemType.Heal:
                return "회복";
            case ItemType.Mental:
                return "정신력";
            case ItemType.ETC:
                return "기타";
            case ItemType.Consumable:
                return "소모품";
            case ItemType.Gun:
                return "총";
            case ItemType.StatBook:
                return "스탯";
            default:
                return "알 수 없음";
        }
    }

    public void CloseItemInfo()
    {
        ItemInfo.SetActive(false);
    }

    public void CloseInventory()
    {
        inventoryObject.SetActive(false);
        boxinventoryUIobject.SetActive(false);
    }
}
