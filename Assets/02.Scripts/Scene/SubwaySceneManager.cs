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
            Debug.LogError("�÷��̾� �������� �������� �ʾҽ��ϴ�.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("���� ����Ʈ�� �������� �ʾҽ��ϴ�.");
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
