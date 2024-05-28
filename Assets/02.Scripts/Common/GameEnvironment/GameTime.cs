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

    void Start()
    {
        CurrentTimeType = TimeType.Day;
    }
}