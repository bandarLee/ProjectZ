using UnityEngine;
using Photon.Pun;

public class CityRespawnPoint : MonoBehaviour
{
    public GameObject[] RespawnPoints;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 randomSpawnPosition = RespawnPoints[Random.Range(0, RespawnPoints.Length)].transform.position;
            GameManager.Instance.SetSpawnPoint(randomSpawnPosition);
        }
    }
}
