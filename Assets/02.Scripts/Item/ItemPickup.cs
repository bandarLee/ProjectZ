using Photon.Pun;
using UnityEngine;

public class ItemPickup : MonoBehaviourPunCallbacks
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�浹");
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            Debug.Log("�浹2");

            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(item);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
