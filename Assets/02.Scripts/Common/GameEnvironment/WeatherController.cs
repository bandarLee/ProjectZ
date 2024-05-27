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
            yield return new WaitForSeconds(10f); // 10�� ���

            while (true)
            {
                SetDailyWeather();
                yield return new WaitForSeconds(10f); // 10�ʸ��� ���� ����
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
                UI_Temperature.Instance.SetTemperature(-5); // �߿�
            }
            else if (randomValue < 0.4f)
            {
                Debug.Log("���� : ������");
                SetWeather(SnowProfile);
                UI_Temperature.Instance.SetTemperature(-15); // �ſ� �߿�
            }
            else
            {
                Debug.Log("���� : ����");
                SetWeather(NoneProfile);
                UI_Temperature.Instance.SetTemperature(15); // ����
            }
        }

        private void SetWeather(WeatherMakerPrecipitationProfileScript profile)
        {
            precipitationManager.SetPrecipitationProfile(profile);
        }
    }
}
