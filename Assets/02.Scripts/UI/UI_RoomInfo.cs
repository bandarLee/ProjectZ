using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_RoomInfo : MonoBehaviourPunCallbacks
{
    public static UI_RoomInfo Instance { get; private set; }
    
    public TextMeshProUGUI LogMessageUI;

    private string _logText = string.Empty;

    private void Awake()
    {
        Instance = this;
    }

    public void AddLog(string logMessage)
    {
        _logText += logMessage;

    }
}
