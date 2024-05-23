using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    [ExecuteInEditMode]
    public class DayNightCycleManager : MonoBehaviour
    {
        [Tooltip("Day night cycle profile and color scheme")]
        public WeatherMakerDayNightCycleProfileScript DayNightProfile;

#if UNITY_EDITOR

#pragma warning disable 0414

        [ReadOnlyLabel]
        [SerializeField]
        private string TimeOfDayLabel = string.Empty;

#pragma warning restore 0414

#endif

        public float Speed { get { return DayNightProfile.Speed; } set { DayNightProfile.Speed = value; } }

        public float NightSpeed { get { return DayNightProfile.NightSpeed; } set { DayNightProfile.NightSpeed = value; } }

        public float TimeOfDay { get { return DayNightProfile.TimeOfDay; } set { DayNightProfile.TimeOfDay = value; } }

        public WeatherMakerTimeOfDayCategory TimeOfDayCategory { get { return DayNightProfile.TimeOfDayCategory; } }

        public TimeSpan TimeOfDayTimespan { get { return DayNightProfile.TimeOfDayTimespan; } set { DayNightProfile.TimeOfDayTimeSpan = value; } }

        public int TimeZoneOffsetSeconds { get { return DayNightProfile.TimeZoneOffsetSeconds; } set { DayNightProfile.TimeZoneOffsetSeconds = value; } }

        public int Year {  get { return DayNightProfile.Year; } set { DayNightProfile.Year = value; } }

        public int Month { get { return DayNightProfile.Month; } set { DayNightProfile.Month = value; } }

        public int Day { get { return DayNightProfile.Day; } set { DayNightProfile.Day = value; } }

        public DateTime DateTime
        {
            get { return DayNightProfile.DateTime; }
            set { DayNightProfile.DateTime = value; }
        }

        public double Latitude { get { return DayNightProfile.Latitude; } set { DayNightProfile.Latitude = value; } }
        
        public double Longitude { get { return DayNightProfile.Longitude; } set { DayNightProfile.Longitude = value; } }

        public float DayMultiplier { get { return DayNightProfile.DayMultiplier; } }

        public float DawnDuskMultiplier { get { return DayNightProfile.DawnDuskMultiplier; } }

        public float NightMultiplier { get { return DayNightProfile.NightMultiplier; } }

        public WeatherMakerDayNightCycleProfileScript.SunInfo SunData { get { return DayNightProfile.SunData; } }

        private void EnsureProfile()
        {
            if (WeatherMakerScript.Instance == null)
            {
                return;
            }

            if (DayNightProfile == null)
            {
                DayNightProfile = WeatherMakerScript.Instance.LoadResource<WeatherMakerDayNightCycleProfileScript>("WeatherMakerDayNightCycleProfile_Default");
            }

            if (Application.isPlaying)
            {
                DayNightProfile = ScriptableObject.Instantiate(DayNightProfile);
            }
        }

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
        }

        private void Awake()
        {
            EnsureProfile();
        }

        private void Start()
        {
            EnsureProfile();
            DayNightProfile.UpdateFromProfile(WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer);
        }

        private void Update()
        {
            DayNightProfile.UpdateFromProfile(WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer);

#if UNITY_EDITOR

            TimeOfDayLabel = DayNightProfile.TimeOfDayLabel;

#endif

        }

        private void OnDestroy()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitOnLoad()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        private static DayNightCycleManager instance;

        public static DayNightCycleManager Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance, true); }
        }

        public static bool HasInstance()
        {
            return instance != null;
        }
    }
}