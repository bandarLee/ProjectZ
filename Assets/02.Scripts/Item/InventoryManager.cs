using System.Collections;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI playerInventoryUI;
    public BoxInventoryUI boxInventoryUI;
    public CharacterRotateAbility characterRotateAbility;
    public void Start()
    {
        characterRotateAbility = Character.LocalPlayerInstance._characterRotateAbility;
        CloseAllInventories();
       /* StartCoroutine(AssignCharacterRotateAbilityAfterDelay(1.5f));*/
    }


    public void CloseAllInventories()
    {
        playerInventoryUI.CloseInventory();
        boxInventoryUI.CloseInventory();
        CloseItemInfo();
        characterRotateAbility.SetMouseLock(true);
        playerInventoryUI.quickSlotManager.ItemUseLock = false;

    }

    public void OpenBoxInventory(BoxInventory boxInventory)
    {
        boxInventoryUI.SetBoxInventory(boxInventory);
        playerInventoryUI.inventoryObject.SetActive(true);
        boxInventoryUI.inventoryObject.SetActive(true);
        characterRotateAbility.SetMouseLock(false);
        playerInventoryUI.quickSlotManager.ItemUseLock = true;


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
            boxInventoryUI.CloseInventory();


            characterRotateAbility.SetMouseLock(true);
            playerInventoryUI.quickSlotManager.ItemUseLock = false;


        }
        else
        {
            playerInventoryUI.inventoryObject.SetActive(true);
            playerInventoryUI.quickSlotManager.ItemUseLock = true;

            characterRotateAbility.SetMouseLock(false);
        }
    }
}