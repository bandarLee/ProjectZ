using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviourPunCallbacks
{
    public static TestScene Instance { get; private set; }

    public List<Transform> SpawnPoints;

    public bool _init = false;

    private void Awake()
    {
        Instance = this;
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

        //PhotonNetwork.LocalPlayer.CustomProperties["CharacterClass"];

        string characterName = "Character_Female"; 
        Vector3 spawnPosition = SpawnPoints[0].transform.position;
        Quaternion spawnRotation = Quaternion.identity;

        GameObject character = PhotonNetwork.Instantiate(characterName, spawnPosition, spawnRotation);


    }
}
