using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static Item currentSelectedItem;

    public GameObject[] inventorySlots;
    private Inventory inventory;

    public GameObject ItemInfo;

    public TMP_Text itemNameText;
    public TMP_Text itemTypeText;
    public TMP_Text itemEffectText;
    public TMP_Text itemDescriptionText;
    public Image itemIconImage;
    public GameObject inventoryobject;
    public QuickSlotManager quickSlotManager;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        UpdateInventoryUI();
        ItemInfo.SetActive(false);
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Slot slot = inventorySlots[i].GetComponent<Slot>();
            if (i < inventory.items.Count)
            {
                int itemIndex = i;
                Item currentItem = inventory.items[itemIndex];

                if (slot != null)
                {
                    slot.normalIcon.sprite = currentItem.icon;
                    slot.highlightedIcon.sprite = currentItem.icon;
                    slot.pressedIcon.sprite = currentItem.icon;

                    slot.normalIcon.transform.localScale = new Vector3(2, 2, 2);
                    slot.highlightedIcon.transform.localScale = new Vector3(2, 2, 2);
                    slot.pressedIcon.transform.localScale = new Vector3(2, 2, 2);
                }

                inventorySlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                int capturedIndex = itemIndex;
                inventorySlots[i].GetComponent<Button>().onClick.AddListener(() => ShowItemInfo(capturedIndex));
            }
            else
            {
                if (slot != null)
                {
                    slot.normalIcon.sprite = slot.normalIcon.sprite;
                    slot.highlightedIcon.sprite = slot.highlightedIcon.sprite;
                    slot.pressedIcon.sprite = slot.pressedIcon.sprite;

                    slot.normalIcon.transform.localScale = new Vector3(0, 0, 0);
                    slot.highlightedIcon.transform.localScale = new Vector3(0, 0, 0);
                    slot.pressedIcon.transform.localScale = new Vector3(0, 0, 0);
                }

                inventorySlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    public void ShowItemInfo(int index)
    {
        ItemInfo.SetActive(true);

        if (index < inventory.items.Count)
        {
            Item item = inventory.items[index];
            currentSelectedItem = item; // 현재 선택된 아이템 설정
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
        inventoryobject.SetActive(false);
    }
}
