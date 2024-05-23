using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAbility))]
[RequireComponent(typeof(Animator))]

public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public PhotonView PhotonView { get; private set; }

    public Stat Stat;
    public State State { get; private set; } = State.Live;
    private Animator _animator;

    private void Awake()
    {
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (!PhotonView.IsMine)
        {
            return;
        }
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
            stream.SendNext(Stat.Stamina);
        }
        else if (stream.IsReading)   // �����͸� �����ϴ� ��Ȳ
        {
            // �����͸� ������ ������ �Ȱ��� ���� �����͸� ĳ�����ؾߵȴ�.
            Stat.Health = (int)stream.ReceiveNext();
            Stat.Mental = (int)stream.ReceiveNext();
            Stat.Hunger = (int)stream.ReceiveNext();
            Stat.Temperature = (int)stream.ReceiveNext();
            Stat.Stamina = (float)stream.ReceiveNext();
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
    public void Damaged(int damage)
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

        //GetComponent<CharacterShakeAbility>().Shake();

    }

    private void OnDeath()
    {
        string logMessage = $"\n{PhotonView.Owner.NickName}�� ����� ���߽��ϴ�.";
        PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
    }

    [PunRPC]
    public void Hunger(int hunger)
    {
        if (State == State.Death)
        {
            return;
        }

        // �ð��� �帣�� ������� ���� �ڵ� (hunger�� �ö󰡴�)
        Stat.Hunger -= hunger;
        if (Stat.Hunger <= 0)
        {
            // ���ŷ��� 10�ʸ��� -10
            
        }
    }

    [PunRPC]
    public void Temperature(int temperature)
    {
        if (State == State.Death)
        {
            return;
        }
        Stat.Temperature -= temperature;
        if (Stat.Temperature <= 0 || Stat.Temperature >= 30)
        {
            // ���ŷ��� 10�ʸ��� -5
        }
        if (Stat.Temperature <= -10 || Stat.Temperature >= 40)
        {
            // ���ŷ��� 10�ʸ��� - 10
        }

    }

    [PunRPC]
    public void Mental(int mental)
    {
        if (State == State.Death)
        {
            return;
        }

        if (Stat.Mental <= 0)
        {
            // ü���� 10�ʸ��� -10
            Stat.Health -= mental;
        }
    }

    [PunRPC]
    private void Death()
    {
        if (State == State.Death)
        {
            return;
        }

        State = State.Death;

        /*GetComponent<Animator>().SetTrigger($"Die");
        GetComponent<CharacterAttackAbility>().InActiveCollider();*/

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

    }
}
