using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class CharacterAttackAbility : CharacterAbility
{
    private Animator _animator;
    private float _attackTimer = 0;

    public Collider WeaponCollider;
    public GameObject WeaponObject;

    // 때린 애들을 기억해 놓는 리스트
    private List<IDamaged> _damagedList = new List<IDamaged>();

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

        if (Input.GetMouseButtonDown(0) && _attackTimer > Owner.Stat.AttackCoolTime)
        {
            _attackTimer = 0f;
            Owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, 1);
            //PlayAttackAnimation(1);
        }
    }

    [PunRPC] 
    public void PlayAttackAnimation(int index)
    {
        _animator.SetTrigger($"Attack{index}");
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
                photonView.RPC("Damaged", RpcTarget.All, Owner.Stat.Damage, Owner.PhotonView.OwnerActorNr);
            }
            // damagedAbleObject.Damaged(Owner.Stat.Damage);
        }
    }

    public void ActiveCollider()
    {
        WeaponCollider.enabled = true;

    }
    public void InActiveCollider()
    {
        WeaponCollider.enabled = false;
        _damagedList.Clear();
    }
}
