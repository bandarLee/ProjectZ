using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherController : MonoBehaviourPun
    {
        public WeatherMakerPrecipitationProfileScript RainProfile;
        public WeatherMakerPrecipitationProfileScript SnowProfile;
        public WeatherMakerPrecipitationProfileScript NoneProfile;

        private WeatherMakerPrecipitationManagerScript precipitationManager;

        void Start()
        {
            precipitationManager = GetComponent<WeatherMakerPrecipitationManagerScript>();
            if (precipitationManager == null)
            {
                Debug.LogError("WeatherMakerPrecipitationManagerScript is not found.");
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(DailyWeatherRoutine());
            }
        }

        private IEnumerator DailyWeatherRoutine()
        {
            yield return new WaitForSeconds(60f); // 1분마다 날씨 변경

            while (true)
            {
                SetDailyWeather();
                yield return new WaitForSeconds(60f); // 1분마다 날씨 변경

            }
        }

        private void SetDailyWeather()
        {
            float randomValue = Random.Range(0f, 1f);
            int weatherType = 0;

            if (randomValue < 0.2f)
            {
                SetWeather(RainProfile);
                SetTemperatureSafely(-5); // 추움
                weatherType = 1;
            }
            else if (randomValue < 0.4f)
            {
                SetWeather(SnowProfile);
                SetTemperatureSafely(-15); // 매우 추움
                weatherType = 2;
            }
            else
            {
                SetWeather(NoneProfile);
                SetTemperatureSafely(15); // 보통
                weatherType = 0;
            }
            photonView.RPC("SyncWeather", RpcTarget.Others, weatherType);
        }

        [PunRPC]
        private void SyncWeather(int weatherType)
        {
            switch (weatherType)
            {
                case 1:
                    SetWeather(RainProfile);
                    SetTemperatureSafely(-5);
                    break;
                case 2:
                    SetWeather(SnowProfile);
                    SetTemperatureSafely(-15);
                    break;
                default:
                    SetWeather(NoneProfile);
                    SetTemperatureSafely(15);
                    break;
            }
        }

        private void SetWeather(WeatherMakerPrecipitationProfileScript profile)
        {
            precipitationManager.SetPrecipitationProfile(profile);
        }

        private void SetTemperatureSafely(int temperature)
        {
            if (UI_Temperature.Instance != null)
            {
                UI_Temperature.Instance.SetTemperature(temperature);
            }
            else
            {
                Debug.LogWarning("UI_Temperature.Instance is null.");
            }
        }
    }
}
