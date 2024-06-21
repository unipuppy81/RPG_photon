using UnityEngine;
using Photon.Pun;
using CharacterOwnedStates;

public class StateMachine<T> where T : Character_Warrior // class
{
    public T ownerEntity; // stateMachine의 소유주
    private CharacterState<T> currentState; // 현재 상태를 담은 프로퍼티
    private CharacterState<T> previousState; // 이전 상태를 담은 프로퍼티


    // FSM에서 사용할 변수들
    public float m_turnSpeed = 200;
    public float currentSpeed { get; private set; }
    //기본 상태를 생성시에 설정하게 생성자 선언
    [PunRPC]
    public void StateMachineSetup(int ownerPhotonViewId, int stateId)
    {
        PhotonView targetView = PhotonView.Find(ownerPhotonViewId);
        if (targetView == null)
        {
            Debug.LogError("PhotonView not found");
            return;
        }

        T owner = targetView.GetComponent<T>();
        if (owner == null)
        {
            Debug.LogError("Component of type T not found on the target view");
            return;
        }

        // WarriorStates 열거형으로 상태 변환
        State entryState = (State)stateId;

        // 구체적인 상태 클래스 생성
        CharacterState<T> state = CreateState(entryState);

        SetupStateMachine(owner, state);
    }

    private CharacterState<T> CreateState(State state)
    {
        switch (state)
        {
            case State.Idle:
                return new WarriorIdle() as CharacterState<T>;
            case State.Walk:
                return new WarriorWalk() as CharacterState<T>;
            case State.Run:
                return new WarriorRun() as CharacterState<T>;
            case State.Warrior_Skill:
                return new WarriorAttack() as CharacterState<T>;
            case State.Die:
                return new WarriorDie() as CharacterState<T>;

            // 다른 상태들 추가
            default:
                return null;
        }
    }

    public void SetupStateMachine(T owner, CharacterState<T> entryState)
    {
        ownerEntity = owner;
        currentState = null;
        previousState = null;

        // entryState 상태로 상태 변경
        ChangeState(owner, entryState);
    }

    public void Execute()
    {
        if (currentState != null)
        {
            currentState.Execute(ownerEntity);
        }
    }
    public void ChangeState(T state, CharacterState<T> newState)
    {
        ownerEntity = state;

        // 새로 바꾸려는 상태가 있으면 상태를 바꾸지 않는다
        if (newState == null) return;



        // 현재 재생중인 상태가 있으면 Exit() 메서드 호출
        if (currentState != null)
        {
            // 상태가 변경되면 현재 상태는 이전 상태가 되기 때문에 previousState에 저장
            previousState = currentState;
            currentState.Exit(ownerEntity);
        }

        // 새로운 상태로 변경하고 새로 바뀐 상태의 enter 메서드 호출
        currentState = newState;
        currentState.Enter(ownerEntity);

    }

    public void RevertToPreviousState()
    {
        //ChangeState(previousState);
    }
}
