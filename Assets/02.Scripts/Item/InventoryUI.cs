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
        for (int i = 0; i < inventory.items.Count * 3; i++)
        {
            if (i < inventory.items.Count * 3)
            {
                inventorySlots[i].GetComponent<Image>().sprite = inventory.items[i/3].icon;
                inventorySlots[i].GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
            }
 
        }
    }
    public void UpdateItemInformation()
    {

    }
}
