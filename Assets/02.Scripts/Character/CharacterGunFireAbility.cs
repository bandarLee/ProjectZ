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

    public Gun CurrentGun; // ���� ����ִ� ��
    public Bullet Bullet;
    public GameObject[] GunObject;
    public GameObject bulletPrefab;
    public Transform FirePos;

    private float _shotTimer;

    public UI_Gunfire uI_Gunfire;
    public List<GameObject> MuzzleEffects; // ����Ʈ ��¦ 

    private bool _isReloading = false;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(InitializeCrossHair());

        _animator = GetComponent<Animator>();

        foreach (GameObject muzzleEffect in MuzzleEffects)
        {
            muzzleEffect.SetActive(false);
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
            if (item.itemType == ItemType.Consumable && item.itemName == "�Ѿ�")
            {
                return item;
            }
        }
        return null;
    }
    void GunShoot() 
    {
        uI_Gunfire.RefreshUI(); // ��� �Ѿ˰���, -�������� ���� �������� ���� �ߴ� �����-

        /* ���ذ� */
        if (Input.GetMouseButtonDown(1)) 
        {
            bool isAiming = !uI_Gunfire.HolographicDotSightUI.activeSelf; // ���� ���� ���� ����
            uI_Gunfire.ToggleSightMode(isAiming);
        }

        /* ������ */
        // RŰ ������ 1.5�� �� ������(�߰��� �� ��� ������ �ϸ� ������ ���) 
        if (Input.GetKeyDown(KeyCode.R) && (CurrentGun.BulletRemainCount < CurrentGun.BulletMaxCount) && GetBulletItem()!= null)
        {
            if (!_isReloading)
            {

                StartCoroutine(Reload_Coroutine()); // ������ ��
            }

        }

        /* �Ѿ˹߻� */
        _shotTimer += Time.deltaTime; // Fire ��Ÿ�Ӷ���

        if (Input.GetMouseButton(0) && _shotTimer >= CurrentGun.FireCooltime && CurrentGun.BulletRemainCount > 0 && !Owner._quickSlotManager.ItemUseLock)
        {
            // ������ ���
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


            /* ü�� ��� ��� */
            if (bulletItem != null)
            {
                FireBullet();
                //Instantiate(bulletPrefab, FirePos.position, FirePos.rotation);
            }

        }
    }

    private void FireBullet()
    {
        Vector3 fireDirection = GetFireDirection();
        GameObject bullet = Instantiate(bulletPrefab, FirePos.position, Quaternion.LookRotation(fireDirection));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = fireDirection * Bullet.Force;
        }
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
            targetPoint = ray.GetPoint(30); // ������ �Ÿ� ����Ʈ ����
        }
        Vector3 fireDirection = (targetPoint - FirePos.position).normalized;
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

    private IEnumerator MuzzleEffectOn_Coroutine()
    {
        // �� ����Ʈ �� �ϳ��� ����
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
        StartCoroutine(ActivateGunAfterDelay(GunNumber, 0.1f));
        // 0.1�� ��..
        //GunObject[GunNumber].SetActive(true);
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
        GunActiveRPC(GunNumber); // ���ÿ����� ����
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
        uI_Gunfire.tpFollow.ShoulderOffset = new Vector3(0.5f, 1f, -1.5f);

        uI_Gunfire.HolographicDotSightUI.SetActive(false);
        uI_Gunfire.CrosshairUI.SetActive(true);
        uI_Gunfire.RemoveRefreshUI();
        DeactivateAllGunsRPC(); // ���ÿ����� ����
    }
}
