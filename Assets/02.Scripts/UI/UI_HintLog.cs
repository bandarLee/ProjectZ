using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_HintLog : MonoBehaviourPunCallbacks
{
    public static UI_HintLog Instance { get; private set; }

    public TextMeshProUGUI HintText;
    public TextMeshProUGUI LogText;

    private string _logText = string.Empty;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHintText(string newText)
    {
        if (HintText != null)
        {
            HintText.text = newText;
        }
    }

    public void AddLog(string logMessage)
    {
        _logText += logMessage;
        LogText.text = _logText;
        StopCoroutine("ClearLogAfterDelay");
        StartCoroutine(ClearLogAfterDelay());
    }



    // 로그 메시지를 5초 후에 클리어하는 코루틴
    private IEnumerator ClearLogAfterDelay()
    {
        yield return new WaitForSeconds(5);
        _logText = ""; // 로그 텍스트 초기화
        LogText.text = _logText;
    }
}
