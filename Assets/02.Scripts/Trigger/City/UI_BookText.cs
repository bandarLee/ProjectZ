
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UI_BookText : MonoBehaviour
{
    public TextMeshProUGUI BookText;
    public float typingSpeed = 0.05f;

    private string fullText;
    private Action onComplete; //콜백 기능을 추가하여 텍스트 표시가 완료되었음을 알림

    public void DisplayText(string text, Action onComplete = null)
    {
        this.onComplete = onComplete;
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

        onComplete?.Invoke(); // 콜백 호출
    }
}
