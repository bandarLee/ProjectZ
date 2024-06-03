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
    private static GameObject playerInstance;
    private static string nextSpawnPointName;

    private void Awake()
    {
        if (playerInstance == null)
        {
            playerInstance = GameObject.FindWithTag("Player");
            if (playerInstance != null)
            {
                DontDestroyOnLoad(playerInstance);
            }
        }
        else
        {
            Destroy(GameObject.FindWithTag("Player"));
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("씬 전환 준비");
            switch (currentZone)
            {
                case CityZoneType.City1_3:
                    Debug.Log("City_2로 이동합니다");
                    nextSpawnPointName = "SpawnPoint1";
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    SceneManager.LoadScene("City_2");
                    break;
                case CityZoneType.City1_4:
                    // SceneManager.LoadScene("City_4");
                    break;
                case CityZoneType.City2_1:
                    Debug.Log("City_1으로 이동합니다");
                    nextSpawnPointName = "SpawnPoint3";
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    SceneManager.LoadScene("City_1");
                    break;
                default:
                    break;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(nextSpawnPointName))
        {
            MovePlayerToSpawnPoint(nextSpawnPointName);
            nextSpawnPointName = null;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void MovePlayerToSpawnPoint(string spawnPointName)
    {
        GameObject player = GameObject.FindWithTag("Player");
        Transform spawnPoint = GameObject.Find(spawnPointName).transform;

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }
    }
}
