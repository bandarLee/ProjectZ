using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterMoveAbilityTwo))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterStatAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterAbility))]
[RequireComponent(typeof(CharacterItemAbility))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public static Character LocalPlayerInstance;

    public PhotonView PhotonView { get; private set; }

    public Stat Stat { get; private set; }
    public State State { get; private set; } = State.Live;
    public Animator _animator;
    public InventoryManager _inventoryManager;
    public QuickSlotManager _quickSlotManager;
    public CharacterAttackAbility _attackability;
    public CharacterGunFireAbility _gunfireAbility;
    public CharacterRotateAbility _characterRotateAbility;
    public CharacterStatAbility _statability;

    private void Awake()
    {
        _statability = GetComponent<CharacterStatAbility>();
        Stat = _statability.Stat;
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _attackability = GetComponent<CharacterAttackAbility>();
        _gunfireAbility = GetComponent<CharacterGunFireAbility>();
        _quickSlotManager = FindObjectOfType<QuickSlotManager>();
        _characterRotateAbility = GetComponent<CharacterRotateAbility>();
        if (PhotonView.IsMine)
        {
            LocalPlayerInstance = this;

        }
    }

    private void Start()
    {
        if (!PhotonView.IsMine)
        {
            return;
        }

    }

    private void OnDestroy()
    {
        if (PhotonView.IsMine && LocalPlayerInstance == this)
        {
            LocalPlayerInstance = null;
        }
    }

    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Mental);
            stream.SendNext(Stat.Hunger);
            stream.SendNext(Stat.Temperature);
        }
        else if (stream.IsReading)
        {
            Stat.Health = (int)stream.ReceiveNext();
            Stat.Mental = (int)stream.ReceiveNext();
            Stat.Hunger = (int)stream.ReceiveNext();
            Stat.Temperature = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void AddLog(string logMessage)
    {
        if (UI_HintLog.Instance != null)
        {
            UI_HintLog.Instance.AddLog(logMessage);
        }
    }

    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        if (State == State.Death)
        {
            return;
        }
        Stat.Health -= damage;
        if (Stat.Health <= 0)
        {
            if (PhotonView.IsMine)
            {
                Death();

            }

        }
        else
        {
            if (this != null) // 오브젝트가 파괴되었는지 확인
            {
                GetComponent<Animator>().SetTrigger("Damage");
            }
        }
    }

    public void Death()
    {
        OnDeath();

        PhotonView.RPC(nameof(DeathRPC), RpcTarget.All);

        _quickSlotManager.DropAllItem();
        Character.LocalPlayerInstance._attackability.DeactivateAllWeapons();
        Character.LocalPlayerInstance._gunfireAbility.DeactivateAllGuns();

    }

    public void OnDeath()
    {
        string logMessage = $"\n{PhotonView.Owner.NickName}이 운명을 다했습니다.";
        PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
    }

    [PunRPC]
    private void DeathRPC()
    {
        if (State == State.Death)
        {
            return;
        }

        State = State.Death;

        if (this != null) // 오브젝트가 파괴되었는지 확인
        {
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<CharacterAttackAbility>().DeactivateAllColliders();

        }

        if (PhotonView.IsMine)
        {
            DropItems();
            StartCoroutine(Death_Coroutine());
        }
    }

    [PunRPC]
    private void DropItems()
    {
        // 아이템 드랍 로직
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        if (this != null) // 오브젝트가 파괴되었는지 확인
        {
            SetRespawnPoint();
            PhotonView.RPC(nameof(Live), RpcTarget.All);
        }
    }

    // 로비 씬에서 플레이어를 랜덤한 섹터의 RespawnPoint에 스폰
    private void SetSpawnPoint()
    {
        Vector3 spawnPoint = GameManager.Instance.GetSpawnPoint();
        GetComponent<CharacterMoveAbilityTwo>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    // 플레이어가 죽었을 때 마지막으로 있던 섹터의 RespawnPoint에 리스폰
    private void SetRespawnPoint()
    {
        Vector3 spawnPoint = GameManager.Instance.GetSpawnPoint();
        GetComponent<CharacterMoveAbilityTwo>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    [PunRPC]
    private void Live()
    {
        State = State.Live;

        Stat = _statability.Stat;
        Stat.Init();

        if (this != null) // 오브젝트가 파괴되었는지 확인
        {
            GetComponent<Animator>().SetTrigger("Live");
        }
    }

/*    public void DeactiveOtherCharacter()
    {
        int myCurrentScene = (int)PhotonNetwork.LocalPlayer.CustomProperties["CurrentScene"];

        foreach (Character character in FindObjectsOfType<Character>())
        {
            if (character.PhotonView.IsMine)
            {
                continue;
            }

            if (character.PhotonView.Owner.CustomProperties.TryGetValue("CurrentScene", out object otherScene))
            {
                if ((int)otherScene != myCurrentScene)
                {
                    character.gameObject.SetActive(false);
                }
            }
        };
    }*/
}