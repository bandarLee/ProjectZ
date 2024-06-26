using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using UnityEngine.UI;

public class TheLastYggdrasilTrigger : MonoBehaviour
{
    public GameObject LastYggdrasilTrigger;
    public TextMeshProUGUI UseSeedText;
    public TextMeshProUGUI NoSeedItemText;
    public GameObject TheLastYggdrasilPrefab;
    public Slider TheLastYggdrasilHPSlider;

    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;
    private InventoryManager inventoryManager;
    private ItemUseManager itemUseManager;

    public bool IsPlayerTrigger = false;
    private Vector3 initialPosition; // ������ �ʱ� ��ġ ����

    private void Start()
    {
        UseSeedText.gameObject.SetActive(false);
        NoSeedItemText.gameObject.SetActive(false);
        TheLastYggdrasilHPSlider.gameObject.SetActive(false);

        initialPosition = new Vector3(-45.53f, 10.7f, -54.78f); // �ʱ� ��ġ ����
        TheLastYggdrasilPrefab.transform.position = initialPosition;
        TheLastYggdrasilPrefab.SetActive(false);

        StartCoroutine(InitializingInventory());
    }

    private IEnumerator InitializingInventory()
    {
        yield return new WaitForSeconds(1.1f);

        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager�� ã�� �� �����ϴ�. ���� InventoryManager�� �ִ��� Ȯ���ϼ���.");
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

    // �÷��̾ EŰ�� ���� ������� �ɴ´�
    private void Update()
    {
        if (IsPlayerTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Item seedItem = GetSeedItem();
            if (seedItem != null)
            {
                itemUseManager.ApplyEffect(seedItem);
                DestroyUseSeedText(); // UseSeedText �ı�

                // TheLastYggdrasilPrefab�� ������ ����
                if (TheLastYggdrasilPrefab != null)
                {
                    TheLastYggdrasilPrefab.SetActive(true);
                    TheLastYggdrasilPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    TheLastYggdrasilPrefab.transform.position = new Vector3(-45.53f, 10.7f, -54.78f);
                    TheLastYggdrasilPrefab.transform.DOScale(new Vector3(1f, 1f, 1f), 60f).OnUpdate(() =>
                    {
                        // ������ ���濡 ���� ��ġ ����
                        Vector3 newPosition = CalculatePositionBasedOnScale(TheLastYggdrasilPrefab.transform.localScale);
                        TheLastYggdrasilPrefab.transform.position = newPosition;
                    }).OnComplete(() =>
                    {
                        // ����� HPBar Ȱ��ȭ
                        TheLastYggdrasilHPSlider.gameObject.SetActive(true);
                    });
                }
            }
            else
            {
                NoSeedItemText.gameObject.SetActive(true);
                Debug.Log("No Seed Item in Quick Slot");
                StartCoroutine(HideNoSeedTextAfterDelay());
            }
        }
    }

    private Vector3 CalculatePositionBasedOnScale(Vector3 scale)
    {
        float x = scale.x;
        if (x <= 0.1f) return new Vector3(-45.53f, 10.7f, -54.78f);
        if (x <= 0.3f) return Vector3.Lerp(new Vector3(-45.53f, 10.7f, -54.78f), new Vector3(-45.23f, 11.1f, -55.37f), (x - 0.1f) / 0.2f);
        if (x <= 0.5f) return Vector3.Lerp(new Vector3(-45.23f, 11.1f, -55.37f), new Vector3(-44.96f, 11.55f, -55.46f), (x - 0.3f) / 0.2f);
        if (x <= 0.7f) return Vector3.Lerp(new Vector3(-44.96f, 11.55f, -55.46f), new Vector3(-44.96f, 11.77f, -55.46f), (x - 0.5f) / 0.2f);
        if (x <= 0.9f) return Vector3.Lerp(new Vector3(-44.96f, 11.77f, -55.46f), new Vector3(-44.96f, 12.17f, -55.48f), (x - 0.7f) / 0.2f);
        return Vector3.Lerp(new Vector3(-44.96f, 12.17f, -55.48f), new Vector3(-44.96f, 12.34f, -55.46f), (x - 0.9f) / 0.1f);
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

        // ������ �˻�
        foreach (var slotItem in quickSlotManager.quickSlotItems)
        {
            if (slotItem != null && slotItem.itemType == ItemType.ETC && slotItem.itemName == "���������")
            {
                return slotItem;
            }
        }

        return null;
    }

    private void DestroyUseSeedText()
    {
        Destroy(UseSeedText.gameObject); // UseSeedText �ı�
    }
}
