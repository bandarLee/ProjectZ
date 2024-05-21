using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class CharacterMoveAbility : MonoBehaviour
{
    // ĳ���� �̵� ��� ���� [WASD] Ű
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
        // 1. �Է¹ޱ�
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. ���ⱸ�ϱ�
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);

        // 3-1. �߷°� ����
        dir.y = -1f;

        // �޸��� ����
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

        // 3. �̵��ϱ�
        _characterController.Move(dir * speed * Time.deltaTime);
        //_animator.SetFloat("Move");
    }
}
