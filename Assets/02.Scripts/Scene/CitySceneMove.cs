using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CityZoneType
{
    City1_3,
    City1_4,
    City2_1,
    City2_2,
    City2_3,
    City2_4,
}

public class CitySceneMove : MonoBehaviour
{
    public CityZoneType currentZone;

    // 씬 이동하는 코드
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("City_2로 이동");
            switch (currentZone)
            {
                // City_1
                case CityZoneType.City1_3:
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    SceneManager.LoadScene("City_2");
                    break;
                case CityZoneType.City1_4:
                    //SceneManager.LoadScene("City_4");
                    break;

                // City_2
                case CityZoneType.City2_1:
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    SceneManager.LoadScene("City_1");
                    break;
            
                default:
                    break;
            }
        }
    }

    // 스폰 포인트로 이동하는 코드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "City_2")
        {

            GameObject player = GameObject.FindWithTag("Player");
            GameObject spawnPoint = GameObject.Find("SpawnPoint1");

            if (player != null && spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        if (scene.name == "City_1")
        {

            GameObject player = GameObject.FindWithTag("Player");
            GameObject spawnPoint = GameObject.Find("SpawnPoint3");

            if (player != null && spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
