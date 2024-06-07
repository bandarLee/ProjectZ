using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterGunFireAbility : CharacterAbility
{
    private Animator _animator;

    public Gun CurrentGun; // 현재 들고있는 총
    public GameObject[] GunObject;

    private float _shotTimer;

    public GameObject CrosshairUI;
    public GameObject HolographicDotSightUI;

    // UI 위에 text로 표시하기 (ex. 30/30, 재장전 중입니다)
    public TextMeshProUGUI GunTextUI;
    public TextMeshProUGUI ReloadTextUI;

    public ParticleSystem HitEffect; // 이펙트 파편 
    public List<GameObject> MuzzleEffects; // 이펙트 반짝 

    private bool _isReloading = false;

    private Inventory _playerinventory;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        DeactivateAllGuns();

        foreach (GameObject muzzleEffect in MuzzleEffects)
        {
            muzzleEffect.SetActive(false);
        }

        RefreshUI();
        _playerinventory = Character.LocalPlayerInstance.GetComponent<Inventory>();
    }

    public void RefreshUI()
    {
        GunTextUI.text = $"{CurrentGun.BulletRemainCount}/{CurrentGun.BulletMaxCount}";

    }



    private void Update()
    {
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine)
        {
            return;
        }
  
        GunShoot();
    }
    private Item GetBulletItem()
    {
        foreach (var item in _playerinventory.items.Values)
        {
            if (item.itemType == ItemType.Heal && item.itemName == "총알")
            {
                return item;
            }
        }
        return null;
    }
    void GunShoot() {
        /* 재장전 */
        // R키 누르면 1.5초 후 재장전(중간에 총 쏘는 행위를 하면 재장전 취소) // todo.총알이 있을 때만!
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


    public void GunActive(int GunNumber)
    {
        foreach (GameObject weapon in GunObject)
        {
            weapon.SetActive(false);

        }
        GunObject[GunNumber].SetActive(true);
    }


    public void DeactivateAllGuns()
    {
        foreach (GameObject weapon in GunObject)
        {
            weapon.SetActive(false);
        }
    }
}
