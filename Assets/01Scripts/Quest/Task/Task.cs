using System.Collections;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using UnityEngine;

public enum TaskState
{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName ="Task_")]
public class Task : ScriptableObject
{
    /*
     * 대표적으로 UI 같은 곳에서 UI Update Code 를 Event에 연결해놓으면
     * Task의 상태를 Update에서 계속 추적할 필용벗이 
     * 상태가 바뀌면 알아서 UI가 Update 된다.
     */

    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion


    [SerializeField]
    private Category category;

    [Header("Text")]
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string description;

    [Header("Action")]
    [SerializeField]
    private TaskAction action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] targets;


    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue initialSuccessValue;
    [SerializeField]
    private int needSuccessToComplete;
    [SerializeField]
    private bool canReceiveReportDuringCompletion; // Item 100개 모아서 완료하는 Quest인데 유저가 아이템100개를 모았지만 퀘스트를 완료하기 전에 50개 버릴수도 있음
    // 이 떄 아이템 100개를 모았다고 더이상 100개를 모았다고 더이상 보고를 안받아버리면 아이템 버려도 Quest 완료 가능해짐
    // 이 기능(Option)은 Action중 Set과 조합해서 사용

    private TaskState state;
    private int currentSuccess;

    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    public int CurrentSuccess 
    {
        get => currentSuccess;
        set
        {
            int prevSuccess = currentSuccess;
            currentSuccess = Mathf.Clamp(value, 0, needSuccessToComplete);
            if(currentSuccess != prevSuccess)
            {
                State = currentSuccess == needSuccessToComplete ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, currentSuccess, prevSuccess);
            }
        }
    }
    public Category Category => category;
    public string CodeName => codeName;
    public string Description => description;
    public int NeedSuccessToComplete => needSuccessToComplete;
    public TaskState State
    {
        get => state;
        set
        {
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
        }
    }

    public bool IsComplete => State == TaskState.Complete;
    public Quest Owner { get; private set; }

    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Task가 시작될때 실행할 함수
    /// </summary>
    public void Start()
    {
        State = TaskState.Running;

        if (initialSuccessValue)
            currentSuccess = initialSuccessValue.GetValue(this);
    }


    /// <summary>
    /// Task가 끝날때 실행될 함수
    /// </summary>
    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;
    }

    /// <summary>
    /// Task를 즉시 완료하는 함수
    /// </summary>
    public void Complete()
    {
        CurrentSuccess = needSuccessToComplete;
    }


    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    /// <summary>
    /// TaskTarget을 통해 이 Task가 성공 횟수를 보고 받을 대상인지 확인하는 함수
    /// Setting 해놓은 Target들 중에 해당하는 Target이 있으면 true, 아니면 false 반환
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsTarget(string category, object target)
        => Category == category &&
        targets.Any(x => x.IsEqual(target)) &&
        (!IsComplete || (IsComplete && canReceiveReportDuringCompletion));

    public bool ContainsTarget(object target) => targets.Any(x => x.IsEqual(target));


}
