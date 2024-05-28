using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveAbility : CharacterAbility
{
    // ĳ���� �̵� ��� ���� [WASD] Ű
    CharacterController _characterController;
    Animator _animator;
    /*public float MoveSpeed = 5f;
    public float RunSpeed = 8f;*/

    private float _yVelocity = 0f;
    private float _gravity = -9.8f;
    private float _velocitySmoothing;
    //public float JumpPower = 3f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
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
        Vector3 horizontalDir = new Vector3(h, 0, v);
        horizontalDir.Normalize();
        horizontalDir = Camera.main.transform.TransformDirection(horizontalDir);

        // 3-1. �߷°� ����
        //_yVelocity += _gravity * Time.deltaTime;
        _yVelocity = Mathf.SmoothDamp(_yVelocity, _gravity * Time.deltaTime, ref _velocitySmoothing, 0.4f);
        Vector3 finalDir = new Vector3(horizontalDir.x, _yVelocity, horizontalDir.z);

        // 4. �޸��� ����
        float speed = Owner.Stat.MoveSpeed;
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

        // 3. �̵��ϱ�
        _characterController.Move(finalDir * speed * Time.deltaTime);

        // 5. ���� ����
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVelocity = Owner.Stat.JumpPower;
            _animator.SetTrigger("Jump");
        }
    }
}
