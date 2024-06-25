using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickSlotResister : MonoBehaviour
{
    public bool isPointerOver = false;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;

    void Start()
    {
        quickSlotManager = FindObjectOfType<QuickSlotManager>();
        inventoryUI = FindObjectOfType<InventoryUI>(); 
    }



    public void RegisterItemToQuickSlot(int slotIndex)
    {
        if (quickSlotManager != null && inventoryUI.currentSelectedItem != null)
        {
            quickSlotManager.RegisterItemToQuickSlot(slotIndex, inventoryUI.currentSelectedItem);
        }
    }
}
