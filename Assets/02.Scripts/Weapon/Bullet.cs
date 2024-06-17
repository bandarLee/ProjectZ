using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 20;
    public float Force = 30f;
    private Rigidbody rb;
    private bool hasDealtDamage = false;

    private void Start()
    {
       rb = GetComponent<Rigidbody>();
       rb.AddForce(transform.forward * Force);
       Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage)
        {
            return;
        }

        var damageable = other.GetComponent<IDamaged>();
        if (damageable != null)
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                hasDealtDamage = true;
                photonView.RPC("Damaged", RpcTarget.All, Damage, PhotonNetwork.LocalPlayer.ActorNumber);
            } 
        }
    }
}
