using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterGunFireAbility : CharacterAbility
{
    private Animator _animator;

    public Gun CurrentGun; // ���� ����ִ� ��

    private float _timer;

    /****/
    private const int DefaultFOV = 60;
    private const int ZoomFOV = 20;
    private bool _isZoomMode = false; // �� ����?

    private const float ZoomInDuration = 0.3f;
    private const float ZoomOutDuration = 0.2f;
    private float _zoomProgress; // �� �����: 0 ~ 1

    public GameObject CrosshairUI;
    public GameObject CrosshairZoomUI;


    // ���콺 ���� ��ư�� ������ �ü��� �ٶ󺸴� �������� ���� �߻��Ѵ�.
    // - �Ѿ� Ƣ�� ����Ʈ ������
    public ParticleSystem HitEffect; // ����
    public List<GameObject> MuzzleEffects; // ��¦

    // UI ���� text�� ǥ���ϱ� (ex. 30/30, ������ ���Դϴ�)
    public TextMeshProUGUI GunTextUI;
    public TextMeshProUGUI ReloadTextUI;

    private bool _isReloading = false;

    // �� ������ Image�� ���� �غ�����(�κ��丮�� �ʿ�)


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
        /* �ܸ�� */
        if (Input.GetMouseButtonDown(2))
        {
            _isZoomMode = !_isZoomMode; // �� ��� ������
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

        /* ������ */
        // RŰ ������ 1.5�� �� ������(�߰��� �� ��� ������ �ϸ� ������ ���)
        if (Input.GetKeyDown(KeyCode.R) && CurrentGun.BulletRemainCount < CurrentGun.BulletMaxCount)
        {
            if (!_isReloading)
            {

                StartCoroutine(Reload_Coroutine()); // ������ ��
            }

        }

        /* �Ѿ˹߻� */
        _timer += Time.deltaTime; // Fire ��Ÿ�Ӷ���

        if (Input.GetMouseButton(0) && _timer >= CurrentGun.FireCooltime && CurrentGun.BulletRemainCount > 0)
        {
            // ������ ���
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
        ReloadTextUI.text = $"������ ��";
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
