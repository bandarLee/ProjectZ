using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour
{
    protected Character Owner { get; private set; }

    protected virtual void Awake()
    {
        Owner = GetComponentInParent<Character>();
    }
}
