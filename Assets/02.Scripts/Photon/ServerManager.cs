using Photon.Pun;

public class ServerManager : MonoBehaviourPunCallbacks
{
    private readonly string _version = "0.0.1";
   

    private void Start()
    {
        PhotonNetwork.GameVersion = _version;

     
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
    }
}