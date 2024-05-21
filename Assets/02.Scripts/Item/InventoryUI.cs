using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject[] inventorySlots; 
    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (i < inventory.items.Count)
            {
                inventorySlots[i].GetComponent<Image>().sprite = inventory.items[i].icon;
                inventorySlots[i].SetActive(true);
            }
            else
            {
                inventorySlots[i].GetComponent<Image>().sprite = null;
                inventorySlots[i].SetActive(false);
            }
        }
    }
}
