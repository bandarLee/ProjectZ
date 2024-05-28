using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¸ð´ÚºÒ¿¡ Æ®¸®°Å!");
            CharacterStatAbility characterStatAbility = other.GetComponent<CharacterStatAbility>();
            if (characterStatAbility != null)
            {
                characterStatAbility.StartCoroutine(characterStatAbility.IncreaseTemperatureRoutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¸ð´ÚºÒÀ» ¹þ¾î³²!");

            CharacterStatAbility characterStatAbility = other.GetComponent<CharacterStatAbility>();
            if (characterStatAbility != null)
            {
                characterStatAbility.StopCoroutine(characterStatAbility.IncreaseTemperatureRoutine());
            }
        }
    }
}
