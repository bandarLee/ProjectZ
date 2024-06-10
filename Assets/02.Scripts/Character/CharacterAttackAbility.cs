using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackAbility : CharacterAbility
{
    private Animator _animator;
    private float _attackTimer = 0;
    public Collider[] WeaponCollider;
    public GameObject[] WeaponObject;
    public int[] WeaponDamage;

    // 때린 애들을 기억해 놓는 리스트
    private List<IDamaged> _damagedList = new List<IDamaged>();
    private int _activeWeaponIndex = -1;

    protected override void Awake() // 이는 가상 메소드 또는 추상 메소드인 경우에만 사용할 수 있습니다.
    {
        base.Awake(); // 부모 클래스의 Awake 메소드 호출
        DeactivateAllWeapons(); // 추가적인 자식 클래스 로직
    }

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
            Owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, 1);
        }
    }

    [PunRPC] 
    public void PlayAttackAnimation(int index)
    {
        _animator.SetTrigger($"Attack{index}");
    }

    [PunRPC]
    public void WeaponActive(int WeaponNumber)
    {
        if (!Owner.PhotonView.IsMine)
        {
            return;
        }
        foreach (GameObject weapon in WeaponObject)
        {
            weapon.SetActive(false);

        }
        WeaponObject[WeaponNumber].SetActive(true);
        _activeWeaponIndex = WeaponNumber;
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
                // 피격 이펙트 생성
                Vector3 hitPosition = (transform.position + other.transform.position) / 2f + new Vector3(0f, 1f, 0f);
                //PhotonNetwork.Instantiate("HitEffect", hitPosition, Quaternion.identity);
                int damage = WeaponDamage[_activeWeaponIndex];
                photonView.RPC("Damaged", RpcTarget.All, damage, Owner.PhotonView.OwnerActorNr);
            }
        }
    }

    [PunRPC]
    public void ActiveColliderRPC(int index)
    {
        WeaponCollider[index].enabled = true;
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
    public void InActiveColliderRPC(int index)
    {
        WeaponCollider[index].enabled = false;
        _damagedList.Clear();
    }

    public void InActiveCollider(int index)
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(InActiveColliderRPC), RpcTarget.All, index);
        }
        InActiveColliderRPC(index); // 로컬에서도 실행
    }

    [PunRPC]
    public void DeactivateAllCollidersRPC()
    {
        foreach (Collider collider in WeaponCollider)
        {
            collider.enabled = false;
        }
        _damagedList.Clear();
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
