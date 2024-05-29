using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveAbilityTwo : CharacterAbility
{
    // 캐릭터 이동 기능 구현 [WASD] 키
    Animator _animator;
    Rigidbody _rigidbody;
    /*public float MoveSpeed = 5f;
    public float RunSpeed = 8f;*/

    private float _yVelocity = 0f;
    private float _gravity = -9.8f;
    private float _velocitySmoothing;
    public float JumpPower = 4f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true; // Rigidbody의 중력 사용 활성화
    }

    private void Update()
    {
        if (!Owner.PhotonView.IsMine)
        {
            return;
        }

        // 1. 입력받기
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 horizontalDir = Camera.main.transform.TransformDirection(new Vector3(h, 0, v));
        horizontalDir.y = 0; // Y 축 제거하여 수평 이동만 할 수 있도록 함
        horizontalDir.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? Owner.Stat.RunSpeed : Owner.Stat.MoveSpeed;
        Vector3 moveVelocity = horizontalDir * speed;

        // 4. 달리기 적용
        //float speed = Owner.Stat.MoveSpeed;
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

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            _animator.SetTrigger("Jump");
        }

        // 이동 로직 (점프 중 중력 적용을 위해 y-속도는 그대로 유지)
        moveVelocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = moveVelocity;

        // 3. 이동하기
        transform.position += moveVelocity * Time.deltaTime;

        
    }
    private bool IsGrounded()
    {
        // 간단한 지면 검사 로직 (바닥에 Raycast 등을 사용하여 구현)
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }
}
