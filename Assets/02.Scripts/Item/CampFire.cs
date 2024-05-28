using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    private Dictionary<CharacterStatAbility, Coroutine> activeCoroutines = new Dictionary<CharacterStatAbility, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("��ںҿ� Ʈ����!");
            CharacterStatAbility characterStatAbility = other.GetComponent<CharacterStatAbility>();
            if (characterStatAbility != null && !activeCoroutines.ContainsKey(characterStatAbility))
            {
                Coroutine coroutine = characterStatAbility.StartCoroutine(characterStatAbility.IncreaseTemperatureRoutine());
                activeCoroutines.Add(characterStatAbility, coroutine);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("��ں��� ���!");
            CharacterStatAbility characterStatAbility = other.GetComponent<CharacterStatAbility>();
            if (characterStatAbility != null && activeCoroutines.ContainsKey(characterStatAbility))
            {
                characterStatAbility.StopCoroutine(activeCoroutines[characterStatAbility]);
                activeCoroutines.Remove(characterStatAbility);
            }
        }
    }
}
