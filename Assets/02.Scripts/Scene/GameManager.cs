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
        Character.LocalPlayerInstance.DeactiveOtherCharacterOrder();

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
            Character.LocalPlayerInstance.DeactiveOtherCharacterOrder();

        }
        else
        {
            /*            GameObject newPlayer = PhotonNetwork.Instantiate("Character_Female_rigid_collid", SceneMovePosition[CharacterInfo.Instance.SpawnDir].position, Quaternion.identity);
            */
            Character.LocalPlayerInstance.GetComponent<CharacterMoveAbilityTwo>().Teleport(SceneMovePosition[CharacterInfo.Instance.SpawnDir].position);
            Character.LocalPlayerInstance._characterRotateAbility.InitializeCamera();
            Character.LocalPlayerInstance.DeactiveOtherCharacterOrder();

        }
    }


    public void LoadCity(CityZoneType cityZoneType)
    {
        string sceneName = "City_" + ((int)cityZoneType + 1).ToString();
        Hashtable SceneProperties = new Hashtable();
        SceneProperties.Add("CurrentScene", (int)cityZoneType);
        PhotonNetwork.LocalPlayer.SetCustomProperties(SceneProperties);
        PhotonNetwork.LoadLevel(sceneName);
    }


}
