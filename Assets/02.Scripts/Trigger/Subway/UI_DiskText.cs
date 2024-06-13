using System.Collections;
using TMPro;
using UnityEngine;

public class UI_DiskText : MonoBehaviour
{
    public TextMeshProUGUI DiskText_1;
    public TextMeshProUGUI DiskText_2;
    public TextMeshProUGUI DiskText_3;
    public float typingSpeed = 0.05f;

    private string fullText;

    public void DisplayText_1(string text)
    {
        fullText = text;
        DiskText_1.text = text;
        DiskText_1.gameObject.SetActive(true);
        StartCoroutine(TypeText_1());
    }

    private IEnumerator TypeText_1()
    {
        int totalVisibleCharacters = fullText.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            DiskText_1.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(1.5f);
        DiskText_1.text = "";
        DiskText_1.gameObject.SetActive(false);
    }

    public void DisplayText_2(string text)
    {
        fullText = text;
        DiskText_2.text = text;
        DiskText_2.gameObject.SetActive(true);
        StartCoroutine(TypeText_2());
    }

    private IEnumerator TypeText_2()
    {
        int totalVisibleCharacters = fullText.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            DiskText_2.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(1.5f);
        DiskText_2.text = "";
        DiskText_2.gameObject.SetActive(false);
    }

    public void DisplayText_3(string text)
    {
        fullText = text;
        DiskText_3.text = text;
        DiskText_3.gameObject.SetActive(true);
        StartCoroutine(TypeText_3());
    }

    private IEnumerator TypeText_3()
    {
        int totalVisibleCharacters = fullText.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            DiskText_3.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(1.5f);
        DiskText_3.text = "";
        DiskText_3.gameObject.SetActive(false);
    }
}
