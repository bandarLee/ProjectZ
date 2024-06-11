using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun; // 포톤 네임스페이스 추가

public class GoToSubwayEntrance : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("도시 -> 지하철");
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                // 자신만 씬을 로드하도록 RPC 호출
                photonView.RPC("LoadSubwayScene", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void LoadSubwayScene()
    {
            PhotonNetwork.LoadLevel("SubwayScene");
    }
}
