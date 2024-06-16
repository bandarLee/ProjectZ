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
    public TextMeshProUGUI NicknameInput;
    public TextMeshProUGUI connectionInfoText;
    public TextMeshProUGUI NicknameInfo;
    public Button joinButton;
    public GameObject loadingScreen;
    public Image loadingFillImage;
    public TextMeshProUGUI loadingText;
    private bool isLoading = false;
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
    public void NicknameEnter()
    {
        NicknameInfo.text = NicknameInput.text;
    }
    public void Connect1()
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
    public void Connect2()
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
            PhotonNetwork.JoinOrCreateRoom("Server2", roomOptions, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Connect3()
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
            PhotonNetwork.JoinOrCreateRoom("Server3", roomOptions, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "��Ƽ�� �����մϴ�.";
        StartLoading("CityScene");

    }
    public void StartLoading(string sceneName)
    {
        loadingScreen.SetActive(true);
        isLoading = true;
        AsyncOperation operation = PhotonNetwork.LoadLevel(sceneName);
        StartCoroutine(UpdateLoadingProgress(operation));
    }

    IEnumerator UpdateLoadingProgress(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingFillImage.fillAmount = progress;
            loadingText.text = (progress * 100f).ToString("F2") + "%";
            yield return null;
        }
        loadingScreen.SetActive(false); // �ε��� �Ϸ�Ǹ� �ε� ȭ�� ��Ȱ��ȭ
        isLoading = false;
    }
}
