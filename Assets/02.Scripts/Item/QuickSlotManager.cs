using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlotManager : MonoBehaviour
{
    public Image[] quickSlotImages;
    public TMP_Text[] quickSlotQuantities;
    public Item[] quickSlotItems;
    public Item currentEquippedItem;

    public Inventory inventory;

    private InventoryManager inventoryManager;

    private void Start()
    {
        quickSlotItems = new Item[quickSlotImages.Length];
        foreach (Image image in quickSlotImages)
        {
            image.gameObject.SetActive(false);
        }

        inventoryManager = FindObjectOfType<InventoryManager>();
        inventory = FindObjectOfType<Inventory>();

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager�� ã�� �� �����ϴ�. ���� InventoryManager�� �ִ��� Ȯ���ϼ���.");
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
            quickSlotQuantities[slotIndex].text = inventory.itemQuantities[itemName].ToString();
        }
    }

    public void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Length || quickSlotItems[slotIndex] == null) return;

        currentEquippedItem = quickSlotItems[slotIndex];
        ItemUseManager.Instance.EquipItem(currentEquippedItem);
    }

    public void DropEquippedItem()
    {
        if (currentEquippedItem == null) return;

        string itemName = currentEquippedItem.itemType == ItemType.Weapon || currentEquippedItem.itemType == ItemType.ETC
                          ? currentEquippedItem.uniqueId : currentEquippedItem.itemName;

        if (inventory.itemQuantities.ContainsKey(itemName))
        {
            Debug.Log("��� ������: " + currentEquippedItem.itemName);

            inventory.itemQuantities[itemName]--;
            if (inventory.itemQuantities[itemName] <= 0)
            {
                inventory.items.Remove(itemName);
                inventory.itemQuantities.Remove(itemName);
                RemoveItemFromQuickSlots(currentEquippedItem);
                DropItemPrefab(currentEquippedItem);
                currentEquippedItem = null;
            }

            inventoryManager.UpdateAllInventories();
        }
    }

    private void DropItemPrefab(Item item)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("ItemPrefabs/" + item.itemName);
        if (itemPrefab != null)
        {
            Vector3 dropPosition = inventory.gameObject.transform.position + transform.forward * 2f + transform.up * 1.5f;
            GameObject droppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);
            ItemPickup itemPickup = droppedItem.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.item = item;
            }
        }
        else
        {
            Debug.LogWarning("Item prefab not found in Resources/ItemPrefabs: " + item.itemName);
        }
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
        UpdateQuickSlotUI();
    }

    public void UnEquipCurrentItem()
    {
        currentEquippedItem = null;
        UpdateQuickSlotUI();
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
                    quickSlotQuantities[i].text = "";
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
        if (Input.GetMouseButtonDown(0)) // ��Ŭ��
        {
            if (currentEquippedItem != null)
            {
                if (currentEquippedItem.itemType == ItemType.Food || currentEquippedItem.itemType == ItemType.Heal || currentEquippedItem.itemType == ItemType.Mental)
                {
                    ItemUseManager.Instance.ApplyEffect(currentEquippedItem);

                    string itemName = currentEquippedItem.itemType == ItemType.Weapon || currentEquippedItem.itemType == ItemType.ETC
                                      ? currentEquippedItem.uniqueId : currentEquippedItem.itemName;

                    if (inventory.itemQuantities.ContainsKey(itemName))
                    {
                        inventory.itemQuantities[itemName]--;
                        if (inventory.itemQuantities[itemName] <= 0)
                        {
                            inventory.items.Remove(itemName);
                            inventory.itemQuantities.Remove(itemName);
                            RemoveItemFromQuickSlots(currentEquippedItem);
                            currentEquippedItem = null;
                        }
                    }
                }
                else if (currentEquippedItem.itemType == ItemType.Weapon || currentEquippedItem.itemType == ItemType.ETC)
                {
                    ItemUseManager.Instance.ApplyEffect(currentEquippedItem);
                }

                inventoryManager.UpdateAllInventories();
            }
        }

        if (Input.GetMouseButtonDown(1)) // ��Ŭ��
        {
            DropEquippedItem();
        }
        if (Input.GetKeyDown(KeyCode.I)) // 'I' Ű�� �κ��丮 ���
        {
            inventoryManager.TogglePlayerInventory();
        }
    }
}
