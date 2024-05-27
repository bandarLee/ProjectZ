using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Temperature : MonoBehaviour
{
    public static UI_Temperature Instance { get; private set; }
    public CharacterStatAbility MyCharacterAbility;

    public RectTransform temperatureBar;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (MyCharacterAbility == null || MyCharacterAbility.Stat == null)
        {
            return;
        }

        TemperatureUI(MyCharacterAbility.Stat.Temperature);
    }

    public void SetTemperature(int temperature)
    {
        if (MyCharacterAbility != null && MyCharacterAbility.Stat != null)
        {
            MyCharacterAbility.Stat.Temperature = temperature;
            TemperatureUI(temperature);
        }
    }

    private void TemperatureUI(int temperature)
    {
        // 온도에 따른 UI 회전값 조정
        if (temperature <= -10) // 매우 추움
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 30);
            temperatureBar.localPosition = new Vector3(-221, -133, 0);
        }
        else if (temperature <= 0) // 추움
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, -25);
            temperatureBar.localPosition = new Vector3(-225, 130, 0);
        }
        else if (temperature >= 40) // 매우더움
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, -20);
            temperatureBar.localPosition = new Vector3(230, -100, 0);
        }
        else if (temperature >= 30)  // 더움
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 30);
            temperatureBar.localPosition = new Vector3(225, 136, 0);
        }
        else if (temperature > 15) // 보통
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 75);
            temperatureBar.localPosition = new Vector3(73, 250, 0);
        }
        else if (temperature == 15) // 보통
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 90);
            temperatureBar.localPosition = new Vector3(0, 250, 0);
        }
        else   // 보통
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, -60);
            temperatureBar.localPosition = new Vector3(-133, 250, 0);
        }
    }
}
