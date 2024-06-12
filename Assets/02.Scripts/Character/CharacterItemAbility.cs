using UnityEngine;
using Photon.Pun;
using System.Collections;

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
    //

   
    public void UnUsingHandAnimation()
    {
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

        yield return new WaitForSeconds(0.5f);
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