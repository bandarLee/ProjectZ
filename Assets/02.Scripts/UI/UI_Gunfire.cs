using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_Gunfire : MonoBehaviour
{
    public GameObject CrosshairUI;
    public GameObject HolographicDotSightUI;
    public GameObject CircleSlider;
    public Slider CircleSliderComponent;
    //������ �� ���׶�� �����̴� ���°ſ���

    public CinemachineVirtualCamera followCamera;
    public Cinemachine3rdPersonFollow tpFollow;

    // UI ���� text�� ǥ���ϱ� (ex. 30/30, ������ ���Դϴ�)
    public TextMeshProUGUI GunTextUI;
    public TextMeshProUGUI ReloadTextUI;
    private CharacterGunFireAbility gunFireAbility;

    public TextMeshProUGUI RemainTime;

    private void Start()
    {
        CircleSlider.SetActive(false);

        StartCoroutine(InitiategunFireAbility());
        HolographicDotSightUI.SetActive(false);

        if (followCamera != null)
        {
            tpFollow = followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }
    }
    private IEnumerator InitiategunFireAbility()
    {
        yield return new WaitForSeconds(1.0f);
        gunFireAbility = Character.LocalPlayerInstance.GetComponent<CharacterGunFireAbility>();
        gunFireAbility.uI_Gunfire = FindObjectOfType<UI_Gunfire>();
    }

    public void ToggleSightMode(bool isAiming)
    {
        HolographicDotSightUI.SetActive(isAiming);
        CrosshairUI.SetActive(!isAiming);

        if (tpFollow != null)
        {
            if (isAiming)
            {
                tpFollow.ShoulderOffset = new Vector3(0f, 1f, 3f);
            }
            else
            {
                tpFollow.ShoulderOffset = new Vector3(0.5f, 1f, -1.5f);
            }
        }
    }

    public void RemoveRefreshUI()
    {
        GunTextUI.text = "";

    }
    public void RefreshUI()
    {
        GunTextUI.text = $"{gunFireAbility.CurrentGun.BulletRemainCount}/{gunFireAbility.CurrentGun.BulletMaxCount}";

    }
    public void RemoveReloadUI()
    {
        ReloadTextUI.text = "";

    }
    public void MakeReloadUI()
    {
        ReloadTextUI.text = $"���� ��";

    }
    public IEnumerator UseItemWithTimer(float duration, System.Action onComplete)
    {
        CrosshairUI.SetActive(false);
        CircleSlider.SetActive(true);
        float timeRemaining = duration;
        while (timeRemaining > 0)
        {
            if (Input.GetMouseButton(0))
            {
                timeRemaining -= Time.deltaTime;
                CircleSliderComponent.value = (duration - timeRemaining) / duration;
                RemainTime.text = $"{timeRemaining:F1}";
                yield return null;
            }
            else
            {
                break;
            }
        }

        CircleSlider.SetActive(false);
        CrosshairUI.SetActive(true);

        RemainTime.text = "";

        if (timeRemaining <= 0)
        {
            onComplete?.Invoke();
        }
    }
}
