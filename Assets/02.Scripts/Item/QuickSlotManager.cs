using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class QuickSlotManager : MonoBehaviour
{
    public Image[] quickSlotImages;
    public TMP_Text[] quickSlotQuantities; 
    public Item[] quickSlotItems;

    public Inventory inventory;

    private void Start()
    {
        quickSlotItems = new Item[quickSlotImages.Length];
        foreach (Image image in quickSlotImages)
        {
            image.gameObject.SetActive(false);
        }
    }

    public void RegisterItemToQuickSlot(int slotIndex, Item item)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Length) return;

        for (int i = 0; i < quickSlotItems.Length; i++)
        {
            if (quickSlotItems[i] == item)
            {
                quickSlotItems[i] = null;
                quickSlotImages[i].sprite = null;
                quickSlotImages[i].gameObject.SetActive(false);
                quickSlotQuantities[i].text = ""; // 수량 텍스트 숨기기
                break;
            }
        }

        quickSlotItems[slotIndex] = item;
        quickSlotImages[slotIndex].sprite = item.icon;
        quickSlotImages[slotIndex].gameObject.SetActive(true);
        if (item.itemType == ItemType.Weapon || item.itemType == ItemType.ETC)
        {
            quickSlotQuantities[slotIndex].text = ""; 
        }
        else
        {
            quickSlotQuantities[slotIndex].text = inventory.itemQuantities[item.itemName].ToString(); 
        }
    }

    public void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Length || quickSlotItems[slotIndex] == null) return;

        Debug.Log("아이템 사용" + quickSlotItems[slotIndex].itemName);

        string itemName = quickSlotItems[slotIndex].itemName;
        inventory.itemQuantities[itemName]--;
        if (inventory.itemQuantities[itemName] <= 0)
        {
            inventory.items.Remove(itemName);
            inventory.itemQuantities.Remove(itemName);
            quickSlotItems[slotIndex] = null;
            quickSlotImages[slotIndex].sprite = null;
            quickSlotImages[slotIndex].gameObject.SetActive(false);
            quickSlotQuantities[slotIndex].text = ""; 

            if (inventory.inventoryUI.currentSelectedItem != null && inventory.inventoryUI.currentSelectedItem.itemName == itemName)
            {
                inventory.inventoryUI.CloseItemInfo();
            }
        }
        else
        {
            quickSlotQuantities[slotIndex].text = inventory.itemQuantities[itemName].ToString(); 
        }

        inventory.inventoryUI.UpdateInventoryUI();
    }
    public void RemoveItemFromQuickSlots(Item item)
    {
        for (int i = 0; i < quickSlotItems.Length; i++)
        {
            if (quickSlotItems[i] == item)
            {
                quickSlotItems[i] = null;
                quickSlotImages[i].sprite = null;
                quickSlotImages[i].gameObject.SetActive(false);
                quickSlotQuantities[i].text = ""; 
            }
        }
    }
    public void UpdateQuickSlotUI()
    {
        for (int i = 0; i < quickSlotItems.Length; i++)
        {
            if (quickSlotItems[i] != null)
            {
                if (quickSlotItems[i].itemType == ItemType.Weapon || quickSlotItems[i].itemType == ItemType.ETC)
                {
                    quickSlotQuantities[i].text = ""; 
                }
                else
                {
                    quickSlotQuantities[i].text = inventory.itemQuantities[quickSlotItems[i].itemName].ToString();
                }
            }
            else
            {
                quickSlotQuantities[i].text = "";
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseQuickSlotItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseQuickSlotItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseQuickSlotItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseQuickSlotItem(3);
        }
    }
}
