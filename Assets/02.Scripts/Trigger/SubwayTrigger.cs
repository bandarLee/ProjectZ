using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubwayTrigger : MonoBehaviour
{
    private GameTime gameTime;

    private void Start()
    {
        gameTime = FindObjectOfType<GameTime>();
    }

    private void Update()
    {
        CheckAndSetActiveState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("SubwayScene");
        }
    }

    private void CheckAndSetActiveState()
    {
        if (gameTime != null)
        {
            gameObject.SetActive(gameTime.CurrentTimeType == GameTime.TimeType.Mystery);
            Debug.Log("¡ˆ«œ√∂ ¿‘±∏ µÓ¿Â µŒµ’~~");
        }
    }
}
