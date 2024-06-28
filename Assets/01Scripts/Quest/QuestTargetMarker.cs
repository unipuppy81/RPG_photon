using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTargetMarker : MonoBehaviour
{
    [SerializeField]
    private TaskTarget target; // 해당 target을 가진 task 찾아와서 감시
    [SerializeField]
    private MarkerMaterialData[] markerMaterialDatas;

    private Dictionary<Quest, Task> targetTasksByQuest = new Dictionary<Quest, Task>();
    private Transform cameraTransform; // marker가 항상 플레이어를 바라보게 해야함
    private Renderer renderer; // task의 카테고리에 따라 이미지를 다르게 보여주기 위함

    private int currentRunningTargetTaskCount; // 진행중인 task의 count세는 변수

    [System.Serializable]
    private struct MarkerMaterialData
    {
        public Category category;
        public Material markerMaterial;
    }

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        gameObject.SetActive(false);

        QuestSystem.Instance.onQuestRegistered += TryAddTargetQuest;
        foreach (var quest in QuestSystem.Instance.ActiveQuests)
            TryAddTargetQuest(quest);
    }

    private void Update()
    {
        var rotation = Quaternion.LookRotation((cameraTransform.position - transform.position).normalized);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y + 180f, 0f);
    }

    private void OnDestroy()
    {
        QuestSystem.Instance.onQuestRegistered -= TryAddTargetQuest;
        foreach ((Quest quest, Task task) in targetTasksByQuest)
        {
            quest.onNewTaskGroup -= UpdateTargetTask;
            quest.onCompleted -= RemoveTargetQuest;
            task.onStateChanged -= UpdateRunningTargetTaskCount;
        }
    }

    /// <summary>
    /// 등록된 Quest를 확인하여 Target인 경우 감시하는 역할
    /// </summary>
    /// <param name="quest"></param>
    private void TryAddTargetQuest(Quest quest)
    {
        if (target != null && quest.ContainsTarget(target))
        {
            quest.onNewTaskGroup += UpdateTargetTask;
            quest.onCompleted += RemoveTargetQuest;

            UpdateTargetTask(quest, quest.CurrentTaskGroup);
        }
    }

    /// <summary>
    /// 감시중인 Task 교체
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="currentTaskGroup"></param>
    /// <param name="prevTaskGroup"></param>
    private void UpdateTargetTask(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        targetTasksByQuest.Remove(quest);

        var task = currentTaskGroup.FindTaskByTarget(target);
        if (task != null)
        {
            targetTasksByQuest[quest] = task;
            task.onStateChanged += UpdateRunningTargetTaskCount;

            UpdateRunningTargetTaskCount(task, task.State);
        }
    }

    /// <summary>
    /// Target에서 지워주는 역할
    /// </summary>
    /// <param name="quest"></param>
    private void RemoveTargetQuest(Quest quest) => targetTasksByQuest.Remove(quest);


    /// <summary>
    /// Task의 상태에 따라 Count를 조절하고 
    /// count가 0이면 Marker를 끄고 0이상이면 Marker를 켜준다
    /// </summary>
    /// <param name="task"></param>
    /// <param name="currentState"></param>
    /// <param name="prevState"></param>
    private void UpdateRunningTargetTaskCount(Task task, TaskState currentState, TaskState prevState = TaskState.Inactive)
    {
        if (currentState == TaskState.Running)
        {
            renderer.material = markerMaterialDatas.First(x => x.category == task.Category).markerMaterial;
            currentRunningTargetTaskCount++;
        }
        else
            currentRunningTargetTaskCount--;

        gameObject.SetActive(currentRunningTargetTaskCount != 0);
    }
}
