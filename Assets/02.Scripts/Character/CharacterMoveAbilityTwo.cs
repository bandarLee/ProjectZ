using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveAbilityTwo : CharacterAbility
{
    // 캐릭터 이동 기능 구현 [WASD] 키
    Animator _animator;
    Rigidbody _rigidbody;

    public float JumpPower = 6f;
    private bool _canJump = true;
    private bool _canMove = true;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true; // Rigidbody의 중력 사용 활성화
    }

    private void Update()
    {
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine || !_canMove)
        {
            return;
        }

        // 1. 입력받기
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. 방향구하기
        Vector3 horizontalDir = Camera.main.transform.TransformDirection(new Vector3(h, 0, v));
        horizontalDir.y = 0; // Y 축 제거하여 수평 이동만 함
        horizontalDir.Normalize();

        _animator.SetFloat("Horizontal", Mathf.Lerp(_animator.GetFloat("Horizontal"), h, Time.deltaTime * 8)); 
        _animator.SetFloat("Vertical", Mathf.Lerp(_animator.GetFloat("Vertical"), v, Time.deltaTime * 8));

        float speed = (Input.GetKey(KeyCode.LeftShift) ? Owner.Stat.RunSpeed : Owner.Stat.MoveSpeed) * horizontalDir.magnitude;
        Vector3 moveVelocity = horizontalDir * speed;
        moveVelocity.y = _rigidbody.velocity.y;  // 수직 속도 유지 (중력과 점프 힘 유지)

        float speedValue = horizontalDir.magnitude > 0 ? (Input.GetKey(KeyCode.LeftShift) && _canJump ? 1f : 0.5f) : 0f;
        float lerpTime = speedValue == 1f ? Time.deltaTime * 3 : speedValue == 0.5f ? Time.deltaTime * 5 : Time.deltaTime * 8;
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), speedValue, lerpTime));
        

        if (_canJump)
        {
            // 점프 로직
            if (Input.GetKey(KeyCode.Space))
            {
                StartCoroutine(JumpCoroutine());

                _canJump = false;
            }
        }

        // 3. 이동하기
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


        _rigidbody.velocity = Vector3.zero; // 리지드바디의 속도와 각속도를 일시적으로 0으로 설정
        _rigidbody.angularVelocity = Vector3.zero;

        transform.position = newPosition; // 캐릭터의 위치를 새로운 위치로 갱신


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