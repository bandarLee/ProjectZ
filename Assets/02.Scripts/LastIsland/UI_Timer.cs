using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using DG.Tweening;

public class UI_Timer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;
    public Image UI_HappyEnding;

    private float timeRemaining;
    private bool isTimerRunning = false;

    private void Start()
    {
        UI_HappyEnding.gameObject.SetActive(false);
    }

    public void StartTimer(float duration)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timeRemaining = duration;
            isTimerRunning = true;
            StartCoroutine(TimerCoroutine());
            Character.LocalPlayerInstance.PhotonView.RPC("SyncTimer", RpcTarget.Others, timeRemaining);
        }
    }

    [PunRPC]
    public void SyncTimer(float time)
    {
        timeRemaining = time;
        isTimerRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (isTimerRunning && timeRemaining > 0)
        {
            yield return new WaitForSeconds(1.0f);
            if (PhotonNetwork.IsMasterClient)
            {
                timeRemaining -= 1.0f;
                Character.LocalPlayerInstance.PhotonView.RPC("UpdateTimer", RpcTarget.Others, timeRemaining);
            }
            UpdateTimerUI();
        }

        if (timeRemaining <= 0)
        {
            isTimerRunning = false;
            TimerEnded();
        }
    }

    [PunRPC]
    public void UpdateTimer(float time)
    {
        timeRemaining = time;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        int seconds = Mathf.FloorToInt(timeRemaining);
        timerText.text = $"{seconds:00}"; // 초만 표시
    }

    // 타이머가 끝남
    private void TimerEnded()
    {
        timerText.text = "00";
        StartCoroutine(HappyEndingFadeImage());
    }

    private IEnumerator HappyEndingFadeImage()
    {
        UI_HappyEnding.gameObject.SetActive(true);
        UI_HappyEnding.color = new Color(1, 1, 1, 0); // 초기 색상 흰색 투명
        Debug.Log("1");

        UI_HappyEnding.DOFade(1, 1.5f); // 투명도 조정
        Debug.Log("1");
        yield return new WaitForSeconds(1.5f);
        Debug.Log("1");
        //LoadHappyEndingScene();
    }

    /* private void LoadHappyEndingScene()
     {
         PhotonNetwork.LoadLevel("");
     }*/
}