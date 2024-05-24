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
        // 입력 필드의 이벤트 리스너 설정
        NicknameInput.onEndEdit.AddListener(UpdateNicknameUI);
    }

    // 닉네임 업데이트 로직
    public void UpdateNicknameUI(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return; // 새로운 값이 비어있으면 아무 작업도 수행하지 않음
        }

        SoldierNameUI.text = newValue; 
        ProfileNicknameUI.text = newValue; 
    }
}
