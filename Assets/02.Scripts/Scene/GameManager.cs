using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    public bool _init = false;

    public int Randomzone;
    public Transform spawnPosition;
    public Transform[] SceneMovePosition;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            if (!_init)
            {
                Init();
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
        }

    }


    public void Init()
    {
        _init = true;
        SpawnPlayer();
    }


    public Vector3 GetSpawnPoint()
    {
        return spawnPosition.position;
    }

    private void SpawnPlayer()
    {
        if (!CharacterInfo.Instance._isGameStart)
        {
            GameObject newPlayer = PhotonNetwork.Instantiate("Character_Female_rigid_collid", spawnPosition.position, Quaternion.identity);

            CharacterInfo.Instance._isGameStart = true;

        }
        else
        {
            GameObject newPlayer = PhotonNetwork.Instantiate("Character_Female_rigid_collid", SceneMovePosition[CharacterInfo.Instance.SpawnDir].position, Quaternion.identity);
            Character.LocalPlayerInstance.GetComponent<CharacterMoveAbilityTwo>().Teleport(SceneMovePosition[CharacterInfo.Instance.SpawnDir].position);
            Character.LocalPlayerInstance._characterRotateAbility.InitializeCamera();

        }
    }


    public void LoadCity(CityZoneType cityZoneType)
    {
        string sceneName = "City_" + ((int)cityZoneType + 1).ToString();
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 20 };
        roomOptions.CustomRoomProperties = new Hashtable { { "SceneName", sceneName } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "SceneName" };

        PhotonNetwork.JoinOrCreateRoom(sceneName, roomOptions, TypedLobby.Default);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("CurrentScene"))
        {
            int newScene = (int)changedProps["CurrentScene"];
            OnSceneChanged(targetPlayer, newScene);
        }
    }

    private void OnSceneChanged(Player player, int newScene)
    {
        Debug.LogError($"Player {player.NickName} changed scene to {newScene}");

    }

}
