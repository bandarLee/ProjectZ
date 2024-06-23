using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���� �߰�
using DG.Tweening; // DOTween�� ����ϱ� ���� �߰�

public class GoToLastIslandTrigger : MonoBehaviour
{
    public TextMeshProUGUI GotoLastIslandText;
    public Image FadeImage; 
    private bool isPlayerInTrigger = false;

    private void Start()
    {
        GotoLastIslandText.gameObject.SetActive(false);
        FadeImage.gameObject.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player triggered GoToLastIslandTrigger");
            isPlayerInTrigger = true;
            GotoLastIslandText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            GotoLastIslandText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        FadeImage.gameObject.SetActive(true); 
        FadeImage.color = new Color(0, 0, 0, 0); // �ʱ� ���� ����
        FadeImage.DOFade(1, 1.5f);
        yield return new WaitForSeconds(1.5f);
        LoadLastIslandScene();
    }

    private void LoadLastIslandScene()
    {
        PhotonNetwork.LoadLevel("LastIsLandScene");
    }
}
