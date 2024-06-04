using Photon.Pun;
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
    private Animator _animator;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        CharacterStatAbility ability = GetComponent<CharacterStatAbility>();
        Stat = ability.Stat;
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _inventoryManager = FindObjectOfType<InventoryManager>();

        if (_inventoryManager != null)
        {
            _inventoryManager.characterRotateAbility = GetComponent<CharacterRotateAbility>();
        }

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

        // �κ� ������ ������ �� ���� ����Ʈ ����
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            SetSpawnPoint();
        }
    }

    private void OnDestroy()
    {
        if (PhotonView.IsMine && LocalPlayerInstance == this)
        {
            LocalPlayerInstance = null;
        }
    }

    // ������ ����ȭ�� ���� ������ ���� �� ���� ����� ���� ���
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
        if (UI_RoomInfo.Instance != null)
        {
            UI_RoomInfo.Instance.AddLog(logMessage);
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
                OnDeath();
            }
            PhotonView.RPC(nameof(Death), RpcTarget.All);
        }
        else
        {
            if (this != null) // ������Ʈ�� �ı��Ǿ����� Ȯ��
            {
                GetComponent<Animator>().SetTrigger("Damage");
            }
        }
    }

    private void OnDeath()
    {
        string logMessage = $"\n{PhotonView.Owner.NickName}�� ����� ���߽��ϴ�.";
        PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
    }

    [PunRPC]
    private void Death()
    {
        if (State == State.Death)
        {
            return;
        }

        State = State.Death;

        if (this != null) // ������Ʈ�� �ı��Ǿ����� Ȯ��
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
        // ������ ��� ����
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        if (this != null) // ������Ʈ�� �ı��Ǿ����� Ȯ��
        {
            SetRespawnPoint();
            PhotonView.RPC(nameof(Live), RpcTarget.All);
        }
    }

    // �κ� ������ �÷��̾ ������ ������ RespawnPoint�� ����
    private void SetSpawnPoint()
    {
        Vector3 spawnPoint = GameManager.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterMoveAbilityTwo>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    // �÷��̾ �׾��� �� ���������� �ִ� ������ RespawnPoint�� ������
    private void SetRespawnPoint()
    {
        Vector3 spawnPoint = GameManager.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterMoveAbilityTwo>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    [PunRPC]
    private void Live()
    {
        State = State.Live;

        CharacterStatAbility ability = GetComponent<CharacterStatAbility>();
        Stat = ability.Stat;
        Stat.Init();

        if (this != null) // ������Ʈ�� �ı��Ǿ����� Ȯ��
        {
            GetComponent<Animator>().SetTrigger("Live");
        }
    }
}
