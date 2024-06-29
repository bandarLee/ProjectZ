using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoxItemPair
{
    public BoxInventory targetBox;
    public string itemNameToAdd;
}

public class GenerateSpecificItem : MonoBehaviour
{
    public ItemPresets itemPresetsContainer;
    public List<BoxItemPair> boxItemPairs;  // ���� �ڽ��� ������ ���� ����

    private void Start()
    {
        if (itemPresetsContainer == null)
        {
            itemPresetsContainer = FindObjectOfType<ItemPresets>();
        }

        // �� �ڽ��� ������ ������ �߰�
        foreach (var pair in boxItemPairs)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AddSpecificItemToBox(pair.targetBox, pair.itemNameToAdd);

            }

        }
    }

    public void AddSpecificItemToBox(BoxInventory box, string itemName)
    {
        if (box == null)
        {
            Debug.LogWarning("Ÿ�� �ڽ��� �������� �ʾҽ��ϴ�.");
            return;
        }

        // Ư�� ������ ã��
        ItemPreset itemPreset = itemPresetsContainer.GetItemPreset(itemName);
        if (itemPreset == null)
        {
            Debug.LogWarning("�ش� �̸��� �������� ã�� �� �����ϴ�: " + itemName);
            return;
        }

        // ������ ����
        Item specificItem = new Item
        {
            itemName = itemPreset.itemName,
            icon = itemPreset.icon,
            itemType = itemPreset.itemType,
            itemEffect = itemPreset.itemEffect,
            itemDescription = itemPreset.itemDescription,
            uniqueId = System.Guid.NewGuid().ToString()
        };

        // ������ �߰�
        box.BoxAddItem(specificItem);
        Debug.Log("������ �߰� �Ϸ�: " + itemName + " to " + box.name);
    }
}
