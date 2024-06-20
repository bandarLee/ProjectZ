using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject[] CityButtons;
    public GameObject[] LockImages;
    public TextMeshProUGUI[] CityTexts;
    public TextMeshProUGUI FindMapText;
    public GameObject[] MapPieceImages;
    private bool[] CityPiecesFound = new bool[6];

    private void Start()
    {
        InitializeMap();
        AssignButtonEvents();
    }

    // 초기 상태 설정 메서드
    private void InitializeMap()
    {
        for (int i = 0; i < CityButtons.Length; i++)
        {
            CityTexts[i].gameObject.SetActive(false);
            LockImages[i].SetActive(true);
            MapPieceImages[i].SetActive(false);
        }
        FindMapText.gameObject.SetActive(false);
    }

    // 지도 조각 등록 메서드
    public void RegisterMapPiece(int pieceIndex)
    {
        if (pieceIndex < 0 || pieceIndex >= CityPiecesFound.Length)
        {
            Debug.LogWarning("Invalid piece index.");
            return;
        }
        CityPiecesFound[pieceIndex] = true;
        LockImages[pieceIndex].SetActive(false);
        CityTexts[pieceIndex].gameObject.SetActive(true);
    }

    // 지도 조각 클릭 처리 메서드
    public void OnCityButtonClick(int pieceIndex)
    {
        if (pieceIndex < 0 || pieceIndex >= CityPiecesFound.Length)
        {
            Debug.LogWarning("Invalid piece index.");
            return;
        }

        if (!CityPiecesFound[pieceIndex])
        {
            Debug.Log("지도를 찾으세요.");
            StartCoroutine(ShowFindMapText());
        }
        else
        {
            Debug.Log("City " + (pieceIndex + 1) + " 버튼 클릭됨");
            FindMapText.gameObject.SetActive(false);
            ShowMapPiece(pieceIndex);
        }
    }

    // 해당 구역의 지도 이미지를 보여주는 메서드
    private void ShowMapPiece(int pieceIndex)
    {
        for (int i = 0; i < MapPieceImages.Length; i++)
        {
            MapPieceImages[i].SetActive(i == pieceIndex);
        }
    }

    // "지도를 찾으세요" 텍스트를 일정 시간 동안 보여주는 코루틴
    private IEnumerator ShowFindMapText()
    {
        FindMapText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f); // 2초 동안 텍스트 표시
        FindMapText.gameObject.SetActive(false);
    }

    // 버튼 클릭 이벤트 연결 메서드
    private void AssignButtonEvents()
    {
        for (int i = 0; i < CityButtons.Length; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 인덱스를 지역 변수로 저장
            CityButtons[i].GetComponent<Button>().onClick.AddListener(() => OnCityButtonClick(index));
        }
    }

    // 지도를 열 때 호출되는 메서드
    public void OpenMap()
    {
        foreach (var button in CityButtons)
        {
            button.SetActive(true); // 모든 버튼을 활성화
        }
        foreach (var mapPiece in MapPieceImages)
        {
            mapPiece.SetActive(false); // 모든 지도 조각을 비활성화
        }
        FindMapText.gameObject.SetActive(false); // "지도를 찾으세요" 텍스트를 비활성화
    }
}
