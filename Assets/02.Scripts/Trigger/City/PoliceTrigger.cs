using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PoliceTrigger : MonoBehaviour
{
    public TextMeshProUGUI OpenText;
    public TextMeshProUGUI NoKeyText;
    //public TextMeshProUGUI BackToCityText;

    public GameObject DoorPrefab;
    public float moveDistance = 5f; // 이동 거리
    public float moveDuration = 2f; // 이동 시간

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
        //BackToCityText.gameObject.SetActive(false);

        playerInventory = FindObjectOfType<Inventory>();
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
            // 아이템 사용 매니저를 통해 키 아이템을 사용
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
            if (item.itemType == ItemType.ETC && item.itemName == "열쇠")
            {
                return item;
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
