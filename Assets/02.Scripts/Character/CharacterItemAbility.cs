using UnityEngine;
using Photon.Pun;

public class CharacterItemAbility : CharacterAbility
{
    public PhotonView PhotonView { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        PhotonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void DropItemPrefab(string itemName, Vector3 position, Vector3 forward)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("ItemPrefabs/" + itemName);
        if (itemPrefab != null)
        {
            Vector3 dropPosition = position + forward * 2f + Vector3.up * 1.5f;
            GameObject droppedItem = PhotonNetwork.Instantiate("ItemPrefabs/" + itemName, dropPosition, Quaternion.identity, 0); 
            ItemPickup itemPickup = droppedItem.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                ItemPreset preset = FindObjectOfType<ItemPresets>().GetItemPreset(itemName);
                if (preset != null)
                {
                    itemPickup.item = new Item
                    {
                        itemName = itemName,
                        icon = preset.icon,
                        itemType = preset.itemType,
                        itemEffect = preset.itemEffect,
                        itemDescription = preset.itemDescription,
                        uniqueId = System.Guid.NewGuid().ToString()
                    };
                }
                else
                {
                    Debug.LogWarning("아이템 프리셋을 찾을 수 없습니다: " + itemName);
                }
            }
        }
        else
        {
            Debug.LogWarning("아이템 프리팹을 찾을 수 없습니다: " + itemName);
        }
    }

}