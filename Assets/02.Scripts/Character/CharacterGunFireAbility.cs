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
    public GameObject[] GunObject;

    private float _shotTimer;


    public UI_Gunfire uI_Gunfire;
    public ParticleSystem HitEffect; // ����Ʈ ���� 
    public List<GameObject> MuzzleEffects; // ����Ʈ ��¦ 

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
        if (Input.GetKeyDown(KeyCode.R) && CurrentGun.BulletRemainCount < CurrentGun.BulletMaxCount && GetBulletItem()!= null)
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


            /* ü�� ��� ��� */
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // ���� ����- ��ġ�� ������ ����
            RaycastHit other; // ���� �ε��� ����� ���� �޾ƿ�
            bool isHit = Physics.Raycast(ray, out other);
            Debug.Log("Raycast hit: " + isHit);  // ����ĳ��Ʈ �浹 ���� ���� �α�
            if (isHit)
            {
                Debug.Log("Hit position: " + other.point); // �ε��� ��ġ �α�
                Debug.Log("Hit normal: " + other.normal); // �ε��� ��ġ�� ���� ���� �α�
                IDamaged damagedAbleObject = other.collider.GetComponent<IDamaged>();
                if (damagedAbleObject != null)
                {
                    Debug.Log("Damaged object found");
                    PhotonView photonView = other.collider.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        Debug.Log("PhotonView found");
                        Vector3 hitPosition = other.point;
                        Quaternion hitRotation = Quaternion.LookRotation(other.normal);
                        ParticleSystem hitEffectInstance = Instantiate(HitEffect, hitPosition, hitRotation);
                        hitEffectInstance.Play();

                        photonView.RPC("Damaged", RpcTarget.All, CurrentGun.Damage, Owner.PhotonView.OwnerActorNr);
                    }
                    
                }
                

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
        GunObject[GunNumber].SetActive(true);
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
        ResetAnimation();
    }

    public void DeactivateAllGuns()
    {
        if (Owner.PhotonView.IsMine)
        {
            Owner.PhotonView.RPC(nameof(DeactivateAllGunsRPC), RpcTarget.All);
        }
        DeactivateAllGunsRPC(); // ���ÿ����� ����
    }

    private void ResetAnimation()
    {
        _animator.SetLayerWeight(2, 0);
    }
}
