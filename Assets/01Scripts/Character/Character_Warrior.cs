using UnityEngine;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public enum Character_Type { Warrior = 0, Mage, Boxer }
public enum State { Idle = 0, Walk, Run, Warrior_Skill, Die }

public class Character_Warrior : MonoBehaviourPunCallbacks
{
    public Character_Type characterType;
    public State curStates;// 현재 상태

    [Header("Component")]
    public PhotonView _photonView;
    public Rigidbody _rigidbody;
    public Transform _transform;
    public Animator _animator;





    /// <summary>
    /// 내부 기능 구현에 필요한 변수
    /// </summary>
    [Header("Setting Value")]
    public float m_turnSpeed = 200;                                 // 화면 회전 속도

    private readonly float m_backwardRunScale = 0.66f;           // 뒷걸음질 속도배수

    public bool isRunning;
    public bool isSafe;


    /// <summary>
    /// 체력, 속도, 힘, 방어
    /// </summary>
    [Header("Character_Status")]

    private float maxHp;           //체력
    [SerializeField]
    private float atk;          // 공격력
    private float def;           // 방어력
    private float walkSpeed;    // 걸을 때 속도
    private float runSpeed;     // 뛸 때 속도

    [SerializeField]
    private float currentSpeed;
    
    public float MaxHp
    {
        set => maxHp = value;
        get => maxHp;
    }
    public float Atk
    {
        set => atk = value;
        get => atk;
    }
    public float Def
    {
        set => def = value;
        get => def;
    }
    public float WalkSpeed
    {
        set => walkSpeed = value;
        get => walkSpeed;
    }
    public float RunSpeed
    {
        set => runSpeed = value;
        get => runSpeed;
    }

    public float MovementSpeed;
    public float curHealth;


    /// <summary>
    /// 공격 변수
    /// </summary>
    [Header("Attack var")]
    public bool isNormalAttack = false;
    public bool isUsingSkill = false;
    private bool isAttacking = false;
    public float skillDuration = 3f;
    public float attackInterval = 0.5f;

    public float attackRadius = 2f;
    public LayerMask enemyLayer;

    [SerializeField]
    private ParticleSystem attackedEffect;

    [Header("Item")]
    public GameObject chest_a;
    public GameObject chest_b;
    public GameObject shoes_a;
    public GameObject shoes_b;

    /// <summary>
    /// FSM 변수
    /// </summary>
    [Header("FSM")]
    public CharacterState<Character_Warrior>[] states;
    public StateMachine<Character_Warrior> _StateMachine;


    /// <summary>
    /// 최적화 변수
    /// </summary>
    [Header("optimization")]
    private byte currentGroup;
    private float updateInterval = 1.0f; // 그룹 업데이트 간격 (초)
    private float nextUpdateTime;

    /// <summary>
    /// 대화 시스템
    /// </summary>
    [Header("Dialogue")]
    [SerializeField] Vector3 dirVec;
    [SerializeField] private float scanRadius = 3.0f;
    public GameObject scanObject;
    [SerializeField] private LayerMask objectLayerMask;

    public bool isCommunicate = false;
    public bool isPressBtnE = false;


    [Header("UI")]
    public GameObject _textMeshProUGUI;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private GameObject damageTextPrefab;

