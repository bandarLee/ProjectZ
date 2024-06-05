using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterAttackAbility MyCharacterAttackAbility;

    // Gun ��ũ��Ʈó�� ���ݷ� �ֱ�-> �� ������Ʈ �ν�����â ����

    private void Start()
    {
        if (MyCharacterAttackAbility == null)
        {
            MyCharacterAttackAbility = GetComponentInParent<CharacterAttackAbility>(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MyCharacterAttackAbility.OnTriggerEnter(other);
    }
}
