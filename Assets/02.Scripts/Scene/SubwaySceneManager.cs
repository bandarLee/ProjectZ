using UnityEngine;
using UnityEngine.UI;

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
    public Image loadingBar;
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
        float elapsedTime = 0f;
        float totalWaitTime = 2f; 

        float[] times = { 0.8f, 1.2f, 1.5f, 2f };
        float[] targets = { 0.5f, 0.7f, 0.85f, 1f };

        LoadingUI.SetActive(true);
        loadingBar.fillAmount = 0f; 

        int currentTargetIndex = 0;
        while (elapsedTime < totalWaitTime)
        {
            elapsedTime += Time.deltaTime;

            if (currentTargetIndex < times.Length && elapsedTime > times[currentTargetIndex])
            {
                currentTargetIndex++;
            }

            if (currentTargetIndex >= times.Length)
            {
                break;
            }

            float startTime = currentTargetIndex == 0 ? 0 : times[currentTargetIndex - 1];
            float endTime = times[currentTargetIndex];
            float startFill = currentTargetIndex == 0 ? 0 : targets[currentTargetIndex - 1];
            float endFill = targets[currentTargetIndex];

            float t = Mathf.InverseLerp(startTime, endTime, elapsedTime);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            loadingBar.fillAmount = Mathf.Lerp(startFill, endFill, smoothT);

            yield return null;
        }

        Player.GetComponent<CharacterMoveAbilityTwo>().Teleport(spawnPoint.position);
        Player.transform.rotation = spawnPoint.rotation;
        LoadingUI.SetActive(false);
    }
}
