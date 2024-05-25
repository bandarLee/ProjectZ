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
            temperatureBar.localEulerAngles = new Vector3(0, 0, 223);
        }
        else if (temperature <= 0)
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 170);
        }
        else if (temperature >= 30)
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, 10);
        }
        else if (temperature >= 40)
        {
            temperatureBar.localEulerAngles = new Vector3(0, 0, -10);
        }
        else
        {
            // �µ��� 0�� 30 ������ �� �ε巴�� ȸ�� ó��
            float rotation = Map(temperature, 0, 30, 170, 10); // ��) 20��, 0��, 30��, 170����, 10����
            temperatureBar.localEulerAngles = new Vector3(0, 0, rotation);
        }

    }

    // ��ġ�� ������ �°� �����ϴ� �Լ�
    float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}
