using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Rain/Snow/Season/Wind 제어를 위한 기본 확장 스크립트
    /// </summary>
    /// <typeparam name="T">제어할 매니저의 타입</typeparam>
    public abstract class WeatherMakerExtensionRainSnowSeasonScript<T> : WeatherMakerExtensionScript<T> where T : UnityEngine.MonoBehaviour
    {
        /// <summary>
        /// 플래그 정의
        /// </summary>
        [System.Flags]
        public enum WeatherMakerRainSnowSeasonScriptFlags
        {
            /// <summary>
            /// 비 제어
            /// </summary>
            ControlRain = 1,

            /// <summary>
            /// 눈 제어
            /// </summary>
            ControlSnow = 2,

            /// <summary>
            /// 계절 제어
            /// </summary>
            ControlSeason = 4,

            /// <summary>
            /// 바람 제어
            /// </summary>
            ControlWind = 8
        }

        /// <summary>
        /// 제어할 속성의 타입. 모든 확장이 모든 플래그를 지원하지는 않습니다.
        /// </summary>
        [WeatherMaker.EnumFlag("What types of properties to control. Not all extensions will support all flags.")]
        public WeatherMakerRainSnowSeasonScriptFlags ControlFlags = WeatherMakerRainSnowSeasonScriptFlags.ControlRain | WeatherMakerRainSnowSeasonScriptFlags.ControlSnow | WeatherMakerRainSnowSeasonScriptFlags.ControlSeason;

        private float lastRain = -1.0f; // 마지막 비 강도 저장
        private float lastSnow = -1.0f; // 마지막 눈 강도 저장
        private float lastSeason = -1.0f; // 마지막 계절 저장

        // 북반구 월별 계절 변환 테이블
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

        // 남반구 월별 계절 변환 테이블
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

        // 비 업데이트
        private void UpdateRain()
        {
            // WeatherMakerScript 인스턴스 또는 강수량 매니저가 null이면 종료
            if (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PrecipitationManager == null)
            {
                return;
            }

            // 현재 비 강도 가져오기
            float rain = WeatherMakerScript.Instance.PrecipitationManager.RainIntensity;
            if (rain != lastRain)
            {
                lastRain = rain;
                rain += (WeatherMakerScript.Instance.PrecipitationManager.SleetIntensity * 0.5f);
                rain += (WeatherMakerScript.Instance.PrecipitationManager.HailIntensity * 0.25f);
                OnUpdateRain(rain); // 비 업데이트 함수 호출
            }
        }

        // 눈 업데이트
        private void UpdateSnow()
        {
            // WeatherMakerScript 인스턴스 또는 강수량 매니저가 null이면 종료
            if (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PrecipitationManager == null)
            {
                return;
            }

            // 현재 눈 강도 가져오기
            float snow = WeatherMakerScript.Instance.PrecipitationManager.SnowIntensity;
            if (snow != lastSnow)
            {
                lastSnow = snow;
                snow += (WeatherMakerScript.Instance.PrecipitationManager.SleetIntensity * 0.5f);
                snow += (WeatherMakerScript.Instance.PrecipitationManager.HailIntensity * 0.5f);
                OnUpdateSnow(snow); // 눈 업데이트 함수 호출
            }
        }

        // 계절 업데이트
        private void UpdateSeason()
        {
            // WeatherMakerDayNightCycleManagerScript 인스턴스가 null이면 종료
            if (WeatherMakerDayNightCycleManagerScript.Instance == null)
            {
                return;
            }

            // 현재 월 가져오기
            int month = WeatherMakerDayNightCycleManagerScript.Instance.Month - 1;
            if (month < 0 || month > 11)
            {
                return;
            }
            float season;
            // 위도가 0보다 크면 북반구, 그렇지 않으면 남반구 계절 변환 테이블 사용
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
                OnUpdateSeason(season); // 계절 업데이트 함수 호출
            }
        }

        /// <summary>
        /// 업데이트
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// 늦은 업데이트
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();

            // TypeScript가 null이면 종료
            if (TypeScript == null)
            {
                return;
            }
            // 비 제어 플래그가 설정되어 있으면 비 업데이트 호출
            else if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlRain) == WeatherMakerRainSnowSeasonScriptFlags.ControlRain)
            {
                UpdateRain();
            }
            // 눈 제어 플래그가 설정되어 있으면 눈 업데이트 호출
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlSnow) == WeatherMakerRainSnowSeasonScriptFlags.ControlSnow)
            {
                UpdateSnow();
            }
            // 계절 제어 플래그가 설정되어 있으면 계절 업데이트 호출
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlSeason) == WeatherMakerRainSnowSeasonScriptFlags.ControlSeason)
            {
                UpdateSeason();
            }
            // 바람 제어 플래그가 설정되어 있으면 바람 업데이트 호출
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlWind) == WeatherMakerRainSnowSeasonScriptFlags.ControlWind)
            {
                OnUpdateWind();
            }
        }

        /// <summary>
        /// 비 업데이트
        /// </summary>
        /// <param name="rain">비 강도 (0 - 1)</param>
        protected virtual void OnUpdateRain(float rain) { }

        /// <summary>
        /// 눈 강도 업데이트
        /// </summary>
        /// <param name="snow">눈 강도 (0 - 1)</param>
        protected virtual void OnUpdateSnow(float snow) { }

        /// <summary>
        /// 계절 업데이트
        /// </summary>
        /// <param name="season">계절: 0-1 겨울, 1-2 봄, 2-3 여름, 3-4 가을</param>
        protected virtual void OnUpdateSeason(float season) { }

        /// <summary>
        /// 바람 업데이트
        /// </summary>
        protected virtual void OnUpdateWind() { }
    }
}
