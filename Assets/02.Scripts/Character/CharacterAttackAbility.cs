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

    // 때린 애들을 기억해 놓는 리스트
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
            Debug.Log("휘두름");

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
        // 개방 폐쇄 원칙 + 인터페이스 // 수정에는 닫혀있고, 확장에는 열려있다.
        IDamaged damagedAbleObject = other.GetComponent<IDamaged>();

        if (damagedAbleObject != null)
        {
            // 내가 이미 때렸던 애라면 안때리겠다..(2번 따닥 타격 안가도록)
            if (_damagedList.Contains(damagedAbleObject))
            {
                return;
            }
            // 안 맞은 애면 때린 리스트에 추가
            _damagedList.Add(damagedAbleObject);


            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                if (other.CompareTag("Monster"))
                {
                    Character.LocalPlayerInstance._effectAudioManager.PlayAudio(1);

                    // 피격 이펙트 생성
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
        ActiveColliderRPC(index); // 로컬에서도 실행
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
        DeactivateAllCollidersRPC(); // 로컬에서도 실행
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
        DeactivateAllWeaponsRPC(); // 로컬에서도 실행
    }
}
