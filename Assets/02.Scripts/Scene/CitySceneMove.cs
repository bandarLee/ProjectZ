using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public enum CityZoneType
{
    City_1,
    City_2,
    City_3,
    City_4,
    City_5,
    City_6,
}

public enum SectorTriggerType
{
    Trigger1,
    Trigger2,
    Trigger3,
    Trigger4,
}



public class CitySceneMove : MonoBehaviour
{
    public CityZoneType ThisTriggerCity;
    public SectorTriggerType ThisTriggerType;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            HandleTriggerEnter(other.gameObject);
        }
    }

    private void HandleTriggerEnter(GameObject player)
    {
        int Direction = ((int)ThisTriggerType + 2) % 4;
        int DestinateScene; 
        if ((int)ThisTriggerType % 2 == 0) 
        {
            if ((int)ThisTriggerType / 2 == 0)
            {
                DestinateScene = ((int)ThisTriggerCity - 1) % 6;
            }
            else
            {
                DestinateScene = ((int)ThisTriggerCity + 1) % 6;
            }
        }
        else
        {
            DestinateScene = ((int)ThisTriggerCity + 3) % 6;
        }

        CharacterInfo.Instance.SpawnDir = Direction + DestinateScene * 4;
        switch (ThisTriggerCity)
        {
            case CityZoneType.City_1:
                if (ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_2);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger4)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_4);
                }
                break;

            case CityZoneType.City_2:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_1);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger4)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_5);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_3);
                }
                break;

            case CityZoneType.City_3:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_2);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger4)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_6);
                }
                break;

            case CityZoneType.City_4:
                if (ThisTriggerType == SectorTriggerType.Trigger2)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_1);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_5);
                }
                break;

            case CityZoneType.City_5:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_4);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger2)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_2);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger3)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_6);
                }
                break;

            case CityZoneType.City_6:
                if (ThisTriggerType == SectorTriggerType.Trigger1)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_5);
                }
                else if (ThisTriggerType == SectorTriggerType.Trigger2)
                {
                    GameManager.Instance.LoadCity(CityZoneType.City_3);
                }
                break;
        }
    }
}