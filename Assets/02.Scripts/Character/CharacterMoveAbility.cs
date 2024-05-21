using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveAbility : MonoBehaviour
{
    // 캐릭터 이동 기능 구현 [WASD] 키
    CharacterController _characterController;
    Animator _animator;
    public float MoveSpeed = 5f;
    public float RunSpeed = 8f;

    public float Stamina = 100f;
    public float MaxStamina = 100f;
    public float RunConsumeStamina = 5f;
    public float RecoveryStamina = 7f;

    private float _yVelocity = 0f;
    private float _gravity = -9.8f;
    public float JumpPower = 5f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. 입력받기
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. 방향구하기
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);

        // 3-1. 중력값 적용
        _yVelocity += _gravity * Time.deltaTime;
        dir.y = _yVelocity;


        // 4. 달리기 적용
        float speed = MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && Stamina > 0)
        {
            speed = RunSpeed;
            _animator.SetFloat("Move", 1.0f); 
            Stamina -= Time.deltaTime * RunConsumeStamina;
        }
        else
        {
            speed = MoveSpeed;
            if (dir.magnitude > 0)
            {
                _animator.SetFloat("Move", 0.5f); 
            }
            else
            {
                _animator.SetFloat("Move", 0); 
            }
            Stamina += Time.deltaTime * RecoveryStamina;
        }

        Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);

        // 3. 이동하기
        _characterController.Move(dir * speed * Time.deltaTime);

        // 5. 점프 적용
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVelocity = JumpPower;
        }
    }
}
