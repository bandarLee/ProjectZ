using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class CharacterGunFireAbility : CharacterAbility
{
    private Animator _animator;

    public Gun CurrentGun; // ���� ����ִ� ��

    private float _shotTimer;

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
        _shotTimer += Time.deltaTime; // Fire ��Ÿ�Ӷ���

        if (Input.GetMouseButton(0) && _shotTimer >= CurrentGun.FireCooltime && CurrentGun.BulletRemainCount > 0)
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
            _shotTimer = 0;
            StartCoroutine(MuzzleEffectOn_Coroutine());


            /* ü�� ��� ��� */
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // ���� ����- ��ġ�� ������ ����
            RaycastHit other; // ���� �ε��� ����� ���� �޾ƿ�
            bool isHit = Physics.Raycast(ray, out other);
            if (isHit)
            {
                IDamaged damagedAbleObject = other.collider.GetComponent<IDamaged>();
                if (damagedAbleObject != null)
                {
                    PhotonView photonView = other.collider.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        // �ǰ� ����Ʈ ����
                        Vector3 hitPosition = (transform.position + other.transform.position) / 2f + new Vector3(0f, 1f, 0f);
                        //PhotonNetwork.Instantiate("HitEffect", hitPosition, Quaternion.identity);
                        photonView.RPC("Damaged", RpcTarget.All, CurrentGun.Damage, Owner.PhotonView.OwnerActorNr);
                    }
                }

                /* �Ѿ� Ƣ�� ����Ʈ */
                HitEffect.gameObject.transform.position = other.point; // �ε��� ��ġ�� ����Ʈ ��ġ
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
        ReloadTextUI.text = $"������ ��";
        yield return new WaitForSeconds(CurrentGun.ReloadTime);

        CurrentGun.BulletRemainCount = CurrentGun.BulletMaxCount;
        RefreshUI();

        ReloadTextUI.text = "";
        _isReloading = false;
        yield break;
    }
    private IEnumerator MuzzleEffectOn_Coroutine()
    {
        // �� ����Ʈ �� �ϳ��� ����
        int randomIndex = UnityEngine.Random.Range(0, MuzzleEffects.Count);
        MuzzleEffects[randomIndex].SetActive(true);

        yield return new WaitForSeconds(0.1f);

        MuzzleEffects[randomIndex].SetActive(false);
    }



    
}
