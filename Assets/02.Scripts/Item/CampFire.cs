using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    private Dictionary<CharacterStatAbility, Coroutine> activeCoroutines = new Dictionary<CharacterStatAbility, Coroutine>();

    private bool IsTrigger = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !IsTrigger )
        {
            IsTrigger = true;   
            CharacterStatAbility characterStatAbility = other.GetComponent<CharacterStatAbility>();
            if (characterStatAbility != null && !activeCoroutines.ContainsKey(characterStatAbility))
            {
                Coroutine coroutine = characterStatAbility.StartCoroutine(characterStatAbility.IncreaseTemperatureRoutine());
                activeCoroutines.Add(characterStatAbility, coroutine);
                StartCoroutine(TrashCode());

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsTrigger = false;
            Debug.Log("¸ð´ÚºÒÀ» ¹þ¾î³²!");
            CharacterStatAbility characterStatAbility = other.GetComponent<CharacterStatAbility>();
            if (characterStatAbility != null && activeCoroutines.ContainsKey(characterStatAbility))
            {
                characterStatAbility.StopCoroutine(activeCoroutines[characterStatAbility]);
                activeCoroutines.Remove(characterStatAbility);
            }
        }
    }

    public IEnumerator TrashCode()
    {
        yield return new WaitForSeconds(3f);
        IsTrigger = false;  
    }
}
