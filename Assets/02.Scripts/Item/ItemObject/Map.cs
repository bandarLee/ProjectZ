using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject[] cityButtons; // City1부터 City6까지의 버튼
    public GameObject[] lockImages; // 각 버튼에 대응하는 잠금 이미지
    public Text[] cityTexts; // 각 버튼에 대응하는 텍스트 (City 1, City 2 등)
    private bool[] cityPiecesFound = new bool[6]; // 각 도시 조각의 발견 상태

    //지도 조각 등록 메서드
    public void RegisterMapPiece(int pieceIndex)
    {
        if (pieceIndex < 0 || pieceIndex >= cityPiecesFound.Length)
        {
            Debug.LogWarning("Invalid piece index.");
            return;
        }

        cityPiecesFound[pieceIndex] = true;
        lockImages[pieceIndex].SetActive(false);
        cityTexts[pieceIndex].gameObject.SetActive(true);
        cityButtons[pieceIndex].GetComponent<Button>().interactable = true;
    }

    //지도 조각 클릭 처리 메서드
    public void OnCityButtonClick(int pieceIndex)
    {
        if (pieceIndex < 0 || pieceIndex >= cityPiecesFound.Length)
        {
            Debug.LogWarning("Invalid piece index.");
            return;
        }

        if (!cityPiecesFound[pieceIndex])
        {
            Debug.Log("지도를 찾으세요.");
            // 여기에 "지도를 찾으세요" 텍스트를 활성화하는 코드를 추가하세요.
        }
        else
        {
            Debug.Log("City " + (pieceIndex + 1) + " 버튼 클릭됨");
            // 여기에 실제로 지도를 여는 코드를 추가하세요.
        }
    }

}
