using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI_DiskText : MonoBehaviour
{
    public TextMeshProUGUI DiskText1;
    public TextMeshProUGUI DiskText2;
    public TextMeshProUGUI DiskText3;
    public float typingSpeed = 0.05f;

    private string fullText1;
    private string fullText2;
    private string fullText3;

    private void Start()
    {
        fullText1 = "마지막 생명과 함께 최후의 섬으로 가라.";
        fullText2 = "가장 깊은 곳에 마지막 생명이 숨쉬고 있다.";
        fullText3 = "The Last Yggdrasil.";

        DiskText1.text = "";
        DiskText2.text = "";
        DiskText3.text = "";

        StartCoroutine(Disk_1_TypeText());
    }

    private IEnumerator Disk_1_TypeText()
    {
        int totalVisibleCharacters = fullText1.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            DiskText1.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

       //yield return new WaitForSeconds(1.0f);
        StartCoroutine(Disk_2_TypeText());
    }

    private IEnumerator Disk_2_TypeText()
    {
        int totalVisibleCharacters = fullText2.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            DiskText2.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        //yield return new WaitForSeconds(1.0f);
        StartCoroutine(Disk_3_TypeText());
    }

    private IEnumerator Disk_3_TypeText()
    {
        int totalVisibleCharacters = fullText3.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            DiskText3.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
