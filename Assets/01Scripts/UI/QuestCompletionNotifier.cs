using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class QuestCompletionNotifier : MonoBehaviour
{
    [SerializeField]
    private string titleDescription;    // 타이틀 문구

    [SerializeField]
    private TextMeshProUGUI titleText;  // 타이틀 텍스트
    [SerializeField]
    private TextMeshProUGUI rewardText; // 보상 텍스트
    [SerializeField]
    private float showTime = 3f;    // notifier 보여줄 시간 변수

    // 한번에 여러개 퀘스트 깨질 경우 대비해 통보 예약 List
    private Queue<Quest> reservedQuests = new Queue<Quest>();
    // 성능 향상에 매우 도움
    private StringBuilder stringBuilder = new StringBuilder();

    private void Start()
    {
        var questSystem = QuestSystem.Instance;
        questSystem.onQuestCompleted += Notify;
        questSystem.onAchievementCompleted += Notify;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var questSysem = QuestSystem.Instance;
        if (questSysem != null)
        {
            questSysem.onQuestCompleted -= Notify;
            questSysem.onAchievementCompleted -= Notify;
        }
    }

    /// <summary>
    /// 완료된 퀘스트 등록
    /// </summary>
    /// <param name="quest"></param>
    private void Notify(Quest quest)
    {
        reservedQuests.Enqueue(quest);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine("ShowNotice");
        }
    }

    /// <summary>
    /// 클리어한 퀘스트의 정보 보여주는 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowNotice()
    {
        var waitSeconds = new WaitForSeconds(showTime);

        Quest quest;
        while (reservedQuests.TryDequeue(out quest))
        {
            titleText.text = titleDescription.Replace("%{dn}", quest.DisplayName);
            foreach (var reward in quest.Rewards)
            {
                stringBuilder.Append(reward.Description);
                stringBuilder.Append(" ");
                stringBuilder.Append(reward.Quantity);
                stringBuilder.Append(" ");
            }
            rewardText.text = stringBuilder.ToString();
            stringBuilder.Clear();

            yield return waitSeconds;
        }

        gameObject.SetActive(false);
    }
}
