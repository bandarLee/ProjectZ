using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterGunFireAbility : CharacterAbility
{
    private Animator _animator;

    public Gun CurrentGun; // ���� ����ִ� ��
    public GameObject[] GunObject;

    private float _shotTimer;

    public GameObject CrosshairUI;
    public GameObject HolographicDotSightUI;

    // UI ���� text�� ǥ���ϱ� (ex. 30/30, ������ ���Դϴ�)
    public TextMeshProUGUI GunTextUI;
    public TextMeshProUGUI ReloadTextUI;

    public ParticleSystem HitEffect; // ����Ʈ ���� 
    public List<GameObject> MuzzleEffects; // ����Ʈ ��¦ 

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
            if (item.itemType == ItemType.Heal && item.itemName == "�Ѿ�")
            {
                return item;
            }
        }
        return null;
    }
    void GunShoot() {
        /* ������ */
        // RŰ ������ 1.5�� �� ������(�߰��� �� ��� ������ �ϸ� ������ ���) // todo.�Ѿ��� ���� ����!
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
