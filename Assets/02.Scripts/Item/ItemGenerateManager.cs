using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemGenerateManager : MonoBehaviour
{
    public ItemPresets itemPresetsContainer; // ItemPresets 스크립트를 참조합니다.
    public List<BoxInventory> allBoxInventories;

    private void Start()
    {
        if (itemPresetsContainer == null)
        {
            Debug.LogError("ItemPresetsContainer is not set.");
            return;
        }

        allBoxInventories = FindObjectsOfType<BoxInventory>().ToList();

        foreach (var box in allBoxInventories)
        {
            GenerateItemsForBox(box);
        }
    }

    public void GenerateItemsForBox(BoxInventory box)
    {
        int itemCount = GetItemCountForBoxType(box.boxType);

        for (int i = 0; i < itemCount; i++)
        {
            Item randomItem = GetRandomItem();
            if (randomItem != null)
            {
                box.AddItem(randomItem);
            }
            else
            {
                Debug.LogWarning("Item preset is empty or null. Cannot add item to box.");
            }
        }
    }

    private int GetItemCountForBoxType(BoxType boxType)
    {
        switch (boxType)
        {
            case BoxType.Small:
                return 2;
            case BoxType.Medium:
                return 3;
            case BoxType.Large:
                return 4;
            default:
                return 0;
        }
    }

    private Item GetRandomItem()
    {
        // 아이템을 랜덤하게 가져오는 로직
        Item randomItem = itemPresetsContainer.GenerateRandomItem((ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length));
        return randomItem;
    }
}
