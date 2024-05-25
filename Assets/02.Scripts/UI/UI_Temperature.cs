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
    private void TemperatureUI(int temperature)
    {
        // �µ��� ���� UI ȸ���� ����
        if (temperature <= -10)
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 30);
            temperatureBar.localPosition = new Vector3(-221, -133, 0);
        }
        else if (temperature <= 0)
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, -30);
            temperatureBar.localPosition = new Vector3(-225, 136, 0);
        }
        else if (temperature >= 40)
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, -30);
            temperatureBar.localPosition = new Vector3(215, -133, 0);
        }
        else if (temperature >= 30)
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 30);
            temperatureBar.localPosition = new Vector3(225, 136, 0);
        }
        else
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 90);
            temperatureBar.localPosition = new Vector3(0, 250, 0);

            // �µ��� 0�� 30 ������ �� ������ �ε巴�� ȸ�� ó��
        }

    }
}
