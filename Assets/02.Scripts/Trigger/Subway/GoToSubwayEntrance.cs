using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun; 

public class GoToSubwayEntrance : MonoBehaviourPunCallbacks
{
    public GameObject UILoading;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("도시 -> 지하철");
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                UILoading.SetActive(true);
                LoadSubwayScene();
            }
        }
    }

    private void LoadSubwayScene()
    {
            PhotonNetwork.LoadLevel("SubwayScene");
    }
}
