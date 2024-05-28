using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerPrecipitationManagerScript : MonoBehaviourPun, IPrecipitationManager, IWeatherMakerManager
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

        private static WeatherMakerPrecipitationManagerScript instance;
        public static WeatherMakerPrecipitationManagerScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }

        public static bool HasInstance()
        {
            return instance != null;
        }

        public float RainIntensity
        {
            get { return RainScript != null ? RainScript.Intensity : 0.0f; }
            set { if (RainScript != null) RainScript.Intensity = value; }
        }

        public float SnowIntensity
        {
            get { return SnowScript != null ? SnowScript.Intensity : 0.0f; }
            set { if (SnowScript != null) SnowScript.Intensity = value; }
        }

        public float HailIntensity
        {
            get { return HailScript != null ? HailScript.Intensity : 0.0f; }
            set { if (HailScript != null) HailScript.Intensity = value; }
        }

        public float SleetIntensity
        {
            get { return SleetScript != null ? SleetScript.Intensity : 0.0f; }
            set { if (SleetScript != null) SleetScript.Intensity = value; }
        }

        public float CustomIntensity
        {
            get { return CustomPrecipitationScript != null ? CustomPrecipitationScript.Intensity : 0.0f; }
            set { if (CustomPrecipitationScript != null) CustomPrecipitationScript.Intensity = value; }
        }

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
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

        [PunRPC]
        public void SyncPrecipitation(int weatherType, float intensity, float nextIntensityChangeSeconds)
        {
            switch (weatherType)
            {
                case 1:
                    Precipitation = WeatherMakerPrecipitationType.Rain;
                    break;
                case 2:
                    Precipitation = WeatherMakerPrecipitationType.Snow;
                    break;
                default:
                    Precipitation = WeatherMakerPrecipitationType.None;
                    break;
            }

            PrecipitationIntensity = intensity;
            this.nextIntensityChangeSeconds = nextIntensityChangeSeconds;
            CheckForPrecipitationChange();
        }

        public void SyncWeatherWithClients()
        {
            int weatherType = (int)Precipitation;
            photonView.RPC("SyncPrecipitation", RpcTarget.Others, weatherType, PrecipitationIntensity, nextIntensityChangeSeconds);
        }

        public void WeatherProfileChanged(WeatherMakerProfileScript oldProfile, WeatherMakerProfileScript newProfile, float transitionDelay, float transitionDuration)
        {
            SetPrecipitationProfile(newProfile.PrecipitationProfile);
        }
    }
}
