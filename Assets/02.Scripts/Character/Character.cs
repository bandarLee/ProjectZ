using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbilityTwo))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterStatAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterAbility))]
[RequireComponent(typeof(CharacterItemAbility))]

[RequireComponent(typeof(Animator))]

public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public PhotonView PhotonView { get; private set; }

    public Stat Stat { get; private set; }
    public State State { get; private set; } = State.Live;
    private Animator _animator;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        CharacterStatAbility ability = GetComponent<CharacterStatAbility>();
        Stat = ability.Stat;
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _inventoryManager = FindObjectOfType<InventoryManager>();

        if (_inventoryManager != null)
        {
            _inventoryManager.characterRotateAbility = GetComponent<CharacterRotateAbility>();

        }
    }

    private void Start()
    {
        if (!PhotonView.IsMine)
        {
            return;
        }

        SetSpawnPoint();
    }

    // ������ ����ȭ�� ���� ������ ���� �� ���� ����� ���� ���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream(���)�� �������� �ְ���� �����Ͱ� ����ִ� ����
        if (stream.IsWriting)        // �����͸� �����ϴ� ��Ȳ
        {
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Mental);
            stream.SendNext(Stat.Hunger);
            stream.SendNext(Stat.Temperature);
        }
        else if (stream.IsReading)   // �����͸� �����ϴ� ��Ȳ
        {
            // �����͸� ������ ������ �Ȱ��� ���� �����͸� ĳ�����ؾߵȴ�.
            Stat.Health = (int)stream.ReceiveNext();
            Stat.Mental = (int)stream.ReceiveNext();
            Stat.Hunger = (int)stream.ReceiveNext();
            Stat.Temperature = (int)stream.ReceiveNext();
        }
        // info�� �ۼ��� ����/���� ���ο� ���� �޽��� ����ִ�. 
    }
    [PunRPC]
    public void AddLog(string logMessage)
    {
        UI_RoomInfo.Instance.AddLog(logMessage);
    }

    // ü��
    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        if (State == State.Death)
        {
            return;
        }
        Stat.Health -= damage;
        if (Stat.Health <= 0)
        {
            if (PhotonView.IsMine)
            {
                OnDeath();
            }
            /* Death();*/
            PhotonView.RPC(nameof(Death), RpcTarget.All); // Death �Լ��� ȣ��
        }
        else
        {
            GetComponent<Animator>().SetTrigger($"Damage");
        }
            

    }

    private void OnDeath()
    {
        string logMessage = $"\n{PhotonView.Owner.NickName}�� ����� ���߽��ϴ�.";
        PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
    }


    [PunRPC]
    private void Death()
    {
        if (State == State.Death)
        {
            return;
        }

        State = State.Death;

        GetComponent<Animator>().SetTrigger($"Die");
        GetComponent<CharacterAttackAbility>().InActiveCollider();

        // �װ��� 5���� ������
        if (PhotonView.IsMine)
        {
            DropItems();
            StartCoroutine(Death_Coroutine());
        }
    }

    [PunRPC]
    private void DropItems()
    {

    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        SetSpawnPoint();
        PhotonView.RPC(nameof(Live), RpcTarget.All);
    }

    private void SetSpawnPoint()
    {
        Vector3 spawnPoint = TestScene.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterMoveAbilityTwo>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    [PunRPC]
    private void Live()
    {
        State = State.Live;

        CharacterStatAbility ability = GetComponent<CharacterStatAbility>();
        Stat = ability.Stat;
        Stat.Init();

        GetComponent<Animator>().SetTrigger("Live");
    }
}