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

    // ���� �ֵ��� ����� ���� ����Ʈ
    private List<IDamaged> _damagedList = new List<IDamaged>();
    private int _activeWeaponIndex = -1;

    protected override void Awake() // �̴� ���� �޼ҵ� �Ǵ� �߻� �޼ҵ��� ��쿡�� ����� �� �ֽ��ϴ�.
    {
        base.Awake(); // �θ� Ŭ������ Awake �޼ҵ� ȣ��
        DeactivateAllWeapons(); // �߰����� �ڽ� Ŭ���� ����
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
                // �ǰ� ����Ʈ ����
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
        ActiveColliderRPC(index); // ���ÿ����� ����
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
        InActiveColliderRPC(index); // ���ÿ����� ����
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
