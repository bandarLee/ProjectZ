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
        // WeatherMakerDayNightCycleManagerScript �ν��Ͻ� ��������
        dayNightCycleManager = WeatherMakerDayNightCycleManagerScript.Instance;

        // �����̴��� �ִ밪�� �Ϸ��� �� �ð����� ����
        ClockSlider.maxValue =900f; 
    }

    private void Update()
    {
        if (dayNightCycleManager != null)
        {
            // �����̴� ���� ���� �ð����� ����
            ClockSlider.value = dayNightCycleManager.TimeOfDay;

            // �ؿ� ���� �̹����� �����̴� ���� �°� ������Ʈ
            UpdateSunAndMoonPosition();
        }
    }

    private void UpdateSunAndMoonPosition()
    {
        // �ؿ� ���� �̹��� �̹����� �����̴� ���� �°� ������Ʈ
        float normalizedTime = ClockSlider.value / ClockSlider.maxValue;
        Sun.rectTransform.anchorMin = new Vector2(normalizedTime, Sun.rectTransform.anchorMin.y);
        Sun.rectTransform.anchorMax = new Vector2(normalizedTime, Sun.rectTransform.anchorMax.y);

        Moon.rectTransform.anchorMin = new Vector2(normalizedTime, Moon.rectTransform.anchorMin.y);
        Moon.rectTransform.anchorMax = new Vector2(normalizedTime, Moon.rectTransform.anchorMax.y);

        // �ؿ� ���� �̹��� Ȱ��ȭ/��Ȱ��ȭ ���� (�ʿ信 ���� ����)
        Sun.gameObject.SetActive(dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Day));
        Moon.gameObject.SetActive(dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Night));
        //Mystery.gameObject.SetActive(dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Dawn) || dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Dusk));
    }
}

