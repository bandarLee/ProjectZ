using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BoatInteract : MonoBehaviourPunCallbacks
{
    private bool isPlayerInRange = false;
    public bool isBoatControlling = false;
    private GameObject player;

    private void OnTriggerStay(Collider other)
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
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isBoatControlling)
        {
            photonView.RPC("StartControllingBoat", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
            isBoatControlling = true;
        }
        else if (isBoatControlling && Input.GetKeyDown(KeyCode.E) )
        {
            photonView.RPC("StopControllingBoat", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
            isBoatControlling = false;

        }
    }

    [PunRPC]
    private void StartControllingBoat(int playerViewID)
    {
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        BoatController boatController = GetComponent<BoatController>();
        boatController.StartControlling(player);
    }
    [PunRPC]
    private void StopControllingBoat(int playerViewID)
    {
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        BoatController boatController = GetComponent<BoatController>();
        boatController.StopControlling(player);
    }
}
