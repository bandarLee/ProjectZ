using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public enum TimeType
    {
        Day,
        Mystery,
        Night,
    }

    public TimeType CurrentTimeType;
    public delegate void TimeTypeChangedHandler(TimeType newTimeType);
    public event TimeTypeChangedHandler OnTimeTypeChanged;

    void Start()
    {
        CurrentTimeType = TimeType.Day;
    }

    // TimeType을 변경할 수 있는 메서드 추가
    public void ChangeTimeType(TimeType newTimeType)
    {
        if (CurrentTimeType != newTimeType)
        {
            CurrentTimeType = newTimeType;
            OnTimeTypeChanged?.Invoke(newTimeType);
        }
    }
}
