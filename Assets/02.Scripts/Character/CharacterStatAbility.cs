using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatAbility : CharacterAbility
{
    public Stat Stat;
    public State State;

    public GameTime gameTime;  

    private void Start()
    {
        gameTime = FindObjectOfType<GameTime>();

        if (Owner.PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacterAbility = this;
            UI_Temperature.Instance.MyCharacterAbility = this;

            StartCoroutine(HungerRoutine());
            StartCoroutine(DecreaseMentalRoutine());
        }
    }

    [PunRPC]
    public void Temperature(int temperature)
    {
        if (State == State.Death)
        {
            return;
        }
        Stat.Temperature += temperature;

    }

    private IEnumerator HungerRoutine()
    {
        while (State != State.Death)
        {
            yield return new WaitForSeconds(3); // 10초마다 실행
            Stat.Hunger -= 1; // 배고픔 수치
            if (Stat.Hunger < 0)
            {
                Stat.Hunger = 0;
            }
        }
    }

    private IEnumerator DecreaseMentalRoutine()
    {
        while (State != State.Death)
        {
            yield return new WaitForSeconds(10); // 10초마다 실행
            if (Stat.Temperature <= 0 || Stat.Temperature >= 30)
            {
                Stat.Mental -= 5; // 정신력 5 감소
            }

            if (Stat.Hunger <= 0)
            {
                Stat.Mental -= 10; // 정신력 10 감소
            }

            if (Stat.Temperature <= -10 || Stat.Temperature >= 40)
            {
                Stat.Mental -= 10; // 정신력 10 감소
            }

            //  밤일 때 정신력 감소(10초마다 5씩 감소)
            if (gameTime.CurrentTimeType == GameTime.TimeType.Night)
            {
                Stat.Mental -= 5;       
            }

            if (Stat.Mental <= 0)
            {
                Stat.Mental = 0;
                StartCoroutine(DecreaseHealthRoutine()); // 체력 감소 시작
            }
        }
    }

    private IEnumerator DecreaseHealthRoutine()
    {
        while (Stat.Mental <= 0 && State != State.Death)
        {
            yield return new WaitForSeconds(10); // 10초마다 실행
            Stat.Health -= 10; // 체력 감소
            if (Stat.Health <= 0)
            {
                Stat.Health = 0;
            }
        }
    }
}
