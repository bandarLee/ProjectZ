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
    private bool[] CityPiecesFound = new bool[6]; // �� ���� ������ �߰� ����

    private void Start()
    {
        InitializeMap();
        AssignButtonEvents();
        FindMapText.gameObject.SetActive(false);
    }

    // �ʱ� ���� ���� �޼���
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

    // ���� ���� ��� �޼���
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

    // ���� ���� Ŭ�� ó�� �޼���
    public void OnCityButtonClick(int pieceIndex)
    {
        if (pieceIndex < 0 || pieceIndex >= CityPiecesFound.Length)
        {
            Debug.LogWarning("Invalid piece index.");
            return;
        }

        if (!CityPiecesFound[pieceIndex])
        {
            Debug.Log("������ ã������.");
            // ���⿡ "������ ã������" �ؽ�Ʈ�� Ȱ��ȭ�ϴ� �ڵ带 �߰��ϼ���.
        }
        else
        {
            Debug.Log("City " + (pieceIndex + 1) + " ��ư Ŭ����");
            ShowMapPiece(pieceIndex);
        }
    }

    // �ش� ������ ���� �̹����� �����ִ� �޼���
    private void ShowMapPiece(int pieceIndex)
    {
        for (int i = 0; i < MapPieceImages.Length; i++)
        {
            MapPieceImages[i].SetActive(i == pieceIndex);
        }
    }

    // ��ư Ŭ�� �̺�Ʈ ���� �޼���
    private void AssignButtonEvents()
    {
        for (int i = 0; i < CityButtons.Length; i++)
        {
            int index = i; // Ŭ���� ������ ���ϱ� ���� �ε����� ���� ������ ����....�������������ٿ�
            CityButtons[i].GetComponent<Button>().onClick.AddListener(() => OnCityButtonClick(index));
        }
    }
    // ������ �� �� ȣ��Ǵ� �޼���
    public void OpenMap()
    {
        foreach (var button in CityButtons)
        {
            button.SetActive(true); // ��� ��ư�� Ȱ��ȭ
        }
        foreach (var mapPiece in MapPieceImages)
        {
            mapPiece.SetActive(false); // ��� ���� ������ ��Ȱ��ȭ
        }
        FindMapText.gameObject.SetActive(false); // "������ ã������" �ؽ�Ʈ�� ��Ȱ��ȭ
    }
}
