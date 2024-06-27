using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public string itemEffect;
    public string itemDescription;
    public string uniqueId;
}

public enum ItemType
{
    Default,
    Food,
    Weapon,
    Heal,
    Mental,
    ETC,
    Consumable,
    Gun,
    StatBook,
    Special
}
