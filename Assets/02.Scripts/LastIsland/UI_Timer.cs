using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class UI_Timer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;
    private float timeRemaining;
    private bool isTimerRunning = false;

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
    }
}
