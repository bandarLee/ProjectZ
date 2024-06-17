using System.Collections;
using TMPro;
using UnityEngine;

public class UI_BookText : MonoBehaviour
{
    public TextMeshProUGUI BookText;
    public float typingSpeed = 0.05f;

    private string fullText;

    public void DisplayText(string text)
    {
        fullText = text;
        BookText.text = text;
        BookText.gameObject.SetActive(true); 
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
        yield return new WaitForSeconds(1.5f);
        BookText.text = "";
        BookText.gameObject.SetActive(false); 
    }
}
