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
    public float attackRange = 3f;
    public float patrolRadius = 20f;
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

        // 초기 순찰 위치 설정
        MoveToRandomPosition();
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
                    // 죽음 상태에서는 아무것도 하지 않음
                    break;
            }

            syncPosition = transform.position;
            syncRotation = transform.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpSpeed);
        }
    }

    private void Patrol()
    {
        // 현재 위치와 목표 위치 사이의 거리가 매우 가까운 경우 새로운 목표 위치 설정
        if (Vector3.Distance(transform.position, syncPosition) < 1f)
        {
            MoveToRandomPosition();
        }

        // 타겟 탐색
        FindTarget();
    }

    private void Chase()
    {
        if (targetCharacter == null || Vector3.Distance(transform.position, targetCharacter.transform.position) > detectRange)
        {
            ChangeState(MonsterState.Patrol, "IsChasing", false);
            MoveToRandomPosition();
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

        Vector3 targetDirection = targetCharacter.transform.position - transform.position;
        targetDirection.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        attackTimer += Time.deltaTime;
        if (attackTimer >= stat.AttackCoolTime)
        {
            attackTimer = 0f;
            RequestPlayAnimation("Attack");
            // AttackAction 호출은 애니메이션 이벤트에서 실행
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void MoveToRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += initialPosition;
        randomDirection.y = initialPosition.y; // 고도는 유지

        syncPosition = randomDirection;
        MoveTowards(syncPosition);
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
    public void Damaged(int damage, int actorNumber)
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

    // AttackAction 메서드 추가
    private void AttackAction()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 공격 범위 내의 모든 타겟에게 데미지를 입히는 로직
        List<Character> targets = FindTargets(attackRange);
        foreach (Character target in targets)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            int viewAngle = 160 / 2;
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
