using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackToCityTextTrigger : MonoBehaviour
{
    public TextMeshProUGUI BackToCityText;

    private void Start()
    {
        BackToCityText.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 트리거");
            BackToCityText.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 트리거 나감");
            BackToCityText.gameObject.SetActive(false);
        }
    }
}
