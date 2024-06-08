using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class CharacterGunFireAbility : CharacterAbility
{
    private Animator _animator;

    public Gun CurrentGun; // 현재 들고있는 총
    public GameObject[] GunObject;

    private float _shotTimer;


    public UI_Gunfire uI_Gunfire;
    public ParticleSystem HitEffect; // 이펙트 파편 
    public List<GameObject> MuzzleEffects; // 이펙트 반짝 

    private bool _isReloading = false;

    private Inventory _playerinventory;

    private void Start()
    {
        StartCoroutine(InitializeCrossHair());

        _animator = GetComponent<Animator>();
        DeactivateAllGuns();

        foreach (GameObject muzzleEffect in MuzzleEffects)
        {
            muzzleEffect.SetActive(false);
        }

        _playerinventory = Character.LocalPlayerInstance.GetComponent<Inventory>();
    }
    private IEnumerator InitializeCrossHair()
    {
        yield return new WaitForSeconds(1.0f);
        uI_Gunfire = FindObjectOfType<UI_Gunfire>();
    }




    private void Update()
    {
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine)
        {
            return;
        }
        if (Character.LocalPlayerInstance._quickSlotManager.currentEquippedItem != null && Character.LocalPlayerInstance._quickSlotManager.currentEquippedItem.itemType == ItemType.Gun)
        {
            Owner.PhotonView.RPC(nameof(PlayAimingIdleAnimation), RpcTarget.All);
            GunShoot();
        }
    }
    private Item GetBulletItem()
    {
        foreach (var item in _playerinventory.items.Values)
        {
            if (item.itemType == ItemType.Consumable && item.itemName == "총알")
            {
                return item;
            }
        }
        return null;
    }
    void GunShoot() 
    {
        uI_Gunfire.RefreshUI(); // 얘는 총알개수, -이제부터 총을 장착했을 때만 뜨는 내용들-

        /* 조준경 */
        if (Input.GetMouseButtonDown(1)) 
        {
            bool isAiming = !uI_Gunfire.HolographicDotSightUI.activeSelf; // 현재 조준 상태 반전
            uI_Gunfire.ToggleSightMode(isAiming);
        }

        /* 재장전 */
        // R키 누르면 1.5초 후 재장전(중간에 총 쏘는 행위를 하면 재장전 취소) 
        if (Input.GetKeyDown(KeyCode.R) && CurrentGun.BulletRemainCount < CurrentGun.BulletMaxCount && GetBulletItem()!= null)
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
                uI_Gunfire.RemoveReloadUI();
                StopAllCoroutines();
                _isReloading = false;
            }

            Owner.PhotonView.RPC(nameof(PlayShotAnimation), RpcTarget.All, 1);

            CurrentGun.BulletRemainCount--;
            Item bulletItem = GetBulletItem();

            ItemUseManager.Instance.UseConsumable(bulletItem);
            uI_Gunfire.RefreshUI();
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
    public void PlayAimingIdleAnimation()
    {
        _animator.SetLayerWeight(2, 1);
    }

    [PunRPC]
    public void PlayShotAnimation(int index)
    {
        _animator.SetTrigger($"Shot{index}");
    }


    private IEnumerator Reload_Coroutine()
    {
        _isReloading = true;
        uI_Gunfire.MakeReloadUI();
        yield return new WaitForSeconds(CurrentGun.ReloadTime);

        Item bulletItem = GetBulletItem();

        if (bulletItem != null)
        {
            int bulletsToReload = CalculateRemainBullet(bulletItem);
      
            CurrentGun.BulletRemainCount = bulletsToReload;
        }
        uI_Gunfire.RefreshUI();
        uI_Gunfire.RemoveReloadUI();
        _isReloading = false;
        yield break;
    }
    public int CalculateRemainBullet(Item bullet)
    {
        if (CurrentGun.BulletMaxCount < _playerinventory.itemQuantities[bullet.itemName])
        {
            return CurrentGun.BulletMaxCount;

        }
        else
        {
            return _playerinventory.itemQuantities[bullet.itemName];
        }

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
        foreach (GameObject gun in GunObject)
        {
            gun.GetComponent<Gun>().BulletRemainCount = 0;
            gun.SetActive(false);
        }
        //uI_Gunfire.RemoveRefreshUI();

        ResetAnimation();
    }

    private void ResetAnimation()
    {
        _animator.SetLayerWeight(2, 0);
    }
}
