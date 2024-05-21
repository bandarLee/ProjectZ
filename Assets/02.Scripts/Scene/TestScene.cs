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
        //PhotonNetwork.Instantiate($"Character_{UI_Lobby.SelectedCharacterType}", Vector3.zero, Quaternion.identity);

        // UI_Lobby 스크립트를 만들기 전에, 캐릭터 바꿀 기존 스크립트를 먼저 찾아야함
        // 기존 스크립트에다가 UI_Lobby를 불러오면 되겠다..?
    }
}
