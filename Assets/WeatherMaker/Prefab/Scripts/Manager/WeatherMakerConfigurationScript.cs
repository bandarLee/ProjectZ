//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// 소스 코드는 개인 또는 상업 프로젝트에 사용할 수 있습니다.
// 소스 코드는 재배포되거나 판매될 수 없습니다.
// 
// *** 불법 복제에 대한 참고 사항 ***
// 
// 이 자산을 해적 사이트에서 얻었다면 Unity 자산 스토어에서 구매하는 것을 고려해 주세요. 이 자산은 Unity Asset Store에서만 합법적으로 제공됩니다.
// 
// 저는 이 자산과 다른 자산에 수백, 수천 시간을 투자하여 저의 가족을 부양하는 단일 인디 개발자입니다. 이렇게 많은 노력을 기울인 소프트웨어를 훔치는 것은 매우 불쾌하고, 무례하며, 단순히 악의적입니다.
// 
// 감사합니다.
//
// *** 불법 복제에 대한 참고 사항 끝 ***
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Weather Maker 설정 측면 패널에 대한 구성 스크립트
    /// </summary>
    public class WeatherMakerConfigurationScript : MonoBehaviour
    {
        /// <summary>
        /// FPS를 표시할지 여부
        /// </summary>
        public bool ShowFPS = true;

        /// <summary>
        /// 하루 시간을 표시할지 여부
        /// </summary>
        public bool ShowTimeOfDay = true;

        /// <summary>
        /// 시작 시 자동으로 조명을 추가할지 여부
        /// </summary>
        public bool AutoAddLightsOnStart = true;

        /// <summary>
        /// 설정 패널
        /// </summary>
        public GameObject ConfigurationPanel;

        /// <summary>
        /// FPS를 표시할 레이블
        /// </summary>
        public UnityEngine.UI.Text LabelFPS;

        /// <summary>
        /// 전환 지속 시간 슬라이더
        /// </summary>
        public UnityEngine.UI.Slider TransitionDurationSlider;

        /// <summary>
        /// 강도 슬라이더
        /// </summary>
        public UnityEngine.UI.Slider IntensitySlider;

        /// <summary>
        /// 마우스 룩 체크박스
        /// </summary>
        public UnityEngine.UI.Toggle MouseLookEnabledCheckBox;

        /// <summary>
        /// 손전등 체크박스
        /// </summary>
        public UnityEngine.UI.Toggle FlashlightToggle;

        /// <summary>
        /// 하루 시간 체크박스
        /// </summary>
        public UnityEngine.UI.Toggle TimeOfDayEnabledCheckBox;

        /// <summary>
        /// 충돌 체크박스
        /// </summary>
        public UnityEngine.UI.Toggle CollisionToggle;

        /// <summary>
        /// 하루 시간 슬라이더
        /// </summary>
        public UnityEngine.UI.Slider DawnDuskSlider;

        /// <summary>
        /// 하루 시간 텍스트
        /// </summary>
        public UnityEngine.UI.Text TimeOfDayText;

        /// <summary>
        /// 하루 시간 범주 텍스트
        /// </summary>
        public UnityEngine.UI.Text TimeOfDayCategoryText;

        /// <summary>
        /// 구름 선택 드롭다운
        /// </summary>
        public UnityEngine.UI.Dropdown CloudDropdown;

        /// <summary>
        /// 날씨 지도 이미지
        /// </summary>
        public UnityEngine.UI.RawImage WeatherMapImage;

        /// <summary>
        /// 이벤트 시스템
        /// </summary>
        public UnityEngine.EventSystems.EventSystem EventSystem;

        /// <summary>
        /// 측면 패널 루트 객체
        /// </summary>
        public GameObject SidePanel;

        // 내부 변수
        private int frameCount = 0;
        private float nextFrameUpdate = 0.0f;
        private float fps = 0.0f;
        private float frameUpdateRate = 4.0f; // 초당 4회 업데이트
        private int frameCounter;
        private WeatherMakerCloudType clouds;
        private WeatherMakerCloudType lastClouds;

        // 하루 시간을 업데이트하는 메서드
        private void UpdateTimeOfDay()
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                DawnDuskSlider.value = WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay;
                if (TimeOfDayText.IsActive() && ShowTimeOfDay)
                {
                    System.TimeSpan t = System.TimeSpan.FromSeconds(WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay);
                    TimeOfDayText.text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
                }
                TimeOfDayCategoryText.text = WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDayCategory.ToString();
            }
        }

        // FPS를 표시하는 메서드
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

        // 초기화 메서드
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

        // 매 프레임마다 업데이트되는 메서드
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
                // Unity 버그: 트랜스폼 스케일을 0으로 설정하면 씬의 모든 프로젝터가 깨짐
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

            /*
            // 추가 씬 로딩 테스트
            if (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.LeftShift))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                return;
            }
            */

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

        // 날씨 구성...

        // 구름을 업데이트하는 메서드
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
                    // 사용자 지정 구름, 현재 구름 스크립트 상태를 수정하지 않음
                }
            }
        }

        /// <summary>
        /// 비 변경 핸들러
        /// </summary>
        /// <param name="isOn">True if on</param>
        public void RainToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Rain : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        /// <summary>
        /// 눈 변경 핸들러
        /// </summary>
        /// <param name="isOn">True if on</param>
        public void SnowToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Snow : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        /// <summary>
        /// 우박 변경 핸들러
        /// </summary>
        /// <param name="isOn">True if on</param>
        public void HailToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Hail : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        /// <summary>
        /// 진눈깨비 변경 핸들러
        /// </summary>
        /// <param name="isOn">True if on</param>
        public void SleetToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Sleet : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        /// <summary>
        /// 구름 변경 핸들러
        /// </summary>
        public void CloudToggleChanged()
        {
            string text = CloudDropdown.captionText.text;
            text = text.Replace("-", string.Empty).Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            clouds = (WeatherMakerCloudType)System.Enum.Parse(typeof(WeatherMakerCloudType), text);
            UpdateClouds();
        }

        /// <summary>
        /// 번개 변경 핸들러
        /// </summary>
        /// <param name="isOn">True if on</param>
        public void LightningToggleChanged(bool isOn)
        {
            if (WeatherMakerThunderAndLightningScript.Instance != null)
            {
                WeatherMakerThunderAndLightningScript.Instance.EnableLightning = isOn;
            }
        }

        /// <summary>
        /// 강수 충돌 변경 핸들러
        /// </summary>
        /// <param name="isOn">True if on</param>
        public void CollisionToggleChanged(bool isOn)
        {
            if (WeatherMakerScript.Instance != null)
            {
                WeatherMakerScript.Instance.PerformanceProfile.EnablePrecipitationCollision = isOn;
            }
        }

        /// <summary>
        /// 바람 변경 핸들러
        /// </summary>
        /// <param name="isOn">True if on</param>
        public void WindToggleChanged(bool isOn)
        {
            if (WeatherMakerScript.Instance == null || WeatherMakerPrecipitationManagerScript.Instance == null || WeatherMakerWindScript.Instance == null)
            {
                return;
            }

            float duration = WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration;
            if (isOn)
            {
                // 실행 중에 변경을 방지하기 위해 복사본을 만듦, 실행 중에 편집하려면 프로필을 인스펙터에 드래그
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_MediumWind"), 0.0f, duration);
            }
            else
            {
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_None"), 0.0f, duration);
            }
        }

        /// <summary>
        /// 전환 지속 시간 슬라이더 변경
        /// </summary>
        /// <param name="val">새 값</param>
        public void TransitionDurationSliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration = val;
            }
        }

        /// <summary>
        /// 강도 지속 시간 슬라이더 변경
        /// </summary>
        /// <param name="val">새 값</param>
        public void IntensitySliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = val;
            }
        }

        /// <summary>
        /// 마우스 룩 값 변경
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

        /// <summary>
        /// 손전등 값 변경
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

        /// <summary>
        /// 안개 변경
        /// </summary>
        /// <param name="val">새 값</param>
        public void FogChanged(bool val)
        {
            if (WeatherMakerFullScreenFogScript.Instance == null || WeatherMakerFullScreenFogScript.Instance.FogProfile == null)
            {
                Debug.LogError("안개가 설정되지 않았습니다. 2D를 사용하는 경우 안개는 아직 지원되지 않습니다.");
            }
            else
            {
                // 안개가 활성화되지 않은 경우, 시작 안개 밀도를 0으로 설정, 그렇지 않으면 현재 밀도로 시작
                float startFogDensity = WeatherMakerFullScreenFogScript.Instance.FogProfile.FogDensity;
                float endFogDensity = (startFogDensity == 0.0f ? 0.007f : 0.0f);
                WeatherMakerFullScreenFogScript.Instance.TransitionFogDensity(startFogDensity, endFogDensity, TransitionDurationSlider.value);
            }
        }

        /// <summary>
        /// 자동 날씨 값 변경
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

        /// <summary>
        /// 하루 시간 자동 값 변경
        /// </summary>
        /// <param name="val">새 값</param>
        public void TimeOfDayEnabledChanged(bool val)
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                WeatherMakerDayNightCycleManagerScript.Instance.Speed = WeatherMakerDayNightCycleManagerScript.Instance.NightSpeed = (val ? 10.0f : 0.0f);
            }
        }

        /// <summary>
        /// 번개 스트라이크 버튼 클릭 핸들러
        /// </summary>
        public void LightningStrikeButtonClicked()
        {
            if (WeatherMakerThunderAndLightningScript.Instance != null)
            {
                WeatherMakerThunderAndLightningScript.Instance.CallIntenseLightning();
            }
        }

        /// <summary>
        /// 하루 시간 슬라이더 변경 핸들러
        /// </summary>
        /// <param name="val">새 값(초 단위, 0 ~ 86400)</param>
        public void DawnDuskSliderChanged(float val)
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay = val;
            }
        }
    }
}
