using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";
    public TMP_InputField NicknameInput;
/*    public TMP_InputField PersonalityInput;
*/
    public TextMeshProUGUI connectionInfoText;
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
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        if (PhotonNetwork.IsConnected)
        {
            int characterType = (int)UI_PlaceholderModel.Instance.SelectedCharacterType;
            Hashtable props = new Hashtable
            {
                { "CharacterType", characterType }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 20 };
            PhotonNetwork.JoinOrCreateRoom("Server1", roomOptions, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }



    public override void OnJoinedRoom()
    {
        //캐릭터 선택한거에서 변수를 Hashtable 로 업로드
        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        connectionInfoText.text = " 파티에 참가합니다. ";
        PhotonNetwork.LoadLevel("TestScene");  
    }

}