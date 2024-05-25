using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Rain/Snow/Season/Wind ��� ���� �⺻ Ȯ�� ��ũ��Ʈ
    /// </summary>
    /// <typeparam name="T">������ �Ŵ����� Ÿ��</typeparam>
    public abstract class WeatherMakerExtensionRainSnowSeasonScript<T> : WeatherMakerExtensionScript<T> where T : UnityEngine.MonoBehaviour
    {
        /// <summary>
        /// �÷��� ����
        /// </summary>
        [System.Flags]
        public enum WeatherMakerRainSnowSeasonScriptFlags
        {
            /// <summary>
            /// �� ����
            /// </summary>
            ControlRain = 1,

            /// <summary>
            /// �� ����
            /// </summary>
            ControlSnow = 2,

            /// <summary>
            /// ���� ����
            /// </summary>
            ControlSeason = 4,

            /// <summary>
            /// �ٶ� ����
            /// </summary>
            ControlWind = 8
        }

        /// <summary>
        /// ������ �Ӽ��� Ÿ��. ��� Ȯ���� ��� �÷��׸� ���������� �ʽ��ϴ�.
        /// </summary>
        [WeatherMaker.EnumFlag("What types of properties to control. Not all extensions will support all flags.")]
        public WeatherMakerRainSnowSeasonScriptFlags ControlFlags = WeatherMakerRainSnowSeasonScriptFlags.ControlRain | WeatherMakerRainSnowSeasonScriptFlags.ControlSnow | WeatherMakerRainSnowSeasonScriptFlags.ControlSeason;

        private float lastRain = -1.0f; // ������ �� ���� ����
        private float lastSnow = -1.0f; // ������ �� ���� ����
        private float lastSeason = -1.0f; // ������ ���� ����

        // �Ϲݱ� ���� ���� ��ȯ ���̺�
        static readonly float[] monthToSeasonLookupNorth = new float[]
        {
            0.33f,
            0.66f,
            0.99f,
            1.34f,
            1.67f,
            2.0f,
            2.33f,
            2.67f,
            3.0f,
            3.33f,
            3.67f,
            4.0f
        };

        // ���ݱ� ���� ���� ��ȯ ���̺�
        static readonly float[] monthToSeasonLookupSouth = new float[]
        {
            2.33f,
            2.66f,
            2.99f,
            3.34f,
            3.67f,
            4.0f,
            0.33f,
            0.67f,
            1.0f,
            1.33f,
            1.67f,
            2.0f
        };

        // �� ������Ʈ
        private void UpdateRain()
        {
            // WeatherMakerScript �ν��Ͻ� �Ǵ� ������ �Ŵ����� null�̸� ����
            if (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PrecipitationManager == null)
            {
                return;
            }

            // ���� �� ���� ��������
            float rain = WeatherMakerScript.Instance.PrecipitationManager.RainIntensity;
            if (rain != lastRain)
            {
                lastRain = rain;
                rain += (WeatherMakerScript.Instance.PrecipitationManager.SleetIntensity * 0.5f);
                rain += (WeatherMakerScript.Instance.PrecipitationManager.HailIntensity * 0.25f);
                OnUpdateRain(rain); // �� ������Ʈ �Լ� ȣ��
            }
        }

        // �� ������Ʈ
        private void UpdateSnow()
        {
            // WeatherMakerScript �ν��Ͻ� �Ǵ� ������ �Ŵ����� null�̸� ����
            if (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PrecipitationManager == null)
            {
                return;
            }

            // ���� �� ���� ��������
            float snow = WeatherMakerScript.Instance.PrecipitationManager.SnowIntensity;
            if (snow != lastSnow)
            {
                lastSnow = snow;
                snow += (WeatherMakerScript.Instance.PrecipitationManager.SleetIntensity * 0.5f);
                snow += (WeatherMakerScript.Instance.PrecipitationManager.HailIntensity * 0.5f);
                OnUpdateSnow(snow); // �� ������Ʈ �Լ� ȣ��
            }
        }

        // ���� ������Ʈ
        private void UpdateSeason()
        {
            // WeatherMakerDayNightCycleManagerScript �ν��Ͻ��� null�̸� ����
            if (WeatherMakerDayNightCycleManagerScript.Instance == null)
            {
                return;
            }

            // ���� �� ��������
            int month = WeatherMakerDayNightCycleManagerScript.Instance.Month - 1;
            if (month < 0 || month > 11)
            {
                return;
            }
            float season;
            // ������ 0���� ũ�� �Ϲݱ�, �׷��� ������ ���ݱ� ���� ��ȯ ���̺� ���
            if (WeatherMakerDayNightCycleManagerScript.Instance.Latitude > 0.0f)
            {
                season = monthToSeasonLookupNorth[month];
            }
            else
            {
                season = monthToSeasonLookupSouth[month];
            }
            if (season != lastSeason)
            {
                lastSeason = season;
                OnUpdateSeason(season); // ���� ������Ʈ �Լ� ȣ��
            }
        }

        /// <summary>
        /// ������Ʈ
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// ���� ������Ʈ
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();

            // TypeScript�� null�̸� ����
            if (TypeScript == null)
            {
                return;
            }
            // �� ���� �÷��װ� �����Ǿ� ������ �� ������Ʈ ȣ��
            else if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlRain) == WeatherMakerRainSnowSeasonScriptFlags.ControlRain)
            {
                UpdateRain();
            }
            // �� ���� �÷��װ� �����Ǿ� ������ �� ������Ʈ ȣ��
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlSnow) == WeatherMakerRainSnowSeasonScriptFlags.ControlSnow)
            {
                UpdateSnow();
            }
            // ���� ���� �÷��װ� �����Ǿ� ������ ���� ������Ʈ ȣ��
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlSeason) == WeatherMakerRainSnowSeasonScriptFlags.ControlSeason)
            {
                UpdateSeason();
            }
            // �ٶ� ���� �÷��װ� �����Ǿ� ������ �ٶ� ������Ʈ ȣ��
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlWind) == WeatherMakerRainSnowSeasonScriptFlags.ControlWind)
            {
                OnUpdateWind();
            }
        }

        /// <summary>
        /// �� ������Ʈ
        /// </summary>
        /// <param name="rain">�� ���� (0 - 1)</param>
        protected virtual void OnUpdateRain(float rain) { }

        /// <summary>
        /// �� ���� ������Ʈ
        /// </summary>
        /// <param name="snow">�� ���� (0 - 1)</param>
        protected virtual void OnUpdateSnow(float snow) { }

        /// <summary>
        /// ���� ������Ʈ
        /// </summary>
        /// <param name="season">����: 0-1 �ܿ�, 1-2 ��, 2-3 ����, 3-4 ����</param>
        protected virtual void OnUpdateSeason(float season) { }

        /// <summary>
        /// �ٶ� ������Ʈ
        /// </summary>
        protected virtual void OnUpdateWind() { }
    }
}
