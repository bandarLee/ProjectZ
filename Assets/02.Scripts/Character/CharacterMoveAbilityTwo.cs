using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CharacterMoveAbilityTwo : CharacterAbility
{
    // ĳ���� �̵� ��� ���� [WASD] Ű
    Animator _animator;
    Rigidbody _rigidbody;

    private float _yVelocity = 0f;
    private float _gravity = -9.8f;
    private float _velocitySmoothing;

    public float JumpPower = 4f;
    private float JumpCOOLTIME = 2.6f;
    private float _lastJumpTime;
    private bool _canJump = true;
    private bool _isJump = false;

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

        Vector3 horizontalDir = Camera.main.transform.TransformDirection(new Vector3(h, 0, v));
        horizontalDir.y = 0; // Y �� �����Ͽ� ���� �̵��� ��
        horizontalDir.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? Owner.Stat.RunSpeed : Owner.Stat.MoveSpeed;
        Vector3 moveVelocity = horizontalDir * speed;

        // 4. �޸��� ����
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = Owner.Stat.RunSpeed;
            _animator.SetFloat("Move", Mathf.Lerp(_animator.GetFloat("Move"), 1.0f, Time.deltaTime * 3));
        }
        else
        {
            speed = Owner.Stat.MoveSpeed;
            if (horizontalDir.magnitude > 0)
            {
                _animator.SetFloat("Move", Mathf.Lerp(_animator.GetFloat("Move"), 0.5f, Time.deltaTime * 5));
            }
            else
            {
                _animator.SetFloat("Move", Mathf.Lerp(_animator.GetFloat("Move"), 0f, Time.deltaTime * 8));
            }
        }

        if (_canJump)
        {
            // ���� ����
            if (Input.GetKeyDown(KeyCode.Space) && Time.time - _lastJumpTime >= JumpCOOLTIME)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, JumpPower, _rigidbody.velocity.z);
                _lastJumpTime = Time.time;  // ���� �ð� ������Ʈ
                _canJump = false;
                _animator.SetTrigger("Jump");
            }
        }
            

        // ���� ��Ÿ�� üũ
        if (Time.time - _lastJumpTime >= JumpCOOLTIME && _rigidbody.velocity.y <= 0.1)
        {
            _canJump = true;  // ��Ÿ���� ������ �ٽ� ���� ����
        }


        /*// ���� �̵� ����
        moveVelocity.y = _rigidbody.velocity.y;  // ���� �ӵ� ����
        _rigidbody.velocity = moveVelocity;*/

        // 3. �̵��ϱ�
        transform.position += moveVelocity * Time.deltaTime;
    }
    

}
