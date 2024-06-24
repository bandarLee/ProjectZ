using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigExplosionEffect : MonoBehaviour, IPooledObject
{
    public void OnObjectSpawn()
    {
        StartCoroutine(DeactivateAfterTime(0.5f));
    }

    private IEnumerator DeactivateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
