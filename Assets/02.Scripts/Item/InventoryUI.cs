using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class InventoryUI : MonoBehaviour
{
    public Item currentSelectedItem;

    public GameObject[] inventorySlots;
    public Inventory inventory;

    public GameObject ItemInfo;

    public TMP_Text itemNameText;
    public TMP_Text itemTypeText;
    public TMP_Text itemEffectText;
    public TMP_Text itemDescriptionText;
    public Image itemIconImage;
    public GameObject inventoryObject;
    public QuickSlotManager quickSlotManager;

    private void Start()
    {
        inventory = Inventory.Instance;
        UpdateInventoryUI();
        ItemInfo.SetActive(false);
    }

    private void OnEnable()
    {
        Inventory.Instance.inventoryUI = GetComponent<InventoryUI>();
    }

    public void UpdateInventoryUI()
    {
        var itemList = inventory.items.Keys.ToList();
        var itemQuantities = inventory.itemQuantities;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Slot slot = inventorySlots[i].GetComponent<Slot>();
            if (i < itemList.Count)
            {
                var itemName = itemList[i];
                var currentItem = inventory.items[itemName];
                int currentQuantity = itemQuantities.ContainsKey(itemName) ? itemQuantities[itemName] : 0;

                if (slot != null)
                {
                    slot.normalIcon.sprite = currentItem.icon;
                    slot.highlightedIcon.sprite = currentItem.icon;
                    slot.pressedIcon.sprite = currentItem.icon;

                    slot.normalIcon.transform.localScale = new Vector3(2, 2, 2);
                    slot.highlightedIcon.transform.localScale = new Vector3(2, 2, 2);
                    slot.pressedIcon.transform.localScale = new Vector3(2, 2, 2);

                    if (currentItem.itemType == ItemType.Weapon || currentItem.itemType == ItemType.ETC)
                    {
                        slot.quantityText.text = "";
                    }
                    else
                    {
                        slot.quantityText.text = currentQuantity.ToString();
                    }
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

        quickSlotManager.UpdateQuickSlotUI();
    }

    public void UseSelectedItem()
    {
        if (currentSelectedItem == null) return;

        string itemName = currentSelectedItem.itemType == ItemType.Weapon || currentSelectedItem.itemType == ItemType.ETC
                          ? currentSelectedItem.uniqueId : currentSelectedItem.itemName;

        if (currentSelectedItem.itemType == ItemType.Weapon || currentSelectedItem.itemType == ItemType.ETC)
        {
            ItemUseManager.Instance.EquipItem(currentSelectedItem);
            quickSlotManager.currentEquippedItem = currentSelectedItem;
        }
        else
        {
            ItemUseManager.Instance.ApplyEffect(currentSelectedItem);
            quickSlotManager.currentEquippedItem = currentSelectedItem;

            inventory.itemQuantities[itemName]--;
            if (inventory.itemQuantities[itemName] <= 0)
            {
                inventory.items.Remove(itemName);
                inventory.itemQuantities.Remove(itemName);
                quickSlotManager.RemoveItemFromQuickSlots(currentSelectedItem);
                currentSelectedItem = null; // 설정 currentSelectedItem을 null로 설정
                CloseItemInfo();
            }
        }

        UpdateInventoryUI();
    }

    public void ShowItemInfo(int index)
    {
        ItemInfo.SetActive(true);

        var itemList = inventory.items.Values.ToList();
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
    }
}
