using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CharacterMoveAbilityTwo : CharacterAbility
{
    // 캐릭터 이동 기능 구현 [WASD] 키
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
        horizontalDir.y = 0; // Y 축 제거하여 수평 이동만 함
        horizontalDir.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? Owner.Stat.RunSpeed : Owner.Stat.MoveSpeed;
        Vector3 moveVelocity = horizontalDir * speed;

        // 4. 달리기 적용
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
            // 점프 로직
            if (Input.GetKeyDown(KeyCode.Space) && Time.time - _lastJumpTime >= JumpCOOLTIME)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, JumpPower, _rigidbody.velocity.z);
                _lastJumpTime = Time.time;  // 점프 시간 업데이트
                _canJump = false;
                _animator.SetTrigger("Jump");
            }
        }
            

        // 점프 쿨타임 체크
        if (Time.time - _lastJumpTime >= JumpCOOLTIME && _rigidbody.velocity.y <= 0.1)
        {
            _canJump = true;  // 쿨타임이 지나면 다시 점프 가능
        }


        /*// 수평 이동 로직
        moveVelocity.y = _rigidbody.velocity.y;  // 수직 속도 유지
        _rigidbody.velocity = moveVelocity;*/

        // 3. 이동하기
        transform.position += moveVelocity * Time.deltaTime;
    }
    

}
