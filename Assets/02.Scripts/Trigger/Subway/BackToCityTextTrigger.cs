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
            BackToCityText.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BackToCityText.gameObject.SetActive(false);
        }
    }
}
