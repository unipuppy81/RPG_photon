using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private Quest[] quests;

    [SerializeField]
    private Achievement[] achievements;

    private void Start()
    {
        StartCoroutine(DelayedSetup());
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => GameManager.isPlayGame);

        yield return new WaitForSeconds(1.0f);

        QuestSetting();
        AchievementSetting();

    }

    private void QuestSetting()
    {
        foreach (var quest in quests)
        {
            if (quest.IsAcceptable && !QuestSystem.Instance.ContainsInCompleteQuests(quest) 
                && !QuestSystem.Instance.ContainsInActiveQuests(quest))
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

    private void AchievementSetting()
    {
        foreach (var achievement in achievements)
        {
            if (!achievement.IsRegistered && !QuestSystem.Instance.ContainsInCompleteAchievement(achievement)
                && !QuestSystem.Instance.ContainsInActiveAchievement(achievement))
            {
                QuestSystem.Instance.Register(achievement);
            }


            DelQuest();
        }
    }
}
