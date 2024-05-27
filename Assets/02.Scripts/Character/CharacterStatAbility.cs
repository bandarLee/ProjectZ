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
            yield return new WaitForSeconds(3); // 10�ʸ��� ����
            Stat.Hunger -= 1; // ����� ��ġ
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
            yield return new WaitForSeconds(10); // 10�ʸ��� ����
            if (Stat.Temperature <= 0 || Stat.Temperature >= 30)
            {
                Stat.Mental -= 5; // ���ŷ� 5 ����
            }

            if (Stat.Hunger <= 0)
            {
                Stat.Mental -= 10; // ���ŷ� 10 ����
            }

            if (Stat.Temperature <= -10 || Stat.Temperature >= 40)
            {
                Stat.Mental -= 10; // ���ŷ� 10 ����
            }

            //  ���� �� ���ŷ� ����(10�ʸ��� 5�� ����)
            if (gameTime.CurrentTimeType == GameTime.TimeType.Night)
            {
                Stat.Mental -= 5;       
            }

            if (Stat.Mental <= 0)
            {
                Stat.Mental = 0;
                StartCoroutine(DecreaseHealthRoutine()); // ü�� ���� ����
            }
        }
    }

    private IEnumerator DecreaseHealthRoutine()
    {
        while (Stat.Mental <= 0 && State != State.Death)
        {
            yield return new WaitForSeconds(10); // 10�ʸ��� ����
            Stat.Health -= 10; // ü�� ����
            if (Stat.Health <= 0)
            {
                Stat.Health = 0;
            }
        }
    }
}
