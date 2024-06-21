using Photon.Pun;
using UnityEngine;

public class FlashlightController : MonoBehaviourPunCallbacks, IPunObservable
{
    private Light Flashlight;
    private bool isFlashlightOn = false;

    void Start()
    {
        Flashlight = GetComponent<Light>();
    }

    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.F))
        {
            isFlashlightOn = !isFlashlightOn;
            Flashlight.enabled = isFlashlightOn;
            photonView.RPC("UpdateFlashlightState", RpcTarget.Others, isFlashlightOn);
        }
    }

    [PunRPC]
    void UpdateFlashlightState(bool state)
    {
        isFlashlightOn = state;
        Flashlight.enabled = isFlashlightOn;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFlashlightOn);
        }
        else
        {
            isFlashlightOn = (bool)stream.ReceiveNext();
            Flashlight.enabled = isFlashlightOn;
        }
    }
}
