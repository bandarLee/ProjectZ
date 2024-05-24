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

    private void Start()
    {
        // WeatherMakerDayNightCycleManagerScript 인스턴스 가져오기
        dayNightCycleManager = WeatherMakerDayNightCycleManagerScript.Instance;

        // 슬라이더의 최대값을 하루의 총 시간으로 설정
        ClockSlider.maxValue =900f; 
    }

    private void Update()
    {
        if (dayNightCycleManager != null)
        {
            // 슬라이더 값을 현재 시간으로 설정
            ClockSlider.value = dayNightCycleManager.TimeOfDay;

            // 해와 달의 이미지를 슬라이더 값에 맞게 업데이트
            UpdateSunAndMoonPosition();
        }
    }

    private void UpdateSunAndMoonPosition()
    {
        // 해와 달의 이미지 이미지를 슬라이더 값에 맞게 업데이트
        float normalizedTime = ClockSlider.value / ClockSlider.maxValue;
        Sun.rectTransform.anchorMin = new Vector2(normalizedTime, Sun.rectTransform.anchorMin.y);
        Sun.rectTransform.anchorMax = new Vector2(normalizedTime, Sun.rectTransform.anchorMax.y);

        Moon.rectTransform.anchorMin = new Vector2(normalizedTime, Moon.rectTransform.anchorMin.y);
        Moon.rectTransform.anchorMax = new Vector2(normalizedTime, Moon.rectTransform.anchorMax.y);

        // 해와 달의 이미지 활성화/비활성화 설정 (필요에 따라 조정)
        Sun.gameObject.SetActive(dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Day));
        Moon.gameObject.SetActive(dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Night));
        //Mystery.gameObject.SetActive(dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Dawn) || dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Dusk));
    }
}

