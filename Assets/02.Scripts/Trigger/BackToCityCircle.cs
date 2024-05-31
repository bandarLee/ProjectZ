using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToCityCircle : MonoBehaviour
{
    public TextMeshProUGUI OpenText;

    private bool isPlayerInTrigger = false;

    private void Start()
    {
        OpenText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 트리거");

            isPlayerInTrigger = true;
            OpenText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 트리거 나감");
            isPlayerInTrigger = false;
            OpenText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            OpenText.gameObject.SetActive(false);
            StartCoroutine(BackToCityWaitForSeconds());
        }
    }

    private IEnumerator BackToCityWaitForSeconds()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("City_1");
    }
}
