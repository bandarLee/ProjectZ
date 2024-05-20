using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class CharacterMoveAbility : MonoBehaviour
{
    // 1] ĳ���� �̵� ��� ���� [WASD] Ű
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
        // 1. �Է¹ޱ�
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. ���ⱸ�ϱ�
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);

        // 3-1. �߷°� ����
        dir.y = -1f;

        // 3. �̵��ϱ�
        _characterController.Move(dir * MoveSpeed * Time.deltaTime);
        //_animator.SetFloat("Move");
    }

}
