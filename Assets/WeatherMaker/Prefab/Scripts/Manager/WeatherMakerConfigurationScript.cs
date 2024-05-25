using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerConfigurationScript : MonoBehaviour
    {
        // 공개 필드, Inspector에서 설정 가능
        public bool ShowFPS = true; // FPS 표시 여부
        public bool ShowTimeOfDay = true; // 현재 시간 표시 여부
        public bool AutoAddLightsOnStart = true; // 시작 시 자동으로 조명을 추가할지 여부
        public GameObject ConfigurationPanel; // 설정 패널
        public UnityEngine.UI.Text LabelFPS; // FPS를 표시할 텍스트 UI 요소
        public UnityEngine.UI.Slider TransitionDurationSlider; // 전환 지속 시간 슬라이더
        public UnityEngine.UI.Slider IntensitySlider; // 강도 슬라이더
        public UnityEngine.UI.Toggle MouseLookEnabledCheckBox; // 마우스 보기 활성화 체크박스
        public UnityEngine.UI.Toggle FlashlightToggle; // 손전등 토글
        public UnityEngine.UI.Toggle TimeOfDayEnabledCheckBox; // 시간 진행 활성화 체크박스
        public UnityEngine.UI.Toggle CollisionToggle; // 충돌 활성화 토글
        public UnityEngine.UI.Slider DawnDuskSlider; // 새벽/황혼 슬라이더
        public UnityEngine.UI.Text TimeOfDayText; // 현재 시간을 표시할 텍스트 UI 요소
        public UnityEngine.UI.Text TimeOfDayCategoryText; // 시간대를 표시할 텍스트 UI 요소
        public UnityEngine.UI.Dropdown CloudDropdown; // 구름 드롭다운
        public UnityEngine.UI.RawImage WeatherMapImage; // 날씨 맵 이미지
        public UnityEngine.EventSystems.EventSystem EventSystem; // 이벤트 시스템
        public GameObject SidePanel; // 사이드 패널

        // 내부 상태를 추적하기 위한 변수들
        private int frameCount = 0;
        private float nextFrameUpdate = 0.0f;
        private float fps = 0.0f;
        private float frameUpdateRate = 4.0f; // 초당 4번 업데이트
        private int frameCounter;
        private WeatherMakerCloudType clouds;
        private WeatherMakerCloudType lastClouds;

        // 시간 업데이트 함수
        private void UpdateTimeOfDay()
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                if (TimeOfDayText.IsActive() && ShowTimeOfDay)
                {
                    System.TimeSpan t = System.TimeSpan.FromSeconds(WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay);
                    TimeOfDayText.text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
                }
                TimeOfDayCategoryText.text = WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDayCategory.ToString();
            }
        }

        // FPS 표시 함수
        private void DisplayFPS()
        {
            if (LabelFPS != null && ShowFPS)
            {
                frameCount++;
                if (Time.time > nextFrameUpdate)
                {
                    nextFrameUpdate += (1.0f / frameUpdateRate);
                    fps = (int)Mathf.Floor((float)frameCount * frameUpdateRate);
                    LabelFPS.text = "FPS: " + fps;
                    frameCount = 0;
                }
            }
        }

        // 초기화 함수
        private void Start()
        {
            if (WeatherMakerPrecipitationManagerScript.HasInstance())
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value = 0.5f;
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration = TransitionDurationSlider.value;
                DawnDuskSlider.value = WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay;
            }
            else
            {
                DawnDuskSlider.value = 0.5f;
            }
            if (WeatherMakerScript.HasInstance())
            {
                CollisionToggle.isOn = WeatherMakerScript.Instance.PerformanceProfile.EnablePrecipitationCollision;
            }
            else
            {
                CollisionToggle.isOn = false;
            }
            nextFrameUpdate = Time.time;
            if (UnityEngine.EventSystems.EventSystem.current == null && ConfigurationPanel != null && ConfigurationPanel.activeInHierarchy)
            {
                EventSystem.gameObject.SetActive(true);
                UnityEngine.EventSystems.EventSystem.current = EventSystem;
            }
            if (AutoAddLightsOnStart && WeatherMakerLightManagerScript.HasInstance())
            {
                foreach (Light light in GameObject.FindObjectsOfType<Light>())
                {
                    if (light.type != LightType.Directional && light.type != LightType.Area &&
                        !WeatherMakerLightManagerScript.Instance.AutoAddLights.Contains(light))
                    {
                        WeatherMakerLightManagerScript.Instance.AutoAddLights.Add(light);
                    }
                }
            }
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                bool val = TimeOfDayEnabledCheckBox.enabled;
                WeatherMakerDayNightCycleManagerScript.Instance.Speed = WeatherMakerDayNightCycleManagerScript.Instance.NightSpeed = (val ? 10.0f : 0.0f);
            }
            if (Camera.main != null && Camera.main.orthographic && WeatherMapImage != null)
            {
                WeatherMapImage.gameObject.SetActive(false);
            }
        }

        // 업데이트 함수
        private void Update()
        {
            DisplayFPS();
            if (WeatherMakerFullScreenCloudsScript.Instance != null)
            {
                WeatherMapImage.texture = WeatherMakerFullScreenCloudsScript.Instance.WeatherMapRenderTexture;
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                LightningStrikeButtonClicked();
            }
            if (Input.GetKeyDown(KeyCode.BackQuote) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                RectTransform r = SidePanel.GetComponent<RectTransform>();
                Vector2 ap = r.anchoredPosition;
                bool visible;
                if (r.anchoredPosition.x < 0.0f)
                {
                    visible = true;
                    ap.x = 110.0f;
                }
                else
                {
                    visible = false;
                    WeatherMapImage.enabled = false;
                    ap.x = -9999.0f;
                }
                r.anchoredPosition = ap;
                if (WeatherMapImage.transform.childCount > 0)
                {
                    WeatherMapImage.transform.GetChild(0).gameObject.SetActive(visible);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) && Input.GetKey(KeyCode.LeftShift))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    UnityEngine.QualitySettings.SetQualityLevel(0, true);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    UnityEngine.QualitySettings.SetQualityLevel(1, true);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    UnityEngine.QualitySettings.SetQualityLevel(2, true);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    UnityEngine.QualitySettings.SetQualityLevel(3, true);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    UnityEngine.QualitySettings.SetQualityLevel(4, true);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    UnityEngine.QualitySettings.SetQualityLevel(5, true);
                }
            }

            if (WeatherMapImage != null && Camera.main != null)
            {
                if (Camera.main.orthographic)
                {
                    WeatherMapImage.enabled = false;
                }
                else if (Input.GetKeyDown(KeyCode.M) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                {
                    WeatherMapImage.enabled = !WeatherMapImage.enabled;
                }
            }
            UpdateTimeOfDay();
            frameCounter++;
        }

        // 구름 업데이트 함수
        private void UpdateClouds()
        {
            if (clouds == lastClouds)
            {
                return;
            }
            lastClouds = clouds;
            if (WeatherMakerLegacyCloudScript2D.Instance != null && WeatherMakerLegacyCloudScript2D.Instance.enabled &&
                WeatherMakerLegacyCloudScript2D.Instance.gameObject.activeInHierarchy)
            {
                if (clouds == WeatherMakerCloudType.None)
                {
                    WeatherMakerLegacyCloudScript2D.Instance.RemoveClouds();
                }
                else if (clouds != WeatherMakerCloudType.Custom)
                {
                    WeatherMakerLegacyCloudScript2D.Instance.CreateClouds();
                }
            }
            else if (WeatherMakerFullScreenCloudsScript.Instance != null && WeatherMakerFullScreenCloudsScript.Instance.enabled &&
                WeatherMakerFullScreenCloudsScript.Instance.gameObject.activeInHierarchy)
            {
                float duration = WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration;
                if (clouds == WeatherMakerCloudType.None)
                {
                    WeatherMakerFullScreenCloudsScript.Instance.HideCloudsAnimated(duration);
                }
                else if (clouds != WeatherMakerCloudType.Custom)
                {
                    WeatherMakerFullScreenCloudsScript.Instance.ShowCloudsAnimated(duration, "WeatherMakerCloudProfile_" + clouds.ToString());
                }
                else
                {
                    // 사용자 정의 구름, 현재 상태를 변경하지 않음
                }
            }
        }

        // 비 토글 변경 이벤트 핸들러
        /// <param name="isOn">토글 상태</param>
        public void RainToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Rain : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // 눈 토글 변경 이벤트 핸들러
        /// <param name="isOn">토글 상태</param>
        public void SnowToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Snow : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // 우박 토글 변경 이벤트 핸들러
        /// <param name="isOn">토글 상태</param>
        public void HailToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Hail : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // 진눈깨비 토글 변경 이벤트 핸들러
        /// <param name="isOn">토글 상태</param>
        public void SleetToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Sleet : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // 구름 토글 변경 이벤트 핸들러
        public void CloudToggleChanged()
        {
            string text = CloudDropdown.captionText.text;
            text = text.Replace("-", string.Empty).Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            clouds = (WeatherMakerCloudType)System.Enum.Parse(typeof(WeatherMakerCloudType), text);
            UpdateClouds();
        }

        // 번개 토글 변경 이벤트 핸들러
        /// <param name="isOn">토글 상태</param>
        public void LightningToggleChanged(bool isOn)
        {
            if (WeatherMakerThunderAndLightningScript.Instance != null)
            {
                WeatherMakerThunderAndLightningScript.Instance.EnableLightning = isOn;
            }
        }

        // 충돌 토글 변경 이벤트 핸들러
        /// <param name="isOn">토글 상태</param>
        public void CollisionToggleChanged(bool isOn)
        {
            if (WeatherMakerScript.Instance != null)
            {
                WeatherMakerScript.Instance.PerformanceProfile.EnablePrecipitationCollision = isOn;
            }
        }

        // 바람 토글 변경 이벤트 핸들러
        /// <param name="isOn">토글 상태</param>
        public void WindToggleChanged(bool isOn)
        {
            if (WeatherMakerScript.Instance == null || WeatherMakerPrecipitationManagerScript.Instance == null || WeatherMakerWindScript.Instance == null)
            {
                return;
            }

            float duration = WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration;
            if (isOn)
            {
                // 복사본을 만들어 런타임 중 변경을 방지
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_MediumWind"), 0.0f, duration);
            }
            else
            {
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_None"), 0.0f, duration);
            }
        }

        // 전환 지속 시간 슬라이더 변경 이벤트 핸들러
        /// <summary>
        /// Transition duration slider change
        /// </summary>
        /// <param name="val">새 값</param>
        public void TransitionDurationSliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration = val;
            }
        }

        // 강도 슬라이더 변경 이벤트 핸들러
        /// <summary>
        /// Intensity duration slider change
        /// </summary>
        /// <param name="val">새 값</param>
        public void IntensitySliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = val;
            }
        }

        // 마우스 보기 활성화 체크박스 변경 이벤트 핸들러
        /// <summary>
        /// Mouse look value change
        /// </summary>
        /// <param name="val">새 값</param>
        public void MouseLookEnabledChanged(bool val)
        {
            MouseLookEnabledCheckBox.isOn = val;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (WeatherMakerScript.IsLocalPlayer(obj.transform))
                {
                    WeatherMakerPlayerControllerScript controller = obj.GetComponent<WeatherMakerPlayerControllerScript>();
                    if (controller != null && controller.enabled)
                    {
                        controller.EnableMouseLook = val;
                        break;
                    }
                }
            }
        }

        // 손전등 체크박스 변경 이벤트 핸들러
        /// <summary>
        /// Flashlight value change
        /// </summary>
        /// <param name="val">새 값</param>
        public void FlashlightChanged(bool val)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (WeatherMakerScript.IsLocalPlayer(obj.transform))
                {
                    Light[] lights = obj.GetComponentsInChildren<Light>();
                    foreach (Light light in lights)
                    {
                        if (light.name == "Flashlight")
                        {
                            light.enabled = val;
                            break;
                        }
                    }
                    break;
                }
            }
            if (FlashlightToggle != null)
            {
                FlashlightToggle.isOn = val;
            }
        }

        // 안개 체크박스 변경 이벤트 핸들러
        /// <summary>
        /// Fog change
        /// </summary>
        /// <param name="val">새 값</param>
        public void FogChanged(bool val)
        {
            if (WeatherMakerFullScreenFogScript.Instance == null || WeatherMakerFullScreenFogScript.Instance.FogProfile == null)
            {
                Debug.LogError("안개가 설정되지 않았습니다. 2D를 사용하는 경우, 안개는 아직 지원되지 않습니다.");
            }
            else
            {
                // 안개가 활성화되지 않은 경우 시작 안개 밀도를 0으로 설정
                float startFogDensity = WeatherMakerFullScreenFogScript.Instance.FogProfile.FogDensity;
                float endFogDensity = (startFogDensity == 0.0f ? 0.007f : 0.0f);
                WeatherMakerFullScreenFogScript.Instance.TransitionFogDensity(startFogDensity, endFogDensity, TransitionDurationSlider.value);
            }
        }

        // 자동 날씨 체크박스 변경 이벤트 핸들러
        /// <summary>
        /// Automatic weather value change
        /// </summary>
        /// <param name="val">새 값</param>
        public void ManagerChanged(bool val)
        {
            if (WeatherMakerScript.Instance != null)
            {
                Transform parent = WeatherMakerScript.Instance.transform;
                for (int i = 0; i < parent.childCount; i++)
                {
                    Transform t = parent.GetChild(i);
                    if (t.GetComponent<WeatherMakerWeatherZoneScript>() != null)
                    {
                        t.gameObject.SetActive(val);
                        break;
                    }
                }
            }
        }

        // 시간 진행 체크박스 변경 이벤트 핸들러
        /// <summary>
        /// Auto time of day value change
        /// </summary>
        /// <param name="val">새 값</param>
        public void TimeOfDayEnabledChanged(bool val)
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                WeatherMakerDayNightCycleManagerScript.Instance.Speed = WeatherMakerDayNightCycleManagerScript.Instance.NightSpeed = (val ? 10.0f : 0.0f);
            }
        }

        // 번개 버튼 클릭 이벤트 핸들러
        /// <summary>
        /// Lightning bolt button click handler
        /// </summary>
        public void LightningStrikeButtonClicked()
        {
            if (WeatherMakerThunderAndLightningScript.Instance != null)
            {
                WeatherMakerThunderAndLightningScript.Instance.CallIntenseLightning();
            }
        }

        // 새벽/황혼 슬라이더 변경 이벤트 핸들러
        /// <summary>
        /// Time of day slider change handler
        /// </summary>
        /// <param name="val">새 값 (초), 0 ~ 86400</param>
        public void DawnDuskSliderChanged(float val)
        {
            // 자동 주기에서는 슬라이더 값에 따라 변경하지 않음
        }
    }
}
