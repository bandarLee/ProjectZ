using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI playerInventoryUI;
    public BoxInventoryUI boxInventoryUI;
    public CharacterRotateAbility characterRotateAbility;

    public void Start()
    {
        CloseAllInventories();

    }
    public void CloseAllInventories()
    {
        Debug.Log("err");
        playerInventoryUI.CloseInventory();
        boxInventoryUI.CloseInventory();
        CloseItemInfo();
        characterRotateAbility.SetMouseLock(true);

    }

    public void OpenBoxInventory(BoxInventory boxInventory)
    {
        boxInventoryUI.SetBoxInventory(boxInventory);
        playerInventoryUI.inventoryObject.SetActive(true);
        boxInventoryUI.inventoryObject.SetActive(true);
        characterRotateAbility.SetMouseLock(false);

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
            characterRotateAbility.SetMouseLock(true);

        }
        else
        {
            playerInventoryUI.inventoryObject.SetActive(true);
            characterRotateAbility.SetMouseLock(false);

        }
    }
}