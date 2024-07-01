using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackAbility : CharacterAbility
{
    private Animator _animator;
    private float _attackTimer = 0;
    public Collider WeaponCollider;
    public GameObject[] WeaponObject;
    public float ShovelDamage = 25;
    public float BatDamage = 30;
    public float AxeDamage = 35;

    // ���� �ֵ��� ����� ���� ����Ʈ
    private List<IDamaged> _damagedList = new List<IDamaged>();
    private int _activeWeaponIndex = -1;


    private void Start()
    {
        _animator = GetComponent<Animator>();

    }

    private void Update()
    {
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine)
        {
            return;
        }

        _attackTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _attackTimer > Owner.Stat.AttackCoolTime && _activeWeaponIndex != -1 && !Owner._characterRotateAbility.CharacterRotateLocked)
        {
            _attackTimer = 0f;
            Debug.Log("�ֵθ�");

            StartCoroutine(PerformAttack());

        }
    }
    public void SwingSound()
    {
        Character.LocalPlayerInstance._effectAudioManager.PlayAudio(0);

    }

    private IEnumerator PerformAttack()
    {
        Owner._animator.SetBool("DoAttack", true);

        yield return new WaitForSeconds(0.08f); 
        Owner._animator.SetBool("DoAttack", false);
    }


    [PunRPC]
    public void WeaponActiveRPC(int WeaponNumber)
    {
        foreach (GameObject weapon in WeaponObject)
        {
            weapon.SetActive(false);
        }

        StartCoroutine(WeaponActiveAfterDelay(WeaponNumber, 0.6f));
        _activeWeaponIndex = WeaponNumber;

        Owner._animator.SetBool("WeaponPullOut", true);
        StartCoroutine(TimeDelayWeapon());
    }
        IEnumerator TimeDelayWeapon()
    {
        yield return new WaitForSeconds(1f);
        Owner._animator.SetBool("WeaponPullOut", false);

    }
    private IEnumerator WeaponActiveAfterDelay(int WeaponNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (WeaponNumber >= 0 && WeaponNumber < WeaponObject.Length)
        {
            WeaponObject[WeaponNumber].SetActive(true);
            WeaponCollider = WeaponObject[WeaponNumber].GetComponent<Collider>();

        }
    }

    public void WeaponActive(int WeaponNumber)
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(WeaponActiveRPC), RpcTarget.All, WeaponNumber);
        }
        WeaponActiveRPC(WeaponNumber);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (Owner.PhotonView.IsMine == false || other.transform == transform)
        {
            return;
        }
        // ���� ��� ��Ģ + �������̽� // �������� �����ְ�, Ȯ�忡�� �����ִ�.
        IDamaged damagedAbleObject = other.GetComponent<IDamaged>();

        if (damagedAbleObject != null)
        {
            // ���� �̹� ���ȴ� �ֶ�� �ȶ����ڴ�..(2�� ���� Ÿ�� �Ȱ�����)
            if (_damagedList.Contains(damagedAbleObject))
            {
                return;
            }
            // �� ���� �ָ� ���� ����Ʈ�� �߰�
            _damagedList.Add(damagedAbleObject);


            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                if (other.CompareTag("Monster"))
                {
                    Character.LocalPlayerInstance._effectAudioManager.PlayAudio(1);

                    // �ǰ� ����Ʈ ����
                    Vector3 hitPosition = other.ClosestPoint(transform.position) + new Vector3(0f, 0.5f, 0f);
                    PhotonView ownerPhotonView = Owner.GetComponent<PhotonView>();
                    if (ownerPhotonView != null)
                    {
                        ownerPhotonView.RPC(nameof(SpawnHitEffectRPC), RpcTarget.All, hitPosition);
                    }
                }
                    
                float damage = GetWeaponDamage(_activeWeaponIndex);
                photonView.RPC("Damaged", RpcTarget.All, damage, Owner.PhotonView.OwnerActorNr);
            }
        }
    }
    [PunRPC]
    private void SpawnHitEffectRPC(Vector3 position)
    {
        GameObject hitEffect = ObjectPool.Instance.SpawnFromPool("ElectricalSparks", position, Quaternion.identity);
        StartCoroutine(DisableHitEffect(hitEffect));
    }

    private IEnumerator DisableHitEffect(GameObject hitEffect)
    {
        ParticleSystem particleSystem = hitEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startSizeX = 0.1f;
            main.startSizeY = 0.01f;
            main.startSizeZ = 0.01f;
        }

        yield return new WaitForSeconds(1f);
        hitEffect.SetActive(false);
    }
    private float GetWeaponDamage(int weaponIndex)
    {
        switch (weaponIndex)
        {
            case 0: return AxeDamage * (Owner.Stat.Damage);
            case 1: return BatDamage * (Owner.Stat.Damage);
            case 2: return ShovelDamage * (Owner.Stat.Damage);
            default: return 0;
        }
    }

    [PunRPC]
    public void ActiveColliderRPC(int index)
    {
        WeaponCollider.enabled = true;
    }

    public void ActiveCollider(int index)
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(ActiveColliderRPC), RpcTarget.All, index);
        }
        ActiveColliderRPC(index); // ���ÿ����� ����
    }


    [PunRPC]
    public void DeactivateAllCollidersRPC()
    {
        if(WeaponCollider != null)
        {
            WeaponCollider.enabled = false;

            _damagedList.Clear();

        }

    }

    public void DeactivateAllColliders()
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(DeactivateAllCollidersRPC), RpcTarget.All);
        }
        DeactivateAllCollidersRPC(); // ���ÿ����� ����
    }

    [PunRPC]
    public void DeactivateAllWeaponsRPC()
    {
        foreach (GameObject weapon in WeaponObject)
        {
            weapon.SetActive(false);
        }
        _activeWeaponIndex = -1;
       
        Owner._animator.SetBool("ReWeaponPullOut", true);
        Owner._animator.SetBool("WeaponPullOut", false);
    }

    public void DeactivateAllWeapons()
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(DeactivateAllWeaponsRPC), RpcTarget.All);
        }
        DeactivateAllWeaponsRPC(); // ���ÿ����� ����
    }
}
