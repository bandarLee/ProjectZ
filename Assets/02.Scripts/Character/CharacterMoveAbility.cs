using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class CharacterMoveAbility : MonoBehaviour
{
    // 1] 캐릭터 이동 기능 구현 [WASD] 키
    CharacterController _characterController;
    Animator _animator;
    public float MoveSpeed = 5f;

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

        // 3. 이동하기
        _characterController.Move(dir * MoveSpeed * Time.deltaTime);
        //_animator.SetFloat("Move");
    }

}
