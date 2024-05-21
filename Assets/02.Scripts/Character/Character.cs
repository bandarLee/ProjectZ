using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(Animator))]

public class Character : MonoBehaviour, IPunObservable
{
    public PhotonView PhotonView { get; private set; }

    public Stat Stat;

    private Animator _animator;

    private Vector3 _recivedPosition;
    private Quaternion _recivedRotation;

    private void Awake()
    {
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
            /*stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);*/
            //stream.SendNext(Stamina);
        }
        else if (stream.IsReading)   // �����͸� �����ϴ� ��Ȳ
        {
            // �����͸� ������ ������ �Ȱ��� ���� �����͸� ĳ�����ؾߵȴ�.
            /*_recivedPosition = (Vector3)stream.ReceiveNext();
            _recivedRotation = (Quaternion)stream.ReceiveNext();*/
            //Stat.Health = (int)stream.ReceiveNext();
            //Stat.Stamina = (float)stream.ReceiveNext();
        }
        // info�� �ۼ��� ����/���� ���ο� ���� �޽��� ����ִ�. 
    }
}
