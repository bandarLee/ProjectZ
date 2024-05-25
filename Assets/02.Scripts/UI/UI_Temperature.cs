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
        // 온도에 따른 UI 회전값 조정
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
            // 온도가 0과 30 사이일 때 부드럽게 회전 처리
            float rotation = Map(temperature, 0, 30, 170, 10); // 예) 20도, 0도, 30도, 170각도, 10각도
            temperatureBar.localEulerAngles = new Vector3(0, 0, rotation);
        }

    }

    // 수치를 범위에 맞게 매핑하는 함수
    float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}
