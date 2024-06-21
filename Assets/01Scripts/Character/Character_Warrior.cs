using UnityEngine;
using System.Collections;
using Photon.Pun;
using static UnityEditor.VersionControl.Asset;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

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

    private float maxHp = 100.0f;           //체력
    private float atk = 100.0f;          // 공격력
    private float def = 100.0f;           // 방어력
    private float walkSpeed = 4.0f;    // 걸을 때 속도
    private float runSpeed = 6.0f;     // 뛸 때 속도
    float currentSpeed;
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

    public float attackRadius = 2f;
    public LayerMask enemyLayer;

    /// <summary>
    /// FSM 변수
    /// </summary>
    [Header("FSM")]
    public CharacterState<Character_Warrior>[] states;
    public StateMachine<Character_Warrior> _StateMachine;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();

        _rigidbody.centerOfMass = new Vector3(0, -1.5f, 0); // 무게 중심점을 변경
    }


    void Update()
    {
        if (!_photonView.IsMine && PhotonNetwork.IsConnected)
            return;


        Move();

        // 스킬 사용 처리
        InputAttackBtn();

        // 현재 상태의 Execute 메서드를 호출
        if (_StateMachine != null)
            _StateMachine.Execute();

 
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedSetup());

        _photonView.RPC("Setup", RpcTarget.All);
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
        // 주변의 적 탐지
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);
    }

    public void Idle()
    {

    }

    public void Walk()
    {

    }

    public void Run()
    {

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
    public void FinishSkill()
    {
        isUsingSkill = false;

        _animator.SetBool("UseSkill", isUsingSkill);

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        // 스킬 사용 후 상태를 Idle로 변경
        ChangeState(State.Idle);
    }

    public void Die()
    {
        ChangeState(State.Die);
        Debug.Log("Player is Dying");
    }


    /// <summary>
    /// Gizmos를 사용해 공격 범위를 시각적으로 표시
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isSafe = false;
        }
    }

    /// <summary>
    /// 캐릭터가 서버에 할당된 후에 카메라 위치 잡아줌
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => _photonView.IsMine);

        Debug.Log("Camera is Mine");
        Camera.main.GetComponent<SmoothFollow>().target = _transform.Find("CameraPos").transform;
    }
}
