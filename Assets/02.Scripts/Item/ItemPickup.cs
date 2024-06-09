using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviourPunCallbacks
{
    public Item item;
    private PhotonView photonView_ItemPickUp;

    private void Awake()
    {
        photonView_ItemPickUp = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Character>().PhotonView.IsMine)
        {
            Debug.Log("충돌2");

            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && inventory.gameObject.GetComponent<Character>().PhotonView.IsMine)
            {
                inventory.AddItem(item);

                // 소유권을 마스터 클라이언트로 전송
                photonView_ItemPickUp.TransferOwnership(PhotonNetwork.MasterClient);

                // 약간의 지연 후 제거
                StartCoroutine(DestroyAfterOwnershipTransfer());
            }
        }
    }

    private IEnumerator DestroyAfterOwnershipTransfer()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 지연

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            photonView_ItemPickUp.RPC("RequestMasterDestroy", RpcTarget.MasterClient, photonView_ItemPickUp.ViewID);
        }
    }

    [PunRPC]
    public void RequestMasterDestroy(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
        }
    }
}
