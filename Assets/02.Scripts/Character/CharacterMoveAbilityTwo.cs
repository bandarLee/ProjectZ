using System.Collections;
using System.Collections.Generic;
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
    private bool _canJump = true;

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

        float speed = (Input.GetKey(KeyCode.LeftShift) ? Owner.Stat.RunSpeed : Owner.Stat.MoveSpeed) * horizontalDir.magnitude;
        Vector3 moveVelocity = horizontalDir * speed;
        moveVelocity.y = _rigidbody.velocity.y;  // 수직 속도 유지 (중력과 점프 힘 유지)

        if (_canJump)
        {
            // 점프 로직
            if (Input.GetKey(KeyCode.Space))
            {
                StartCoroutine(JumpCoroutine());

                _canJump = false;
            }
        }
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

        // 3. 이동하기
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