    [Header("Sound")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip walkClip;
    [SerializeField]
    private AudioClip runClip;

    private AudioClip currentClip;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();

        _rigidbody.centerOfMass = new Vector3(0, -1.5f, 0); // 무게 중심점을 변경

        if (photonView.IsMine)
        {
            // 초기 그룹 설정
            //UpdateInterestGroup();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    

    void Update()
    {
        if (!_photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        if (photonView.IsMine && Time.time >= nextUpdateTime)
        {
            // 특정 간격마다 그룹 업데이트
            //UpdateInterestGroup();
            nextUpdateTime = Time.time + updateInterval;
        }

        // 움직임, 스킬
        if (GameManager.isPlayGame && !GameManager.isChatting && !GameManager.isTradeChatting && !DialogueManager.Instance.isAction)
        {
            Move();
            InputAttackBtn();

            UsePotion();

            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log($"HP : {MaxHp}, Atk : {Atk}, Def : {Def}, WalkSpeed : {WalkSpeed}, RunSpeed : {RunSpeed}");
            }
        }

        dirVec = this.gameObject.transform.forward; // Quest
        // scanObj
        if (Input.GetKeyDown(KeyCode.E) && scanObject != null)
        {
            isPressBtnE = true;
            DialogueManager.Instance.Action(scanObject);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            isPressBtnE = false;
        }

        // 현재 상태의 Execute 메서드를 호출
        if (_StateMachine != null)
            _StateMachine.Execute();
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedSetup());

        TextMeshPro tmp = _textMeshProUGUI.GetComponent<TextMeshPro>();

        _photonView.RPC("SetStats", PhotonNetwork.LocalPlayer);
        _photonView.RPC("Setup", PhotonNetwork.LocalPlayer);

        if (_photonView.IsMine)
        {
            tmp.text = PhotonNetwork.NickName;
            curHealth = MaxHp;
            photonView.RPC("UpdateHealthSlider", RpcTarget.All, curHealth);

            EquipmentSetValue.Instance.setValue(_photonView.ViewID, PhotonNetwork.NickName, MaxHp, Atk, Def, walkSpeed, runSpeed);

            GameManager.Instance.playerName = PhotonNetwork.NickName;
            GameManager.Instance.LoadGold(PhotonNetwork.NickName);
            GameManager.Instance.player = this.gameObject;
            //GameManager.Instance.GameLoad();

            Vector3 newPos = GameManager.Instance.GameLoad2();
            if(newPos.x != 0)
            {
                gameObject.transform.position = newPos;
            }

            UIManager.Instance.CloseEquip();
        }
        else
        {
            // 다른 클라이언트의 화면에서는 소유자의 닉네임을 설정
            tmp.text = _photonView.Owner.NickName;
        }

        GameManager.isPlayGame = true;

    }

    private void FixedUpdate()
    {
        // 스캔 위치 설정 (현재 오브젝트의 위치)
        Vector3 scanPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        // 주변에 있는 "Object" 레이어의 콜라이더 탐지
        Collider[] hitColliders = Physics.OverlapSphere(scanPosition, scanRadius, objectLayerMask);

        // 탐지된 콜라이더가 있을 경우
        if (hitColliders.Length > 0)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Object"))
                {
                    scanObject = hitCollider.gameObject;
                    break;
                }
            }
        }
        else
        {
            scanObject = null;
        }

