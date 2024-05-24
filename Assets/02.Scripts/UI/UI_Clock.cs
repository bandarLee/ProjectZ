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

    public float timeOfDay; 

    private WeatherMakerDayNightCycleManagerScript dayNightCycleManager;

    private void Start()
    {
        dayNightCycleManager = WeatherMakerDayNightCycleManagerScript.Instance;
        timeOfDay = dayNightCycleManager.TimeOfDay - 31600;
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

            // ���� ���� �����ϰ� 0-86400 ������ ������ ����
            if (timeOfDay < 0)
            {
                timeOfDay += 86400f;
            }

            ClockSlider.value = timeOfDay % 86400f;

            UpdateSunAndMoonVisibility();
        }
    }
    public enum TimeType 
    {
        Day,
        Mystery,
        Night,
    }
    TimeType timeType = TimeType.Day;
    private void UpdateSunAndMoonVisibility()
    {
        // ���� �� �� �̹����� ǥ���ϰ� ���� �� �� �̹����� ǥ��
        if (timeOfDay >= 42240f && timeOfDay < 44160f)
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

