using System.Collections;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherController : MonoBehaviour
    {
        [Tooltip("Precipitation profile for rain")]
        public WeatherMakerPrecipitationProfileScript RainProfile;

        [Tooltip("Precipitation profile for snow")]
        public WeatherMakerPrecipitationProfileScript SnowProfile;

        private WeatherMakerPrecipitationManagerScript precipitationManager;

        void Start()
        {
            precipitationManager = GetComponent<WeatherMakerPrecipitationManagerScript>();
            if (precipitationManager == null)
            {
                Debug.LogError("WeatherMakerPrecipitationManagerScript is not found.");
                return;
            }

            // 게임 시작 시 비가 내리도록 설정
            SetWeather(RainProfile);

            // 6분 후부터 날씨를 랜덤하게 변경하는 코루틴 시작
            StartCoroutine(DailyWeatherRoutine());
        }

        private IEnumerator DailyWeatherRoutine()
        {
            yield return new WaitForSeconds(10f); // 6분 대기

            while (true)
            {
                SetDailyWeather();
                yield return new WaitForSeconds(10f); // 6분마다 날씨 변경
            }
        }

        private void SetDailyWeather()
        {
            float randomValue = Random.Range(0f, 1f);
            Debug.Log("Random Value for Weather: " + randomValue);

            if (randomValue < 0.2f)
            {
                Debug.Log("Setting Weather to Rain");
                SetWeather(RainProfile);
            }
            else if (randomValue < 0.4f)
            {
                Debug.Log("Setting Weather to Snow");
                SetWeather(SnowProfile);
            }
            else
            {
                Debug.Log("Setting Weather to None");
                SetWeather(null);
            }
        }

        private void SetWeather(WeatherMakerPrecipitationProfileScript profile)
        {
            precipitationManager.SetPrecipitationProfile(profile);
        }
    }
}
