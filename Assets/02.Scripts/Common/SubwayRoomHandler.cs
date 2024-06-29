using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SubwayRoomHandler : MonoBehaviourPunCallbacks
{
    public bool isTryingToJoinSubway = false;
    public bool isTryingToJoinCity = false; // 추가된 플래그
    public bool isTryingToLastScene = false;
    private string subwayRoomName;
    private string cityRoomName;
    private string lastIslandName;
    public void InitiateSubwayRoomTransition(string currentRoomName)
    {
        switch (currentRoomName)
        {
            case "Server1":
                subwayRoomName = "Subway1";
                cityRoomName = "Server1";
                break;
            case "Server2":
                subwayRoomName = "Subway2";
                cityRoomName = "Server2";
                break;
            case "Server3":
                subwayRoomName = "Subway3";
                cityRoomName = "Server3";
                break;
            default:
                Debug.LogError("Unknown Room");
                return;
        }
        isTryingToJoinSubway = true;
        StartCoroutine(TryLeaveRoom());
    }
    public void InitiateLastIslandTransition(string currentRoomName)
    {
        switch (currentRoomName)
        {
            case "Subway1":
                lastIslandName = "LastIsland1";
                break;
            case "Subway2":
                lastIslandName = "LastIsland2";

                break;
            case "Subway3":
                lastIslandName = "LastIsland3";

                break;
            default:
                Debug.LogError("Unknown Room");
                return;
        }
        isTryingToLastScene = true;
        StartCoroutine(TryLeaveRoom());
    }
    public void InitiateCityRoomTransition()
    {
        cityRoomName = PhotonNetwork.CurrentRoom.Name.Replace("Subway", "Server");
        isTryingToJoinCity = true;
        StartCoroutine(TryLeaveRoom());
    }

    private IEnumerator TryLeaveRoom()
    {
        while (PhotonNetwork.NetworkClientState == ClientState.Leaving || PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            Debug.LogWarning("Not in room, retrying... Current state: " + PhotonNetwork.NetworkClientState);
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Leaving room...");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom called.");
        if (isTryingToJoinSubway)
        {
            Debug.Log("Left room, now joining lobby for subway...");
            PhotonNetwork.JoinLobby();
        }
        else if (isTryingToJoinCity)
        {
            Debug.Log("Left room, now joining lobby for city...");
            PhotonNetwork.JoinLobby();
        }
        else if (isTryingToLastScene)
        {
            Debug.Log("Left room, now joining lobby for city...");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby called.");
        if (isTryingToJoinSubway)
        {
            Debug.Log("Joined lobby, now joining or creating Subway room...");
            JoinOrCreateSubwayRoom();
        }
        else if (isTryingToJoinCity)
        {
            Debug.Log("Joined lobby, now joining or creating City room...");
            JoinOrCreateCityRoom();
        }
        else if (isTryingToLastScene)
        {
            Debug.Log("Left room, now joining lobby for city...");
            JoinOrLastRoom();
        }
    }

    private void JoinOrCreateSubwayRoom()
    {
        Debug.Log("Joining or creating Subway room...");
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 20 };
        PhotonNetwork.JoinOrCreateRoom(subwayRoomName, roomOptions, TypedLobby.Default);
    }

    private void JoinOrCreateCityRoom()
    {
        Debug.Log("Joining or creating City room...");
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 20 };
        PhotonNetwork.JoinOrCreateRoom(cityRoomName, roomOptions, TypedLobby.Default);
    }
    private void JoinOrLastRoom()
    {
        Debug.Log("Joining or creating City room...");
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 20 };
        PhotonNetwork.JoinOrCreateRoom(lastIslandName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (isTryingToJoinSubway)
        {
            Debug.Log("Joined Subway Room");
            isTryingToJoinSubway = false;
            PhotonNetwork.LoadLevel("SubwayScene");
        }
        else if (isTryingToJoinCity)
        {
            Debug.Log("Joined City Room");
            isTryingToJoinCity = false;
            PhotonNetwork.LoadLevel("CityScene");
        }
        else if (isTryingToLastScene)
        {
            Debug.Log("Joined Last Room");
            isTryingToJoinCity = false;
            PhotonNetwork.LoadLevel("LastIsLandScene");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster called.");
        if (isTryingToJoinSubway && !PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinLobby();
        }
        else if (isTryingToJoinCity && !PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinLobby();
        }
        else if (isTryingToLastScene && !PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from Photon with reason: {cause}");
        isTryingToJoinSubway = false;
        isTryingToJoinCity = false;
        isTryingToLastScene = false;
    }
}
