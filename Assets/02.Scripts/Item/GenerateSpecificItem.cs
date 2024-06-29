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
    public List<BoxItemPair> boxItemPairs;  // 여러 박스와 아이템 쌍을 관리

    private void Start()
    {
        if (itemPresetsContainer == null)
        {
            itemPresetsContainer = FindObjectOfType<ItemPresets>();
        }

        // 각 박스에 지정된 아이템 추가
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
            Debug.LogWarning("타겟 박스가 설정되지 않았습니다.");
            return;
        }

        // 특정 아이템 찾기
        ItemPreset itemPreset = itemPresetsContainer.GetItemPreset(itemName);
        if (itemPreset == null)
        {
            Debug.LogWarning("해당 이름의 아이템을 찾을 수 없습니다: " + itemName);
            return;
        }

        // 아이템 생성
        Item specificItem = new Item
        {
            itemName = itemPreset.itemName,
            icon = itemPreset.icon,
            itemType = itemPreset.itemType,
            itemEffect = itemPreset.itemEffect,
            itemDescription = itemPreset.itemDescription,
            uniqueId = System.Guid.NewGuid().ToString()
        };

        // 아이템 추가
        box.BoxAddItem(specificItem);
        Debug.Log("아이템 추가 완료: " + itemName + " to " + box.name);
    }
}
