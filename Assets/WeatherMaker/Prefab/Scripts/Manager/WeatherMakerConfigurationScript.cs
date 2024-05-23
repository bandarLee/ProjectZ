//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// �ҽ� �ڵ�� ���� �Ǵ� ��� ������Ʈ�� ����� �� �ֽ��ϴ�.
// �ҽ� �ڵ�� ������ǰų� �Ǹŵ� �� �����ϴ�.
// 
// *** �ҹ� ������ ���� ���� ���� ***
// 
// �� �ڻ��� ���� ����Ʈ���� ����ٸ� Unity �ڻ� ������ �����ϴ� ���� ����� �ּ���. �� �ڻ��� Unity Asset Store������ �չ������� �����˴ϴ�.
// 
// ���� �� �ڻ�� �ٸ� �ڻ꿡 ����, ��õ �ð��� �����Ͽ� ���� ������ �ξ��ϴ� ���� �ε� �������Դϴ�. �̷��� ���� ����� ����� ����Ʈ��� ��ġ�� ���� �ſ� �����ϰ�, �����ϸ�, �ܼ��� �������Դϴ�.
// 
// �����մϴ�.
//
// *** �ҹ� ������ ���� ���� ���� �� ***
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Weather Maker ���� ���� �гο� ���� ���� ��ũ��Ʈ
    /// </summary>
    public class WeatherMakerConfigurationScript : MonoBehaviour
    {
        /// <summary>
        /// FPS�� ǥ������ ����
        /// </summary>
        public bool ShowFPS = true;

        /// <summary>
        /// �Ϸ� �ð��� ǥ������ ����
        /// </summary>
        public bool ShowTimeOfDay = true;

        /// <summary>
        /// ���� �� �ڵ����� ������ �߰����� ����
        /// </summary>
        public bool AutoAddLightsOnStart = true;

        /// <summary>
        /// ���� �г�
        /// </summary>
        public GameObject ConfigurationPanel;

        /// <summary>
        /// FPS�� ǥ���� ���̺�
        /// </summary>
        public UnityEngine.UI.Text LabelFPS;

        /// <summary>
        /// ��ȯ ���� �ð� �����̴�
        /// </summary>
        public UnityEngine.UI.Slider TransitionDurationSlider;

        /// <summary>
        /// ���� �����̴�
        /// </summary>
        public UnityEngine.UI.Slider IntensitySlider;

        /// <summary>
        /// ���콺 �� üũ�ڽ�
        /// </summary>
        public UnityEngine.UI.Toggle MouseLookEnabledCheckBox;

        /// <summary>
        /// ������ üũ�ڽ�
        /// </summary>
        public UnityEngine.UI.Toggle FlashlightToggle;

        /// <summary>
        /// �Ϸ� �ð� üũ�ڽ�
        /// </summary>
        public UnityEngine.UI.Toggle TimeOfDayEnabledCheckBox;

        /// <summary>
        /// �浹 üũ�ڽ�
        /// </summary>
        public UnityEngine.UI.Toggle CollisionToggle;

        /// <summary>
        /// �Ϸ� �ð� �����̴�
        /// </summary>
        public UnityEngine.UI.Slider DawnDuskSlider;

        /// <summary>
        /// �Ϸ� �ð� �ؽ�Ʈ
        /// </summary>
        public UnityEngine.UI.Text TimeOfDayText;

        /// <summary>
        /// �Ϸ� �ð� ���� �ؽ�Ʈ
        /// </summary>
        public UnityEngine.UI.Text TimeOfDayCategoryText;

        /// <summary>
        /// ���� ���� ��Ӵٿ�
        /// </summary>
        public UnityEngine.UI.Dropdown CloudDropdown;

        /// <summary>
        /// ���� ���� �̹���
        /// </summary>
        public UnityEngine.UI.RawImage WeatherMapImage;

        /// <summary>
        /// �̺�Ʈ �ý���
        /// </summary>
        public UnityEngine.EventSystems.EventSystem EventSystem;

        /// <summary>
        /// ���� �г� ��Ʈ ��ü
        /// </summary>
        public GameObject SidePanel;

        // ���� ����
        private int frameCount = 0;
        private float nextFrameUpdate = 0.0f;
        private float fps = 0.0f;
        private float frameUpdateRate = 4.0f; // �ʴ� 4ȸ ������Ʈ
        private int frameCounter;
        private WeatherMakerCloudType clouds;
        private WeatherMakerCloudType lastClouds;

        // �Ϸ� �ð��� ������Ʈ�ϴ� �޼���
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

        // FPS�� ǥ���ϴ� �޼���
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

        // �ʱ�ȭ �޼���
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

        // �� �����Ӹ��� ������Ʈ�Ǵ� �޼���
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
                // Unity ����: Ʈ������ �������� 0���� �����ϸ� ���� ��� �������Ͱ� ����
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
            // �߰� �� �ε� �׽�Ʈ
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

        // ���� ����...

        // ������ ������Ʈ�ϴ� �޼���
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
                    // ����� ���� ����, ���� ���� ��ũ��Ʈ ���¸� �������� ����
                }
            }
        }

        /// <summary>
        /// �� ���� �ڵ鷯
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
        /// �� ���� �ڵ鷯
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
        /// ��� ���� �ڵ鷯
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
        /// �������� ���� �ڵ鷯
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
        /// ���� ���� �ڵ鷯
        /// </summary>
        public void CloudToggleChanged()
        {
            string text = CloudDropdown.captionText.text;
            text = text.Replace("-", string.Empty).Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            clouds = (WeatherMakerCloudType)System.Enum.Parse(typeof(WeatherMakerCloudType), text);
            UpdateClouds();
        }

        /// <summary>
        /// ���� ���� �ڵ鷯
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
        /// ���� �浹 ���� �ڵ鷯
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
        /// �ٶ� ���� �ڵ鷯
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
                // ���� �߿� ������ �����ϱ� ���� ���纻�� ����, ���� �߿� �����Ϸ��� �������� �ν����Ϳ� �巡��
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_MediumWind"), 0.0f, duration);
            }
            else
            {
                WeatherMakerWindScript.Instance.SetWindProfileAnimated(WeatherMakerScript.Instance.LoadResource<WeatherMakerWindProfileScript>("WeatherMakerWindProfile_None"), 0.0f, duration);
            }
        }

        /// <summary>
        /// ��ȯ ���� �ð� �����̴� ����
        /// </summary>
        /// <param name="val">�� ��</param>
        public void TransitionDurationSliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationChangeDuration = val;
            }
        }

        /// <summary>
        /// ���� ���� �ð� �����̴� ����
        /// </summary>
        /// <param name="val">�� ��</param>
        public void IntensitySliderChanged(float val)
        {
            if (WeatherMakerPrecipitationManagerScript.Instance != null)
            {
                WeatherMakerPrecipitationManagerScript.Instance.PrecipitationIntensity = val;
            }
        }

        /// <summary>
        /// ���콺 �� �� ����
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

        /// <summary>
        /// ������ �� ����
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

        /// <summary>
        /// �Ȱ� ����
        /// </summary>
        /// <param name="val">�� ��</param>
        public void FogChanged(bool val)
        {
            if (WeatherMakerFullScreenFogScript.Instance == null || WeatherMakerFullScreenFogScript.Instance.FogProfile == null)
            {
                Debug.LogError("�Ȱ��� �������� �ʾҽ��ϴ�. 2D�� ����ϴ� ��� �Ȱ��� ���� �������� �ʽ��ϴ�.");
            }
            else
            {
                // �Ȱ��� Ȱ��ȭ���� ���� ���, ���� �Ȱ� �е��� 0���� ����, �׷��� ������ ���� �е��� ����
                float startFogDensity = WeatherMakerFullScreenFogScript.Instance.FogProfile.FogDensity;
                float endFogDensity = (startFogDensity == 0.0f ? 0.007f : 0.0f);
                WeatherMakerFullScreenFogScript.Instance.TransitionFogDensity(startFogDensity, endFogDensity, TransitionDurationSlider.value);
            }
        }

        /// <summary>
        /// �ڵ� ���� �� ����
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

        /// <summary>
        /// �Ϸ� �ð� �ڵ� �� ����
        /// </summary>
        /// <param name="val">�� ��</param>
        public void TimeOfDayEnabledChanged(bool val)
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                WeatherMakerDayNightCycleManagerScript.Instance.Speed = WeatherMakerDayNightCycleManagerScript.Instance.NightSpeed = (val ? 10.0f : 0.0f);
            }
        }

        /// <summary>
        /// ���� ��Ʈ����ũ ��ư Ŭ�� �ڵ鷯
        /// </summary>
        public void LightningStrikeButtonClicked()
        {
            if (WeatherMakerThunderAndLightningScript.Instance != null)
            {
                WeatherMakerThunderAndLightningScript.Instance.CallIntenseLightning();
            }
        }

        /// <summary>
        /// �Ϸ� �ð� �����̴� ���� �ڵ鷯
        /// </summary>
        /// <param name="val">�� ��(�� ����, 0 ~ 86400)</param>
        public void DawnDuskSliderChanged(float val)
        {
            if (WeatherMakerDayNightCycleManagerScript.HasInstance())
            {
                WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay = val;
            }
        }
    }
}
