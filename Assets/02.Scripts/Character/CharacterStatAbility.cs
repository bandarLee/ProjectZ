using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CharacterStatAbility : CharacterAbility
{
    public Stat Stat;
    public State State;

    public GameTime gameTime;
    private UI_Effect _uiEffect;

    private void Start()
    {
        gameTime = FindObjectOfType<GameTime>();
        _uiEffect = FindObjectOfType<UI_Effect>();

        if (Owner.PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacterAbility = this;
            UI_Temperature.Instance.MyCharacterAbility = this;

            StartCoroutine(HungerRoutine());
            StartCoroutine(DecreaseMentalRoutine());
            StartCoroutine(CheckTemperatureRoutine());
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
            yield return new WaitForSeconds(3);
            Stat.Hunger -= 1;
            LimitStat();
        }
    }

    public void LimitStat()
    {
        if (Stat.Hunger <= 0)
        {
            Stat.Hunger = 0;
        }
        if (Stat.Hunger >= Stat.MaxHunger)
        {
            Stat.Hunger = Stat.MaxHunger;
        }
        if (Stat.Mental >= Stat.MaxMental)
        {
            Stat.Mental = Stat.MaxMental;
        }

        if (Stat.Health <= 0)
        {
            if (Owner.PhotonView.IsMine)
            {
                Owner.Death();
            }
        }
    }

    private IEnumerator DecreaseMentalRoutine()
    {
        while (State != State.Death)
        {
            yield return new WaitForSeconds(10);
            if (Stat.Temperature <= 0 || Stat.Temperature >= 30)
            {
                Stat.Mental -= 5;
            }

            if (Stat.Hunger <= 0)
            {
                Stat.Mental -= 10;
            }

            if (Stat.Temperature <= -10 || Stat.Temperature >= 40)
            {
                Stat.Mental -= 10;
            }

            if (gameTime.CurrentTimeType == GameTime.TimeType.Night)
            {
                Stat.Mental -= 5;
            }

            if (Stat.Mental <= 0)
            {
                Stat.Mental = 0;
                StartCoroutine(DecreaseHealthRoutine());
            }
        }
    }

    private IEnumerator DecreaseHealthRoutine()
    {
        while (Stat.Mental <= 0 && State != State.Death)
        {
            yield return new WaitForSeconds(10);
            Stat.Health -= 10;
            LimitStat();
        }
    }

    public IEnumerator IncreaseTemperatureRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Stat.Temperature += 5;

            // 정신력 증가
            Stat.Mental += 5;
            if (Stat.Mental > Stat.MaxMental)
            {
                Stat.Mental = Stat.MaxMental;
            }

            if (Owner.PhotonView.IsMine)
            {
                UI_Temperature.Instance.SetTemperature(Stat.Temperature);
                // 정신력 UI 업데이트
                UI_CharacterStat.Instance.SetMental(Stat.Mental);
            }
        }
    }

    private IEnumerator IncreaseMentalRoutine(int amount)
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Stat.Mental += amount;
            LimitStat();

            if (Owner.PhotonView.IsMine)
            {
                UI_CharacterStat.Instance.SetMental(Stat.Mental); // 정신력 UI 업데이트
            }
        }
    }

    public void StartIncreaseMentalRoutine(int amount)
    {
        StartCoroutine(IncreaseMentalRoutine(amount));
    }

    private IEnumerator CheckTemperatureRoutine()
    {
        while (true)
        {
            CheckTemperature();
            yield return new WaitForSeconds(1f);
        }
    }

    private void CheckTemperature()
    {
        if (_uiEffect == null) return;

        if (Stat.Temperature <= -10)
        {
            _uiEffect.ShowVeryColdEffect();
        }
        else if (Stat.Temperature <= 0)
        {
            _uiEffect.ShowColdEffect();
        }
        else if (Stat.Temperature >= 40)
        {
            _uiEffect.ShowVeryHotEffect();
        }
        else if (Stat.Temperature >= 30)
        {
            _uiEffect.ShowHotEffect();
        }
        else
        {
            _uiEffect.HideTemperatureEffects();
        }
    }
}
