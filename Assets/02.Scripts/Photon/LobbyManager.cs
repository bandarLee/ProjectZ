using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";
    public TMP_InputField NicknameInput;
    public TMP_InputField PersonalityInput;

    public Text connectionInfoText;
    //네트워크 정보 표시 텍스트
    public Button joinButton;
    //룸 접속 버튼



    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion; 
        PhotonNetwork.ConnectUsingSettings(); 

        joinButton.interactable = false; 
        connectionInfoText.text = "마스터 서버에 접속중...";
    }

    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼을 활성화
        joinButton.interactable = true;
        // 접속 정보 표시
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 룸 접속 버튼을 비활성화
        joinButton.interactable = false;
        // 접속 정보 표시
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        Hashtable playerProperties = new Hashtable();
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        if (gameObject.GetComponent<PhotonView>().IsMine)
        {
            string personalityValue = PersonalityInput.text;

            playerProperties.Add("Personality", personalityValue);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        joinButton.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "룸에 접속...";
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 6 };
            PhotonNetwork.JoinOrCreateRoom("MyUniqueRoom", roomOptions, TypedLobby.Default);

        }
        else
        {
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }



    public override void OnJoinedRoom()
    {
        // 접속 상태 표시
        connectionInfoText.text = " 파티에 참가합니다. ";
        // 모든 룸 참가자들이 Main 씬을 로드하게 함    
        PhotonNetwork.LoadLevel("TestScene");   // -> 임시: "TestScene"
    }

}