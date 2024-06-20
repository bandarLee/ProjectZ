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
    private bool[] CityPiecesFound = new bool[6]; // 각 도시 조각의 발견 상태

    private void Start()
    {
        InitializeMap();
        AssignButtonEvents();
        FindMapText.gameObject.SetActive(false);
    }

    // 초기 상태 설정 메서드
    private void InitializeMap()
    {
        for (int i = 0; i < CityButtons.Length; i++)
        {
            CityButtons[i].GetComponent<Button>().interactable = false;
            CityTexts[i].gameObject.SetActive(false);
            LockImages[i].SetActive(true);
            MapPieceImages[i].SetActive(false);
        }
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
        CityButtons[pieceIndex].GetComponent<Button>().interactable = true;
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
            // 여기에 "지도를 찾으세요" 텍스트를 활성화하는 코드를 추가하세요.
        }
        else
        {
            Debug.Log("City " + (pieceIndex + 1) + " 버튼 클릭됨");
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

    // 버튼 클릭 이벤트 연결 메서드
    private void AssignButtonEvents()
    {
        for (int i = 0; i < CityButtons.Length; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 인덱스를 지역 변수로 저장....무슨말인진몰겟움
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
