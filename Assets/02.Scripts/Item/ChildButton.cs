using UnityEngine;
using UnityEngine.EventSystems;

public class ChildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UI_QuickSlotResister parentQuickSlot;
    [HideInInspector]
    public bool isPointerOver = false;
    public int slotIndex; 

    private void Start()
    {
        parentQuickSlot = GetComponentInParent<UI_QuickSlotResister>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        if (parentQuickSlot != null && parentQuickSlot.targetObject != null)
        {
            parentQuickSlot.targetObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        if (parentQuickSlot != null)
        {
            StartCoroutine(parentQuickSlot.DeactivateAfterDelay());
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RegisterToQuickSlot();
    }

    public void RegisterToQuickSlot()
    {
        if (parentQuickSlot != null)
        {
            parentQuickSlot.RegisterItemToQuickSlot(slotIndex);
        }
    }
}
