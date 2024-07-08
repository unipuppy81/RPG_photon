using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EnemyState;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField]
    private ParticleSystem attackedEffect;

    [Header("FSM")]
    public EnemyState _curState;
    private FSMEnemy _fsmEnemy;

    [Header("Photon")]
    public PhotonView enemyPhotonview;
    public int photonviewID;
    private PhotonView _lastAttacker;

    [Header("UI")]
    [SerializeField]
    private GameObject damageText;
    private TextMeshPro dText;
    private bool isFadingOut = false;
    private Camera cam;
    private Transform hp;

    [SerializeField]
    private Slider healthSlider;


    [Header("Enemy Stats")]
    public float curHealth; // 현재 체력
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
    private bool isAttacking = false;

    private Vector3 spawnPosition; // 초기 스폰 위치
    private bool isReturn;


    [Header("Quest")]
    public UnityEngine.Events.UnityEvent onDead;
    private bool goldRewardGiven = false; // 골드를 지급했는지 여부를 나타내는 변수


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _curState = EnemyState.Idle;
        _fsmEnemy = new FSMEnemy(new IdleState(this));

        dText = damageText.GetComponent<TextMeshPro>();

        cam = Camera.main;
        hp = healthSlider.gameObject.transform;

        isReturn = false;
        curHealth = maxHealth;
        UpdateHealthSlider();

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
    }

    void OnEnabled()
    {
        enemyPhotonview = GetComponent<PhotonView>();
        photonviewID = enemyPhotonview.ViewID;
    }


    private void UpdateHealthSlider()
    {
        healthSlider.value = curHealth / maxHealth;
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
        float attackRange = 1f;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        return distanceToTarget <= attackRange;
    }

    public void PerformAttack()
    {
        if (target == null) return;

        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, 1.25f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // 플레이어의 체력 감소
                Character_Warrior playerHealth = hit.GetComponent<Character_Warrior>();

                if (playerHealth != null)
                {
                    playerHealth._photonView.RPC("TakeDamage", RpcTarget.All, Atk);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    /// <summary>
    /// Idle 상태에 있으면 체력을 자동으로 회복
    /// </summary>
    public void RegenerateHealth()
    {
        if (Time.time >= lastHealthRegenTime + healthRegenInterval)
        {
            curHealth += healMount; // 체력 50 회복
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth); // 최대 체력을 넘지 않도록 제한
            lastHealthRegenTime = Time.time; // 마지막 체력 회복 시간 갱신
        }
    }


    [PunRPC]
    public void TakeDamage(float attack, int attackerID)
    {
        float damage = CombatCalculator.CalculateDamage(attack, Def);
        float ceilDamage = Mathf.Ceil(damage);
        curHealth -= ceilDamage;

        damageText.SetActive(true);


        dText.text = ceilDamage.ToString();

        _lastAttacker = PhotonView.Find(attackerID);

        Destroy(Instantiate(attackedEffect.gameObject, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z),
    Quaternion.FromToRotation(Vector3.up, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z))),
    attackedEffect.main.startLifetimeMultiplier);

        if (!isFadingOut)
        {
            StartCoroutine(FadeOutText());
        }

        StartCoroutine(SmoothHealthChange(ceilDamage));

        if (isDie())
        {
            ChangeState(EnemyState.Die);
        }
    }

    private IEnumerator FadeOutText()
    {
        isFadingOut = true;

        float duration = .8f;
        float elapsedTime = 0f;

        Vector3 originalPosition = dText.transform.position;
        Color originalColor = dText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            float offsetY = Mathf.Lerp(0f, .1f, elapsedTime / duration);

            dText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            dText.transform.position = originalPosition + new Vector3(0f, offsetY, 0f);

            yield return null;
        }

        // After fading out, reset the text position and hide it
        dText.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        dText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Reset damageText position and deactivate it
        damageText.transform.localPosition = new Vector3(0, 1.2f, 0);
        damageText.SetActive(false);

        isFadingOut = false;
    }

    private IEnumerator SmoothHealthChange(float damage)
    {
        float targetValue = curHealth / maxHealth;
        float startValue = healthSlider.value;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null;
        }

        healthSlider.value = targetValue;
    }

    public void Die()
    {
        _animator.SetTrigger("isDie");

        Destroy(healthSlider);

        Vector3 hitPoint = transform.position;
        hitPoint.y += 0.5f;

        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.up, hitPoint)), deathEffect.main.startLifetimeMultiplier);

        // 나를 죽인 플레이어에게 골드 지급
        if (_lastAttacker != null && !goldRewardGiven)
        {
            int goldReward = Random.Range(50, 200); // minGoldReward와 maxGoldReward 사이의 랜덤 값 생성
            Debug.Log(_lastAttacker.ViewID);
            photonView.RPC("RewardGold", _lastAttacker.Owner, goldReward);

            goldRewardGiven = true; // 골드를 지급했음을 표시
        }
    }

    [PunRPC]
    void RewardGold(int gold)
    {
        // 플레이어에게 골드를 지급하는 로직 (PlayerController 예시)
        Character_Warrior playerController = PhotonView.Find(_lastAttacker.ViewID).GetComponent<Character_Warrior>();
        playerController.AddGold(gold);
    }

    // 애니메이션 이벤트로 호출될 메서드
    public void DieComplete()
    {
        // 객체 파괴
        Destroy(gameObject);

        onDead.Invoke();

        // 적이 사망하는걸 마스터 클라이언트에서만 처리
        SpawnManager.instance.RemoveGhost();

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnManager.instance.CreateComputerPlayer();
        }
    }

    public bool isDie()
    {
        return curHealth <= 0;
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
