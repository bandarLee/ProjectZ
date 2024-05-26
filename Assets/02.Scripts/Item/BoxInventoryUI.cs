using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BoxInventoryUI : MonoBehaviour
{
    public Item currentSelectedItem;
    public GameObject[] inventorySlots;
    private BoxInventory boxInventory;
    private Inventory playerInventory;

    public GameObject ItemInfo;
    public TMP_Text itemNameText;
    public TMP_Text itemTypeText;
    public TMP_Text itemEffectText;
    public TMP_Text itemDescriptionText;
    public Image itemIconImage;
    public GameObject inventoryObject;

    private void Start()
    {
        boxInventory = FindObjectOfType<BoxInventory>();
        playerInventory = FindObjectOfType<Inventory>();
        UpdateInventoryUI();
        ItemInfo.SetActive(false);
    }

    public void UpdateInventoryUI()
    {
        var itemList = boxInventory.items.Keys.ToList();
        var itemQuantities = boxInventory.itemQuantities;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Slot slot = inventorySlots[i].GetComponent<Slot>();
            if (i < itemList.Count)
            {
                var itemName = itemList[i];
                var currentItem = boxInventory.items[itemName];
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

        var itemList = boxInventory.items.Values.ToList();
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
        if (currentSelectedItem == null) return;

        string itemName = currentSelectedItem.itemName;
        boxInventory.RemoveItem(itemName);
        playerInventory.AddItem(currentSelectedItem);

        // ������ Ÿ�Կ� ������� ��� ������ ��� ������ ���� â�� ��
        if (!boxInventory.items.ContainsKey(itemName))
        {
            currentSelectedItem = null;
            ItemInfo.SetActive(false);
        }
    }

    public void TransferToBoxInventory()
    {
        if (playerInventory.inventoryUI.currentSelectedItem == null) return;

        Item item = playerInventory.inventoryUI.currentSelectedItem;
        string itemName = item.itemType == ItemType.Weapon || item.itemType == ItemType.ETC
                          ? item.uniqueId : item.itemName;

        playerInventory.RemoveItem(itemName);
        boxInventory.AddItem(item);

        // ������ Ÿ�Կ� ������� ��� ������ ��� ������ ���� â�� ��
        if (!playerInventory.items.ContainsKey(itemName))
        {
            playerInventory.inventoryUI.currentSelectedItem = null;
            playerInventory.inventoryUI.CloseItemInfo();
        }
    }

    public string GetItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Food:
                return "����";
            case ItemType.Weapon:
                return "����";
            case ItemType.Heal:
                return "ȸ��";
            case ItemType.Mental:
                return "���ŷ�";
            case ItemType.ETC:
                return "��Ÿ";
            default:
                return "�� �� ����";
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
