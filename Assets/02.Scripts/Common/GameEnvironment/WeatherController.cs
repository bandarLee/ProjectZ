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

            // ���� ���� ���� : ��
            SetWeather(RainProfile);

            // n�� �ĺ��� ������ �����ϰ� �����ϴ� �ڷ�ƾ ����
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
            Debug.Log("���� ����");

            if (randomValue < 0.2f)
            {
                Debug.Log("���� : ��");
                SetWeather(RainProfile);
            }
            else if (randomValue < 0.4f)
            {
                Debug.Log("���� : ������");
                SetWeather(SnowProfile);
            }
            else
            {
                Debug.Log("���� : ����");
                SetWeather(NoneProfile);
            }
        }

        private void SetWeather(WeatherMakerPrecipitationProfileScript profile)
        {
            precipitationManager.SetPrecipitationProfile(profile);
        }
    }
}
