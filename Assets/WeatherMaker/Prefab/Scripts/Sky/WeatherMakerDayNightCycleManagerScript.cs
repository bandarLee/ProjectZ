using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Day night cycle manager
    /// </summary>
    [ExecuteInEditMode]
    public class WeatherMakerDayNightCycleManagerScript : MonoBehaviour
    {
        /// <summary>Day night cycle profile and color scheme</summary>
        [Tooltip("Day night cycle profile and color scheme")]
        public WeatherMakerDayNightCycleProfileScript DayNightProfile;

#if UNITY_EDITOR

#pragma warning disable 0414

        [ReadOnlyLabel]
        [SerializeField]
        private string TimeOfDayLabel = string.Empty;

#pragma warning restore 0414

#endif

        /// <summary>
        /// Day speed
        /// </summary>
        public float Speed { get { return DayNightProfile.Speed; } set { DayNightProfile.Speed = value; } }

        /// <summary>
        /// Night speed
        /// </summary>
        public float NightSpeed { get { return DayNightProfile.NightSpeed; } set { DayNightProfile.NightSpeed = value; } }

        /// <summary>
        /// Get / set the time of day in seconds, 0 to 86400
        /// </summary>
        public float TimeOfDay { get { return DayNightProfile.TimeOfDay; } set { DayNightProfile.TimeOfDay = value; } }

        /// <summary>
        /// Time of day category
        /// </summary>
        public WeatherMakerTimeOfDayCategory TimeOfDayCategory { get { return DayNightProfile.TimeOfDayCategory; } }

        /// <summary>
        /// Time of day as a TimeSpan object
        /// </summary>
        public TimeSpan TimeOfDayTimespan { get { return DayNightProfile.TimeOfDayTimespan; } set { DayNightProfile.TimeOfDayTimeSpan = value; } }

        /// <summary>
        /// Time zone offset in seconds
        /// </summary>
        public int TimeZoneOffsetSeconds { get { return DayNightProfile.TimeZoneOffsetSeconds; } set { DayNightProfile.TimeZoneOffsetSeconds = value; } }

        /// <summary>
        /// Year
        /// </summary>
        public int Year {  get { return DayNightProfile.Year; } set { DayNightProfile.Year = value; } }

        /// <summary>
        /// Month
        /// </summary>
        public int Month { get { return DayNightProfile.Month; } set { DayNightProfile.Month = value; } }

        /// <summary>
        /// Day
        /// </summary>
        public int Day { get { return DayNightProfile.Day; } set { DayNightProfile.Day = value; } }

        /// <summary>
        /// Get a date time object representing the current year, month, day and time of day in local time
        /// </summary>
        public DateTime DateTime
        {
            get { return DayNightProfile.DateTime; }
            set { DayNightProfile.DateTime = value; }
        }

        /// <summary>
        /// Latitude in degrees
        /// </summary>
        public double Latitude { get { return DayNightProfile.Latitude; } set { DayNightProfile.Latitude = value; } }

        /// <summary>
        /// Longitude in degrees
        /// </summary>
        public double Longitude { get { return DayNightProfile.Longitude; } set { DayNightProfile.Longitude = value; } }

        /// <summary>
        /// 1 if it is fully day
        /// </summary>
        public float DayMultiplier { get { return DayNightProfile.DayMultiplier; } }

        /// <summary>
        /// 1 if it is fully dawn or dusk
        /// </summary>
        public float DawnDuskMultiplier { get { return DayNightProfile.DawnDuskMultiplier; } }

        /// <summary>
        /// 1 if it is fully night
        /// </summary>
        public float NightMultiplier { get { return DayNightProfile.NightMultiplier; } }

        /// <summary>
        /// Sun data
        /// </summary>
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

        private static WeatherMakerDayNightCycleManagerScript instance;
        /// <summary>
        /// Shared instance of day/night cycle manager script
        /// </summary>
        public static WeatherMakerDayNightCycleManagerScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance, true); }
        }

        /// <summary>
        /// Check if there is a day night manager instance
        /// </summary>
        /// <returns>True if instance, false otherwise</returns>
        public static bool HasInstance()
        {
            return instance != null;
        }
    }
}
