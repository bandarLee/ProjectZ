using Photon.Pun;
using System.Collections;
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
        if (!_init)
        {
            Init();
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

        // Character_Male <- 리소스에 넣어줄 이름
        // Character_Female
        PhotonNetwork.Instantiate($"Character_{UI_PlaceholderModel.SelectedCharacterType}", Vector3.zero, Quaternion.identity);

    }
}
