using UnityEngine;
using UnityEngine.UI;

public class QuickSlotManager : MonoBehaviour
{
    public Image[] quickSlotImages; 
    public Item[] quickSlotItems;  

    private void Start()
    {
        quickSlotItems = new Item[quickSlotImages.Length];
    }

    public void RegisterItemToQuickSlot(int slotIndex, Item item)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Length) return;
        quickSlotItems[slotIndex] = item;
        quickSlotImages[slotIndex].sprite = item.icon;
        quickSlotImages[slotIndex].gameObject.SetActive(true);
    }

    public void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Length || quickSlotItems[slotIndex] == null) return;

        // ������ ��� ����
        Debug.Log("Using item: " + quickSlotItems[slotIndex].itemName);
        // ���⼭ �������� ȿ���� �����ϴ� �ڵ带 �߰��ϼ���.
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseQuickSlotItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseQuickSlotItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseQuickSlotItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseQuickSlotItem(3);
        }
    }
}
