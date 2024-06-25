using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TheLastYggdrasilTrigger : MonoBehaviour
{
    public GameObject LastYggdrasilTrigger;
    public TextMeshProUGUI UseSeedText;
    public TextMeshProUGUI NoSeedItemText;

    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;
    private InventoryManager inventoryManager;
    private ItemUseManager itemUseManager;

    private bool IsPlayerTrigger = false;

    private void Start()
    {
        UseSeedText.gameObject.SetActive(false);
        NoSeedItemText.gameObject.SetActive(false );

        playerInventory = Inventory.Instance;
        quickSlotManager = FindObjectOfType<QuickSlotManager>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        itemUseManager = FindObjectOfType<ItemUseManager>();
        StartCoroutine(InitializingInventory());

        if (playerInventory == null)
        {
            Debug.LogError("Inventory not found");
        }

        if (quickSlotManager == null)
        {
            Debug.LogError("QuickSlotManager not found");
        }

        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI not found");
        }

        if (itemUseManager == null)
        {
            Debug.LogError("ItemUseManager not found");
        }
    }
    private IEnumerator InitializingInventory()
    {
        yield return new WaitForSeconds(1.0f);

        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다. 씬에 InventoryManager가 있는지 확인하세요.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Triggered on LastYggdrasilTrigger");

            IsPlayerTrigger = true;
            UseSeedText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerTrigger = false;
            UseSeedText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (IsPlayerTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Item seedItem = GetSeedItem();
            if (seedItem != null)
            {
                StartCoroutine(HideNoSeedTextAfterDelay());

            }
            else
            {
                NoSeedItemText.gameObject.SetActive(true);
                Debug.Log("No Seed Item in Quick Slot");
            }
        }
    }

    private IEnumerator HideNoSeedTextAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        NoSeedItemText.gameObject.SetActive(false );
    }

    private Item GetSeedItem()
    {
        if (quickSlotManager == null || quickSlotManager.quickSlotItems == null)
        {
            Debug.LogError("QuickSlotManager or quickSlotItems is null");
            return null;
        }

        // 퀵슬롯 검사
        foreach (var slotItem in quickSlotManager.quickSlotItems)
        {
            if (slotItem != null && slotItem.itemType == ItemType.ETC && slotItem.itemName == "세계수씨앗")
            {
                return slotItem;
            }
        }

        return null;
    }
}