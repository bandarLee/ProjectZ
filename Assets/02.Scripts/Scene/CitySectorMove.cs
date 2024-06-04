using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public enum CityZoneType
{
    Sector1,
    Sector2,
    Sector3,
    Sector4,
    Sector5,
    Sector6,
}

public enum SectorTriggerType
{
    Trigger1,
    Trigger2,
    Trigger3,
    Trigger4,
}

public class CitySectorMove : MonoBehaviour
{
    public CityZoneType ThisTriggerCity;
    public SectorTriggerType ThisTriggerType;

    public GameObject[] SectorSpawnpoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleTriggerEnter(other.gameObject));
        }
    }

    private IEnumerator HandleTriggerEnter(GameObject player)
    {
        switch (ThisTriggerCity)
        {
            case CityZoneType.Sector1:
                if (ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector2);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[2].transform.position;
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger4)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector4);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[7].transform.position;
                }
                break;

            case CityZoneType.Sector2:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector1);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[0].transform.position;
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger4)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector5);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[10].transform.position;
                }
                else if(ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector3);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[5].transform.position;
                }
                break;

            case CityZoneType.Sector3:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector2);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[3].transform.position;
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger4)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector6);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[13].transform.position;
                }
                break;

            case CityZoneType.Sector4:
                if (ThisTriggerType == SectorTriggerType.Trigger2)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector1);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[1].transform.position;
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector5);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[9].transform.position;
                }
                break;

            case CityZoneType.Sector5:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector4);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[8].transform.position;
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger2)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector2);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[4].transform.position;
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector6);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[12].transform.position;
                }
                break;

            case CityZoneType.Sector6:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector5);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[11].transform.position;
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger2)
                {
                    GameManager.Instance.ActiveSector(CityZoneType.Sector3);
                    yield return new WaitForSeconds(1f);
                    player.transform.position = SectorSpawnpoints[6].transform.position;
                }
                break;
        }
    }

    
}
