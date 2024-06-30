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
    public Image UI_Ending;

    private float timeRemaining;
    private bool isTimerRunning = false;
    private TheLastYggdrasilWave theLastYggdrasilWave;
    private bool IsEnding= false;
    private void Start()
    {
        UI_Ending.gameObject.SetActive(false);
        theLastYggdrasilWave = FindObjectOfType<TheLastYggdrasilWave>();
    }

    private void Update()
    {
        if (theLastYggdrasilWave.Health <= 0)
        {
            StartCoroutine(BadEnding());
        }
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
    public void TimerEnded()
    {
        timerText.text = "00";
        StartCoroutine(HappyEndingFadeImage());
    }

    private IEnumerator HappyEndingFadeImage()
    {
        UI_Ending.gameObject.SetActive(true);
        UI_Ending.color = new Color(1, 1, 1, 0);
        Debug.Log("Happy");

        UI_Ending.DOFade(1, 1.5f);
        Debug.Log("Happy");
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Happy");
    }
    private IEnumerator BadEnding()
    {
        if (!IsEnding)
        {
            IsEnding = true;    
            UI_Ending.gameObject.SetActive(true);
            UI_Ending.color = new Color(1, 1, 1, 0);
            Debug.Log("Bad");

            UI_Ending.DOFade(1, 1.5f);
            Debug.Log("Bad");
            yield return new WaitForSeconds(1.5f);
            Debug.Log("Bad");
        }
    }
}