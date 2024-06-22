using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UMA;
using UMA.CharacterSystem;
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
        connectionInfoText.text = "마스터 서버에 접속중...";
    }

    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
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
        base.OnJoinedRoom();
        Debug.Log("Joined Room");

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("CharacterRecipe", out object characterRecipe))
        {
            Debug.Log("Character Recipe in OnJoinedRoom: " + characterRecipe);
        }
        StartLoading("CityScene");

    }
    public void StartLoading(string sceneName)
    {
        loadingScreen.SetActive(true);
        isLoading = true;
        DynamicCharacterAvatar avatar = FindObjectOfType<DynamicCharacterAvatar>(); // DynamicCharacterAvatar 객체 가져오기
        if (avatar == null)
        {
            Debug.LogError("DynamicCharacterAvatar object not found!");
            return;
        }

        string characterRecipe = GetRecipeString(avatar);
        Debug.Log("Character Recipe: " + characterRecipe);
        Hashtable props = new Hashtable
    {
        { "CharacterRecipe", characterRecipe }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);


        StartCoroutine(LoadSceneAfterPropertiesSet(sceneName));

    }
    private IEnumerator LoadSceneAfterPropertiesSet(string sceneName)
    {
        // 커스텀 프로퍼티가 설정될 때까지 대기
        yield return new WaitForSeconds(1);

        AsyncOperation operation = PhotonNetwork.LoadLevel(sceneName);
        StartCoroutine(UpdateLoadingProgress(operation));
    }
    public static string GetRecipeString(DynamicCharacterAvatar avatar)
    {
        if (avatar != null)
        {
            string recipe = avatar.GetCurrentRecipe();
            return recipe;
        }
        return null;
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
        loadingScreen.SetActive(false); // 로딩이 완료되면 로딩 화면 비활성화
        isLoading = false;
    }
}
