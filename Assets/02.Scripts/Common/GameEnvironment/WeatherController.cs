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

            // ���� ���� �� �� �������� ����
            SetWeather(RainProfile);

            // 6�� �ĺ��� ������ �����ϰ� �����ϴ� �ڷ�ƾ ����
            StartCoroutine(DailyWeatherRoutine());
        }

        private IEnumerator DailyWeatherRoutine()
        {
            yield return new WaitForSeconds(10f); // 6�� ���

            while (true)
            {
                SetDailyWeather();
                yield return new WaitForSeconds(10f); // 6�и��� ���� ����
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
