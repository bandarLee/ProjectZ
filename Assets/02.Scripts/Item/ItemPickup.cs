using Photon.Pun;
using UnityEngine;

public class ItemPickup : MonoBehaviourPunCallbacks
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�浹");
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Character>().PhotonView.IsMine)
        {
            Debug.Log("�浹2");

            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && inventory.gameObject.GetComponent<Character>().PhotonView.IsMine)
            {
                inventory.AddItem(item);
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
                else
                {
                    photonView.RPC("RequestMasterDestroy", RpcTarget.MasterClient, photonView.ViewID);
                }
            }
        }
    }
    [PunRPC]
    public void RequestMasterDestroy(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && targetView.IsMine)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
        }
    }
}
