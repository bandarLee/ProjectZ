using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    public bool _init = false;

    public CityZoneType lastZone;

    public GameObject[] SpawnPoints;
    public GameObject[] Sectors;

    public int Randomzone;
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
                SpawnPlayer();
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
            SpawnPlayer();
        }
    }

    public void Init()
    {
        _init = true;
    }


    private void SpawnPlayer()
    {
        Vector3 spawnPosition = GetRandomSpawnPoint();
        PhotonNetwork.Instantiate("Character_Female_rigid_collid", spawnPosition, Quaternion.identity);

        StartCoroutine(InactiveSector());
    }

    public Vector3 GetRandomSpawnPoint()
    {
        Randomzone = Random.Range(0, 6);
        lastZone = (CityZoneType)Randomzone; // 마지막 섹터를 업데이트
        return SpawnPoints[Randomzone].transform.position;
    }

    //10초뒤에 내가 있는섹터빼고 섹터 5개 끄기
    private IEnumerator InactiveSector()
    {
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < 6; i++)
        {
            if( i != Randomzone)
            {
                Sectors[i].SetActive(false);
            }
        }
    }

    public void ActiveSector(CityZoneType cityZoneType)
    {
        Randomzone = (int)cityZoneType;
        lastZone = cityZoneType;
        for (int i = 0; i < 6; i++)
        {
            if (i != Randomzone)
            {
                Sectors[i].SetActive(false);
            }
        }
        Sectors[Randomzone].SetActive(true);


    }
}
