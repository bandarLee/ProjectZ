using UnityEngine;
using Photon.Pun;
using System.Collections;

public class BackToCityDoorTrigger : MonoBehaviourPunCallbacks
{
    public GameObject inventoryObject;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                StartCoroutine(BackToCityWaitForSeconds());
            }
        }
    }

    private IEnumerator BackToCityWaitForSeconds()
    {
        yield return new WaitForSeconds(2f);
        inventoryObject = FindObjectOfType<SubwayRoomHandler>().gameObject;

        if (inventoryObject != null)
        {
            SubwayRoomHandler subwayHandler = inventoryObject.GetComponent<SubwayRoomHandler>();
            if (subwayHandler != null)
            {
                subwayHandler.isTryingToJoinSubway = false;
                subwayHandler.isTryingToJoinCity = true;
                subwayHandler.InitiateCityRoomTransition();
            }
            else
            {
                Debug.LogError("SubwayRoomHandler component not found on the inventoryObject.");
            }
        }
        else
        {
            Debug.LogError("InventoryObject reference is not set.");
        }
    }
}
