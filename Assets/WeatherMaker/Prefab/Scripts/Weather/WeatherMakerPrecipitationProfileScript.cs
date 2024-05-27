using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// 강수 유형을 정의하는 열거형
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
    /// 강수 프로필 클래스, 강수 렌더링 속성을 포함
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherMakerPrecipitationProfile", menuName = "WeatherMaker/Precipitation Profile", order = 25)]
    [System.Serializable]
    public class WeatherMakerPrecipitationProfileScript : WeatherMakerBaseScriptableObjectScript
    {
        /// <summary>강수 유형</summary>
        [Tooltip("Type of precipitation")]
        public WeatherMakerPrecipitationType PrecipitationType = WeatherMakerPrecipitationType.Rain;

        /// <summary>강도 범위</summary>
        [MinMaxSlider(0.0f, 1.0f, "Range of intensities")]
        public RangeOfFloats IntensityRange = new RangeOfFloats { Minimum = 0.1f, Maximum = 0.3f };

        /// <summary>강도 범위에서 새로운 값을 선택하는 빈도</summary>
        [MinMaxSlider(0.0f, 120.0f, "How often a new value from IntensityRange should be chosen")]
        public RangeOfFloats IntensityRangeDuration = new RangeOfFloats { Minimum = 10.0f, Maximum = 60.0f };

        /// <summary>강수 색조, 산성비 또는 마법 효과에 유용함</summary>
        [Tooltip("Tint the precipitation, useful for acid rain or other magical effects.")]
        [ColorUsage(true, true)]
        public Color PrecipitationTintColor = Color.white;

        /// <summary>강수 미스트 색조, 산성비 또는 마법 효과에 유용함</summary>
        [Tooltip("Tint the precipitation mist, useful for acid rain or other magical effects.")]
        [ColorUsage(true, true)]
        public Color PrecipitationMistTintColor = Color.white;

        /// <summary>강수 2차 색조, 산성비 또는 마법 효과에 유용함</summary>
        [Tooltip("Tint the precipitation secondary, useful for acid rain or other magical effects.")]
        [ColorUsage(true, true)]
        public Color PrecipitationSecondaryTintColor = Color.white;
    }
}
