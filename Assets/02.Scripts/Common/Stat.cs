using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // 데이터 직렬화
public class Stat 
{
    public int Health;
    public int MaxHealth;
    public int Mental;
    public int MaxMental;
    public int Hunger;
    public int MaxHunger;
    public int Temperature;
    public int StandardTemperature;

    public int Damage;

    public float MoveSpeed;
    public float RunSpeed;
    public float RotationSpeed;

    public float AttackCoolTime;

    public float JumpPower;

    public void Init()
    {
        Health = MaxHealth;
        Mental = MaxMental;
        Hunger = MaxHunger;
        Temperature = StandardTemperature;
    }
}
