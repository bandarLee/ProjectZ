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

    // ���� �ֵ��� ����� ���� ����Ʈ
    private List<IDamaged> _damagedList = new List<IDamaged>();

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

    [PunRPC]
    public void WeaponActive(int WeaponNumber)
    {
        foreach (GameObject weapon in WeaponObject)
        {
            weapon.SetActive(false);

        }
        WeaponObject[WeaponNumber].SetActive(true);
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
                photonView.RPC("Damaged", RpcTarget.All, Owner.Stat.Damage, Owner.PhotonView.OwnerActorNr);
            }
            //damagedAbleObject.Damaged(Owner.Stat.Damage);
        }
    }

    public void ActiveCollider(int index)
    {
        WeaponCollider[index].enabled = true;

    }
    public void InActiveCollider(int index)
    {
        WeaponCollider[index].enabled = false;
        _damagedList.Clear();
    }

    // ��� �ݶ��̴��� ��Ȱ��ȭ
    public void DeactivateAllColliders()
    {
        foreach (Collider collider in WeaponCollider)
        {
            collider.enabled = false;
        }
        _damagedList.Clear(); // ��Ȱ��ȭ�ϸ鼭 ���� ��ϵ� �ʱ�ȭ
    }

    public void DeactivateAllWeapons()
    {
        foreach (GameObject weapon in WeaponObject)
        {
            weapon.SetActive(false);
        }
    }
}
