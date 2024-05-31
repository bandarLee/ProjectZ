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
    public float attackRange = 3f;
    public float patrolRadius = 20f;
    public Stat stat;

    public MonsterState state = MonsterState.Patrol;
    private Character targetCharacter;
    private Vector3 initialPosition;

    private void Start()
    {
        agent.speed = stat.MoveSpeed;
        agent.avoidancePriority = Random.Range(0, 100);
        initialPosition = transform.position;

        SphereCollider collisionAvoidanceCollider = gameObject.AddComponent<SphereCollider>();
        collisionAvoidanceCollider.isTrigger = true;
        collisionAvoidanceCollider.radius = 3.0f;

        MoveToRandomPosition(); // 초기 위치 설정
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

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
                break;
        }
    }

    private void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToRandomPosition();
        }

        FindTarget();
    }

    private void Chase()
    {
        if (targetCharacter == null || !IsTargetOnNavMesh(targetCharacter.transform.position) || Vector3.Distance(transform.position, targetCharacter.transform.position) > detectRange)
        {
            state = MonsterState.Patrol;
            animator.SetBool("IsChasing", false);
            MoveToRandomPosition(); // 플레이어를 놓쳤을 때 다시 무작위로 이동
            return;
        }

        agent.SetDestination(targetCharacter.transform.position);

        if (Vector3.Distance(transform.position, targetCharacter.transform.position) <= attackRange)
        {
            state = MonsterState.Attack;
            animator.SetBool("IsAttacking", true);
        }
    }

    private void Attack()
    {
        if (targetCharacter == null || Vector3.Distance(transform.position, targetCharacter.transform.position) > attackRange)
        {
            state = MonsterState.Chase;
            animator.SetBool("IsAttacking", false);
            return;
        }

        agent.SetDestination(transform.position);

        Vector3 targetDirection = targetCharacter.transform.position - transform.position;
        targetDirection.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void FindTarget()
    {
        Character nearestCharacter = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var player in FindObjectsOfType<Character>())
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < nearestDistance && IsTargetOnNavMesh(player.transform.position))
            {
                nearestCharacter = player;
                nearestDistance = distance;
            }
        }

        if (nearestCharacter != null && nearestDistance <= detectRange)
        {
            targetCharacter = nearestCharacter;
            state = MonsterState.Chase;
            animator.SetBool("IsChasing", true);
        }
    }

    private bool IsTargetOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    private void MoveToRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += initialPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas);

        agent.SetDestination(hit.position);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)state);
            stream.SendNext(transform.position);
            stream.SendNext(stat.Health);
        }
        else
        {
            state = (MonsterState)(int)stream.ReceiveNext();
            transform.position = (Vector3)stream.ReceiveNext();
            stat.Health = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        if (state == MonsterState.Death || !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        stat.Health -= damage;

        if (stat.Health <= 0)
        {
            state = MonsterState.Death;
            animator.SetTrigger("Die");
            StartCoroutine(DeathCoroutine());
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && other.gameObject != this.gameObject)
        {
            Vector3 avoidDirection = transform.position - other.transform.position;
            Vector3 newPos = transform.position + avoidDirection.normalized * 10f;
            agent.SetDestination(newPos);
        }
    }
}
