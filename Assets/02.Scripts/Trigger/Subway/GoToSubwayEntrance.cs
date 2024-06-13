using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun; // ���� ���ӽ����̽� �߰�

public class GoToSubwayEntrance : MonoBehaviourPunCallbacks
{
    public GameObject UILoading;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("���� -> ����ö");
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
