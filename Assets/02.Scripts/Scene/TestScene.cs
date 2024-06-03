using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviourPunCallbacks
{
    public static TestScene Instance { get; private set; }

    public List<Transform> SpawnPoints;

    public bool _init = false;
    public string specificSpawnPointName = null;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[randomIndex].position;
    }

    public Transform GetSpawnPointByName(string spawnPointName)
    {
        return SpawnPoints.Find(sp => sp.name == spawnPointName);
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

        if (SpawnPoints == null || SpawnPoints.Count == 0)
        {
            Debug.LogError("Spawn points are not set!");
            return;
        }

        int characterType = (int)PhotonNetwork.LocalPlayer.CustomProperties["CharacterType"];
        string characterName = characterType == 0 ? "Character_Female_rigid_collid" : "Character_Male";

        Vector3 spawnPosition;
        if (!string.IsNullOrEmpty(specificSpawnPointName))
        {
            Transform spawnPoint = GetSpawnPointByName(specificSpawnPointName);
            spawnPosition = spawnPoint != null ? spawnPoint.position : GetRandomSpawnPoint();
            specificSpawnPointName = null; // reset after use
        }
        else
        {
            spawnPosition = GetRandomSpawnPoint();
        }

        Quaternion spawnRotation = Quaternion.identity;
        PhotonNetwork.Instantiate(characterName, spawnPosition, spawnRotation);
    }
}
