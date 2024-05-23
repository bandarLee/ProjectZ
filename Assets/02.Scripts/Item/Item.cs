using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    public string itemEffect;
    public string itemDescription;
}

public enum ItemType
{
    Food,
    Weapon
}
