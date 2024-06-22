using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Map : MonoBehaviourPunCallbacks
{
    public GameObject[] CityButtons;
    public GameObject[] LockImages;
    public TextMeshProUGUI[] CityTexts;
    public TextMeshProUGUI FindMapText;
    public GameObject[] MapPieceImages;
    private bool[] CityPiecesFound = new bool[6];
    private bool mapOpen = false; // ������ ���� �ִ��� ���¸� ����

    private MapController mapController; // MapController�� �����ϱ� ���� ����

    private void Start()
    {
        InitializeMap();
        AssignButtonEvents();
        SyncMapPieces();

        // MapController�� ã���ϴ�.
        mapController = FindObjectOfType<MapController>();

        // �ʱ⿡�� ��� �������� ��Ȱ��ȭ�մϴ�.
        if (mapController != null)
        {
            mapController.SetPlayerIconActive(false);
            mapController.SetOtherPlayerIconsActive(false);
        }
    }

    // �ʱ� ���� ���� �޼���
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

    // ���� ���� ��� �޼���
    [PunRPC]
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
        ShowMapPiece(pieceIndex);

        // ���� �÷��̾��� Ŀ���� �Ӽ� ������Ʈ
        UpdatePlayerMapProperties();
        // ������ Ŭ���̾�Ʈ�� ��� ���� Ŀ���� �Ӽ� ������Ʈ = �̸� ����� ���� ������ �����ϱ� ����
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateRoomMapProperties();
        }

    }
    private void UpdateRoomMapProperties()
    {
        Hashtable roomProps = new Hashtable
    {
        { "MapPieces", CityPiecesFound }
    };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }

    // RPC ȣ�� �޼��� �߰�
    public void RegisterMapPieceRPC(int pieceIndex)
    {
        photonView.RPC("RegisterMapPiece", RpcTarget.OthersBuffered, pieceIndex);
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
            StartCoroutine(ShowFindMapText());
        }
        else
        {
            Debug.Log("City " + (pieceIndex + 1) + " ��ư Ŭ����");
            FindMapText.gameObject.SetActive(false);
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

    // "������ ã������" �ؽ�Ʈ�� ���� �ð� ���� �����ִ� �ڷ�ƾ
    private IEnumerator ShowFindMapText()
    {
        FindMapText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f); // 3�� ���� �ؽ�Ʈ ǥ��
        FindMapText.gameObject.SetActive(false);
    }

    // ��ư Ŭ�� �̺�Ʈ ���� �޼���
    private void AssignButtonEvents()
    {
        for (int i = 0; i < CityButtons.Length; i++)
        {
            int index = i; // Ŭ���� ������ ���ϱ� ���� �ε����� ���� ������ ����
            CityButtons[i].GetComponent<Button>().onClick.AddListener(() => OnCityButtonClick(index));
        }
    }

    // ������ �� �� ȣ��Ǵ� �޼���
    public void OpenMap()
    {
        mapOpen = true; // ������ �����ִ� ���·� ����

        foreach (var button in CityButtons)
        {
            button.SetActive(true); // ��� ��ư�� Ȱ��ȭ
        }
        foreach (var mapPiece in MapPieceImages)
        {
            mapPiece.SetActive(false); // ��� ���� ������ ��Ȱ��ȭ
        }
        FindMapText.gameObject.SetActive(false); // "������ ã������" �ؽ�Ʈ�� ��Ȱ��ȭ

        // �÷��̾� �����ܰ� �ٸ� �÷��̾� �������� Ȱ��ȭ�մϴ�.
        if (mapController != null)
        {
            mapController.SetPlayerIconActive(true);
            mapController.SetOtherPlayerIconsActive(true);
        }
    }

    // ������ ���� �� ȣ��Ǵ� �޼��� �߰�
    public void CloseMap()
    {
        mapOpen = false; // ������ �����ִ� ���·� ����

        foreach (var button in CityButtons)
        {
            button.SetActive(false); // ��� ��ư�� ��Ȱ��ȭ
        }
        foreach (var mapPiece in MapPieceImages)
        {
            mapPiece.SetActive(false); // ��� ���� ������ ��Ȱ��ȭ
        }
        FindMapText.gameObject.SetActive(false); // "������ ã������" �ؽ�Ʈ�� ��Ȱ��ȭ

        // �÷��̾� �����ܰ� �ٸ� �÷��̾� �������� ��Ȱ��ȭ�մϴ�.
        if (mapController != null)
        {
            mapController.SetPlayerIconActive(false);
            mapController.SetOtherPlayerIconsActive(false);
        }
    }

    // ������ �����ִ��� Ȯ���ϴ� �޼��� �߰�
    public bool IsMapOpen()
    {
        return mapOpen;
    }

    // ������ ���� ���¸� �����ϴ� �޼��� �߰�
    public void KeepMapOpen()
    {
        mapOpen = true;
    }

    // ���� ���� ������ ��� �÷��̾�� ����ȭ
    private void SyncMapPieces()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MapPieces"))
        {
            bool[] syncedPieces = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["MapPieces"];
            for (int i = 0; i < syncedPieces.Length; i++)
            {
                if (syncedPieces[i] && !CityPiecesFound[i])
                {
                    CityPiecesFound[i] = true;
                    LockImages[i].SetActive(false);
                    CityTexts[i].gameObject.SetActive(true);
                    if (mapOpen)
                    {
                        ShowMapPiece(i);
                    }
                }
            }
        }
    }

    // ���� �÷��̾��� Ŀ���� �Ӽ� ������Ʈ
    private void UpdatePlayerMapProperties()
    {
        Hashtable props = new Hashtable
        {
            { "MapPieces", CityPiecesFound }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // ���ο� �÷��̾ ���� �� ���� ���� ������ ����ȭ
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        SyncMapPieces();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("MapPieces"))
        {
            bool[] updatedPieces = (bool[])changedProps["MapPieces"];
            for (int i = 0; i < updatedPieces.Length; i++)
            {
                if (updatedPieces[i] && !CityPiecesFound[i])
                {
                    CityPiecesFound[i] = true;
                    LockImages[i].SetActive(false);
                    CityTexts[i].gameObject.SetActive(true);
                    if (mapOpen)
                    {
                        ShowMapPiece(i);
                    }
                }
            }
        }
    }
}
