using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SubwaySceneManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; 
    public Transform spawnPoint;

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

        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = spawnPoint.rotation;

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation, 0);
    }
}
