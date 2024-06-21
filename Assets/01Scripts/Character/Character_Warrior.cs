using UnityEngine;
using System.Collections;
using Photon.Pun;
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



    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();

        _rigidbody.centerOfMass = new Vector3(0, -1.5f, 0); // ���� �߽����� ����
    }


    void Update()
    {
        if (!_photonView.IsMine && PhotonNetwork.IsConnected)
            return;


        Move();
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedSetup());
    }


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


    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => _photonView.IsMine);

        Debug.Log("Camera is Mine");
        Camera.main.GetComponent<SmoothFollow>().target = _transform.Find("CameraPos").transform;
    }
}
