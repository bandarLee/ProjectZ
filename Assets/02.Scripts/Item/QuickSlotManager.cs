using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

public class QuickSlotManager : MonoBehaviour
{
    public Image[] quickSlotImages;
    public TMP_Text[] quickSlotQuantities;
    public Item[] quickSlotItems;
    public Item currentEquippedItem;

    public Inventory inventory;

    private InventoryManager inventoryManager;
    private CharacterItemAbility characterItemAbility;

    private void Start()
    {
        quickSlotItems = new Item[quickSlotImages.Length];
        foreach (Image image in quickSlotImages)
        {
            image.gameObject.SetActive(false);
        }
        StartCoroutine(InitializeInventory());

       

    }

    private IEnumerator InitializeInventory()
    {
        yield return new WaitForSeconds(1.0f);

        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다. 씬에 InventoryManager가 있는지 확인하세요.");
        }

        GameObject localPlayer = Character.LocalPlayerInstance.gameObject;
        if (localPlayer != null)
        {
            inventory = localPlayer.GetComponent<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("Inventory를 찾을 수 없습니다. Player 객체에 Inventory가 있는지 확인하세요.");
            }
            else
            {
                characterItemAbility = localPlayer.GetComponent<CharacterItemAbility>();
                if (characterItemAbility == null)
                {
                    Debug.LogError("CharacterItemAbility를 찾을 수 없습니다. Player 객체에 CharacterItemAbility가 있는지 확인하세요.");
                }
            }
        }
        else
        {
            Debug.LogError("로컬 플레이어 오브젝트를 찾을 수 없습니다.");
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
            Debug.Log("드랍 아이템: " + currentEquippedItem.itemName);

            inventory.itemQuantities[itemName]--;
            if (inventory.itemQuantities[itemName] <= 0)
            {
                inventory.items.Remove(itemName);
                inventory.itemQuantities.Remove(itemName);
                RemoveItemFromQuickSlots(currentEquippedItem);

                if (characterItemAbility != null && characterItemAbility.PhotonView != null)
                {
                    characterItemAbility.PhotonView.RPC("DropItemPrefab", RpcTarget.AllBuffered, currentEquippedItem.itemName, inventory.transform.position, inventory.transform.forward);
                }
                else
                {
                    Debug.LogError("CharacterItemAbility 또는 PhotonView가 null입니다. RPC를 호출할 수 없습니다.");
                }
                currentEquippedItem = null;
            }

            inventoryManager.UpdateAllInventories();
        }
        inventoryManager.CloseItemInfo();

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
        if (Input.GetMouseButtonDown(0))
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropEquippedItem();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryManager.TogglePlayerInventory();
        }
    }
}