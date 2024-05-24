using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Lobby : MonoBehaviour
{
    public void OnNicknameValueChanged(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }

        PhotonNetwork.NickName = newValue;
    }
}
