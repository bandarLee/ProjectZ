using UnityEngine;
using UnityEngine.EventSystems;

public class ChildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI_QuickSlotResister parentQuickSlot;
    [HideInInspector]
    public bool isPointerOver = false;

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



 
}
