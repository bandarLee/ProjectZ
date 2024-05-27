using System.Collections;
using UnityEngine;
using Photon.Pun;


public class BoxInteract : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WaitForInteraction(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            inventoryManager.CloseAllInventories();
        }
    }

    private IEnumerator WaitForInteraction(Collider player)
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                var boxInventory = GetComponent<BoxInventory>();
                if (boxInventory != null)
                {
                    inventoryManager.OpenBoxInventory(boxInventory);
                    inventoryManager.boxInventoryUI.boxinventoryUIobject.SetActive(true);

                }
                break;
            }
            yield return null;
        }
    }
}
