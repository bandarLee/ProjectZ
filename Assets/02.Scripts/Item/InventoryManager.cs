using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI playerInventoryUI;
    public BoxInventoryUI boxInventoryUI;
    public void Start()
    {
        CloseAllInventories();
    }
    public void CloseAllInventories()
    {
        playerInventoryUI.CloseInventory();
        boxInventoryUI.CloseInventory();
        CloseItemInfo();

    }

    public void OpenBoxInventory(BoxInventory boxInventory)
    {
        boxInventoryUI.SetBoxInventory(boxInventory);
        playerInventoryUI.inventoryObject.SetActive(true);
        boxInventoryUI.inventoryObject.SetActive(true);
    }
    public void CloseItemInfo()
    {
        playerInventoryUI.CloseItemInfo();
        boxInventoryUI.CloseItemInfo();
    }

    public void UpdateAllInventories()
    {
        playerInventoryUI.UpdateInventoryUI();
        boxInventoryUI.UpdateInventoryUI();
    }
    public void TogglePlayerInventory()
    {
        if (playerInventoryUI.inventoryObject.activeSelf)
        {
            playerInventoryUI.CloseInventory();
        }
        else
        {
            playerInventoryUI.inventoryObject.SetActive(true);
        }
    }
}
