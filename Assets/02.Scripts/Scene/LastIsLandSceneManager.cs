using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastIsLandSceneManager : MonoBehaviour
{
    public Transform spawnPoint;
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


        if (spawnPoint == null)
        {
            Debug.LogError("���� ����Ʈ�� �������� �ʾҽ��ϴ�.");
            return;
        }
        Player = PhotonNetwork.Instantiate("Character_Female_rigid_collid", spawnPoint.position, spawnPoint.rotation);

    }

}
