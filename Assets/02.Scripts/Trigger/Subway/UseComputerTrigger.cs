using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UseComputerTrigger : MonoBehaviour
{
    public TextMeshProUGUI UseComputerText;
    public TextMeshProUGUI NoDiskText;

    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;
    private InventoryManager inventoryManager;
    private ItemUseManager itemUseManager;
    private Coroutine noDiskTextCoroutine;

    public bool isPlayerInTrigger = false;
    public Item DiskItem;


    private void Start()
    {
        UseComputerText.gameObject.SetActive(false);
        NoDiskText.gameObject.SetActive(false);

        playerInventory = Inventory.Instance;
        quickSlotManager = FindObjectOfType<QuickSlotManager>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        itemUseManager = FindObjectOfType<ItemUseManager>();

        StartCoroutine(InitializingInventory());
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
    void Update()
    {
        if ((isPlayerInTrigger)&& Input.GetMouseButtonDown(0))
        {
            DiskItem = null;

            foreach (Item item in playerInventory.items.Values)
                {
                    if (item.itemType == ItemType.Special && (item.itemName == "디스크1" || item.itemName == "디스크2" || item.itemName == "디스크3"))
                    {
                        DiskItem = item;
                    break;

                }
                if (DiskItem == null)
                    {
                        if (noDiskTextCoroutine != null)
                        {
                            StopCoroutine(noDiskTextCoroutine);
                        }
                        NoDiskText.gameObject.SetActive(true);
                        noDiskTextCoroutine = StartCoroutine(HideNoDiskTextAfterDelay(1.5f));
                    }
                }
            
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Character.LocalPlayerInstance.PhotonView.IsMine)
        {
            UseComputerText.gameObject.SetActive(true);
            isPlayerInTrigger = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UseComputerText.gameObject.SetActive(false);
            isPlayerInTrigger= false;
        }
    }


    

    private IEnumerator HideNoDiskTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(1.5f);
        NoDiskText.gameObject.SetActive(false);
    }
}
