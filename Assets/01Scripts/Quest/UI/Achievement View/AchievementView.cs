using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementView : MonoBehaviour
{
    [SerializeField]
    private RectTransform achievementGroup;
    [SerializeField]
    private AchievementDetailView achievementDetailViewPrefab;

    private void Start()
    {
        StartCoroutine(DelayedSetup());
        //var questSystem = QuestSystem.Instance;
        //CreateDetailViews(questSystem.ActiveAchievements);
        //CreateDetailViews(questSystem.CompletedAchievements);

        //gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Input V");

            var questSystem = QuestSystem.Instance;
            CreateDetailViews(questSystem.ActiveAchievements);
            CreateDetailViews(questSystem.CompletedAchievements);
        }
    }

    private void CreateDetailViews(IReadOnlyList<Quest> achievements)
    {
        foreach (var achievement in achievements)
            Instantiate(achievementDetailViewPrefab, achievementGroup).Setup(achievement);
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => GameManager.isPlayGame);

        yield return new WaitForSeconds(1.0f);

        Debug.Log("DelaySetUp");
        var questSystem = QuestSystem.Instance;
        CreateDetailViews(questSystem.ActiveAchievements);
        CreateDetailViews(questSystem.CompletedAchievements);

        gameObject.SetActive(false);

        NetworkManager.Instance.HideLoadingScreen();
    }
}
