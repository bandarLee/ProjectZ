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
    public Image Mystery;

    private WeatherMakerDayNightCycleManagerScript dayNightCycleManager;

    public enum TimeType
    {
        Day,
        Mystery,
        Night,
    }

    public TimeType timeType = TimeType.Day;

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
        UpdateSunAndMoonVisibility();
    }

    private void UpdateSunAndMoonVisibility()
    {
        
            // 슬라이더 값을 현재 시간으로 설정 (하루의 중간 시간인 43200초를 기준으로)
            float timeOfDay = dayNightCycleManager.TimeOfDay - 31600;

            // 음수 값을 방지하고 0-86400 사이의 값으로 조정
            if (timeOfDay < 0)
            {
                timeOfDay += 86400f;
            }
      
        ClockSlider.value = timeOfDay % 86400f;
        // 낮일 때 해 이미지를 표시하고 밤일 때 달 이미지를 표시
        if (dayNightCycleManager.TimeOfDay >= 42240f && dayNightCycleManager.TimeOfDay < 44160f)
        {
            timeType = TimeType.Mystery;
        }
        else if (dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Day))
        {
            timeType = TimeType.Day;
        }
        else
        {
            timeType = TimeType.Night;
        }

        switch (timeType)
        {
            case TimeType.Day:
                Sun.gameObject.SetActive(true);
                Moon.gameObject.SetActive(false);
                Mystery.gameObject.SetActive(false);
                break;

            case TimeType.Night:
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(true);
                Mystery.gameObject.SetActive(false);
                break;

            case TimeType.Mystery:
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(false);
                Mystery.gameObject.SetActive(true);
                break;
        }
    }
}

