using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;

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
        playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("Player Inventory not found!");
        }
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
        Debug.Log(currentSelectedItem);
    }

    public void TransferToPlayerInventory()
    {
        if (currentSelectedItem == null) return;

        string itemName = currentSelectedItem.uniqueId;
        currentBoxInventory.RemoveItem(itemName);

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<Inventory>();
        }

        if (playerInventory != null)
        {
            playerInventory.AddItem(currentSelectedItem);

            // BoxInventory의 PhotonView를 통해 RPC 호출
            if (currentBoxInventory.photonView != null)
            {
                currentBoxInventory.photonView.RPC("AddItemRPC", RpcTarget.OthersBuffered, currentSelectedItem.itemName, currentSelectedItem.itemType.ToString(), currentSelectedItem.uniqueId, currentSelectedItem.icon.name, currentSelectedItem.itemEffect, currentSelectedItem.itemDescription);
            }

            currentSelectedItem = null;
            ItemInfo.SetActive(false);

            UpdateInventoryUI();
            playerInventory.inventoryUI.UpdateInventoryUI();
        }
        else
        {
            Debug.LogError("Player Inventory not found when trying to transfer item!");
        }
    }
    public void TransferToBoxInventory()
    {
        Item selectedItem = playerInventory.inventoryUI.currentSelectedItem;


            currentBoxInventory.AddItem(selectedItem);

            if (currentBoxInventory.photonView != null)
            {
                currentBoxInventory.photonView.RPC("AddItemRPC", RpcTarget.OthersBuffered, selectedItem.itemName, selectedItem.itemType.ToString(), selectedItem.uniqueId, selectedItem.itemEffect, selectedItem.itemDescription);
                Debug.Log("RPC 호출: " + selectedItem.itemName);
            }
            else
            {
                Debug.LogError("Box Inventory PhotonView not found!");
            }
        if (playerInventory != null)
        {
            string itemName = selectedItem.itemType == ItemType.Weapon || selectedItem.itemType == ItemType.ETC
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
            playerInventory.inventoryUI.currentSelectedItem = null;
            playerInventory.inventoryUI.ItemInfo.SetActive(false);

            UpdateInventoryUI();
            playerInventory.inventoryUI.UpdateInventoryUI();
            Debug.Log("Item transferred to box inventory.");
        }
        else
        {
            Debug.LogError("Player Inventory not found when trying to transfer item to box!");
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
