using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// ��ƼŬ �ý����� ����Ͽ� ������ �����ϴ� ��ũ��Ʈ
    /// </summary>
    public class WeatherMakerFallingParticleScript : MonoBehaviour
    {
        // �� ���� ������ ���� ����� �ҽ�
        [Tooltip("Light particle looping audio source")]
        public AudioSource LoopSourceLight;

        [Tooltip("Medium particle looping audio source")]
        public AudioSource LoopSourceMedium;

        [Tooltip("Heavy particle looping audio source")]
        public AudioSource LoopSourceHeavy;

        // �߰� ���� ����� �Ӱ谪
        [Tooltip("Intensity threshold for medium looping sound")]
        public float SoundMediumIntensityThreshold = 0.33f;

        // ���� ���� ����� �Ӱ谪
        [Tooltip("Intensity threshold for heavy loop sound")]
        public float SoundHeavyIntensityThreshold = 0.67f;

        // ��ü ���� (0-1)
        [Tooltip("Overall intensity of the system (0-1)")]
        [Range(0.0f, 1.0f)]
        public float Intensity;

        // ���� ����
        [Tooltip("Intensity multiplier for fewer or extra particles")]
        [Range(0.1f, 10.0f)]
        public float IntensityMultiplier = 1.0f;

        [Tooltip("Intensity multiplier for fewer or extra secondary particles")]
        [Range(0.1f, 10.0f)]
        public float SecondaryIntensityMultiplier = 1.0f;

        [Tooltip("Intensity multiplier for fewer or extra mist particles")]
        [Range(0.1f, 10.0f)]
        public float MistIntensityMultiplier = 1.0f;

        // �ܺ� ���� ����
        [Tooltip("External intensity modifier, for example if the player goes in a cave, this could be reduced to slow or stop particles.")]
        [Range(0.0f, 1.0f)]
        public float ExternalIntensityMultiplier = 1.0f;

        // �⺻ ��ƼŬ �����
        [Tooltip("Base number of particles to emit per second. This is multiplied by intensity and intensity multiplier.")]
        [Range(100, 10000)]
        public int BaseEmissionRate = 1000;

        [Tooltip("Base number of secondary particles to emit per second. This is multiplied by intensity and intensity multiplier.")]
        [Range(100, 10000)]
        public int BaseEmissionRateSecondary = 100;

        [Tooltip("Base number of mist particles to emit per second. This is multiplied by intensity and intensity multiplier.")]
        [Range(5, 500)]
        public int BaseMistEmissionRate = 50;

        // ���� ���� ����
        [Tooltip("Precipitation tint color")]
        public Color PrecipitationTintColor = Color.white;

        [Tooltip("Precipitation mist tint color")]
        public Color PrecipitationMistTintColor = Color.white;

        [Tooltip("Precipitation secondary tint color")]
        public Color PrecipitationSecondaryTintColor = Color.white;

        // ��ƼŬ �ý��� ����
        [Tooltip("Particle system")]
        public ParticleSystem ParticleSystem;

        [Tooltip("Particle system that is secondary and optional")]
        public ParticleSystem ParticleSystemSecondary;

        [Tooltip("Particle system to use for mist")]
        public ParticleSystem MistParticleSystem;

        [Tooltip("Particles system for when particles hit something")]
        public ParticleSystem ExplosionParticleSystem;

        [Tooltip("The threshold that Intensity must pass for secondary particles to appear (0 - 1). Set to 1 for no secondary particles. Set this before changing Intensity.")]
        [Range(0.0f, 1.0f)]
        public float SecondaryThreshold = 0.75f;

        [Tooltip("The threshold that Intensity must pass for mist particles to appear (0 - 1). Set to 1 for no mist. Set this before changing Intensity.")]
        [Range(0.0f, 1.0f)]
        public float MistThreshold = 0.5f;

        [Tooltip("Particle dithering factor")]
        [Range(0.0f, 1.0f)]
        public float DitherLevel = 0.002f;

        // ���� ���� ������ ���� ������
        public WeatherMakerLoopingAudioSource AudioSourceLight { get; private set; }
        public WeatherMakerLoopingAudioSource AudioSourceMedium { get; private set; }
        public WeatherMakerLoopingAudioSource AudioSourceHeavy { get; private set; }
        public WeatherMakerLoopingAudioSource AudioSourceCurrent { get; private set; }

        public ParticleSystemRenderer ParticleSystemRenderer { get; private set; }
        public ParticleSystemRenderer MistParticleSystemRenderer { get; private set; }
        public ParticleSystemRenderer ExplosionParticleSystemRenderer { get; private set; }
        public ParticleSystemRenderer ParticleSystemSecondaryRenderer { get; private set; }

        public Material Material { get; private set; }
        public Material MaterialSecondary { get; private set; }
        public Material ExplosionMaterial { get; private set; }
        public Material MistMaterial { get; private set; }

        private float lastIntensityValue = -1.0f;
        private float lastIntensityMultiplierValue = -1.0f;
        private float lastSecondaryIntensityMultiplierValue = -1.0f;
        private float lastMistIntensityMultiplierValue = -1.0f;
        private float lastExternalIntensityMultiplierValue = -1.0f;

        private readonly Dictionary<ParticleSystem, bool> wasPlayingDictionary = new Dictionary<ParticleSystem, bool>();

        // ��ƼŬ ���� ���� �Լ�
        private void TintParticleSystem(ParticleSystem p, Color tintColor)
        {
            if (p == null)
            {
                return;
            }

            var m = p.main;
            var startColor = m.startColor;
            Color color1 = startColor.colorMin;
            Color color2 = startColor.colorMax;
            color1 = new Color(tintColor.r, tintColor.g, tintColor.b, color1.a);
            color2 = new Color(tintColor.r, tintColor.g, tintColor.b, color2.a);
            startColor.colorMin = color1;
            startColor.colorMax = color2;
            m.startColor = startColor;
        }

        // ��ƼŬ �ý��� ��� �Լ�
        private void PlayParticleSystem(ParticleSystem p, int baseEmissionRate, float intensityMultiplier, Color tintColor)
        {
            var e = p.emission;
            ParticleSystem.MinMaxCurve rate = p.emission.rateOverTime;
            rate.mode = ParticleSystemCurveMode.Constant;
            rate.constantMin = rate.constantMax = baseEmissionRate * Intensity * intensityMultiplier;
            e.rateOverTime = rate;
            var m = p.main;
            m.maxParticles = (int)Mathf.Max(m.maxParticles, rate.constantMax * m.startLifetime.constantMax);
            TintParticleSystem(p, tintColor);
            if (!p.isEmitting)
            {
                p.Play();
            }
        }

        // �ܺ� ���� ������ ������Ʈ
        private void UpdateExternalModifiers()
        {
            if (WeatherMakerScript.Instance != null)
            {
                CollisionEnabled = WeatherMakerScript.Instance.PerformanceProfile.EnablePrecipitationCollision;
                if (WeatherMakerScript.Instance.IntensityModifierDictionary.Count != 0)
                {
                    ExternalIntensityMultiplier = 1.0f;
                    foreach (float multiplier in WeatherMakerScript.Instance.IntensityModifierDictionary.Values)
                    {
                        ExternalIntensityMultiplier *= multiplier;
                    }
                }
            }
        }

        // ���� ���� Ȯ�� �� ó��
        private void CheckForIntensityChange()
        {
            if (lastIntensityValue == Intensity &&
                lastIntensityMultiplierValue == IntensityMultiplier &&
                lastSecondaryIntensityMultiplierValue == SecondaryIntensityMultiplier &&
                lastMistIntensityMultiplierValue == MistIntensityMultiplier &&
                lastExternalIntensityMultiplierValue == ExternalIntensityMultiplier)
            {
                return;
            }

            lastIntensityValue = Intensity;
            lastIntensityMultiplierValue = IntensityMultiplier;
            lastSecondaryIntensityMultiplierValue = SecondaryIntensityMultiplier;
            lastMistIntensityMultiplierValue = MistIntensityMultiplier;
            lastExternalIntensityMultiplierValue = ExternalIntensityMultiplier;

            if (Intensity < 0.01f)
            {
                // ������ ���� ��� ��� ����� �� ��ƼŬ �ý��� ����
                if (AudioSourceCurrent != null)
                {
                    AudioSourceCurrent.Stop();
                    AudioSourceCurrent = null;
                }
                if (ParticleSystem != null && ParticleSystem.isEmitting)
                {
                    ParticleSystem.Stop();
                }
                if (ParticleSystemSecondary != null && ParticleSystemSecondary.isEmitting)
                {
                    ParticleSystemSecondary.Stop();
                }
                if (MistParticleSystem != null && MistParticleSystem.isEmitting)
                {
                    MistParticleSystem.Stop();
                }
            }
            else
            {
                // ������ ���� ����� �ҽ� ��ȯ �� ���
                WeatherMakerLoopingAudioSource newSource;
                if (Intensity >= SoundHeavyIntensityThreshold)
                {
                    newSource = AudioSourceHeavy;
                }
                else if (Intensity >= SoundMediumIntensityThreshold)
                {
                    newSource = AudioSourceMedium;
                }
                else
                {
                    newSource = AudioSourceLight;
                }
                if (AudioSourceCurrent != newSource)
                {
                    if (AudioSourceCurrent != null)
                    {
                        AudioSourceCurrent.Stop();
                    }
                    AudioSourceCurrent = newSource;
                    if (AudioSourceCurrent != null)
                    {
                        AudioSourceCurrent.Play(1.0f);
                    }
                }
                if (AudioSourceCurrent != null)
                {
                    AudioSourceCurrent.SecondaryVolumeModifier = Mathf.Pow(Intensity, 0.3f);
                }
                // �� ��ƼŬ �ý��� ���
                if (ParticleSystem != null)
                {
                    PlayParticleSystem(ParticleSystem, BaseEmissionRate, IntensityMultiplier * ExternalIntensityMultiplier, PrecipitationTintColor);
                    TintParticleSystem(ExplosionParticleSystem, PrecipitationTintColor);
                }
                if (ParticleSystemSecondary != null)
                {
                    if (SecondaryThreshold >= Intensity)
                    {
                        ParticleSystemSecondary.Stop();
                    }
                    else
                    {
                        PlayParticleSystem(ParticleSystemSecondary, BaseEmissionRateSecondary, SecondaryIntensityMultiplier * ExternalIntensityMultiplier, PrecipitationSecondaryTintColor);
                    }
                }
                if (MistParticleSystem != null)
                {
                    if (MistThreshold >= Intensity)
                    {
                        MistParticleSystem.Stop();
                    }
                    else
                    {
                        PlayParticleSystem(MistParticleSystem, BaseMistEmissionRate, MistIntensityMultiplier * ExternalIntensityMultiplier, PrecipitationMistTintColor);
                    }
                }
            }
        }

        // ��ƼŬ �ý��� ��ȿ�� �˻�
        private void CheckForParticleSystem()
        {

#if DEBUG

            if (ParticleSystem == null)
            {
                Debug.LogError("Particle system is null");
                return;
            }

#endif

        }

        // �ʱ� ��ƼŬ �ý��� �� ������Ʈ
        private void UpdateInitialParticleSystemValues(ParticleSystem p, System.Action<Vector2> startSpeed, System.Action<KeyValuePair<Vector3, Vector3>> startSize)
        {
            if (p != null)
            {
                var m = p.main;
                startSpeed(new Vector2(m.startSpeed.constantMin, m.startSpeed.constantMax));
                var curveX = m.startSizeX;
                var curveY = m.startSizeY;
                var curveZ = m.startSizeZ;
                startSize(new KeyValuePair<Vector3, Vector3>(new Vector3(curveX.constantMin, curveY.constantMin, curveZ.constantMin),
                    new Vector3(curveX.constantMax, curveY.constantMax, curveZ.constantMax)));
            }
        }

        // ��ƼŬ �ý��� ��� ���� ����
        private void ResumeParticleSystem(ParticleSystem p)
        {
            if (p != null && wasPlayingDictionary.ContainsKey(p) && wasPlayingDictionary[p])
            {
                p.Play();
            }
        }

        // ��ƼŬ �ý��� ��� ���� ������Ʈ
        private void UpdateParticleSystemPlayState()
        {
            if (ParticleSystem != null)
            {
                wasPlayingDictionary[ParticleSystem] = ParticleSystem.isPlaying;
            }
            if (ParticleSystemSecondary != null)
            {
                wasPlayingDictionary[ParticleSystemSecondary] = ParticleSystemSecondary.isPlaying;
            }
            if (MistParticleSystem != null)
            {
                wasPlayingDictionary[MistParticleSystem] = MistParticleSystem.isPlaying;
            }
        }

        // ī�޶� ���� �� �̺�Ʈ ó��
        private void CameraPreCull(Camera camera)
        {
            if (Application.isPlaying && !WeatherMakerScript.ShouldIgnoreCamera(this, camera) && WeatherMakerCommandBufferManagerScript.CameraStackCount < 2)
            {
                OnCameraPreCull(camera);
            }
        }

        // ī�޶� ��ó�� �̺�Ʈ ó��
        private void CameraPostRender(Camera camera)
        {
            if (Application.isPlaying && !WeatherMakerScript.ShouldIgnoreCamera(this, camera) && WeatherMakerCommandBufferManagerScript.CameraStackCount < 2)
            {
                OnCameraPostRender(camera);
            }
        }

        /// <summary>
        /// �浹 Ȱ��ȭ ���� �� ȣ��
        /// </summary>
        protected virtual void OnCollisionEnabledChanged() { }

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // ��ƼŬ �ý��� ��ȿ�� �˻� �� �ʱⰪ ����
            CheckForParticleSystem();
            UpdateInitialParticleSystemValues(ParticleSystem, (f) => InitialStartSpeed = f, (f) => InitialStartSize = f);
            UpdateInitialParticleSystemValues(ParticleSystemSecondary, (f) => InitialStartSpeedSecondary = f, (f) => InitialStartSizeSecondary = f);
            UpdateInitialParticleSystemValues(MistParticleSystem, (f) => InitialStartSpeedMist = f, (f) => InitialStartSizeMist = f);
            Material = new Material(ParticleSystem.GetComponent<Renderer>().sharedMaterial);
            MaterialSecondary = (ParticleSystemSecondary == null ? null : new Material(ParticleSystemSecondary.GetComponent<Renderer>().sharedMaterial));
            MistMaterial = (MistParticleSystem == null ? null : new Material(MistParticleSystem.GetComponent<Renderer>().sharedMaterial));
            ExplosionMaterial = (ExplosionParticleSystem == null ? null : new Material(ExplosionParticleSystem.GetComponent<Renderer>().sharedMaterial));
            UpdateParticleSystemPlayState();
        }

        /// <summary>
        /// Start
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        /// Update
        /// </summary>
        protected virtual void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // �ܺ� ������ ������Ʈ �� ���� ���� Ȯ��
            UpdateExternalModifiers();
            CheckForIntensityChange();
            if (AudioSourceLight != null)
            {
                AudioSourceLight.Update();
            }
            if (AudioSourceMedium != null)
            {
                AudioSourceMedium.Update();
            }
            if (AudioSourceHeavy != null)
            {
                AudioSourceHeavy.Update();
            }
            if (MistMaterial != null)
            {
                MistMaterial.SetFloat(WMS._ParticleDitherLevel, DitherLevel);
            }
            UpdateParticleSystemPlayState();
            SetVolumeModifier(WeatherMakerAudioManagerScript.CachedWeatherVolumeModifier);
        }

        /// <summary>
        /// OnEnable
        /// </summary>
        protected virtual void OnEnable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // ī�޶� �̺�Ʈ ��� �� ��ƼŬ �ý��� ����
            if (WeatherMakerCommandBufferManagerScript.Instance != null)
            {
                WeatherMakerCommandBufferManagerScript.Instance.RegisterPreCull(CameraPreCull, this);
                WeatherMakerCommandBufferManagerScript.Instance.RegisterPostRender(CameraPostRender, this);
            }
            ParticleSystemRenderer = (ParticleSystem == null ? null : ParticleSystem.gameObject.GetComponent<ParticleSystemRenderer>());
            MistParticleSystemRenderer = (MistParticleSystem == null ? null : MistParticleSystem.gameObject.GetComponent<ParticleSystemRenderer>());
            ExplosionParticleSystemRenderer = (ExplosionParticleSystem == null ? null : ExplosionParticleSystem.gameObject.GetComponent<ParticleSystemRenderer>());
            ParticleSystemSecondaryRenderer = (ParticleSystemSecondary == null ? null : ParticleSystemSecondary.gameObject.GetComponent<ParticleSystemRenderer>());
            ResumeParticleSystem(ParticleSystem);
            ResumeParticleSystem(ParticleSystemSecondary);
            ResumeParticleSystem(MistParticleSystem);
            if (AudioSourceLight == null && LoopSourceLight != null)
            {
                AudioSourceLight = new WeatherMakerLoopingAudioSource(LoopSourceLight);
            }
            if (AudioSourceMedium == null && LoopSourceMedium != null)
            {
                AudioSourceMedium = new WeatherMakerLoopingAudioSource(LoopSourceMedium);
            }
            if (AudioSourceHeavy == null && LoopSourceHeavy != null)
            {
                AudioSourceHeavy = new WeatherMakerLoopingAudioSource(LoopSourceHeavy);
            }
            if (AudioSourceLight != null)
            {
                AudioSourceLight.Resume();
            }
            if (AudioSourceMedium != null)
            {
                AudioSourceMedium.Resume();
            }
            if (AudioSourceHeavy != null)
            {
                AudioSourceHeavy.Resume();
            }
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // ī�޶� �̺�Ʈ ����
            if (WeatherMakerCommandBufferManagerScript.Instance != null)
            {
                WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreCull(this);
                WeatherMakerCommandBufferManagerScript.Instance.UnregisterPostRender(this);
            }
        }

        /// <summary>
        /// FixedUpdate
        /// </summary>
        protected virtual void FixedUpdate()
        {
        }

        /// <summary>
        /// LateUpdate
        /// </summary>
        protected virtual void LateUpdate()
        {
        }

        /// <summary>
        /// ī�޶� ���� �� �̺�Ʈ ó��
        /// </summary>
        /// <param name="c">ī�޶�</param>
        protected virtual void OnCameraPreCull(Camera c)
        {
        }

        /// <summary>
        /// ī�޶� ��ó�� �̺�Ʈ ó��
        /// </summary>
        /// <param name="c">ī�޶�</param>
        protected virtual void OnCameraPostRender(Camera c)
        {
        }

        /// <summary>
        /// �ʱ� �ӵ�
        /// </summary>
        protected Vector2 InitialStartSpeed { get; private set; }

        /// <summary>
        /// �ʱ� ũ��
        /// </summary>
        protected KeyValuePair<Vector3, Vector3> InitialStartSize { get; private set; }

        /// <summary>
        /// �ʱ� �ӵ�(����)
        /// </summary>
        protected Vector2 InitialStartSpeedSecondary { get; private set; }

        /// <summary>
        /// �ʱ� ũ��(����)
        /// </summary>
        protected KeyValuePair<Vector3, Vector3> InitialStartSizeSecondary { get; private set; }

        /// <summary>
        /// �ʱ� �ӵ�(�Ȱ�)
        /// </summary>
        protected Vector2 InitialStartSpeedMist { get; private set; }

        /// <summary>
        /// �ʱ� ũ��(�Ȱ�)
        /// </summary>
        protected KeyValuePair<Vector3, Vector3> InitialStartSizeMist { get; private set; }

        /// <summary>
        /// ��ƼŬ �ý��� ǥ��/�����
        /// </summary>
        /// <param name="visible">true�� ǥ��, false�� ����</param>
        public void SetParticleSystemsVisible(bool visible)
        {
            if (ParticleSystemRenderer != null)
            {
                ParticleSystemRenderer.enabled = visible;
            }
            if (MistParticleSystemRenderer != null)
            {
                MistParticleSystemRenderer.enabled = visible;
            }
            if (ExplosionParticleSystemRenderer != null)
            {
                ExplosionParticleSystemRenderer.enabled = visible;
            }
            if (ParticleSystemSecondaryRenderer != null)
            {
                ParticleSystemSecondaryRenderer.enabled = visible;
            }
        }

        /// <summary>
        /// ���� ������ ����
        /// </summary>
        /// <param name="modifier">�� ������</param>
        public void SetVolumeModifier(float modifier)
        {
            if (AudioSourceLight == null)
            {
                return;
            }
            AudioSourceLight.VolumeModifier = AudioSourceMedium.VolumeModifier = AudioSourceHeavy.VolumeModifier = modifier;
        }

        private bool? collisionEnabled;

        /// <summary>
        /// �浹 Ȱ��ȭ ����
        /// </summary>
        public bool CollisionEnabled
        {
            get { return collisionEnabled ?? false; }
            set
            {
                if (collisionEnabled == null || value != collisionEnabled)
                {
                    collisionEnabled = value;
                    OnCollisionEnabledChanged();
                }
            }
        }
    }
}
