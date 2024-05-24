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

        ClockSlider.maxValue = 86400f;

        Sun.rectTransform.anchoredPosition = Vector2.zero;
        Moon.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (dayNightCycleManager != null)
        {
            ClockSlider.value = dayNightCycleManager.TimeOfDay  ;

            UpdateSunAndMoonVisibility();
            Debug.Log(dayNightCycleManager.TimeOfDay);
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
