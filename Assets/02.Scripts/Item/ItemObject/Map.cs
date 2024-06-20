using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject[] cityButtons; // City1���� City6������ ��ư
    public GameObject[] lockImages; // �� ��ư�� �����ϴ� ��� �̹���
    public Text[] cityTexts; // �� ��ư�� �����ϴ� �ؽ�Ʈ (City 1, City 2 ��)
    private bool[] cityPiecesFound = new bool[6]; // �� ���� ������ �߰� ����

    //���� ���� ��� �޼���
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

    //���� ���� Ŭ�� ó�� �޼���
    public void OnCityButtonClick(int pieceIndex)
    {
        if (pieceIndex < 0 || pieceIndex >= cityPiecesFound.Length)
        {
            Debug.LogWarning("Invalid piece index.");
            return;
        }

        if (!cityPiecesFound[pieceIndex])
        {
            Debug.Log("������ ã������.");
            // ���⿡ "������ ã������" �ؽ�Ʈ�� Ȱ��ȭ�ϴ� �ڵ带 �߰��ϼ���.
        }
        else
        {
            Debug.Log("City " + (pieceIndex + 1) + " ��ư Ŭ����");
            // ���⿡ ������ ������ ���� �ڵ带 �߰��ϼ���.
        }
    }

}
