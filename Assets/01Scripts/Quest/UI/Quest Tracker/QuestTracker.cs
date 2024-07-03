using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questTitleText;
    [SerializeField]
    private TaskDescriptor taskDescriptorPrefab;

    private Dictionary<Task, TaskDescriptor> taskDesriptorsByTask = new Dictionary<Task, TaskDescriptor>();

    private Quest targetQuest;

    /// <summary>
    /// 후처리 과정
    /// </summary>
    private void OnDestroy()
    {
        if (targetQuest != null)
        {
            targetQuest.onNewTaskGroup -= UpdateTaskDescriptos;
            targetQuest.onCompleted -= DestroySelf;
        }

        foreach (var tuple in taskDesriptorsByTask)
        {
            var task = tuple.Key;
            task.onSuccessChanged -= UpdateText;
        }
    }

    /// <summary>
    /// Tracker를 Setup 하는 함수
    /// </summary>
    /// <param name="targetQuest"></param>
    /// <param name="titleColor"></param>
    public void Setup(Quest targetQuest, Color titleColor)
    {
        if(targetQuest != null)
        {
            this.targetQuest = targetQuest;

            questTitleText.text = targetQuest.Category == null ?
                targetQuest.DisplayName :
                $"[{targetQuest.Category.DisplayName}] {targetQuest.DisplayName}";

            questTitleText.color = titleColor;
            questTitleText.alpha = 1.0f;

            targetQuest.onNewTaskGroup += UpdateTaskDescriptos;
            targetQuest.onCompleted += DestroySelf;

            var taskGroups = targetQuest.TaskGroups;


            UpdateTaskDescriptos(targetQuest, taskGroups[0]);

            // 깨진 Task 정보들도 보여줌
            if (taskGroups[0] != targetQuest.CurrentTaskGroup)
            {
                for (int i = 1; i < taskGroups.Count; i++)
                {
                    var taskGroup = taskGroups[i];
                    UpdateTaskDescriptos(targetQuest, taskGroup, taskGroups[i - 1]);

                    if (taskGroup == targetQuest.CurrentTaskGroup)
                        break;
                }
            }
        }
        else
        {
            return;
        }


    }

    /// <summary>
    /// TestDescriptor 자체를 Update하는 함수
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="currentTaskGroup"></param>
    /// <param name="prevTaskGroup"></param>
    private void UpdateTaskDescriptos(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach (var task in currentTaskGroup.Tasks)
        {
            var taskDesriptor = Instantiate(taskDescriptorPrefab, transform);
            taskDesriptor.UpdateText(task);
            task.onSuccessChanged += UpdateText;

            taskDesriptorsByTask.Add(task, taskDesriptor);
        }

        if (prevTaskGroup != null)
        {
            foreach (var task in prevTaskGroup.Tasks)
            {
                var taskDesriptor = taskDesriptorsByTask[task];
                taskDesriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }

    /// <summary>
    /// Text Update 하는 Callback 함수
    /// </summary>
    /// <param name="task"></param>
    /// <param name="currentSucess"></param>
    /// <param name="prevSuccess"></param>
    private void UpdateText(Task task, int currentSucess, int prevSuccess)
    {
        taskDesriptorsByTask[task].UpdateText(task);
    }

    /// <summary>
    /// 퀘스트 완료 시 Tracker 제거해줄 Callback 함수
    /// </summary>
    /// <param name="quest"></param>
    private void DestroySelf(Quest quest)
    {
        Destroy(gameObject);
    }
}
