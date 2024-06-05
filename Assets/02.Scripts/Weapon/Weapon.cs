using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterAttackAbility MyCharacterAttackAbility;

    // Gun 스크립트처럼 공격력 넣기-> 각 오브젝트 인스펙터창 조정

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
