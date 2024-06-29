using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Monster_Final : MonoBehaviourPun, IPunObservable, IDamaged
{
    public enum MonsterState
    {
        Rush,
        Chase,
        Attack,
        Death
    }

    public Animator animator;
    public float detectRange = 15f;
    public float attackRange = 5f;
    public float attackDamageRange = 3f;
    public float moveSpeed = 5f;
    public Stat stat;

    public MonsterState state = MonsterState.Rush;
    private Character targetCharacter;
    private Vector3 initialPosition;
    private float attackTimer = 0f;

    private Vector3 syncPosition;
    private Quaternion syncRotation;

    private float lerpSpeed = 4f;

    private Rigidbody rb;

    private Vector3 targetPosition;
    public float changeDirectionInterval = 2f; // 방향을 변경하는 간격
    public bool IsMonsterTrigger = false;

    public GameObject RushTargetObject;
    private float findTargetInterval = 0.2f; // 타겟 탐색 간격

    private void Awake()
    {
        initialPosition = this.gameObject.transform.position;
    }
    public void StartMethod()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        syncPosition = transform.position;
        syncRotation = transform.rotation;

        if (!PhotonNetwork.IsMasterClient)
        {
            rb.isKinematic = true;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(FindTargetRoutine());

        }
    }

    private IEnumerator FindTargetRoutine()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient && (state == MonsterState.Rush || state == MonsterState.Chase))
            {
                FindTarget();
            }
            yield return new WaitForSeconds(findTargetInterval);
        }
    }

    private void OnEnable()
    {
        stat.Init();
        state = MonsterState.Rush;
        this.gameObject.transform.position = initialPosition;
        targetCharacter = null;
        StartMethod();

    }

    private void OnDisable()
    {
        stat.Init();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            switch (state)
            {
                case MonsterState.Rush:
                    Rush();
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
            SmoothSyncTransform();
        }
    }

    private void SmoothSyncTransform()
    {
        if (Vector3.Distance(transform.position, syncPosition) > 0.1f || Quaternion.Angle(transform.rotation, syncRotation) > 1f)
        {
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpSpeed);
        }
    }

    private void Rush()
    {
        if (targetCharacter == null)
        {
            MoveTowards(RushTargetObject.transform.position);
        }
    }

    private void Chase()
    {
        if (targetCharacter == null || Vector3.Distance(transform.position, targetCharacter.transform.position) > detectRange)
        {
            ChangeState(MonsterState.Rush, "IsChasing", false);
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

        RotateTowardsTarget(targetCharacter.transform.position);

        attackTimer += Time.deltaTime;
        if (attackTimer >= stat.AttackCoolTime)
        {
            attackTimer = 0f;
            RequestPlayAnimation("Attack");
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {      

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        RotateTowards(direction);
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
            Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * lerpSpeed);
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 targetDirection = targetPosition - transform.position;
        targetDirection.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lerpSpeed);
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
            ChangeState(MonsterState.Rush, "IsChasing", false);
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

    // AttackAction 메서드 추가
    private void AttackAction()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 공격 범위 내의 모든 타겟에게 데미지를 입히는 로직
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
