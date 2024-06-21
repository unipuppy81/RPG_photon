using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EnemyState;



public class Enemy : Monster
{
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Die
    }

    [Header("Animator")]
    public Animator _animator;
    public ParticleSystem deathEffect;

    [Header("FSM")]
    public EnemyState _curState;
    private FSMEnemy _fsmEnemy;

    [Header("Photon")]
    public PhotonView enemyPhotonview;
    public int photonviewID;

    [Header("Enemy Stats")]
    public float curhealth; // 현재 체력
    public float maxHealth; // 최대 체력

    private float healthRegenInterval = 1f; // 체력 회복 간격 (1초)
    private float lastHealthRegenTime = 1000; // 마지막 체력 회복 시간

    public float Atk;
    public float Def;

    public float moveSpeed;
    public float healMount;

    public float detectionRange = 5f;
    public float chaseRange = 7f;
    public LayerMask targetLayer;
    public Transform target;

    private Vector3 spawnPosition; // 초기 스폰 위치
    private bool isReturn;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _curState = EnemyState.Idle;
        _fsmEnemy = new FSMEnemy(new IdleState(this));



        isReturn = false;

        spawnPosition = transform.position; // 스폰 위치 저장
    }

    private void Update()
    {
        switch (_curState)
        {
            case EnemyState.Idle:
                if (DetectTargets())
                {
                    if (AttackPlayer())
                        ChangeState(EnemyState.Attack);
                    else
                        ChangeState(EnemyState.Move);
                }
                break;

            case EnemyState.Move:
                if (DetectTargets())
                {
                    if (AttackPlayer())
                        ChangeState(EnemyState.Attack);
                }
                else
                    ChangeState(EnemyState.Idle);

                break;

            case EnemyState.Attack:
                if (DetectTargets())
                {
                    if (!AttackPlayer())
                    {
                        ChangeState(EnemyState.Move);
                    }
                }
                else
                    ChangeState(EnemyState.Idle);

                break;

            case EnemyState.Return:
                ReturnToSpawn();

                if (spawnPosition.x == transform.position.x && spawnPosition.z == transform.position.z && Mathf.Approximately(spawnPosition.z, transform.position.z))
                    ChangeState(EnemyState.Idle);

                break;

            case EnemyState.Die:
                if (isDie())
                    ChangeState(EnemyState.Die);

                break;
        }

        _fsmEnemy.UpdateState();
    }

    private void OnEnable()
    {
        SetStatsEnemy();
        //photonView.RPC("SetStatsEnemy", RpcTarget.All);
    }
    void OnEnabled()
    {
        enemyPhotonview = GetComponent<PhotonView>();
        photonviewID = enemyPhotonview.ViewID;
    }


    private void ChangeState(EnemyState nextState)
    {
        _curState = nextState;

        switch (_curState)
        {
            case EnemyState.Idle:
                _fsmEnemy.ChangeState(new IdleState(this));
                break;

            case EnemyState.Move:
                _fsmEnemy.ChangeState(new MoveState(this));
                break;

            case EnemyState.Attack:
                _fsmEnemy.ChangeState(new AttackState(this));
                break;

            case EnemyState.Return:
                _fsmEnemy.ChangeState(new ReturnState(this));
                break;

            case EnemyState.Die:
                _fsmEnemy.ChangeState(new DieState(this));
                break;
        }
    }

    [PunRPC]
    public void SetStatsEnemy() // 체력, 공격력, 방어, 이동속도 + 
    {
        for (int i = 0; i < StatsDBManager.instance.statsDB.Enemy.Count; ++i)
        {
            if (StatsDBManager.instance.statsDB.Enemy[i].Name == "Ghost")
            {
                maxHealth = StatsDBManager.instance.statsDB.Enemy[i].Maxhp;
                Atk = StatsDBManager.instance.statsDB.Enemy[i].Atk;
                Def = StatsDBManager.instance.statsDB.Enemy[i].Def;
                moveSpeed = StatsDBManager.instance.statsDB.Enemy[i].Speed;
                healMount = StatsDBManager.instance.statsDB.Enemy[i].Healamount;
                detectionRange = StatsDBManager.instance.statsDB.Enemy[i].Radius;
                return;
            }
        }
    }


    /// <summary>
    /// 플레이어 감지
    /// </summary>
    /// <returns></returns>
    public bool DetectTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);
        if (hits.Length > 0)
        {
            // 감지된 첫 번째 적을 타겟으로 설정
            target = hits[0].transform;
            return true;
        }
        target = null;
        return false;
    }

    /// <summary>
    /// 플레이어 쪽으로 이동
    /// </summary>
    public void MoveTowardsPlayer()
    {
        Character_Warrior cw = target.GetComponent<Character_Warrior>();

        if (cw.isSafe)
        {
            ChangeState(EnemyState.Return);
            target = null;
        }


        if (target == null) return;
        
        // 플레이어를 향해 이동하는 로직
        Vector3 direction = (target.position - transform.position).normalized;

        // 회전 로직
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 회전 속도 조절 (5f는 임의의 값)

        transform.position += direction * Time.deltaTime * moveSpeed;
    }

    /// <summary>
    /// 플레이어 공격
    /// </summary>
    /// <returns></returns>
    public bool AttackPlayer()
    {
        if (target == null) return false;

        // 플레이어를 공격할 수 있는 조건 (예: 사정거리 안에 있는지)
        float attackRange = 2f; // 예시로 공격 사정거리를 2로 설정
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        return distanceToTarget <= attackRange;
    }

    public void PerformAttack()
    {
        if (target == null) return;

        // 플레이어를 공격하는 로직
    }

    /// <summary>
    /// Idle 상태에 있으면 체력을 자동으로 회복
    /// </summary>
    public void RegenerateHealth()
    {
        if (Time.time >= lastHealthRegenTime + healthRegenInterval)
        {
            curhealth += healMount; // 체력 50 회복
            curhealth = Mathf.Clamp(curhealth, 0, maxHealth); // 최대 체력을 넘지 않도록 제한
            lastHealthRegenTime = Time.time; // 마지막 체력 회복 시간 갱신
        }
    }


    [PunRPC]
    public void TakeDamage(float attack)
    {
        float damage = CombatCalculator.CalculateDamage(attack, Def);
        curhealth -= damage;
        if (isDie())
        {
            ChangeState(EnemyState.Die);
        }
    }

    public void Die()
    {
        _animator.SetTrigger("isDie");

        Vector3 hitPoint = transform.position;
        hitPoint.y += 0.5f;

        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.up, hitPoint)), deathEffect.main.startLifetimeMultiplier);

    }

    // 애니메이션 이벤트로 호출될 메서드
    public void DieComplete()
    {
        // 객체 파괴
        Destroy(gameObject);

        // 적이 사망하는걸 마스터 클라이언트에서만 처리
        SpawnManager.instance.RemoveGhost();

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnManager.instance.CreateComputerPlayer();
        }
    }

    public bool isDie()
    {
        return curhealth <= 0;
    }

    public void ReturnToSpawn()
    {
        isReturn = true;
        Vector3 direction = (new Vector3(spawnPosition.x, transform.position.y, spawnPosition.z) - transform.position).normalized;
        transform.position += direction * Time.deltaTime * moveSpeed;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // y축 회전을 무시하고 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IdlePoint") && isReturn)
        {
            ChangeState(EnemyState.Idle);
            isReturn = false;
        }
    }


}
