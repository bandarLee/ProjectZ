using Cinemachine;
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
    public Bullet Bullet;
    public string bulletTag = "Bullet";
    public string muzzleEffectTag = "SmallExplosion";
    public string explosionEffectTag = "BigExplosion";
    public GameObject[] GunObject;
    public GameObject bulletPrefab;
    public Transform FirePos;

    private float _shotTimer;

    public UI_Gunfire uI_Gunfire;

    private bool _isReloading = false;

    private Camera mainCamera;
    private CinemachineImpulseSource cameraShakeImpulse;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(InitializeCrossHair());

        _animator = GetComponent<Animator>();

        if (bulletPrefab != null)
        {
            Bullet = bulletPrefab.GetComponent<Bullet>();
        }
        GameObject followCamera = GameObject.FindWithTag("FollowCamera");
        //if (followCamera != null)
        {
            cameraShakeImpulse = followCamera.GetComponent<CinemachineImpulseSource>();
        }
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
        foreach (var item in Inventory.Instance.items.Values)
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
        uI_Gunfire.RefreshUI(); 

        /* 조준경 */
        if (Input.GetMouseButtonDown(1) && !Owner._characterRotateAbility.CharacterRotateLocked) 
        {
            bool isAiming = !uI_Gunfire.HolographicDotSightUI.activeSelf; // 현재 조준 상태 반전
            uI_Gunfire.ToggleSightMode(isAiming);
        }

        /* 재장전 */
        // R키 누르면 1.5초 후 재장전(중간에 총 쏘는 행위를 하면 재장전 취소) 
        if (Input.GetKeyDown(KeyCode.R) && (CurrentGun.BulletRemainCount < CurrentGun.BulletMaxCount) && GetBulletItem()!= null)
        {
            if (!_isReloading)
            {

                StartCoroutine(Reload_Coroutine()); 
            }

        }

        /* 총알발사 */
        _shotTimer += Time.deltaTime; 

        if (Input.GetMouseButton(0) && _shotTimer >= CurrentGun.FireCooltime && CurrentGun.BulletRemainCount > 0 && !Owner._quickSlotManager.ItemUseLock)
        {
            Owner._animator.SetInteger("UsingHand", 2);
            Character.LocalPlayerInstance._effectAudioManager.PlayAudio(3);

            StartCoroutine(TimeDelayGunShoot());
            // 재장전 취소
            if (_isReloading)
            {
                uI_Gunfire.RemoveReloadUI();
                StopAllCoroutines();
                _isReloading = false;
            }

            //if (cameraShakeImpulse != null)
            {
                cameraShakeImpulse.GenerateImpulse();
            }

            CurrentGun.BulletRemainCount--;
            Item bulletItem = GetBulletItem();

            ItemUseManager.Instance.UseConsumable(bulletItem);
            uI_Gunfire.RefreshUI();
            _shotTimer = 0;
            SpawnMuzzleEffect();


            if (bulletItem != null)
            {
                FireBullet();
            }
        }
    }
    IEnumerator TimeDelayGunShoot()
    {
        yield return new WaitForSeconds(0.2f);
        Owner._animator.SetInteger("UsingHand", 0);

    }
    private void FireBullet()
    {
        Vector3 fireDirection = GetFireDirection();
        GameObject bullet = ObjectPool.Instance.SpawnFromPool(bulletTag, FirePos.position, Quaternion.LookRotation(fireDirection));
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.OwnerAbility = this;
        }
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = fireDirection * Bullet.Force;
        }
    }

    public void SpawnExplosion(Vector3 position)
    {
        if (Owner.PhotonView.IsMine)
        {
            Character.LocalPlayerInstance._effectAudioManager.PlayAudio(2);

            Owner.PhotonView.RPC(nameof(SpawnExplosionRPC), RpcTarget.All, position);
        }
    }

    [PunRPC]
    private void SpawnExplosionRPC(Vector3 position)
    {
        GameObject explosionEffect = ObjectPool.Instance.SpawnFromPool(explosionEffectTag, position, Quaternion.identity);
        StartCoroutine(DisableExplosionEffect(explosionEffect));
    }

    private IEnumerator DisableExplosionEffect(GameObject explosionEffect)
    {
        ParticleSystem particleSystem = explosionEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startSizeX = 0.1f;
            main.startSizeY = 0.1f;
            main.startSizeZ = 0.1f;
        }

        yield return new WaitForSeconds(1f);
        explosionEffect.SetActive(false);
    }

    private void SpawnMuzzleEffect()
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(SpawnMuzzleEffectRPC), RpcTarget.All, FirePos.position);
        }
        
    }

    [PunRPC]
    private void SpawnMuzzleEffectRPC(Vector3 position)
    {
        GameObject muzzleEffect = ObjectPool.Instance.SpawnFromPool(muzzleEffectTag, position, Quaternion.identity);
        StartCoroutine(DisableMuzzleEffect(muzzleEffect));
    }

    private IEnumerator DisableMuzzleEffect(GameObject muzzleEffect)
    {
        ParticleSystem particleSystem = muzzleEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startSizeX = 0.1f;
            main.startSizeY = 0.1f;
            main.startSizeZ = 0.1f;
        }

        yield return new WaitForSeconds(0.1f);
        muzzleEffect.SetActive(false);
    }

    private Vector3 GetFireDirection()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(30); // 적당한 거리 포인트 설정
        }
        Vector3 fireDirection = (targetPoint - FirePos.position).normalized;

        if (Vector3.Dot(fireDirection, FirePos.forward) < 0)
        {
            Vector3 screenCenterDirection = (ray.origin + ray.direction * 30f) - FirePos.position;
            fireDirection = screenCenterDirection.normalized;
        }

        return fireDirection;
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
        if (CurrentGun.BulletMaxCount < Inventory.Instance.itemQuantities[bullet.itemName])
        {
            return CurrentGun.BulletMaxCount;

        }
        else
        {
            return Inventory.Instance.itemQuantities[bullet.itemName];
        }

    }

 [PunRPC]
    public void GunActiveRPC(int GunNumber)
    {
        foreach (GameObject weapon in GunObject)
        {
            weapon.SetActive(false);
        }
        StartCoroutine(ActivateGunAfterDelay(GunNumber, 0.1f));
    }

    private IEnumerator ActivateGunAfterDelay(int gunNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gunNumber >= 0 && gunNumber < GunObject.Length)
        {
            GunObject[gunNumber].SetActive(true);
        }
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

        Owner._animator.SetInteger("UsingHand", 0);
    }

    public void DeactivateAllGuns()
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(DeactivateAllGunsRPC), RpcTarget.All);
        }
        if(uI_Gunfire != null)
        {
            uI_Gunfire.tpFollow.ShoulderOffset = new Vector3(0.5f, 1f, -1.5f);

            uI_Gunfire.HolographicDotSightUI.SetActive(false);
            uI_Gunfire.CrosshairUI.SetActive(true);
            uI_Gunfire.RemoveRefreshUI();
            StartCoroutine(TimedelayDropItem());
        }
       
        DeactivateAllGunsRPC(); // 로컬에서도 실행
    }
    IEnumerator TimedelayDropItem()
    {
        Owner._animator.SetBool("IsDrop", true);

        yield return new WaitForSeconds(0.5f);
        Owner._animator.SetBool("IsDrop", false);

    }
}
