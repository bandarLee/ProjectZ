using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_LobbyMainPanels : MonoBehaviour
{

    public TMP_InputField NicknameInput;
    public TextMeshProUGUI SoldierNameUI;
    public TextMeshProUGUI ProfileNicknameUI;

    void Start()
    {
        // �ʱ� �г��� ���� (�ɼ�)
        if (!string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            NicknameInput.text = PhotonNetwork.NickName;
            UpdateNicknameUI(PhotonNetwork.NickName);
        }

        // �Է� �ʵ��� �̺�Ʈ ������ ����
        NicknameInput.onEndEdit.AddListener(UpdateNicknameUI);
    }

    // �г��� ������Ʈ ����
    public void UpdateNicknameUI(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return; // ���ο� ���� ��������� �ƹ� �۾��� �������� ����
        }

        PhotonNetwork.NickName = newValue; // Photon ��Ʈ��ũ�� �г��� ����
        SoldierNameUI.text = newValue; 
        ProfileNicknameUI.text = newValue; 
    }
}
