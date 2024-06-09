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
        Debug.Log("�浹");
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Character>().PhotonView.IsMine)
        {
            Debug.Log("�浹2");

            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && inventory.gameObject.GetComponent<Character>().PhotonView.IsMine)
            {
                inventory.AddItem(item);

                // �������� ������ Ŭ���̾�Ʈ�� ����
                photonView_ItemPickUp.TransferOwnership(PhotonNetwork.MasterClient);

                // �ణ�� ���� �� ����
                StartCoroutine(DestroyAfterOwnershipTransfer());
            }
        }
    }

    private IEnumerator DestroyAfterOwnershipTransfer()
    {
        yield return new WaitForSeconds(0.1f); // 0.1�� ����

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
