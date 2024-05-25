using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerConfigurationScript : MonoBehaviour
    {
        // ���� �ʵ�, Inspector���� ���� ����
        public bool ShowFPS = true; // FPS ǥ�� ����
        public bool ShowTimeOfDay = true; // ���� �ð� ǥ�� ����
        public bool AutoAddLightsOnStart = true; // ���� �� �ڵ����� ������ �߰����� ����
        public GameObject ConfigurationPanel; // ���� �г�
        public UnityEngine.UI.Text LabelFPS; // FPS�� ǥ���� �ؽ�Ʈ UI ���
        public UnityEngine.UI.Slider TransitionDurationSlider; // ��ȯ ���� �ð� �����̴�
        public UnityEngine.UI.Slider IntensitySlider; // ���� �����̴�
        public UnityEngine.UI.Toggle MouseLookEnabledCheckBox; // ���콺 ���� Ȱ��ȭ üũ�ڽ�
        public UnityEngine.UI.Toggle FlashlightToggle; // ������ ���
        public UnityEngine.UI.Toggle TimeOfDayEnabledCheckBox; // �ð� ���� Ȱ��ȭ üũ�ڽ�
        public UnityEngine.UI.Toggle CollisionToggle; // �浹 Ȱ��ȭ ���
        public UnityEngine.UI.Slider DawnDuskSlider; // ����/Ȳȥ �����̴�
        public UnityEngine.UI.Text TimeOfDayText; // ���� �ð��� ǥ���� �ؽ�Ʈ UI ���
        public UnityEngine.UI.Text TimeOfDayCategoryText; // �ð��븦 ǥ���� �ؽ�Ʈ UI ���
        public UnityEngine.UI.Dropdown CloudDropdown; // ���� ��Ӵٿ�
        public UnityEngine.UI.RawImage WeatherMapImage; // ���� �� �̹���
        public UnityEngine.EventSystems.EventSystem EventSystem; // �̺�Ʈ �ý���
        public GameObject SidePanel; // ���̵� �г�

        // ���� ���¸� �����ϱ� ���� ������
        private int frameCount = 0;
        private float nextFrameUpdate = 0.0f;
        private float fps = 0.0f;
        private float frameUpdateRate = 4.0f; // �ʴ� 4�� ������Ʈ
        private int frameCounter;
        private WeatherMakerCloudType clouds;
        private WeatherMakerCloudType lastClouds;

        // �ð� ������Ʈ �Լ�
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

        // FPS ǥ�� �Լ�
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

        // �ʱ�ȭ �Լ�
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

        // ������Ʈ �Լ�
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

        // ���� ������Ʈ �Լ�
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
                    // ����� ���� ����, ���� ���¸� �������� ����
                }
            }
        }

        // �� ��� ���� �̺�Ʈ �ڵ鷯
        /// <param name="isOn">��� ����</param>
        public void RainToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Rain : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // �� ��� ���� �̺�Ʈ �ڵ鷯
        /// <param name="isOn">��� ����</param>
        public void SnowToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Snow : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // ��� ��� ���� �̺�Ʈ �ڵ鷯
        /// <param name="isOn">��� ����</param>
        public void HailToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Hail : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // �������� ��� ���� �̺�Ʈ �ڵ鷯
        /// <param name="isOn">��� ����</param>
        public void SleetToggleChanged(bool isOn)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.Precipitation = (isOn ? WeatherMakerPrecipitationType.Sleet : WeatherMakerPrecipitationType.None);
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = IntensitySlider.value;
            }
        }

        // ���� ��� ���� �̺�Ʈ �ڵ鷯
        public void CloudToggleChanged()
        {
            string text = CloudDropdown.captionText.text;
            text = text.Replace("-", string.Empty).Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            clouds = (WeatherMakerCloudType)System.Enum.Parse(typeof(WeatherMakerCloudType), text);
            UpdateClouds();
        }

        // ���� ��� ���� �̺�Ʈ �ڵ鷯
        /// <param name="isOn">��� ����</param>
        public void LightningToggleChanged(bool isOn)
        {
            if (WeatherMakerThunderAndLightningScript.Instance != null)
            {
                WeatherMakerThunderAndLightningScript.Instance.EnableLightning = isOn;
            }
        }

        // �浹 ��� ���� �̺�Ʈ �ڵ鷯
        /// <param name="isOn">��� ����</param>
        public void CollisionToggleChanged(bool isOn)
        {
            if (WeatherMakerScript.Instance != null)
            {
                WeatherMakerScript.Instance.PerformanceProfile.EnablePrecipitationCollision = isOn;
            }
        }

        // �ٶ� ��� ���� �̺�Ʈ �ڵ鷯
        /// <param name="isOn">��� ����</param>
        public void WindToggleChanged(bool isOn)
        {
            if (WeatherMakerScript.Instance == null || WeatherMakerPrecipitationManagerScript.Instance == null || WeatherMakerWindScript.Instance == null)
            {
                return;
            }

            float duration = WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration;
            if (isOn)
            {
                // ���纻�� ����� ��Ÿ�� �� ������ ����
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_MediumWind"), 0.0f, duration);
            }
            else
            {
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_None"), 0.0f, duration);
            }
        }

        // ��ȯ ���� �ð� �����̴� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Transition duration slider change
        /// </summary>
        /// <param name="val">�� ��</param>
        public void TransitionDurationSliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration = val;
            }
        }

        // ���� �����̴� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Intensity duration slider change
        /// </summary>
        /// <param name="val">�� ��</param>
        public void IntensitySliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = val;
            }
        }

        // ���콺 ���� Ȱ��ȭ üũ�ڽ� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Mouse look value change
        /// </summary>
        /// <param name="val">�� ��</param>
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

        // ������ üũ�ڽ� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Flashlight value change
        /// </summary>
        /// <param name="val">�� ��</param>
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

        // �Ȱ� üũ�ڽ� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Fog change
        /// </summary>
        /// <param name="val">�� ��</param>
        public void FogChanged(bool val)
        {
            if (WeatherMakerFullScreenFogScript.Instance == null || WeatherMakerFullScreenFogScript.Instance.FogProfile == null)
            {
                Debug.LogError("�Ȱ��� �������� �ʾҽ��ϴ�. 2D�� ����ϴ� ���, �Ȱ��� ���� �������� �ʽ��ϴ�.");
            }
            else
            {
                // �Ȱ��� Ȱ��ȭ���� ���� ��� ���� �Ȱ� �е��� 0���� ����
                float startFogDensity = WeatherMakerFullScreenFogScript.Instance.FogProfile.FogDensity;
                float endFogDensity = (startFogDensity == 0.0f ? 0.007f : 0.0f);
                WeatherMakerFullScreenFogScript.Instance.TransitionFogDensity(startFogDensity, endFogDensity, TransitionDurationSlider.value);
            }
        }

        // �ڵ� ���� üũ�ڽ� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Automatic weather value change
        /// </summary>
        /// <param name="val">�� ��</param>
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

        // �ð� ���� üũ�ڽ� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Auto time of day value change
        /// </summary>
        /// <param name="val">�� ��</param>
        public void TimeOfDayEnabledChanged(bool val)
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                WeatherMakerDayNightCycleManagerScript.Instance.Speed = WeatherMakerDayNightCycleManagerScript.Instance.NightSpeed = (val ? 10.0f : 0.0f);
            }
        }

        // ���� ��ư Ŭ�� �̺�Ʈ �ڵ鷯
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

        // ����/Ȳȥ �����̴� ���� �̺�Ʈ �ڵ鷯
        /// <summary>
        /// Time of day slider change handler
        /// </summary>
        /// <param name="val">�� �� (��), 0 ~ 86400</param>
        public void DawnDuskSliderChanged(float val)
        {
            // �ڵ� �ֱ⿡���� �����̴� ���� ���� �������� ����
        }
    }
}
