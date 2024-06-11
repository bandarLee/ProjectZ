using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 20;
    public float Force = 1500f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * Force);
       // Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamaged>();
        if (damageable != null)
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                photonView.RPC("Damaged", RpcTarget.All, Damage, PhotonNetwork.LocalPlayer.ActorNumber);
            } 
        }
       // Destroy(gameObject);
    }
}
