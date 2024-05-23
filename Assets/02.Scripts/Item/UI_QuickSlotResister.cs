using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickSlotResister : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject targetObject;
    public bool isPointerOver = false;
    private QuickSlotManager quickSlotManager;

    void Start()
    {
        targetObject.SetActive(false);
        quickSlotManager = FindObjectOfType<QuickSlotManager>();

        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            var childButton = targetObject.transform.GetChild(i).GetComponent<ChildButton>();
            if (childButton != null)
            {
                childButton.slotIndex = i;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        StartCoroutine(DeactivateAfterDelay());
    }

    public IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        if (!isPointerOver && !IsPointerOverChild())
        {
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }
        }
    }

    public bool IsPointerOverChild()
    {
        foreach (Transform child in targetObject.transform)
        {
            if (child.GetComponent<ChildButton>() != null && child.GetComponent<ChildButton>().isPointerOver)
            {
                return true;
            }
        }
        return false;
    }

    public void RegisterItemToQuickSlot(int slotIndex)
    {
        if (quickSlotManager != null && InventoryUI.currentSelectedItem != null)
        {
            quickSlotManager.RegisterItemToQuickSlot(slotIndex, InventoryUI.currentSelectedItem);
        }
    }
}
