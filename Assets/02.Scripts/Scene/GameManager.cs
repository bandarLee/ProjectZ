using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    public bool _init = false;

    public CityZoneType lastZone;
    public int Randomzone;
    public Transform spawnPosition;



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

    // 캐릭터 리스폰 코드
 /*   private void LoadRandomCity()
    {
        Randomzone = 0;
        lastZone = (CityZoneType)Randomzone;
        string sceneName = "City_" + (Randomzone + 1).ToString();
        PhotonNetwork.LoadLevel(sceneName);
    }*/


    public Vector3 GetSpawnPoint()
    {
        return spawnPosition.position;
    }

    private void SpawnPlayer()
    {

        GameObject newPlayer = PhotonNetwork.Instantiate("Character_Female_rigid_collid", spawnPosition.position, Quaternion.identity);
    }





    public void LoadCity(CityZoneType cityZoneType)
    {
        lastZone = cityZoneType;
        string sceneName = "City_" + ((int)cityZoneType + 1).ToString();
        PhotonNetwork.LoadLevel(sceneName);
    }
}