        // 디버그 용도로 스캔 영역을 그립니다.
        Debug.DrawRay(scanPosition, Vector3.up * scanRadius, Color.green);
    }




    [PunRPC]
    void ChangeSceneForLocalPlayer(string sceneName)
    {
        //PhotonNetwork.Disconnect();

        PhotonNetwork.LoadLevel(sceneName);
    }









    ///// FSM 패턴 관련 구현 /////
    [PunRPC]
    public void Setup() // 가질 수 있는 상태만큼 클래스 메모리 할당.
    {
        states = new CharacterState<Character_Warrior>[5];
        states[(int)State.Idle] = new CharacterOwnedStates.WarriorIdle();
        states[(int)State.Walk] = new CharacterOwnedStates.WarriorWalk();
        states[(int)State.Run] = new CharacterOwnedStates.WarriorRun();
        states[(int)State.Warrior_Skill] = new CharacterOwnedStates.WarriorAttack();
        states[(int)State.Die] = new CharacterOwnedStates.WarriorDie();


        // State Machine 설정
        _StateMachine = new StateMachine<Character_Warrior>();
        _StateMachine.SetupStateMachine(this, states[(int)State.Idle]);
    }

    
    public void ChangeState(State newState) // 패턴 전환 함수
    {
        curStates = newState;

        _StateMachine.ChangeState(this, states[(int)newState]);
    }

    ///// 캐릭터 관련 구현 //////

    /// <summary>
    ///  이동과 달리기, 애니메이션 구현
    /// </summary>
    public void Move()
    {
        float moveVertical = Input.GetAxis("Vertical"); //상하 이동 W키 : 0~1, S키: -1~0
        float moveHorizontal = Input.GetAxis("Horizontal"); //좌우 이동 D키: 0~1, A키: -1~0

        // 달리기 상태 체크
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;

            _animator.SetBool("isRun", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;

            _animator.SetBool("isRun", false);
        }

        //뒤로 걷는 속도 적용
        if (moveVertical < 0)
        {
            moveVertical *= m_backwardRunScale;
        }

        // 달리기와 걷기 속도 설정
        currentSpeed = isRunning ? RunSpeed : WalkSpeed;

        Vector3 movement = Vector3.forward * moveVertical * currentSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);

        float turn = moveHorizontal * m_turnSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0);

        MovementSpeed = new Vector3(moveVertical * currentSpeed, 0, moveHorizontal * currentSpeed).magnitude;
        _animator.SetFloat("Speed", MovementSpeed);
    }

    /// <summary>
    /// 스킬 구현
    /// </summary>
    public void InputAttackBtn()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isUsingSkill)
        {
            ChangeState(State.Warrior_Skill);
        }
    }

    public void Skill()
    {
        if(!isAttacking)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        float elapsed = 0f;

        while (elapsed < skillDuration)
        {
            // 주변의 적 탐지
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

            // 탐지된 적들에게 데미지 주기
            foreach (Collider enemy in hitEnemies)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                PhotonView photonView = enemy.GetComponent<PhotonView>();

                if (enemyScript != null)
                {
                    photonView.RPC("TakeDamage", RpcTarget.All, Atk, _photonView.ViewID);
                }
            }

            elapsed += attackInterval;
            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
    }

    public void FinishSkill()
    {
        isUsingSkill = false;

        _animator.SetBool("UseSkill", isUsingSkill);

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        // 스킬 사용 후 상태를 Idle로 변경
        ChangeState(State.Idle);
    }

    // 포션
    private void UsePotion()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ItemDataManager.Instance.UseConsumeItem("meat");

            curHealth += 30;
            if(curHealth > maxHp)
            {
                curHealth = maxHp;
            }

            // 모든 클라이언트에게 체력 업데이트 RPC 호출
            photonView.RPC("UpdateHealthSlider", RpcTarget.All, curHealth);
            //UpdateHealthSlider();
        }
    }


    /// <summary>
    /// 캐릭터 스탯 설정
    /// </summary>
    [PunRPC]
    public void SetStats() // 체력, 공격력, 방어, 이동속도 + 
    {
        for (int i = 0; i < StatsDBManager.instance.statsDB.Character.Count; ++i)
        {
            if (StatsDBManager.instance.statsDB.Character[i].Type == characterType.ToString())
            {
                MaxHp = StatsDBManager.instance.statsDB.Character[i].Maxhp;
                Atk = StatsDBManager.instance.statsDB.Character[i].Atk;
                Def = StatsDBManager.instance.statsDB.Character[i].Def;
                WalkSpeed = StatsDBManager.instance.statsDB.Character[i].Wspeed;
                RunSpeed = StatsDBManager.instance.statsDB.Character[i].Rspeed;

                return;
            }
        }
    }


    [PunRPC]
    public void AddGold(int amount)
    {
        GameManager.Instance.ChangeGold(PhotonNetwork.NickName, amount);
    }

    [PunRPC]
    public void StateUpdate(float atk, float def, float walkSpeed, float runSpeed)
    {
        Atk = atk;
        Def = def;
        WalkSpeed = walkSpeed;
        RunSpeed= runSpeed;

        Debug.Log("Atk : " + Atk);
    }

    public void UpdateLocalStats(float atk, float def, float walkSpeed, float runSpeed)
    {
        if (!_photonView.IsMine) return;

        Atk = atk;
        Def = def;
        WalkSpeed = walkSpeed;
        RunSpeed = runSpeed;

        // 다른 플레이어에게 업데이트 알림
        _photonView.RPC("StateUpdate", RpcTarget.Others, atk, def, walkSpeed, runSpeed);
    }
    /// <summary>
    /// 상태 구현
    /// </summary>

    public void Idle()
    {
        audioSource.Stop();
        currentClip = null;
    }

    public void Walk()
    {
        if(currentClip != walkClip)
        {
            currentClip = walkClip;
            PlaySound(currentClip);
        }
    }

    public void Run()
    {
        if(currentClip != runClip)
        {
            currentClip = runClip;
            PlaySound(currentClip);
        }

    }

    public void Attack()
    {
        isUsingSkill = true;

        if (curStates != State.Warrior_Skill)
        {
            ChangeState(State.Warrior_Skill);
        }

        // 스킬 애니메이션 재생
        _animator.SetBool("UseSkill", isUsingSkill);
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // 예를 들어, 스킬이 1초 동안 지속된다고 가정
        Invoke("FinishSkill", 2.5f);
    }

    public bool isDie()
    {
        return curHealth <= 0;
    }

    /// <summary>
    /// Gizmos를 사용해 공격 범위를 시각적으로 표시
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    /// <summary>
    /// 데미지 함수
    /// </summary>
    /// <param name="attack"></param>
    [PunRPC]
    public void TakeDamage(float attack)
    {
        float damage = CombatCalculator.CalculateDamage(attack, Def);
        float ceilDamage = Mathf.Ceil(damage);

        curHealth -= ceilDamage;


        // 데미지 텍스트
        Canvas canva = GetComponentInChildren<Canvas>();

        GameObject damageUI = Instantiate(damageTextPrefab) as GameObject;
        //GameObject damageUI = PhotonNetwork.Instantiate("DamageText", 
        //    new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z), Quaternion.identity);

        damageUI.transform.SetParent(canva.transform, false);

        DamageText dText = damageUI.GetComponent<DamageText>();
        dText.SetDamage(ceilDamage);


        Destroy(Instantiate(attackedEffect.gameObject, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), 
            Quaternion.FromToRotation(Vector3.up, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z))),
            attackedEffect.main.startLifetimeMultiplier);

        // 체력바
        StartCoroutine(SmoothHealthChange(ceilDamage));

        if (isDie())
        {
            ChangeState(State.Die);
        }
    }

    private IEnumerator SmoothHealthChange(float damage)
    {
        float targetValue = curHealth / maxHp;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isSafe = true;
            GameManager.isTown = true;
            GameManager.isBattle = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BattleZone"))
        {
            isSafe = false;
            GameManager.isTown = false;
            GameManager.isBattle = true;
        }
    }

    /// <summary>
    /// UI
    /// </summary>

    [PunRPC]
    public void UpdateHealthSlider(float newHealth)
    {
        curHealth = newHealth;
        healthSlider.value = curHealth / maxHp;
    }

    /// <summary>
    /// 멀리 떨어진 객체는 패킷을 받지 않습니다. -> 네트워크 트래픽 최적화
    /// </summary>
    void UpdateInterestGroup()
    {
        // 예시: x 위치에 따라 그룹 설정 (1-255 범위의 byte 사용)
        byte newGroup = (byte)Mathf.Clamp(Mathf.FloorToInt(transform.position.x / 10.0f) + 1, 1, 255);

        if (newGroup != currentGroup)
        {
            //Debug.Log($"Changing group from {currentGroup} to {newGroup}");
            // 현재 그룹만 구독하도록 설정
            if (currentGroup > 0)
            {
                PhotonNetwork.SetInterestGroups(currentGroup, false);
            }
            PhotonNetwork.SetInterestGroups(newGroup, true);
            currentGroup = newGroup;
            photonView.Group = currentGroup;
        }
    }

    /// <summary>
    /// Sound
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();

        SoundManager.Instance.playerSource = audioSource;
    }

    /// <summary>
    /// 캐릭터가 서버에 할당된 후에 카메라 위치 잡아줌
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => _photonView.IsMine);

        Camera.main.GetComponent<SmoothFollow>().target = _transform.Find("CameraPos").transform;
    }
}
