using System.Collections;
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
    public TextMeshProUGUI connectionInfoText;
    public Button joinButton;
    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
        connectionInfoText.text = "������ ������ ������...";
    }

    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "�¶��� : ������ ������ �����";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.PhotonServerSettings.DevRegion = "kr";
        PhotonNetwork.AutomaticallySyncScene = false;
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


    // �÷��̾�� �������� �� ����
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "��Ƽ�� �����մϴ�.";

        PhotonNetwork.LoadLevel("City_1");
    }
}
