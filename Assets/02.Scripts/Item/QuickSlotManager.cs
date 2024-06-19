using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditorInternal.ReorderableList;

public class QuickSlotManager : MonoBehaviour
{
    public Image[] quickSlotImages;
    public TMP_Text[] quickSlotQuantities;
    public Item[] quickSlotItems;
    public Item currentEquippedItem;

    public Inventory inventory;

    private InventoryManager inventoryManager;
    private CharacterItemAbility characterItemAbility;

    public bool ItemUseLock = false;

    private Animator _animator;
    public Image[] SelectColors;

    private void Start()
    {
        _animator = GetComponent<Animator>();

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
        }

        GameObject localPlayer = Character.LocalPlayerInstance.gameObject;
        if (localPlayer != null)
        {
            inventory = Inventory.Instance;
            if (inventory == null)
            {
            }
            else
            {
                characterItemAbility = localPlayer.GetComponent<CharacterItemAbility>();
                if (characterItemAbility == null)
                {
                }
            }
        }
        else
        {
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

        foreach (Image selectColor in SelectColors)
        {
            selectColor.color = Color.white;
        }

        Color customOrangeColor = new Color(255f / 255f, 120f / 255f, 0f / 255f);
        SelectColors[slotIndex].color = customOrangeColor;

        ItemUseManager.Instance.EquipItem(currentEquippedItem);
    }

    public void DropEquippedItem()
    {
        if (currentEquippedItem == null || currentEquippedItem.itemName == null) return;

        string itemName = currentEquippedItem.itemType == ItemType.Weapon || currentEquippedItem.itemType == ItemType.ETC
                          ? currentEquippedItem.uniqueId : currentEquippedItem.itemName;

        if (inventory.itemQuantities.ContainsKey(itemName))
        {
            Debug.Log("드랍 아이템: " + currentEquippedItem.itemName);

            inventory.itemQuantities[itemName]--;
            // usinghand = 0;
            characterItemAbility.UnUsingHandAnimation();
            characterItemAbility.DeactivateAllItems();

            if (characterItemAbility != null && characterItemAbility.PhotonView != null)
            {
                characterItemAbility.PhotonView.RPC("DropItemPrefab", RpcTarget.AllBuffered, currentEquippedItem.itemName, Character.LocalPlayerInstance.gameObject.transform.position, Character.LocalPlayerInstance.gameObject.transform.forward);
            }
            if (inventory.itemQuantities[itemName] <= 0)
            {
                inventory.items.Remove(itemName);
                inventory.itemQuantities.Remove(itemName);
                RemoveItemFromQuickSlots(currentEquippedItem);

                currentEquippedItem = null;
            }

            inventoryManager.UpdateAllInventories();
        }
        inventoryManager.CloseItemInfo();
    }

    public void DropAllItem()
    {
        List<Item> itemsToDrop = new List<Item>(inventory.items.Values);

        foreach (Item item in itemsToDrop)
        {
            DropItem(item);
        }
    }

    private void DropItem(Item item)
    {
        if (item == null || item.itemName == null) return;

        string itemName = item.itemType == ItemType.Weapon || currentEquippedItem.itemType == ItemType.ETC
                          ? item.uniqueId : currentEquippedItem.itemName;

        if (inventory.itemQuantities.ContainsKey(itemName))
        {
            Debug.Log("드랍 아이템: " + item.itemName);

            inventory.itemQuantities[itemName]--;
            // usinghand = 0;
            characterItemAbility.UnUsingHandAnimation();
            characterItemAbility.DeactivateAllItems();

            if (characterItemAbility != null && characterItemAbility.PhotonView != null)
            {
                characterItemAbility.PhotonView.RPC("DropItemPrefab", RpcTarget.AllBuffered, currentEquippedItem.itemName, Character.LocalPlayerInstance.gameObject.transform.position, Character.LocalPlayerInstance.gameObject.transform.forward);
            }
            if (inventory.itemQuantities[itemName] <= 0)
            {
                inventory.items.Remove(itemName);
                inventory.itemQuantities.Remove(itemName);
                RemoveItemFromQuickSlots(item);

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

                if (quickSlotItems[i].itemType == ItemType.Weapon || quickSlotItems[i].itemType == ItemType.ETC || quickSlotItems[i].itemType == ItemType.Gun)
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

    private void CheckQuickSlotInput()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseQuickSlotItem(i);
            }
        }
    }

    private void Update()
    {
        CheckQuickSlotInput();

        CheckQuickSlotInput();

        if (Input.GetMouseButtonDown(0) && currentEquippedItem != null && !ItemUseLock)
        {
            switch (currentEquippedItem.itemType)
            {
                case ItemType.Food:
                    ItemUseManager.Instance.UseItem(currentEquippedItem, 2f);
                    inventoryManager.UpdateAllInventories();
                    break;
                case ItemType.ETC:
                    ItemUseManager.Instance.UseItem(currentEquippedItem, 0f);
                    inventoryManager.UpdateAllInventories();
                    break;
                case ItemType.Heal:
                    ItemUseManager.Instance.UseItem(currentEquippedItem, 2f);
                    inventoryManager.UpdateAllInventories();
                    break;
                case ItemType.Mental:
                    ItemUseManager.Instance.UseItem(currentEquippedItem, 2f);
                    inventoryManager.UpdateAllInventories();
                    break;
                case ItemType.Default:

                    break;
                default:

                    break;
            }



        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropEquippedItem();
            Character.LocalPlayerInstance._attackability.DeactivateAllWeapons();
            Character.LocalPlayerInstance._gunfireAbility.DeactivateAllGuns();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryManager.TogglePlayerInventory();
        }
    }
}
