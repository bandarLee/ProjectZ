using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace Michsky.UI.Shift
{
    public class QuickMatchButton : MonoBehaviourPunCallbacks
    {
        [Header("Text")]
        public bool useCustomText = false;
        public string buttonTitle = "My Title";

        [Header("Image")]
        public bool useCustomImage = false;
        public Sprite backgroundImage;

        [Header("Photon Settings")]
        public string defaultRoomName = "QuickMatchRoom";
        public int maxPlayers = 4;

        TextMeshProUGUI titleText;
        Image image1;
        Button button;


        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(JoinOrCreateRoom);

            if (useCustomText == false)
            {
                titleText = gameObject.transform.Find("Content/Title").GetComponent<TextMeshProUGUI>();
                titleText.text = buttonTitle;
            }

            if (useCustomImage == false)
            {
                image1 = gameObject.transform.Find("Content/Background").GetComponent<Image>();
                image1.sprite = backgroundImage;
            }
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby(); // Ensure the client is in a lobby.
        }

        void JoinOrCreateRoom()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                RoomOptions options = new RoomOptions { MaxPlayers = (byte)maxPlayers };
                PhotonNetwork.JoinOrCreateRoom(defaultRoomName, options, TypedLobby.Default);
            }
            else
            {
                Debug.LogError("Photon Network is not ready. Check the network connection.");
            }
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom != null)
                Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
            else
                Debug.LogError("Failed to access the current room.");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Failed to join room: " + message);
        }
    }
}