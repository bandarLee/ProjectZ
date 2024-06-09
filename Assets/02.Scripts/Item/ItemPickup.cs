using Photon.Pun;
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

                photonView_ItemPickUp.RPC(nameof(RequestOwnerDestroy), photonView_ItemPickUp.Owner, photonView_ItemPickUp.ViewID);
            }
        }
    }

    [PunRPC]
    private void RequestOwnerDestroy(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && targetView.IsMine)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
        }

    }
}
