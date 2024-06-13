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
        fullText = "소의 뿔이 사라지는 시간에 중앙에서 20초간 모습을 드러낸다.";
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
