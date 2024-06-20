using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage = 20;
    public float Force = 30f;
    private Rigidbody rb;
    private bool hasDamaged = false;

    private void Start()
    {
       rb = GetComponent<Rigidbody>();
       rb.AddForce(transform.forward * Force);
       Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDamaged) return;

        var damageable = other.GetComponent<IDamaged>();
        if (damageable != null)
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                photonView.RPC("Damaged", RpcTarget.All, Damage * (Character.LocalPlayerInstance._statability.Stat.Damage), PhotonNetwork.LocalPlayer.ActorNumber);
                hasDamaged = true;
            } 
        }
    }
}
