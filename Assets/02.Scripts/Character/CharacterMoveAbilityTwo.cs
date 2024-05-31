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

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true; // Rigidbody�� �߷� ��� Ȱ��ȭ
    }

    private void Update()
    {
        if (!Owner.PhotonView.IsMine)
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
        _animator.SetFloat("Horizontal", h);
        _animator.SetFloat("Vertical", v);

        // ������ ������ ��ȯ
        /*float direction = Mathf.Atan2(horizontalDir.x, horizontalDir.z) * Mathf.Rad2Deg;
        _animator.SetFloat("Direction", direction);*/

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

    public IEnumerator JumpCoroutine()
    {
        _animator.SetTrigger("Jump");
        _rigidbody.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);

        yield return new WaitForSeconds(1.25f);
        _canJump = true;
    }
}