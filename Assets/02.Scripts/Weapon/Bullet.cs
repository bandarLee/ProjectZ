using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{
    public float Damage = 20;
    public float Force = 30f;
    private Rigidbody rb;
    private bool hasDamaged = false;

    public void OnObjectSpawn()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * Force);
        hasDamaged = false;
        Invoke("Deactivate", 5f);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
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

                ObjectPool.Instance.SpawnFromPool("BigExplosion", transform.position, Quaternion.identity);
                Deactivate();
            } 
        }
    }
    
}
