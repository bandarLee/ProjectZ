using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Lev : MonoBehaviourPun, IPunObservable, IDamaged
{
    public enum MonsterState
    {
        Patrol,
        Chase,
        Attack,
        Death
    }

    public Animator animator;
    public NavMeshAgent agent;
    public float detectRange = 30f;
    public float attackRange = 2f;
    public float attackDamageRange = 5f;
    public float patrolRadius = 20f;
    public Stat stat;

    public MonsterState state = MonsterState.Patrol;
    private Character targetCharacter;
    private Vector3 initialPosition;
    private float attackTimer = 0f;

    private Vector3 syncPosition;
    private Quaternion syncRotation;

    private float lerpSpeed = 6f;

    private float updateInterval = 0.2f; // 업데이트 간격 (초)
    private float nextUpdate;

    public string navMeshAreaName;

    private SphereCollider collisionAvoidanceCollider;

    private void Awake()
    {
        initialPosition = this.gameObject.transform.position;
    }

    public void StartMethod()
    {
        agent.speed = stat.MoveSpeed;
        agent.avoidancePriority = Random.Range(0, 100);
        syncPosition = transform.position;
        syncRotation = transform.rotation;

        if (!PhotonNetwork.IsMasterClient)
        {
            agent.enabled = false;
        }

        collisionAvoidanceCollider = gameObject.AddComponent<SphereCollider>();
        collisionAvoidanceCollider.isTrigger = true;
        collisionAvoidanceCollider.radius = 3.0f;
        SetNavMeshArea(navMeshAreaName);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(FindTargetRoutine());
        }

        nextUpdate = Time.time + Random.Range(0f, updateInterval); // 랜덤한 초기 딜레이 설정
    }

    private void OnEnable()
    {
        StartMethod();

        stat.Init();
        state = MonsterState.Patrol;
        this.gameObject.transform.position = initialPosition;

        targetCharacter = null;

    }

    private void OnDisable()
    {
        stat.Init();
    }

    private void SetNavMeshArea(string areaName)
    {
        int areaMask = 1 << NavMesh.GetAreaFromName(areaName);
        agent.areaMask = areaMask;
    }

    private IEnumerator FindTargetRoutine()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient && (state == MonsterState.Patrol || state == MonsterState.Chase))
            {
                FindTarget();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToRandomPosition();
        }

        FindTarget();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time >= nextUpdate)
            {
                nextUpdate = Time.time + updateInterval;
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
                        // 죽음 상태에서는 아무것도 하지 않음
                        break;
                }

                syncPosition = transform.position;
                syncRotation = transform.rotation;
            }
            CheckAndResetAgent();

        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpSpeed);
        }
    }
    private void CheckAndResetAgent()
    {
        if (agent.isStopped || agent.velocity.sqrMagnitude < 0.1f) // 에이전트가 멈춰있거나 매우 느리게 움직일 때
        {
            Debug.LogWarning("Agent stopped or stuck. Resetting path.");
            agent.ResetPath(); // 경로 재설정
            MoveToRandomPosition(); // 새로운 랜덤 위치로 이동
        }
    }

    private void Chase()
    {
        if (targetCharacter == null || Vector3.Distance(transform.position, targetCharacter.transform.position) > detectRange)
        {
            ChangeState(MonsterState.Patrol, "IsChasing", false);
            MoveToRandomPosition();
            return;
        }

        agent.SetDestination(targetCharacter.transform.position);

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

        agent.SetDestination(transform.position);

        Vector3 targetDirection = targetCharacter.transform.position - transform.position;
        targetDirection.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        attackTimer += Time.deltaTime;
        if (attackTimer >= stat.AttackCoolTime)
        {
            attackTimer = 0f;
            RequestPlayAnimation("Attack");
        }
    }

    public void AttackAction()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

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
            if (targetCharacter != nearestCharacter)
            {
                targetCharacter = nearestCharacter;
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("SetTarget", RpcTarget.Others, nearestCharacter.PhotonView.ViewID);
                }
            }
            ChangeState(MonsterState.Chase, "IsChasing", true);
        }
        else
        {
            if (targetCharacter != null)
            {
                targetCharacter = null;
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("SetTarget", RpcTarget.Others, -1);
                }
            }
            ChangeState(MonsterState.Patrol, "IsChasing", false);
        }
    }

    [PunRPC]
    private void SetTarget(int targetViewID)
    {
        if (targetViewID == -1)
        {
            targetCharacter = null;
        }
        else
        {
            PhotonView targetView = PhotonView.Find(targetViewID);
            if (targetView != null)
            {
                targetCharacter = targetView.GetComponent<Character>();
            }
        }
    }

    private bool IsTargetOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    private void MoveToRandomPosition()
    {
        Vector3 randomDirection;
        NavMeshHit hit;
        bool foundPosition = false;
        int attempts = 0;
        int maxAttempts = 10;

        while (!foundPosition && attempts < maxAttempts)
        {
            randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += initialPosition;

            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    foundPosition = true;
                    agent.SetDestination(hit.position);
                    syncPosition = hit.position;
                }
            }
            attempts++;
        }

        if (!foundPosition)
        {
            Debug.LogWarning("Failed to find valid NavMesh position after multiple attempts.");
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
            stat.Health = (float)stream.ReceiveNext();
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
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (other.CompareTag("Monster") && other.gameObject != this.gameObject)
        {
            Vector3 avoidDirection = transform.position - other.transform.position;
            Vector3 newPos = transform.position + avoidDirection.normalized * 10f;
            agent.SetDestination(newPos);
            syncPosition = newPos;
        }
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
}
