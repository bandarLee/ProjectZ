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
            Debug.LogError("플레이어 프리팹이 지정되지 않았습니다.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("스폰 포인트가 지정되지 않았습니다.");
            return;
        }

        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = spawnPoint.rotation;

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation, 0);
    }
}
