using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class CharacterItemAbility : CharacterAbility
{
    public PhotonView PhotonView { get; private set; }

    public GameObject[] ItemObject;
    private Dictionary<string, int> itemIndexMap;
    private int _activeItemIndex = -1;

    protected override void Awake()
    {
        base.Awake();
        PhotonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InitializeItemIndexMap();
        DeactivateAllItems();
    }

    private void InitializeItemIndexMap()
    {
        itemIndexMap = new Dictionary<string, int>();

        for (int i = 0; i < ItemObject.Length; i++)
        {
            itemIndexMap[ItemObject[i].name] = i;  
        }
    }

    [PunRPC]
    public void DropItemPrefab(string itemName, Vector3 position, Vector3 forward)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("ItemPrefabs/" + itemName);
        if (itemPrefab != null)
        {
            Vector3 dropPosition = position + forward * 2f + Vector3.up * 1.5f;
            GameObject droppedItem = PhotonNetwork.Instantiate("ItemPrefabs/" + itemName, dropPosition, Quaternion.identity, 0);
            Debug.Log("Item Prefab Instantiated: " + itemName);

            ItemPickup itemPickup = droppedItem.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                ItemPreset preset = FindObjectOfType<ItemPresets>().GetItemPreset(itemName);
                if (preset != null)
                {
                    Item newItem = new Item
                    {
                        itemName = itemName,
                        icon = preset.icon,
                        itemType = preset.itemType,
                        itemEffect = preset.itemEffect,
                        itemDescription = preset.itemDescription,
                        uniqueId = System.Guid.NewGuid().ToString()
                    };
                    itemPickup.InitializeItem(newItem); 
                }
                else
                {
                }
            }
        }
        else
        {
            Debug.LogWarning("아이템 프리팹을 찾을 수 없습니다: " + itemName);
        }
    }

    [PunRPC]
    public void ItemActiveRPC(int ItemNumber)
    {
        foreach (GameObject item in ItemObject)
        {
            item.SetActive(false);
        }
        StartCoroutine(ItemActiveAfterDelay(ItemNumber, 0.11f));
        _activeItemIndex = ItemNumber;
    }
    private IEnumerator ItemActiveAfterDelay(int ItemNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ItemNumber >= 0 && ItemNumber < ItemObject.Length)
        {
            ItemObject[ItemNumber].SetActive(true);
        }
    }
    public void ItemActive(string itemName)
    {
        if (itemIndexMap.ContainsKey(itemName))
        {
            int itemIndex = itemIndexMap[itemName];
            if (Owner.PhotonView.IsMine)
            {
                Owner.PhotonView.RPC(nameof(ItemActiveRPC), RpcTarget.All, itemIndex);
            }
            ItemActiveRPC(itemIndex);
        }
        else
        {
            Debug.LogWarning("Item not found: " + itemName);
        }
    }

    [PunRPC]
    public void DeactivateAllItemsRPC()
    {
        foreach (GameObject item in ItemObject)
        {
            item.SetActive(false);
        }
        _activeItemIndex = -1;

    }

    public void DeactivateAllItems()
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(DeactivateAllItemsRPC), RpcTarget.All);
        }
        DeactivateAllItemsRPC();
    }

    /***/

    public void UnUsingHandAnimation()
    {
        StartCoroutine(UnUsingHandAnimationAfterSeconds());
        

    }
    private IEnumerator UnUsingHandAnimationAfterSeconds()
    {

        yield return new WaitForSeconds(0.1f);
        Owner.PhotonView.RPC(nameof(UnUsingHandAnimationRPC), RpcTarget.All);

    }


    [PunRPC]
    public void UnUsingHandAnimationRPC()
    {
        Owner._animator.SetBool("isPullOut", false);

        Owner._animator.SetBool("RePullOut", true);
        Owner._animator.SetInteger("UsingHand", 0);


    }

    public void OneHandAnimation()
    {

        StartCoroutine(OneHandAnimationAfterSeconds());
    }
    private IEnumerator OneHandAnimationAfterSeconds()
    {

        yield return new WaitForSeconds(0.6f);
        Owner.PhotonView.RPC(nameof(OneHandAnimationRPC), RpcTarget.All);

    }
    public void TwoHandAnimation()
    {
        StartCoroutine(TwoHandAnimationAfterSeconds());

    }
    private IEnumerator TwoHandAnimationAfterSeconds()
    {

        yield return new WaitForSeconds(1f);
        Owner.PhotonView.RPC(nameof(TwoHandAnimationRPC), RpcTarget.All);

    }

    [PunRPC]
    public void OneHandAnimationRPC()
    {
        Owner._animator.SetBool("isPullOut", true);
        Owner._animator.SetBool("RePullOut", false);

        Owner._animator.SetInteger("UsingHand", 1);
    }

    [PunRPC]
    public void TwoHandAnimationRPC()
    {

        Owner._animator.SetBool("isPullOut", true);
        Owner._animator.SetBool("RePullOut", false);

        Owner._animator.SetInteger("UsingHand", 2);

    }
}