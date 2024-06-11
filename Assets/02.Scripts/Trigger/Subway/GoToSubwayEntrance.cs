using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun; // ���� ���ӽ����̽� �߰�

public class GoToSubwayEntrance : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("���� -> ����ö");
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                // �ڽŸ� ���� �ε��ϵ��� RPC ȣ��
                photonView.RPC("SubwayScene", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.UserId);
            }
        }
    }

    [PunRPC]
    private void LoadSubwayScene(string userId)
    {
        if (PhotonNetwork.LocalPlayer.UserId == userId)
        {
            SceneManager.LoadScene("SubwayScene");
        }
    }
}
