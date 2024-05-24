using DigitalRuby.WeatherMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Clock : MonoBehaviour
{
    public Slider ClockSlider;
    public Image Sun;
    public Image Moon;
    //public Image Mystery;

    private WeatherMakerDayNightCycleManagerScript dayNightCycleManager;

    private void Start()
    {
        dayNightCycleManager = WeatherMakerDayNightCycleManagerScript.Instance;

        Debug.Log(dayNightCycleManager.TimeOfDay);

        ClockSlider.maxValue = 86400f;

        Sun.rectTransform.anchoredPosition = Vector2.zero;
        Moon.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (dayNightCycleManager != null)
        {
            // �����̴� ���� ���� �ð����� ���� (�Ϸ��� �߰� �ð��� 43200�ʸ� ��������)
            float timeOfDay = dayNightCycleManager.TimeOfDay - 31600;

            // ���� ���� �����ϰ� 0-86400 ������ ������ ����
            if (timeOfDay < 0)
            {
                timeOfDay += 86400f;
            }

            ClockSlider.value = timeOfDay % 86400f;

            UpdateSunAndMoonVisibility();
        }
    }

    private void UpdateSunAndMoonVisibility()
    {
        // ���� �� �� �̹����� ǥ���ϰ� ���� �� �� �̹����� ǥ��
        bool isDay = dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Day);

        Sun.gameObject.SetActive(isDay);
        Moon.gameObject.SetActive(!isDay);
    }
}
