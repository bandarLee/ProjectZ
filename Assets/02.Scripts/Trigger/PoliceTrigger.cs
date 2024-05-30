using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoliceTrigger : MonoBehaviour
{
    public TextMeshProUGUI OpenText;
    public TextMeshProUGUI NoKeyText;

    public GameObject DoorPrefab;
    public float moveDistance = 5f; // ¿Ãµø ∞≈∏Æ
    public float moveDuration = 2f; // ¿Ãµø Ω√∞£

    private bool isPlayerInTrigger = false;
    private bool isDoorMoving = false; 
    private bool hasDoorOpened = false; 

    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;

    private Coroutine noKeyTextCoroutine;

    private void Start()
    {
        OpenText.gameObject.SetActive(false);
        NoKeyText.gameObject.SetActive(false);

        playerInventory = FindObjectOfType<Inventory>();
        quickSlotManager = FindObjectOfType<QuickSlotManager>();

        if (playerInventory == null)
        {
            Debug.LogError("Inventory not found");
        }

        if (quickSlotManager == null)
        {
            Debug.LogError("QuickSlotManager not found");
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
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !isDoorMoving && !hasDoorOpened)
        {
            if (HasKeyItem(true))
            {
                StartCoroutine(MoveDoor());
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

    private bool HasKeyItem(bool removeAfterUse = false)
    {
        // ¿Œ∫•ø°º≠ø≠ºË √£±‚
        foreach (var item in playerInventory.items.Values)
        {
            if (item.itemType == ItemType.ETC && item.itemName == "ø≠ºË")
            {
                if (removeAfterUse)
                {
                    playerInventory.RemoveItem(item.itemName);
                }
                return true;
            }
        }

        // ƒ¸ΩΩ∑‘ø°º≠ ø≠ºË √£±‚
        for (int i = 0; i < quickSlotManager.quickSlotItems.Length; i++)
        {
            var quickSlotItem = quickSlotManager.quickSlotItems[i];
            if (quickSlotItem != null && quickSlotItem.itemType == ItemType.ETC && quickSlotItem.itemName == "ø≠ºË")
            {
                if (removeAfterUse)
                {
                    quickSlotManager.quickSlotItems[i] = null;
                    quickSlotManager.quickSlotImages[i].sprite = null;
                    quickSlotManager.quickSlotImages[i].gameObject.SetActive(false);
                    quickSlotManager.quickSlotQuantities[i].text = "";
                }
                return true;
            }
        }

        return false;
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
