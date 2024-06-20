using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveAbilityTwo : CharacterAbility
{
    // ĳ���� �̵� ��� ���� [WASD] Ű
    Animator _animator;
    Rigidbody _rigidbody;

    public float JumpPower = 6f;
    private bool _canJump = true;
    private bool _canMove = true;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true; // Rigidbody�� �߷� ��� Ȱ��ȭ
    }

    private void Update()
    {
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine || !_canMove)
        {
            return;
        }

        // 1. �Է¹ޱ�
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. ���ⱸ�ϱ�
        Vector3 horizontalDir = Camera.main.transform.TransformDirection(new Vector3(h, 0, v));
        horizontalDir.y = 0; // Y �� �����Ͽ� ���� �̵��� ��
        horizontalDir.Normalize();

        _animator.SetFloat("Horizontal", Mathf.Lerp(_animator.GetFloat("Horizontal"), h, Time.deltaTime * 8)); 
        _animator.SetFloat("Vertical", Mathf.Lerp(_animator.GetFloat("Vertical"), v, Time.deltaTime * 8));

        float speed = (Input.GetKey(KeyCode.LeftShift) ? Owner.Stat.RunSpeed : Owner.Stat.MoveSpeed) * horizontalDir.magnitude;
        Vector3 moveVelocity = horizontalDir * speed;
        moveVelocity.y = _rigidbody.velocity.y;  // ���� �ӵ� ���� (�߷°� ���� �� ����)

        float speedValue = horizontalDir.magnitude > 0 ? (Input.GetKey(KeyCode.LeftShift) && _canJump ? 1f : 0.5f) : 0f;
        float lerpTime = speedValue == 1f ? Time.deltaTime * 3 : speedValue == 0.5f ? Time.deltaTime * 5 : Time.deltaTime * 8;
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), speedValue, lerpTime));
        

        if (_canJump)
        {
            // ���� ����
            if (Input.GetKey(KeyCode.Space))
            {
                StartCoroutine(JumpCoroutine());

                _canJump = false;
            }
        }

        // 3. �̵��ϱ�
        transform.position += moveVelocity * Time.deltaTime;
    }
    private void RequestPlayAnimation(string animationName)
    {
        GetComponent<PhotonView>().RPC(nameof(PlayAnimation), RpcTarget.All, animationName);
    }

    public IEnumerator JumpCoroutine()
    {
        RequestPlayAnimation("Jump");

        _rigidbody.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);

        yield return new WaitForSeconds(1.25f);
        _canJump = true;

    }
    [PunRPC]
    private void PlayAnimation(string animationName)
    {
        _animator.SetTrigger(animationName);
    }


    public void Teleport(Vector3 newPosition)
    {


        _rigidbody.velocity = Vector3.zero; // ������ٵ��� �ӵ��� ���ӵ��� �Ͻ������� 0���� ����
        _rigidbody.angularVelocity = Vector3.zero;

        transform.position = newPosition; // ĳ������ ��ġ�� ���ο� ��ġ�� ����


    }
    public void PlayerMoveLock()
    {
        _canMove = false;
    }
    public void PlayerMoveFree()
    {
        _canMove = true;
    }
}