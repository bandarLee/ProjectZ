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
        // �̼� ���۽� �޽��� ǥ��
        DisplayMission("Mission 1 : �����");
    }

    // �̼� �޽��� ǥ��
    public void DisplayMission(string message)
    {
        MissionMessageUI.text = message;
    }

    // �̼� �Ϸ� �޼ҵ�
    public void MissionCompleted()
    {
        StartCoroutine(ClearTextAfterDelay(MissionMessageUI, 0)); // �̼� �Ϸ� ��� �޽��� Ŭ����
    }

    public void AddLog(string logMessage)
    {
        _logText += logMessage;
        LogMessageUI.text = _logText;
        StopCoroutine("ClearLogAfterDelay");
        StartCoroutine(ClearLogAfterDelay());
    }


    // �ؽ�Ʈ�� ������ �ð� �Ŀ� Ŭ�����ϴ� �ڷ�ƾ // ��) �̼� �����
    private IEnumerator ClearTextAfterDelay(TextMeshProUGUI textUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        textUI.text = ""; // �ؽ�Ʈ UI Ŭ����
    }

    // �α� �޽����� 5�� �Ŀ� Ŭ�����ϴ� �ڷ�ƾ
    private IEnumerator ClearLogAfterDelay()
    {
        yield return new WaitForSeconds(5);
        _logText = ""; // �α� �ؽ�Ʈ �ʱ�ȭ
        LogMessageUI.text = _logText;
    }
}
