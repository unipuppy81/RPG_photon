using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

using Debug = UnityEngine.Debug;

public enum QuestState
{
    Inactive,
    Running,
    Complete,   // 퀘스트 자동 완료
    Cancel,
    WaitingForcompletion // 퀘스트 완료시 User가 퀘스트 완료를 눌러야하는 퀘스트
}

[CreateAssetMenu(menuName = "Quest/Quest", fileName = "Quest_")]
public class Quest : ScriptableObject
{
    #region Event
    // 보고받았을떄 실행할 event
    public delegate void TaskSuccessChangedHandler(Quest quest, Task task, int currentSuccess, int prevSuccess);
    
    // Quest 완료했을때 실행할 event
    public delegate void CompletedHandler(Quest quest);
    
    // Quest 취소했을때 실행할 event
    public delegate void CancelHandler(Quest quest);
    
    // 새로운 TaskGroup이 시작될때 실행할 event
    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup); 
    #endregion

    [SerializeField]
    private Category category;
    [SerializeField]
    private Sprite icon;


    [Header("Text")]
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string displayName;
    [SerializeField, TextArea]
    private string description;

    [Header("Task")]
    [SerializeField]
    private TaskGroup[] taskGroups;

    [Header("Reward")]
    [SerializeField]
    private Reward[] rewards;

    [Header("Option")]
    [SerializeField]
    private bool useAutoComplete; // 설정하면 
    [SerializeField]
    private bool isCancelable; // 취소 가능한가
    [SerializeField]
    private bool isSavable;

    private int currentTaskGroupIndex;  // 현재 TaskGruop이 몇번깨인지 확인하는 index
    
    [Header("Condition")]
    [SerializeField]
    private Condition[] acceptionConditions;
    [SerializeField]
    private Condition[] cancelConditons;

    public Category Category => category;
    public Sprite Icon => icon;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    public string Description => description;
    public QuestState State {  get; private set; }
    // 슬라임을 10마리 잡아라
    // 
    // 레드 슬라임을 10마리 잡아라
    // 블루 슬라임을 10마리 잡아라 -> 슬라임을 10마리 잡아라 
    // 즉 단순히 task 변수 하나 추가하는게 아니라 task들의 집합인 task group 만들고 
    // 그 taskgroup을 배열로 추가


    /// <summary>
    /// 현재 taskgruop을 바로 가져올수있게하는 Property
    /// </summary>
    public TaskGroup CurrentTaskGroup
    {
        get
        {
            if (currentTaskGroupIndex < 0 || currentTaskGroupIndex >= taskGroups.Length)
            {
                throw new IndexOutOfRangeException("currentTaskGroupIndex is out of bounds");
            }
            return taskGroups[currentTaskGroupIndex];
        }
    }

    //public TaskGroup CurrentTaskGroup => taskGroups[currentTaskGroupIndex];
    public IReadOnlyList<TaskGroup> TaskGroups => taskGroups;
    public IReadOnlyList<Reward> Rewards => rewards;
    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsCompletable => State == QuestState.WaitingForcompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCancelable => isCancelable && cancelConditons.All(x => x.IsPass(this));
    public bool IsAcceptable => acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => isSavable;

    public event TaskSuccessChangedHandler onTaskSuccessChanged;
    public event CompletedHandler onCompleted;
    public event CancelHandler onCanceled;
    public event NewTaskGroupHandler onNewTaskGroup;

    /// <summary>
    /// Awake 역할의 함수 Quest가 System에 등록되었을때 실행
    /// </summary>
    public void OnRegister()
    {
        Debug.Assert(!IsRegistered, "This quest has already been registerd.");


        /*
         * Quest에서 만든 event를 실행하는 callbacks 함수를 task에 등록하므로서
         * 외부에서 task에 일일이 event를 등록할 필요 없이 
         * quest의 event에 등록해주면 task의 성공 횟수가 변했다는 것을 알 수 있음
         */
        foreach(var taskGroup in taskGroups)
        {
            taskGroup.Setup(this);
            foreach(var task in taskGroup.Tasks)
            {
                task.onSuccessChanged += OnSuccessChanged;
            }

            State = QuestState.Running;
            CurrentTaskGroup.Start();
        }
    }

    /// <summary>
    /// 보고를 받는 함수
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>

    public void ReceiveReport(string category, object target, int successCount)
    {
        Debug.Assert(IsRegistered, "This quest has already been registerd.");
        Debug.Assert(!IsCancel, "This quest has been canceled.");

        if (IsComplete)
            return;

        CurrentTaskGroup.ReceiveReport(category, target, successCount);

        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            if (currentTaskGroupIndex + 1 == taskGroups.Length)
            {
                State = QuestState.WaitingForcompletion;
                if (useAutoComplete)
                {
                    Complete();
                }
                else
                {
                    var prevTaskGroup = taskGroups[currentTaskGroupIndex++];
                    prevTaskGroup.End();

                    CurrentTaskGroup.Start();
                    onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
                }
            }
        }
        else
        {        //Task option중에 완료 되어도 계속해서 보고 받는 옵션
            State = QuestState.Running;
        }

    }
    
    /*
    public void ReceiveReport(string category, object target, int successCount)
    {
        Debug.Assert(IsRegistered, "This quest has not been registered.");
        Debug.Assert(!IsCancel, "This quest has been canceled.");

        if (IsComplete)
            return;

        CurrentTaskGroup.ReceiveReport(category, target, successCount);

        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            var prevTaskGroup = taskGroups[currentTaskGroupIndex];

            if (currentTaskGroupIndex + 1 < taskGroups.Length)
            {
                currentTaskGroupIndex++;
                CurrentTaskGroup.Start();
                onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
            else
            {
                State = QuestState.WaitingForcompletion;
                if (useAutoComplete)
                {
                    Complete();
                }
            }
        }
        else // Task option 중에 완료되어도 계속해서 보고받는 옵션
        {
            State = QuestState.Running;
        }
    }
    */
    /// <summary>
    /// Quest를 완료하는 함수
    /// </summary>
    public void Complete()
    {
        CheckIsRunning();

        foreach (var taskGroup in taskGroups)
        {
            taskGroup.Complete();
        }

        State = QuestState.Complete;

        // 퀘스트의 보상
        foreach(var reward in rewards)
        {
            reward.Give(this);
        }

        onCompleted?.Invoke(this);

        onTaskSuccessChanged = null;
        onCompleted = null;
        onCanceled = null;
        onNewTaskGroup = null;
    }

    /// <summary>
    /// 퀘스트를 캔슬하는 함수
    /// </summary>
    public virtual void Cancel()
    {
        CheckIsRunning();
        Debug.Assert(IsCancelable, "This quest can't be canceled");
            
        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }

    public bool ContainsTarget(object target) => taskGroups.Any(x => x.ContainsTarget(target));
    public bool ContainsTarget(TaskTarget target) => ContainsTarget(target.Value);

    /// <summary>
    /// 지금은 복사본을 만들떄 task만 복사해서 넣어주지만
    /// 만약 quest의 다른 module 중에서 task 처럼
    /// 실시간으로 변수값이 바뀌는 경우가 있다면 
    /// 그 module 들도 task처럼 복사본을 만들어서 넣어줘야함 
    /// </summary>
    /// <returns></returns>
    public Quest Clone()
    {
        var clone = Instantiate(this);
        clone.taskGroups = taskGroups.Select(x => new TaskGroup(x)).ToArray();
        
        return clone;
    }
    
    public QuestSaveData ToSaveData()
    {
        return new QuestSaveData
        {
            codeName = codeName,
            state = State,
            taskGroupIndex = currentTaskGroupIndex,
            taskSuccessCounts = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray()
        };
    }
    public void LoadFrom(QuestSaveData saveData)
    {
        State = saveData.state;
        currentTaskGroupIndex = saveData.taskGroupIndex;

        for (int i = 0; i < currentTaskGroupIndex; i++)
        {
            var taskGroup = taskGroups[i];
            taskGroup.Start();
            taskGroup.Complete();
        }

        if (currentTaskGroupIndex < taskGroups.Length)
        {
            CurrentTaskGroup.Start();
            for (int i = 0; i < saveData.taskSuccessCounts.Length; i++)
            {
                CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.taskSuccessCounts[i];
            }
        }
        else
        {
            throw new IndexOutOfRangeException("currentTaskGroupIndex is out of bounds during LoadFrom");
        }
    }
    /*
    public void LoadFrom(QuestSaveData saveData)
    {
        State = saveData.state;
        currentTaskGroupIndex = saveData.taskGroupIndex;

        for(int i =0; i < currentTaskGroupIndex; i++)
        {
            var taskGroup = taskGroups[i];
            taskGroup.Start();
            taskGroup.Complete();
        }

        for(int i =0;i < saveData.taskSuccessCounts.Length; i++)
        {
            CurrentTaskGroup.Start();
            CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.taskSuccessCounts[i];
        }
    }
    */
    private void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess)
        => onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);


    [Conditional("UNITY_EDITOR")]
    private void CheckIsRunning()
    {
        Debug.Assert(IsRegistered, "This quest has already been registerd.");
        Debug.Assert(!IsCancel, "This quest has been canceled.");
        Debug.Assert(!IsComplete, "This quest has already been completed.");
    }
}
