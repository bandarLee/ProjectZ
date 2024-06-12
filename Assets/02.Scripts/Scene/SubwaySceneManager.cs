using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class SubwaySceneManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; 
    public Transform spawnPoint;
    public Transform readyPoint;
    private GameObject Player;
    public GameObject LoadingUI;
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



        Player = PhotonNetwork.Instantiate(playerPrefab.name, readyPoint.position, readyPoint.rotation, 0);
        StartCoroutine(LoadingCoroutine());
    }
    public IEnumerator LoadingCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Player.GetComponent<CharacterMoveAbilityTwo>().Teleport(spawnPoint.position);
        Player.transform.rotation = spawnPoint.rotation;
        LoadingUI.SetActive(false);

    }
}
