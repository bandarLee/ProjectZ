using System.Collections.Generic;
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
    public string iconPath;
}

public class ItemPresets : MonoBehaviour
{
    public ItemPreset[] presets;
    private Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();

    private void Start()
    {
        foreach (var preset in presets)
        {
            if (!iconCache.ContainsKey(preset.iconPath))
            {
                Sprite icon = Resources.Load<Sprite>(preset.iconPath);
                if (icon != null)
                {
                    iconCache[preset.iconPath] = icon;
                }
                else
                {
                    Debug.LogError($"Failed to load icon at path: {preset.iconPath}");
                }
            }
        }
    }

    public Item GenerateRandomItem(ItemType type)
    {
        var possibleItems = presets.Where(p => p.itemType == type).ToArray();
        if (possibleItems.Length == 0) return null;

        var preset = possibleItems[Random.Range(0, possibleItems.Length)];
        return new Item
        {
            itemName = preset.itemName,
            iconPath = preset.iconPath,
            icon = iconCache.ContainsKey(preset.iconPath) ? iconCache[preset.iconPath] : null,
            itemType = preset.itemType,
            itemEffect = preset.itemEffect,
            itemDescription = preset.itemDescription,
            uniqueId = System.Guid.NewGuid().ToString()
        };
    }
}
