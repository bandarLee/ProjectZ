using System.Collections;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerPrecipitationManagerScript : MonoBehaviour, IPrecipitationManager
    {
        private WeatherMakerPrecipitationType precipitation = WeatherMakerPrecipitationType.None;

        [Header("Precipitation")]
        [Tooltip("Current precipitation")]
        public WeatherMakerPrecipitationType Precipitation = WeatherMakerPrecipitationType.None;

        [Tooltip("Intensity of precipitation (0-1)")]
        [Range(0.0f, 1.0f)]
        public float PrecipitationIntensity;

        [Tooltip("How long in seconds to fully change from one precipitation type to another")]
        [Range(0.0f, 300.0f)]
        public float PrecipitationChangeDuration = 4.0f;

        [Tooltip("How long to delay before applying a change in precipitation intensity.")]
        [Range(0.0f, 300.0f)]
        public float PrecipitationChangeDelay = 0.0f;

        [Tooltip("The threshold change in intensity that will cause a cross-fade between precipitation changes. Intensity changes smaller than this value happen quickly.")]
        [Range(0.0f, 0.2f)]
        public float PrecipitationChangeThreshold = 0.1f;

        [Header("Dependencies")]
        [Tooltip("Rain script")]
        public WeatherMakerFallingParticleScript RainScript;

        [Tooltip("Snow script")]
        public WeatherMakerFallingParticleScript SnowScript;

        [Tooltip("Hail script")]
        public WeatherMakerFallingParticleScript HailScript;

        [Tooltip("Sleet script")]
        public WeatherMakerFallingParticleScript SleetScript;

        [Tooltip("Set a custom precipitation script for use with Precipitation = WeatherMakerPrecipitationType.Custom ")]
        public WeatherMakerFallingParticleScript CustomPrecipitationScript;

        [Tooltip("Whether to allow precipitation to follow other cameras, such as reflection cameras")]
        public bool FollowNonNormalCameras;

        public WeatherMakerFallingParticleScript PrecipitationScript { get; private set; }

        private float lastPrecipitationIntensity = -1.0f;
        private float nextIntensityChangeSeconds = -1.0f;
        private RangeOfFloats nextIntensityChangeRange;
        private RangeOfFloats nextIntensityDurationRange;
        private Color precipitationTintColor = Color.white;
        private Color precipitationMistTintColor = Color.white;
        private Color precipitationSecondaryTintColor = Color.white;
        private bool transitionInProgress;

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
            StartCoroutine(ChangeWeatherRandomly());
            
        }


        private void LateUpdate()
        {
            CheckForPrecipitationChange();
        }

        private void OnDestroy()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        private void TweenPrecipitationScript(WeatherMakerFallingParticleScript script, float end)
        {
            if (PrecipitationChangeDuration < 0.1f)
            {
                script.Intensity = end;
                return;
            }

            float duration = (Mathf.Abs(script.Intensity - end) < PrecipitationChangeThreshold ? 0.0f : PrecipitationChangeDuration);
            FloatTween tween = TweenFactory.Tween("WeatherMakerPrecipitationChange_" + script.gameObject.GetInstanceID(), script.Intensity, end, duration, TweenScaleFunctions.Linear, (t) =>
            {
                script.Intensity = t.CurrentValue;
                transitionInProgress = true;
            }, (t) =>
            {
                transitionInProgress = false;
            });
            tween.Delay = PrecipitationChangeDelay;
            PrecipitationChangeDelay = 0.0f;
        }

        private void ChangePrecipitation(WeatherMakerFallingParticleScript newPrecipitation)
        {
            if (newPrecipitation != PrecipitationScript && PrecipitationScript != null)
            {
                TweenPrecipitationScript(PrecipitationScript, 0.0f);
                lastPrecipitationIntensity = -1.0f;
            }

            PrecipitationScript = newPrecipitation;
        }

        private void CheckForPrecipitationChange()
        {
            if (Precipitation != precipitation)
            {
                precipitation = Precipitation;
                switch (precipitation)
                {
                    default:
                        ChangePrecipitation(null);
                        break;

                    case WeatherMakerPrecipitationType.Rain:
                        ChangePrecipitation(RainScript);
                        break;

                    case WeatherMakerPrecipitationType.Snow:
                        ChangePrecipitation(SnowScript);
                        break;

                    case WeatherMakerPrecipitationType.Hail:
                        ChangePrecipitation(HailScript);
                        break;

                    case WeatherMakerPrecipitationType.Sleet:
                        ChangePrecipitation(SleetScript);
                        break;

                    case WeatherMakerPrecipitationType.Custom:
                        ChangePrecipitation(CustomPrecipitationScript);
                        break;
                }
            }

            if (nextIntensityChangeSeconds > 0.0f && (nextIntensityChangeSeconds -= Time.deltaTime) <= 0.0f)
            {
                if (!transitionInProgress)
                {
                    PrecipitationIntensity = nextIntensityChangeRange.Random();
                }
                nextIntensityChangeSeconds = nextIntensityDurationRange.Random();
            }

            if (PrecipitationScript != null && PrecipitationIntensity != lastPrecipitationIntensity)
            {
                lastPrecipitationIntensity = PrecipitationIntensity;
                TweenPrecipitationScript(PrecipitationScript, PrecipitationIntensity);
                PrecipitationScript.PrecipitationTintColor = precipitationTintColor;
                PrecipitationScript.PrecipitationMistTintColor = precipitationMistTintColor;
                PrecipitationScript.PrecipitationSecondaryTintColor = precipitationSecondaryTintColor;
            }
        }

        public void SetPrecipitationProfile(WeatherMakerPrecipitationProfileScript profile)
        {
            if (profile == null)
            {
                return;
            }

            Precipitation = profile.PrecipitationType;
            PrecipitationIntensity = profile.IntensityRange.Random();
            nextIntensityChangeSeconds = (profile.IntensityRangeDuration.Maximum <= 0.0f ? 0.0f : profile.IntensityRangeDuration.Random());
            nextIntensityChangeRange = profile.IntensityRange;
            nextIntensityDurationRange = profile.IntensityRangeDuration;
            precipitationTintColor = profile.PrecipitationTintColor;
            precipitationMistTintColor = profile.PrecipitationMistTintColor;
            precipitationSecondaryTintColor = profile.PrecipitationSecondaryTintColor;

            CheckForPrecipitationChange();
        }

        public void WeatherProfileChanged(WeatherMakerProfileScript oldProfile, WeatherMakerProfileScript newProfile, float transitionDelay, float transitionDuration)
        {
            SetPrecipitationProfile(newProfile.PrecipitationProfile);
        }

        private IEnumerator ChangeWeatherRandomly()
        {
            while (true)
            {
                yield return new WaitForSeconds(360f); // 6분마다 실행

                int randomWeather = Random.Range(0, 3);
                switch (randomWeather)
                {
                    case 0:
                        Precipitation = WeatherMakerPrecipitationType.None;
                        break;
                    case 1:
                        Precipitation = WeatherMakerPrecipitationType.Rain;
                        break;
                    case 2:
                        Precipitation = WeatherMakerPrecipitationType.Snow;
                        break;
                }
                Debug.Log($"날씨 : {randomWeather}");
                CheckForPrecipitationChange();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitOnLoad()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        public void GetSnowIntensityUnity(WeatherMakerOutputParameterFloat value) { value.Value = SnowIntensity; }
        public void GetWetnessIntensityUnity(WeatherMakerOutputParameterFloat value) { value.Value = Mathf.Max(RainIntensity, SleetIntensity); }

        public float RainIntensity { get { return RainScript.Intensity; } }
        public float SnowIntensity { get { return SnowScript.Intensity; } }
        public float HailIntensity { get { return HailScript.Intensity; } }
        public float SleetIntensity { get { return SleetScript.Intensity; } }
        public float CustomIntensity { get { return CustomPrecipitationScript.Intensity; } }

        private static WeatherMakerPrecipitationManagerScript instance;
        public static WeatherMakerPrecipitationManagerScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }

        public static bool HasInstance()
        {
            return instance != null;
        }
    }
}
