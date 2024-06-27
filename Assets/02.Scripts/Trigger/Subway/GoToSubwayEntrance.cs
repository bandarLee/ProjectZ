using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GoToSubwayEntrance : MonoBehaviourPunCallbacks
{
    public GameObject inventoryObject;
    public GameObject LoadingImage;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                LoadingImage.SetActive(true);

                TriggerSubwayTransition();
            }
        }
    }

    private void TriggerSubwayTransition()
    {
        inventoryObject = FindObjectOfType<SubwayRoomHandler>().gameObject;

        if (inventoryObject != null)
        {
            SubwayRoomHandler subwayHandler = inventoryObject.GetComponent<SubwayRoomHandler>();
            if (subwayHandler != null)
            {
                subwayHandler.isTryingToJoinSubway = true;
                CharacterInfo.Instance._isGameStart = false;
                string currentRoomName = PhotonNetwork.CurrentRoom?.Name;
                if (string.IsNullOrEmpty(currentRoomName))
                {
                    Debug.LogError("Not in a room or not connected.");
                    return;
                }
                subwayHandler.InitiateSubwayRoomTransition(currentRoomName);
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
