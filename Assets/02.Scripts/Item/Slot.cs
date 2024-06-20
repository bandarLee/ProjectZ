using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Image normalIcon;
    public Image highlightedIcon;
    public Image pressedIcon;
    public TMP_Text quantityText;

    public InventoryUI inventoryUI;
    public BoxInventoryUI boxInventoryUI;

    public Item slotitem;

    public enum SlotType
    {
        PlayerInventorySlot,
        BoxInventorySlot
    }
    public SlotType slotType = SlotType.BoxInventorySlot;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
            Debug.Log("¿ìÅ¬¸¯");
        }
    
    }

    public void OnRightClick()
    {
        switch (slotType)
        {
            case SlotType.PlayerInventorySlot:
                if (boxInventoryUI != null)
                {

                    boxInventoryUI.TransferToBoxInventory();
                }
                break;
            case SlotType.BoxInventorySlot:
                if (boxInventoryUI != null)
                {
                    boxInventoryUI.TransferToPlayerInventorySlot(slotitem);
                }
                break;
        }



    }
}