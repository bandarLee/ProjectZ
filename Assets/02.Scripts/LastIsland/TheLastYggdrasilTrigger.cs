using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

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

    public bool IsPlayerTrigger = false;

    private void Start()
    {
        UseSeedText.gameObject.SetActive(false);
        NoSeedItemText.gameObject.SetActive(false);

        StartCoroutine(InitializingInventory());
    }

    private IEnumerator InitializingInventory()
    {
        yield return new WaitForSeconds(1.1f);

        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다. 씬에 InventoryManager가 있는지 확인하세요.");
        }
        playerInventory = Inventory.Instance;
        quickSlotManager = FindObjectOfType<QuickSlotManager>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        itemUseManager = FindObjectOfType<ItemUseManager>();
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

    // 플레이어가 E키를 눌러 세계수를 심는다
    private void Update()
    {
        if (IsPlayerTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Item seedItem = GetSeedItem();
            if (seedItem != null)
            {
                itemUseManager.ApplyEffect(seedItem);
                DestroyUseSeedText(); // UseSeedText 파괴
        
            }
            else
            {
                NoSeedItemText.gameObject.SetActive(true);
                Debug.Log("No Seed Item in Quick Slot");
                StartCoroutine(HideNoSeedTextAfterDelay());
            }
        }
    }

    private IEnumerator HideNoSeedTextAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        NoSeedItemText.gameObject.SetActive(false);
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

    private void DestroyUseSeedText()
    {
        Destroy(UseSeedText.gameObject); // UseSeedText 파괴
    }
}
