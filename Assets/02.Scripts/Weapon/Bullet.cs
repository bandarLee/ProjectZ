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

    private PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

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

                photonView.RPC(nameof(SpawnExplosionRPC), RpcTarget.All, transform.position);


                Deactivate();
            } 
        }
    }
    [PunRPC]
    private void SpawnExplosionRPC(Vector3 position)
    {
        GameObject explosion = ObjectPool.Instance.SpawnFromPool("BigExplosion", position, Quaternion.identity);
        StartCoroutine(DisableExplosion(explosion));
    }

    private IEnumerator DisableExplosion(GameObject explosion)
    {
        yield return new WaitForSeconds(1f);
        explosion.SetActive(false);
    }
}
