using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Monster_Bat : MonoBehaviourPun, IPunObservable, IDamaged
{
    public enum MonsterState
    {
        Patrol,
        Chase,
        Attack,
        Death
    }

    public Animator animator;
    public float detectRange = 30f;
    public float attackRange = 2f;
    public float attackDamageRange = 5f;
    public float moveSpeed = 5f;
    public Stat stat;

    public MonsterState state = MonsterState.Patrol;
    private Character targetCharacter;
    private Vector3 initialPosition;
    private float attackTimer = 0f;

    private Vector3 syncPosition;
    private Quaternion syncRotation;

    private float lerpSpeed = 10f;

    private Rigidbody rb;

    private Vector3 targetPosition;
    public float changeDirectionInterval = 2f; // ������ �����ϴ� ����

    public GameObject[] CanMoveArea;
    public GameObject[] CantMoveArea;

    private float findTargetInterval = 0.5f; // Ÿ�� Ž�� ����
    private float findTargetTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        initialPosition = transform.position;
        syncPosition = transform.position;
        syncRotation = transform.rotation;

        if (!PhotonNetwork.IsMasterClient)
        {
            rb.isKinematic = true;
        }

        // �ʱ� ���� ��ġ ����
        StartCoroutine(ChangeDirectionRoutine());
    }
    private void OnEnable()
    {
        stat.Init();
        state = MonsterState.Patrol;
        Debug.Log("��Ʈ���� ����");

    }
    private void OnDisable()
    {
        stat.Init();
        Debug.Log("��Ʈ���� ���");
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            switch (state)
            {
                case MonsterState.Patrol:
                    Patrol();
                    break;
                case MonsterState.Chase:
                    Chase();
                    break;
                case MonsterState.Attack:
                    Attack();
                    break;
                case MonsterState.Death:
                    // ���� ���¿����� �ƹ��͵� ���� ����
                    break;
            }

            syncPosition = transform.position;
            syncRotation = transform.rotation;
        }
        else
        {
            if (Vector3.Distance(transform.position, syncPosition) > 0.1f || Quaternion.Angle(transform.rotation, syncRotation) > 1f)
            {
                transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpSpeed);
            }
        }
    }

    private void Patrol()
    {
        MoveTowardsTarget();

        // ���� �������� Ÿ�� Ž��
        findTargetTimer += Time.deltaTime;
        if (findTargetTimer >= findTargetInterval)
        {
            FindTarget();
            findTargetTimer = 0f;
        }
    }

    private void Chase()
    {
        if (targetCharacter == null || Vector3.Distance(transform.position, targetCharacter.transform.position) > detectRange)
        {
            ChangeState(MonsterState.Patrol, "IsChasing", false);
            return;
        }

        MoveTowards(targetCharacter.transform.position);

        if (Vector3.Distance(transform.position, targetCharacter.transform.position) <= attackRange)
        {
            ChangeState(MonsterState.Attack, "IsAttacking", true);
        }
    }

    private void Attack()
    {
        if (targetCharacter == null || Vector3.Distance(transform.position, targetCharacter.transform.position) > attackRange)
        {
            ChangeState(MonsterState.Chase, "IsAttacking", false);
            return;
        }

        // ���� �� ��ǥ�� ���� �ùٸ��� ȸ���ϵ��� ����
        Vector3 targetDirection = targetCharacter.transform.position - transform.position;
        targetDirection.y = 0; // y�� ȸ���� ���
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        attackTimer += Time.deltaTime;
        if (attackTimer >= stat.AttackCoolTime)
        {
            attackTimer = 0f;
            RequestPlayAnimation("Attack");
            // AttackAction ȣ���� �ִϸ��̼� �̺�Ʈ���� ����
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        if (!IsPositionInCanMoveArea(targetPosition) || IsPositionInCantMoveArea(targetPosition))
        {
            SetRandomTargetPosition();
            return;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        RotateTowards(direction);
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            RotateTowards(direction);
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
            Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime);
        }
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            SetRandomTargetPosition();
            yield return new WaitForSeconds(changeDirectionInterval);
        }
    }

    private void SetRandomTargetPosition()
    {
        Vector3 randomPosition;
        do
        {
            randomPosition = GetRandomPositionInCanMoveArea();
        }
        while (!IsPositionInCanMoveArea(randomPosition) || IsPositionInCantMoveArea(randomPosition));

        targetPosition = randomPosition;
    }

    private Vector3 GetRandomPositionInCanMoveArea()
    {
        if (CanMoveArea.Length == 0)
            return initialPosition;

        var area = CanMoveArea[Random.Range(0, CanMoveArea.Length)];
        var collider = area.GetComponent<Collider>();

        if (collider == null)
            return initialPosition;

        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;

        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
    }

    private bool IsPositionInCanMoveArea(Vector3 position)
    {
        foreach (var area in CanMoveArea)
        {
            if (area.GetComponent<Collider>().bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPositionInCantMoveArea(Vector3 position)
    {
        foreach (var area in CantMoveArea)
        {
            if (area.GetComponent<Collider>().bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    private void FindTarget()
    {
        Character nearestCharacter = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var player in FindObjectsOfType<Character>())
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < nearestDistance)
            {
                nearestCharacter = player;
                nearestDistance = distance;
            }
        }

        if (nearestCharacter != null && nearestDistance <= detectRange)
        {
            targetCharacter = nearestCharacter;
            ChangeState(MonsterState.Chase, "IsChasing", true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)state);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(stat.Health);
        }
        else
        {
            state = (MonsterState)(int)stream.ReceiveNext();
            syncPosition = (Vector3)stream.ReceiveNext();
            syncRotation = (Quaternion)stream.ReceiveNext();
            stat.Health = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void Damaged(float damage, int actorNumber)
    {
        if (state == MonsterState.Death || !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        stat.Health -= damage;

        if (stat.Health <= 0)
        {
            ChangeState(MonsterState.Death, "Die", true);
            StartCoroutine(DeathCoroutine());
        }
        else
        {
            RequestPlayAnimation("Hit");
        }
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }

    private void RequestPlayAnimation(string animationName)
    {
        GetComponent<PhotonView>().RPC(nameof(PlayAnimation), RpcTarget.All, animationName);
    }

    [PunRPC]
    private void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    private void ChangeState(MonsterState newState, string animationBool, bool value)
    {
        state = newState;
        animator.SetBool(animationBool, value);
    }

    // AttackAction �޼��� �߰�
    private void AttackAction()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // ���� ���� ���� ��� Ÿ�ٿ��� �������� ������ ����
        List<Character> targets = FindTargets(attackDamageRange);
        foreach (Character target in targets)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            int viewAngle = 200 / 2;
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle < viewAngle)
            {
                target.PhotonView.RPC("Damaged", RpcTarget.All, stat.Damage, -1);
            }
        }
    }

    private List<Character> FindTargets(float distance)
    {
        List<Character> characters = new List<Character>();
        Vector3 myPosition = transform.position;

        foreach (Character character in FindObjectsOfType<Character>())
        {
            if (character.State == State.Death)
            {
                continue;
            }

            if (Vector3.Distance(character.transform.position, myPosition) <= distance)
            {
                characters.Add(character);
            }
        }

        return characters;
    }
}
