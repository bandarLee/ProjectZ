using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Gunfire : MonoBehaviour
{
    public GameObject CrosshairUI;
    public GameObject HolographicDotSightUI;

    // UI 위에 text로 표시하기 (ex. 30/30, 재장전 중입니다)
    public TextMeshProUGUI GunTextUI;
    public TextMeshProUGUI ReloadTextUI;
    private CharacterGunFireAbility gunFireAbility;

    private void Start()
    {
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
}
