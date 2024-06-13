using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI_BookText : MonoBehaviour
{
    public TextMeshProUGUI BookText;
    public float typingSpeed = 0.05f;

    private string fullText;

    private void Start()
    {
        BookText.text = "";
    }

    public void DisplayText(string text)
    {
        fullText = text;
        BookText.text = "";
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        int totalVisibleCharacters = fullText.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            BookText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
