using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

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
        dir.y = -1f;

        // 달리기 적용
        float speed = MoveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Stamina > 0)
            {
                speed = RunSpeed;
                Stamina -= Time.deltaTime * RunConsumeStamina;
            }
            else
            {
                speed = MoveSpeed;
            }
        }
        else
        {
            Stamina += Time.deltaTime * RecoveryStamina;
        }

        Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);

        // 3. 이동하기
        _characterController.Move(dir * speed * Time.deltaTime);
        //_animator.SetFloat("Move");
    }
}
