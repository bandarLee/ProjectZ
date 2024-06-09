using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BoatInteract : MonoBehaviourPunCallbacks
{
    private bool isPlayerInRange = false;
    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            photonView.RPC("StartControllingBoat", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    private void StartControllingBoat(int playerViewID)
    {
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        BoatController boatController = GetComponent<BoatController>();
        boatController.StartControlling(player);
    }
}
