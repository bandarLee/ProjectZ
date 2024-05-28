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
            GameObject droppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);
            ItemPickup itemPickup = droppedItem.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.item = new Item
                {
                    itemName = itemName,
                    icon = Resources.Load<Sprite>("ItemIcons/" + itemName),
                    itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemName),
                    itemEffect = "",
                    itemDescription = "",
                    uniqueId = System.Guid.NewGuid().ToString()
                };
            }

        }
   
    }
}