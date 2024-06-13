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
    public GameObject bulletPrefab;
    public Transform FirePos;

    private float _shotTimer;


    public UI_Gunfire uI_Gunfire;
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
        _playerinventory = Inventory.Instance;

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

        if (Input.GetMouseButton(0) && _shotTimer >= CurrentGun.FireCooltime && CurrentGun.BulletRemainCount > 0 && !Owner._quickSlotManager.ItemUseLock)
        {
            // 재장전 취소
            if (_isReloading)
            {
                uI_Gunfire.RemoveReloadUI();
                StopAllCoroutines();
                _isReloading = false;
            }

            CurrentGun.BulletRemainCount--;
            Item bulletItem = GetBulletItem();

            ItemUseManager.Instance.UseConsumable(bulletItem);
            uI_Gunfire.RefreshUI();
            _shotTimer = 0;
            StartCoroutine(MuzzleEffectOn_Coroutine());


            /* 체력 닳는 기능 */
            if (bulletItem != null)
            {
                Instantiate(bulletPrefab, FirePos.position, FirePos.rotation);
            }

        }
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


 [PunRPC]
    public void GunActiveRPC(int GunNumber)
    {
        foreach (GameObject weapon in GunObject)
        {
            weapon.SetActive(false);
        }
        // 0.1초 후..
        GunObject[GunNumber].SetActive(true);
    }

    public void GunActive(int GunNumber)
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(GunActiveRPC), RpcTarget.All, GunNumber);
        }
        GunActiveRPC(GunNumber); // 로컬에서도 실행
    }

    [PunRPC]
    public void DeactivateAllGunsRPC()
    {
        foreach (GameObject gun in GunObject)
        {
            gun.GetComponent<Gun>().BulletRemainCount = 0;
            gun.SetActive(false);
        }
        //ResetAnimation();
        //characterItemAbility.UnUsingHandAnimation();
        Owner._animator.SetBool("RePullOut", true);
        Owner._animator.SetBool("isPullOut", false);
        Owner._animator.SetInteger("UsingHand", 0);
    }

    public void DeactivateAllGuns()
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(DeactivateAllGunsRPC), RpcTarget.All);
        }
        DeactivateAllGunsRPC(); // 로컬에서도 실행
    }
}
