using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI playerInventoryUI;
    public BoxInventoryUI boxInventoryUI;
    public CharacterRotateAbility characterRotateAbility;

    public void Start()
    {
        characterRotateAbility = null; 
        CloseAllInventories();
        StartCoroutine(AssignCharacterRotateAbilityAfterDelay(1f));
    }

    public void CloseAllInventories()
    {
        playerInventoryUI.CloseInventory();
        boxInventoryUI.CloseInventory();
        CloseItemInfo();
        if (characterRotateAbility != null)
        {
            characterRotateAbility.SetMouseLock(true);
        }
        playerInventoryUI.quickSlotManager.ItemUseLock = false;
    }

    public void OpenBoxInventory(BoxInventory boxInventory)
    {
        boxInventoryUI.SetBoxInventory(boxInventory);
        playerInventoryUI.inventoryObject.SetActive(true);
        boxInventoryUI.inventoryObject.SetActive(true);
        if (characterRotateAbility != null)
        {
            characterRotateAbility.SetMouseLock(false);
        }
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

            if (characterRotateAbility != null)
            {
                characterRotateAbility.SetMouseLock(true);
            }
            playerInventoryUI.quickSlotManager.ItemUseLock = false;
        }
        else
        {
            playerInventoryUI.inventoryObject.SetActive(true);
            playerInventoryUI.quickSlotManager.ItemUseLock = true;

            if (characterRotateAbility != null)
            {
                characterRotateAbility.SetMouseLock(false);
            }
        }
    }

    private IEnumerator AssignCharacterRotateAbilityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        characterRotateAbility = Character.LocalPlayerInstance._characterRotateAbility;

        if (characterRotateAbility == null)
        {
            Debug.LogError("CharacterRotateAbility를 찾을 수 없습니다.");
        }
    }
}
