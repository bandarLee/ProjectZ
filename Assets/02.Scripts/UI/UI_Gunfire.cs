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
    //먹을때 이 동그라미 슬라이더 쓰는거에용

    // UI 위에 text로 표시하기 (ex. 30/30, 재장전 중입니다)
    public TextMeshProUGUI GunTextUI;
    public TextMeshProUGUI ReloadTextUI;
    private CharacterGunFireAbility gunFireAbility;

    public TextMeshProUGUI RemainTime;

    private void Start()
    {
        CircleSlider.SetActive(false);

        StartCoroutine(InitiategunFireAbility());

    }
    private IEnumerator InitiategunFireAbility()
    {
        yield return new WaitForSeconds(1.0f);
        gunFireAbility = Character.LocalPlayerInstance.GetComponent<CharacterGunFireAbility>();

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
        ReloadTextUI.text = $"재장전 중";

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
