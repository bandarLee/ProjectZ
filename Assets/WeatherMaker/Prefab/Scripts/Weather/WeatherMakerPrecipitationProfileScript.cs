using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// ���� ������ �����ϴ� ������
    /// </summary>
    public enum WeatherMakerPrecipitationType
    {
        None = 0,

        Rain = 1,

        Snow = 2,

        Sleet = 3,

        Hail = 4,

        Custom = 127
    }

    /// <summary>
    /// ���� ������ Ŭ����, ���� ������ �Ӽ��� ����
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherMakerPrecipitationProfile", menuName = "WeatherMaker/Precipitation Profile", order = 25)]
    [System.Serializable]
    public class WeatherMakerPrecipitationProfileScript : WeatherMakerBaseScriptableObjectScript
    {
        /// <summary>���� ����</summary>
        [Tooltip("Type of precipitation")]
        public WeatherMakerPrecipitationType PrecipitationType = WeatherMakerPrecipitationType.Rain;

        /// <summary>���� ����</summary>
        [MinMaxSlider(0.0f, 1.0f, "Range of intensities")]
        public RangeOfFloats IntensityRange = new RangeOfFloats { Minimum = 0.1f, Maximum = 0.3f };

        /// <summary>���� �������� ���ο� ���� �����ϴ� ��</summary>
        [MinMaxSlider(0.0f, 120.0f, "How often a new value from IntensityRange should be chosen")]
        public RangeOfFloats IntensityRangeDuration = new RangeOfFloats { Minimum = 10.0f, Maximum = 60.0f };

        /// <summary>���� ����, �꼺�� �Ǵ� ���� ȿ���� ������</summary>
        [Tooltip("Tint the precipitation, useful for acid rain or other magical effects.")]
        [ColorUsage(true, true)]
        public Color PrecipitationTintColor = Color.white;

        /// <summary>���� �̽�Ʈ ����, �꼺�� �Ǵ� ���� ȿ���� ������</summary>
        [Tooltip("Tint the precipitation mist, useful for acid rain or other magical effects.")]
        [ColorUsage(true, true)]
        public Color PrecipitationMistTintColor = Color.white;

        /// <summary>���� 2�� ����, �꼺�� �Ǵ� ���� ȿ���� ������</summary>
        [Tooltip("Tint the precipitation secondary, useful for acid rain or other magical effects.")]
        [ColorUsage(true, true)]
        public Color PrecipitationSecondaryTintColor = Color.white;
    }
}
