using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastIsLandSceneManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    public Transform readyPoint;
    private GameObject Player;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }

    }
    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("�÷��̾� �������� �������� �ʾҽ��ϴ�.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("���� ����Ʈ�� �������� �ʾҽ��ϴ�.");
            return;
        }



        Player = PhotonNetwork.Instantiate(playerPrefab.name, readyPoint.position, readyPoint.rotation, 0);
    }

}
