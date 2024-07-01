using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private Quest[] quests;

    private void Start()
    {
        QuestSetting();
    }

    private void QuestSetting()
    {
        foreach (var quest in quests)
        {
            if (quest.IsAcceptable && !QuestSystem.Instance.ContainsInCompleteQuests(quest))
                QuestSystem.Instance.Register(quest);

            DelQuest();
        }
    }

    public void AddQuest(Quest newQuest)
    {
        List<Quest> questList = new List<Quest>(quests);

        questList.Clear();
        questList.Add(newQuest);
        quests = questList.ToArray();

        QuestSetting();
    }

    private void DelQuest()
    {
        List<Quest> questList = new List<Quest>(quests);

        // 리스트의 제일 앞에 있는 원소 제거
        if (questList.Count > 0)
        {
            questList.RemoveAt(0);
        }

        quests = questList.ToArray();
    }
}
