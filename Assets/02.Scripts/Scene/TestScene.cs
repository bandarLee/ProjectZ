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

       // PhotonNetwork.ConnectUsingSettings(); // 마스터 서버에 연결
    }

    /*public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom(); // 랜덤한 방에 참여 시도
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null); // 랜덤 참여 실패 시 새 방 생성
    }*/

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
        PhotonNetwork.Instantiate($"Character_{UI_PlaceholderModel.SelectedCharacterType}", SpawnPoints[0].transform.position, Quaternion.identity);

    }
}
