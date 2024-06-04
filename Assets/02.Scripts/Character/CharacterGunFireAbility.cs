using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class CharacterGunFireAbility : CharacterAbility
{
    private Animator _animator;

    public Gun CurrentGun; // 현재 들고있는 총

    private float _shotTimer;

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
        _animator = GetComponent<Animator>();

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
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine)
        {
            return;
        }

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
        _shotTimer += Time.deltaTime; // Fire 쿨타임땜에

        if (Input.GetMouseButton(0) && _shotTimer >= CurrentGun.FireCooltime && CurrentGun.BulletRemainCount > 0)
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
            _shotTimer = 0;
            StartCoroutine(MuzzleEffectOn_Coroutine());


            /* 체력 닳는 기능 */
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // 레이 생성- 위치와 방향을 설정
            RaycastHit other; // 레이 부딪힌 대상의 정보 받아옴
            bool isHit = Physics.Raycast(ray, out other);
            if (isHit)
            {
                IDamaged damagedAbleObject = other.collider.GetComponent<IDamaged>();
                if (damagedAbleObject != null)
                {
                    PhotonView photonView = other.collider.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        // 피격 이펙트 생성
                        Vector3 hitPosition = (transform.position + other.transform.position) / 2f + new Vector3(0f, 1f, 0f);
                        //PhotonNetwork.Instantiate("HitEffect", hitPosition, Quaternion.identity);
                        photonView.RPC("Damaged", RpcTarget.All, CurrentGun.Damage, Owner.PhotonView.OwnerActorNr);
                    }
                }

                /* 총알 튀는 이펙트 */
                HitEffect.gameObject.transform.position = other.point; // 부딪힌 위치에 이펙트 위치
                HitEffect.gameObject.transform.forward = other.normal;
                HitEffect.Play();
            }
        }
    }

    [PunRPC]
    public void PlayShotAnimation(int index)
    {
        _animator.SetTrigger($"Shot{index}");
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
    private IEnumerator MuzzleEffectOn_Coroutine()
    {
        // 총 이펙트 중 하나를 켜줌
        int randomIndex = UnityEngine.Random.Range(0, MuzzleEffects.Count);
        MuzzleEffects[randomIndex].SetActive(true);

        yield return new WaitForSeconds(0.1f);

        MuzzleEffects[randomIndex].SetActive(false);
    }



    
}
