using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemPreset
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public string itemEffect;
    public string itemDescription;
}

public class ItemPresets : MonoBehaviour
{
    public ItemPreset[] presets;

    public Item GenerateRandomItem(ItemType type)
    {
        var possibleItems = presets.Where(p => p.itemType == type).ToArray();
        if (possibleItems.Length == 0) return null;

        var preset = possibleItems[Random.Range(0, possibleItems.Length)];
        return new Item
        {
            itemName = preset.itemName,
            icon = preset.icon,
            itemType = preset.itemType,
            itemEffect = preset.itemEffect,
            itemDescription = preset.itemDescription,
            uniqueId = System.Guid.NewGuid().ToString()
        };
    }
}
