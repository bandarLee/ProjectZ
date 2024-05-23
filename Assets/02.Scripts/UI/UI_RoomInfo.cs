using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_RoomInfo : MonoBehaviourPunCallbacks
{
    public static UI_RoomInfo Instance { get; private set; }

    public TextMeshProUGUI MissionMessageUI;
    public TextMeshProUGUI LogMessageUI;

    private string _logText = string.Empty;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 미션 시작시 메시지 표시
        DisplayMission("Mission 1 : 솰라솰라");
    }

    // 미션 메시지 표시
    public void DisplayMission(string message)
    {
        MissionMessageUI.text = message;
    }

    // 미션 완료 메소드
    public void MissionCompleted()
    {
        StartCoroutine(ClearTextAfterDelay(MissionMessageUI, 0)); // 미션 완료 즉시 메시지 클리어
    }

    public void AddLog(string logMessage)
    {
        _logText += logMessage;
        LogMessageUI.text = _logText;
        StopCoroutine("ClearLogAfterDelay");
        StartCoroutine(ClearLogAfterDelay());
    }


    // 텍스트를 지정된 시간 후에 클리어하는 코루틴 // 예) 미션 수행시
    private IEnumerator ClearTextAfterDelay(TextMeshProUGUI textUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        textUI.text = ""; // 텍스트 UI 클리어
    }

    // 로그 메시지를 5초 후에 클리어하는 코루틴
    private IEnumerator ClearLogAfterDelay()
    {
        yield return new WaitForSeconds(5);
        _logText = ""; // 로그 텍스트 초기화
        LogMessageUI.text = _logText;
    }
}
