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
    private bool mapOpen = false; // 지도가 열려 있는지 상태를 추적

    private MapController mapController; // MapController를 참조하기 위한 변수

    private void Start()
    {
        InitializeMap();
        AssignButtonEvents();
        SyncMapPieces();

        // MapController를 찾습니다.
        mapController = FindObjectOfType<MapController>();

        // 초기에는 모든 아이콘을 비활성화합니다.
        if (mapController != null)
        {
            mapController.SetPlayerIconActive(false);
            mapController.SetOtherPlayerIconsActive(false);
        }
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

        // 현재 플레이어의 커스텀 속성 업데이트
        UpdatePlayerMapProperties();
        // 마스터 클라이언트인 경우 방의 커스텀 속성 업데이트 = 미리 등록한 지도 조각을 공유하기 위함
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

    // RPC 호출 메서드 추가
    public void RegisterMapPieceRPC(int pieceIndex)
    {
        photonView.RPC("RegisterMapPiece", RpcTarget.OthersBuffered, pieceIndex);
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
        yield return new WaitForSeconds(3f); // 3초 동안 텍스트 표시
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
        mapOpen = true; // 지도가 열려있는 상태로 설정

        foreach (var button in CityButtons)
        {
            button.SetActive(true); // 모든 버튼을 활성화
        }
        foreach (var mapPiece in MapPieceImages)
        {
            mapPiece.SetActive(false); // 모든 지도 조각을 비활성화
        }
        FindMapText.gameObject.SetActive(false); // "지도를 찾으세요" 텍스트를 비활성화

        // 플레이어 아이콘과 다른 플레이어 아이콘을 활성화합니다.
        if (mapController != null)
        {
            mapController.SetPlayerIconActive(true);
            mapController.SetOtherPlayerIconsActive(true);
        }
    }

    // 지도를 닫을 때 호출되는 메서드 추가
    public void CloseMap()
    {
        mapOpen = false; // 지도가 닫혀있는 상태로 설정

        foreach (var button in CityButtons)
        {
            button.SetActive(false); // 모든 버튼을 비활성화
        }
        foreach (var mapPiece in MapPieceImages)
        {
            mapPiece.SetActive(false); // 모든 지도 조각을 비활성화
        }
        FindMapText.gameObject.SetActive(false); // "지도를 찾으세요" 텍스트를 비활성화

        // 플레이어 아이콘과 다른 플레이어 아이콘을 비활성화합니다.
        if (mapController != null)
        {
            mapController.SetPlayerIconActive(false);
            mapController.SetOtherPlayerIconsActive(false);
        }
    }

    // 지도가 열려있는지 확인하는 메서드 추가
    public bool IsMapOpen()
    {
        return mapOpen;
    }

    // 지도가 열린 상태를 유지하는 메서드 추가
    public void KeepMapOpen()
    {
        mapOpen = true;
    }

    // 현재 지도 정보를 모든 플레이어와 동기화
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

    // 현재 플레이어의 커스텀 속성 업데이트
    private void UpdatePlayerMapProperties()
    {
        Hashtable props = new Hashtable
        {
            { "MapPieces", CityPiecesFound }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // 새로운 플레이어가 들어올 때 현재 지도 정보를 동기화
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
