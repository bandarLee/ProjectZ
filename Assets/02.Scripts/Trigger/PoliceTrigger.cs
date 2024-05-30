using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PoliceTrigger : MonoBehaviour
{
    public TextMeshProUGUI OpenText;
    public TextMeshProUGUI NoKeyText;

    public GameObject DoorPrefab;
    public float moveDistance = 5f; // �̵� �Ÿ�
    public float moveDuration = 2f; // �̵� �ð�

    private bool isPlayerInTrigger = false;
    private bool isDoorMoving = false;
    private bool hasDoorOpened = false;

    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;
    private InventoryManager inventoryManager;
    private ItemUseManager itemUseManager;
    private Coroutine noKeyTextCoroutine;

    private void Start()
    {
        OpenText.gameObject.SetActive(false);
        NoKeyText.gameObject.SetActive(false);

        playerInventory = FindObjectOfType<Inventory>();
        quickSlotManager = FindObjectOfType<QuickSlotManager>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        itemUseManager = FindObjectOfType<ItemUseManager>();

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenText.gameObject.SetActive(true);
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenText.gameObject.SetActive(false);
            NoKeyText.gameObject.SetActive(false);
            isPlayerInTrigger = false;
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // ������ ��� �Ŵ����� ���� Ű �������� ���
            Item keyItem = GetKeyItem();
            if (keyItem != null)
            {
                itemUseManager.ApplyEffect(keyItem);
            }
            else
            {
                if (noKeyTextCoroutine != null)
                {
                    StopCoroutine(noKeyTextCoroutine);
                }
                NoKeyText.gameObject.SetActive(true);
                noKeyTextCoroutine = StartCoroutine(HideNoKeyTextAfterDelay(3f));
            }
        }
    }

    private Item GetKeyItem()
    {
        foreach (var item in playerInventory.items.Values)
        {
            if (item.itemType == ItemType.ETC && item.itemName == "����")
            {
                return item;
            }
        }

        for (int i = 0; i < quickSlotManager.quickSlotItems.Length; i++)
        {
            var quickSlotItem = quickSlotManager.quickSlotItems[i];
            if (quickSlotItem != null && quickSlotItem.itemType == ItemType.ETC && quickSlotItem.itemName == "����")
            {
                return quickSlotItem;
            }
        }

        return null;
    }

    public void UseKeyToOpenDoor()
    {
        if (isPlayerInTrigger && !isDoorMoving && !hasDoorOpened)
        {
            StartCoroutine(MoveDoor());
        }
    }

    private IEnumerator HideNoKeyTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NoKeyText.gameObject.SetActive(false);
    }

    private IEnumerator MoveDoor()
    {
        isDoorMoving = true;
        hasDoorOpened = true;

        Vector3 startPos = DoorPrefab.transform.position;
        Vector3 endPos = startPos + Vector3.forward * moveDistance;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            DoorPrefab.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        DoorPrefab.transform.position = endPos;
        isDoorMoving = false;
    }
}
