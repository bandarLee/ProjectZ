using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterGunFireAbility : CharacterAbility
{
    private Animator _animator;

    public Gun CurrentGun; // 현재 들고있는 총

    private float _timer;

    /****/
    private const int DefaultFOV = 60;
    private const int ZoomFOV = 20;
    private bool _isZoomMode = false; // 줌 모드냐?

    private const float ZoomInDuration = 0.3f;
    private const float ZoomOutDuration = 0.2f;
    private float _zoomProgress; // 줌 진행률: 0 ~ 1

    public GameObject CrosshairUI;
    public GameObject CrosshairZoomUI;


    // 마우스 왼쪽 버튼을 누르면 시선이 바라보는 방향으로 총을 발사한다.
    // - 총알 튀는 이펙트 프리팹
    public ParticleSystem HitEffect; // 파편
    public List<GameObject> MuzzleEffects; // 반짝

    // UI 위에 text로 표시하기 (ex. 30/30, 재장전 중입니다)
    public TextMeshProUGUI GunTextUI;
    public TextMeshProUGUI ReloadTextUI;

    private bool _isReloading = false;

    // 총 아이콘 Image도 따로 준비하자(인벤토리에 필요)


    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        foreach (GameObject muzzleEffect in MuzzleEffects)
        {
            muzzleEffect.SetActive(false);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        GunTextUI.text = $"{CurrentGun.BulletRemainCount}/{CurrentGun.BulletMaxCount}";

        CrosshairUI.SetActive(!_isZoomMode);
        CrosshairZoomUI.SetActive(_isZoomMode);
    }



    private void Update()
    {
        /* 줌모드 */
        if (Input.GetMouseButtonDown(2))
        {
            _isZoomMode = !_isZoomMode; // 줌 모드 뒤집기
            _zoomProgress = 0f;
            RefreshUI();
        }

        if(_zoomProgress < 1)
        {
            if (_isZoomMode)
            {
                _zoomProgress += Time.deltaTime / ZoomInDuration;
                Camera.main.fieldOfView = Mathf.Lerp(DefaultFOV, ZoomFOV, _zoomProgress);
            }
            else
            {
                _zoomProgress += Time.deltaTime / ZoomOutDuration;
                Camera.main.fieldOfView = Mathf.Lerp(ZoomFOV, DefaultFOV, _zoomProgress);
            }
        }

        /* 재장전 */
        // R키 누르면 1.5초 후 재장전(중간에 총 쏘는 행위를 하면 재장전 취소)
        if (Input.GetKeyDown(KeyCode.R) && CurrentGun.BulletRemainCount < CurrentGun.BulletMaxCount)
        {
            if (!_isReloading)
            {

                StartCoroutine(Reload_Coroutine()); // 재장전 중
            }

        }

        /* 총알발사 */
        _timer += Time.deltaTime; // Fire 쿨타임땜에

        if (Input.GetMouseButton(0) && _timer >= CurrentGun.FireCooltime && CurrentGun.BulletRemainCount > 0)
        {
            // 재장전 취소
            if (_isReloading)
            {
                ReloadTextUI.text = "";
                StopAllCoroutines();
                _isReloading = false;
            }


            //Owner.PhotonView.RPC(nameof(PlayShotAnimation), RpcTarget.All, 1);

            CurrentGun.BulletRemainCount--;
            RefreshUI();
            _timer = 0;
        }

    }

    private IEnumerator Reload_Coroutine()
    {
        _isReloading = true;
        ReloadTextUI.text = $"재장전 중";
        yield return new WaitForSeconds(CurrentGun.ReloadTime);

        CurrentGun.BulletRemainCount = CurrentGun.BulletMaxCount;
        RefreshUI();

        ReloadTextUI.text = "";
        _isReloading = false;
        yield break;
    }

    [PunRPC]
    public void PlayShotAnimation(int index)
    {
        _animator.SetTrigger($"Shot{index}");
    }
}
