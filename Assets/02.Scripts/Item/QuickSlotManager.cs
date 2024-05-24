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
                quickSlotQuantities[i].text = "";
                break;
            }
        }

        quickSlotItems[slotIndex] = item;
        quickSlotImages[slotIndex].sprite = item.icon;
        quickSlotImages[slotIndex].gameObject.SetActive(true);
        string itemName = item.itemType == ItemType.Weapon || item.itemType == ItemType.ETC
                          ? item.uniqueId : item.itemName;

        if (item.itemType == ItemType.Weapon || item.itemType == ItemType.ETC)
        {
            quickSlotQuantities[slotIndex].text = ""; 
        }
        else
        {
            quickSlotQuantities[slotIndex].text = inventory.itemQuantities[itemName].ToString(); // 수량 표시
        }
    }

    public void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Length || quickSlotItems[slotIndex] == null) return;

        Debug.Log("아이템 사용: " + quickSlotItems[slotIndex].itemName);

        string itemName = quickSlotItems[slotIndex].itemType == ItemType.Weapon || quickSlotItems[slotIndex].itemType == ItemType.ETC
                          ? quickSlotItems[slotIndex].uniqueId : quickSlotItems[slotIndex].itemName;

        if (!inventory.itemQuantities.ContainsKey(itemName))
        {
            Debug.LogError("KeyNotFoundException: The given key '" + itemName + "' was not present in the dictionary.");
            return;
        }

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
            if (quickSlotItems[slotIndex].itemType == ItemType.Weapon || quickSlotItems[slotIndex].itemType == ItemType.ETC)
            {
                quickSlotQuantities[slotIndex].text = "";
            }
            else
            {
                quickSlotQuantities[slotIndex].text = inventory.itemQuantities[itemName].ToString();
            }
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
                quickSlotQuantities[i].text = ""; // 수량 텍스트 숨기기
            }
        }
    }

    public void UpdateQuickSlotUI()
    {
        for (int i = 0; i < quickSlotItems.Length; i++)
        {
            if (quickSlotItems[i] != null)
            {
                string itemName = quickSlotItems[i].itemType == ItemType.Weapon || quickSlotItems[i].itemType == ItemType.ETC
                                  ? quickSlotItems[i].uniqueId : quickSlotItems[i].itemName;

                if (quickSlotItems[i].itemType == ItemType.Weapon || quickSlotItems[i].itemType == ItemType.ETC)
                {
                    quickSlotQuantities[i].text = ""; // 빈 칸으로 설정
                }
                else
                {
                    quickSlotQuantities[i].text = inventory.itemQuantities[itemName].ToString();
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
