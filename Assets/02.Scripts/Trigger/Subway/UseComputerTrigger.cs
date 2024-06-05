using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UseComputerTrigger : MonoBehaviour
{
    public TextMeshProUGUI MissionText;
    public TextMeshProUGUI UseComputerText;
    public TextMeshProUGUI NoDiskText;

    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;
    private InventoryManager inventoryManager;
    private ItemUseManager itemUseManager;
    private Coroutine noDiskTextCoroutine;

    public bool isPlayerInTrigger = false;

    private void Start()
    {
        UseComputerText.gameObject.SetActive(false);
        NoDiskText.gameObject.SetActive(false);

        playerInventory = FindObjectOfType<Inventory>();
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
            Debug.LogError("InventoryManager�� ã�� �� �����ϴ�. ���� InventoryManager�� �ִ��� Ȯ���ϼ���.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // ������ ��� �Ŵ����� ���� ��ũ �������� ���
            Item DiskItem = GetDiskItem();
            if (DiskItem != null)
            {
                itemUseManager.ApplyEffect(DiskItem);
            }
            else
            {
                if (noDiskTextCoroutine != null)
                {
                    StopCoroutine(noDiskTextCoroutine);
                }
                NoDiskText.gameObject.SetActive(true);
                noDiskTextCoroutine = StartCoroutine(HideNoDiskTextAfterDelay(3f));
            }
        }
    }

    private Item GetDiskItem()
    {
        foreach (var item in playerInventory.items.Values)
        {
            if (item.itemType == ItemType.ETC && item.itemName == "��ũ1"|| item.itemName == "��ũ2"||item.itemName == "��ũ3")
            {
                return item;
            }
        }
        return null;
    }

    private IEnumerator HideNoDiskTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NoDiskText.gameObject.SetActive(false);
    }


    public void UpdateMissionText(string newText)
    {
        if (MissionText != null)
        {
            MissionText.text = newText;
        }
    }
}
