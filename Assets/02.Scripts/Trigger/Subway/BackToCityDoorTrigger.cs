using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToCityDoorTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(BackToCityWaitForSeconds());
        }
    }

    private IEnumerator BackToCityWaitForSeconds()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("City_1");
    }
}
