using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    public bool _init = false;

    public CityZoneType lastZone;
    public int Randomzone;
    private Vector3 spawnPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 GameManager를 파괴하지 않음
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
                LoadRandomCity();
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
            LoadRandomCity();
        }
    }

    public void Init()
    {
        _init = true;
    }

    private void LoadRandomCity()
    {
        Randomzone = Random.Range(0, 6);
        lastZone = (CityZoneType)Randomzone;
        string sceneName = "City_" + (Randomzone + 1).ToString();
        PhotonNetwork.LoadLevel(sceneName);
    }

    public void SetSpawnPoint(Vector3 position)
    {
        spawnPosition = position;
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject newPlayer = PhotonNetwork.Instantiate("Character_Female_rigid_collid", spawnPosition, Quaternion.identity);
    }

    public void LoadCity(CityZoneType cityZoneType)
    {
        lastZone = cityZoneType;
        string sceneName = "City_" + ((int)cityZoneType + 1).ToString();
        PhotonNetwork.LoadLevel(sceneName);
    }
}
