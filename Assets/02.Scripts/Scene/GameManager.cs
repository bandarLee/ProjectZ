using Photon.Pun;
using Photon.Realtime;
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
        GameObject NewPlayer = PhotonNetwork.Instantiate("Character_Female_rigid_collid", spawnPosition, Quaternion.identity);

        StartCoroutine(InactiveSector(NewPlayer));
    }

    public Vector3 GetRandomSpawnPoint()
    {
        Randomzone = Random.Range(0, 6);
        lastZone = (CityZoneType)Randomzone; // 마지막 섹터를 업데이트
        return SpawnPoints[Randomzone].transform.position;
    }

    //10초뒤에 내가 있는섹터빼고 섹터 5개 끄기
    private IEnumerator InactiveSector(GameObject newplayer)
    {
        yield return new WaitForSeconds(10f);
        Vector3 currentPosition = newplayer.transform.position;

        for (int i = 0; i < 6; i++)
        {
            if (i != Randomzone)
            {
                Sectors[i].SetActive(false);
            }
        }

        // Ensure player is at the correct position after sectors are deactivated
        newplayer.transform.position = currentPosition;
    }

    public void ActiveSector(CityZoneType cityZoneType)
    {
        Randomzone = (int)cityZoneType;
        lastZone = cityZoneType;
        StartCoroutine(DeactivateAndActivateSectors());
    }

    private IEnumerator DeactivateAndActivateSectors()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i != Randomzone)
            {
                Sectors[i].SetActive(false);
            }
        }
        yield return null; // 다음 프레임까지 대기
        Sectors[Randomzone].SetActive(true);
    }
}
