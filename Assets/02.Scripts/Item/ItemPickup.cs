using Photon.Pun;
using UnityEngine;

public class ItemPickup : MonoBehaviourPunCallbacks
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            Debug.Log("충돌2");

            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(item);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
