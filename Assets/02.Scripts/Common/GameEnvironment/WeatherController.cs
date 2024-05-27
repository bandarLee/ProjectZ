using System.Collections;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherController : MonoBehaviour
    {
        public WeatherMakerPrecipitationProfileScript RainProfile;
        public WeatherMakerPrecipitationProfileScript SnowProfile;
        public WeatherMakerPrecipitationProfileScript NoneProfile;
        //public WeatherMakerPrecipitationProfileScript Thunder;

        private WeatherMakerPrecipitationManagerScript precipitationManager;

        void Start()
        {
            precipitationManager = GetComponent<WeatherMakerPrecipitationManagerScript>();
            if (precipitationManager == null)
            {
                Debug.LogError("WeatherMakerPrecipitationManagerScript is not found.");
                return;
            }

            // 게임 시작 날씨 : 비
            SetWeather(RainProfile);

            // n분 후부터 날씨를 랜덤하게 변경하는 코루틴 시작
            StartCoroutine(DailyWeatherRoutine());
        }

        private IEnumerator DailyWeatherRoutine()
        {
            yield return new WaitForSeconds(10f); // 10초 대기

            while (true)
            {
                SetDailyWeather();
                yield return new WaitForSeconds(10f); // 10초마다 날씨 변경
            }
        }

        private void SetDailyWeather()
        {
            float randomValue = Random.Range(0f, 1f);
            Debug.Log("날씨 랜덤");

            if (randomValue < 0.2f)
            {
                Debug.Log("날씨 : 비");
                SetWeather(RainProfile);
                UI_Temperature.Instance.SetTemperature(-5); // 추움
            }
            else if (randomValue < 0.4f)
            {
                Debug.Log("날씨 : 눈보라");
                SetWeather(SnowProfile);
                UI_Temperature.Instance.SetTemperature(-15); // 매우 추움
            }
            else
            {
                Debug.Log("날씨 : 맑음");
                SetWeather(NoneProfile);
                UI_Temperature.Instance.SetTemperature(15); // 보통
            }
        }

        private void SetWeather(WeatherMakerPrecipitationProfileScript profile)
        {
            precipitationManager.SetPrecipitationProfile(profile);
        }
    }
}
