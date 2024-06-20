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

    public float Damage;

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
    public void InitializeStat()
    {
        Health = 100;
        MaxHealth = 100;
        Mental = 100;
        MaxMental = 100;
        Hunger = 100;
        MaxHunger = 100;
        Temperature = 20;
        StandardTemperature = 20;
        Damage = 1;
        MoveSpeed = 10;
        RunSpeed = 1.5f * MoveSpeed;
        RotationSpeed = 200;
        AttackCoolTime = 5;
        JumpPower = 6;
    }
}
