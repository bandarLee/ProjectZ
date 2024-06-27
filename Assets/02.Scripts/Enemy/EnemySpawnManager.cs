using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] Leviatans;
    public GameObject[] Bats;
    PhotonView pv;


    private void Start()
    {
        pv = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("StartEnemy", RpcTarget.AllBuffered);
        }
        StartCoroutine(DelaySpawn());
    }
    public IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(1.6f);
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("Day", RpcTarget.AllBuffered);
        }
    }
    public void DayEnemySpawn(GameTime.TimeType newTimeType)
    {
        if ((newTimeType == GameTime.TimeType.Day)&& PhotonNetwork.IsMasterClient)
        {
            pv.RPC("Day", RpcTarget.AllBuffered);
        }


    }

    public void NightEnemySpawn(GameTime.TimeType newTimeType)
    {
        if ((newTimeType == GameTime.TimeType.Night) && PhotonNetwork.IsMasterClient)
        {
            pv.RPC("Night", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void Day()
    {
        foreach (GameObject lev in Leviatans)
        {
            lev.SetActive(true);
        }
        foreach (GameObject bat in Bats)
        {
            bat.SetActive(false);
        }
    }
    [PunRPC]
    public void StartEnemy()
    {
        foreach (GameObject lev in Leviatans)
        {
            lev.SetActive(false);
        }
        foreach (GameObject bat in Bats)
        {
            bat.SetActive(false);
        }
    }
    [PunRPC]
    public void Night()
    {
        foreach (GameObject lev in Leviatans)
        {
            lev.SetActive(true);
        }
        foreach (GameObject bat in Bats)
        {
            bat.SetActive(false);
        }
    }
}
