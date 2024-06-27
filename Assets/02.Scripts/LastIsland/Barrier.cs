using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class Barrier : MonoBehaviourPun
{
    public TextMeshProUGUI NoSeedText;
    public GameObject BarrierPrefab;

    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;
    private InventoryManager inventoryManager;
    private ItemUseManager itemUseManager;

    private void Start()
    {
        NoSeedText.gameObject.SetActive(false);

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
            Debug.LogError("InventoryManager�� ã�� �� �����ϴ�. ���� InventoryManager�� �ִ��� Ȯ���ϼ���.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player triggered on barrier");

            Item seedItem = GetSeedItem();
            if (seedItem != null)
            {
                itemUseManager.ApplyEffect(seedItem);
                photonView.RPC("RPC_DestroyBarrier", RpcTarget.All);
            }
            else // ��������� ���� ���
            {
                NoSeedText.gameObject.SetActive(true);
                StartCoroutine(HideNoSeedTextAfterDelay(2f));
            }
        }
    }

    private IEnumerator HideNoSeedTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NoSeedText.gameObject.SetActive(false);
    }

    private Item GetSeedItem()
    {
        foreach (var item in playerInventory.items.Values)
        {
            if (item.itemType == ItemType.Special && item.itemName == "���������")
            {
                return item;
            }
        }
        return null;
    }

    [PunRPC]
    private void RPC_DestroyBarrier()
    {
        StartCoroutine(DestroyBarrier());
    }

    private IEnumerator DestroyBarrier()
    {
        Renderer renderer = BarrierPrefab.GetComponent<Renderer>();
        Color originalColor = renderer.material.color;
        float duration = 1f; // �����̴� ���� �ð�

        // �����̴� ȿ��
        for (int i = 0; i < 5; i++) // 5�� ������
        {
            renderer.material.DOColor(Color.clear, duration / 10f);
            yield return new WaitForSeconds(duration / 10f);
            renderer.material.DOColor(originalColor, duration / 10f);
            yield return new WaitForSeconds(duration / 10f);
        }

        // Barrier ����
        Destroy(BarrierPrefab);
    }
}
