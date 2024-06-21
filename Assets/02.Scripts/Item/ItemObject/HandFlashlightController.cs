using Photon.Pun;
using UnityEngine;

public class HandFlashlightController : MonoBehaviourPunCallbacks
{

    private void Update()
    {
        if (photonView.IsMine)
        {
            Character.LocalPlayerInstance._characterItemAbility.Flashlight.transform.position = Character.LocalPlayerInstance._characterItemAbility.ChracterFlashlightTransform.position;
            Character.LocalPlayerInstance._characterItemAbility.Flashlight.transform.rotation = Character.LocalPlayerInstance._characterItemAbility.ChracterFlashlightTransform.rotation;


            Vector3 flashlightDirection = Character.LocalPlayerInstance._characterItemAbility.ChracterFlashlightTransform.forward;
            photonView.RPC("UpdateFlashlightDirection", RpcTarget.Others, flashlightDirection);
        }
    }

    [PunRPC]
    void UpdateFlashlightDirection(Vector3 direction)
    {
       Character.LocalPlayerInstance._characterItemAbility.Flashlight.transform.forward = direction;
    }
}
