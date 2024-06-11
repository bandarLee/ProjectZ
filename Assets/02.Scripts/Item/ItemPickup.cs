using Photon.Pun;
using UnityEngine;

public class ItemPickup : MonoBehaviourPunCallbacks
{
    public Item SpawnedItem;
    private PhotonView photonView_ItemPickUp;
    private bool isPickedUp = false;
    private void Awake()
    {
        photonView_ItemPickUp = GetComponent<PhotonView>();
    }

    public void InitializeItem(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.uniqueId))
        {
            Debug.LogWarning("InitializeItem: null or invalid item");
            return;
        }
        SpawnedItem = item;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Character>().PhotonView.IsMine)
        {


            if (Inventory.Instance.pv.IsMine)
            {
                Debug.Log(SpawnedItem.uniqueId);
                isPickedUp = true;

                Inventory.Instance.AddItem(SpawnedItem);

                photonView_ItemPickUp.RPC(nameof(RequestOwnerDestroy), photonView_ItemPickUp.Owner, photonView_ItemPickUp.ViewID);
                gameObject.SetActive(false);

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
