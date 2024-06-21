using Photon.Pun;
using UnityEngine;

public class HandFlashlightController : MonoBehaviourPunCallbacks
{
    public Transform CharacterFlashlightTransform; // ĳ���� �� ��ġ
    public Light flashlight; // ������ ����Ʈ

    private void Update()
    {
        if (photonView.IsMine)
        {
            flashlight.transform.position = CharacterFlashlightTransform.position;
            flashlight.transform.rotation = CharacterFlashlightTransform.rotation;

            Vector3 flashlightDirection = CharacterFlashlightTransform.forward;
            photonView.RPC("UpdateFlashlightDirection", RpcTarget.Others, flashlightDirection);
        }
    }

    [PunRPC]
    void UpdateFlashlightDirection(Vector3 direction)
    {
        flashlight.transform.forward = direction;
    }
}
