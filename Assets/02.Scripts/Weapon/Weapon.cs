using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterAttackAbility MyCharacterAttackAbility;

    private void Start()
    {
        if (MyCharacterAttackAbility == null)
        {
            MyCharacterAttackAbility = GetComponentInParent<CharacterAttackAbility>(); // �θ� ���� ������Ʈ���� ã��
            if (MyCharacterAttackAbility == null)
            {
                Debug.LogError("CharacterAttackAbility component not found on the parent!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MyCharacterAttackAbility.OnTriggerEnter(other);
    }
}
