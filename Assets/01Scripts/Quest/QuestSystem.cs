using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestSystem : MonoBehaviour
{
    #region Save Path
    private const string kSaveRootPath = "questSystem";
    private const string kActiveQuestsSavePath = "activeQuests";
    private const string kCompletedQuestsSavePath = "completedQuests";
    private const string kActiveAchievementsSavePath = "activeAchievement";
    private const string kCompletedAchievementsSavePath = "completedAchievement";
    #endregion

    #region Events
    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    #endregion

    private static QuestSystem instance;
    private static bool isApplicationQuitting;

    public static QuestSystem Instance
    {
        get
        {
            if(!isApplicationQuitting && instance == null)
            {
                instance = FindObjectOfType<QuestSystem>();
                if(instance == null)
                {
                    instance = new GameObject("QuestSystem").AddComponent<QuestSystem>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private List<Quest> activeQuests = new List<Quest>();
    [SerializeField]
    private List<Quest> completedQuests = new List<Quest>();

    private List<Quest> activeAchievements = new List<Quest>();
    private List<Quest> completedAchievements = new List<Quest>();

    [SerializeField]
    private QuestDatabase questDatabase;
    [SerializeField]
    private QuestDatabase achievementDatabase;

    public event QuestRegisteredHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;

    public event QuestRegisteredHandler onAchievementRegistered;
    public event QuestCompletedHandler onAchievementCompleted;
    public event QuestCanceledHandler onArchievementCanceled;
    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => completedQuests;

    public IReadOnlyList<Quest> ActiveAchievements => activeAchievements;
    public IReadOnlyList<Quest> CompletedAchievements => completedAchievements;


    private void Awake()
    {
        StartCoroutine(DelayedSetup());

        questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
        achievementDatabase = Resources.Load<QuestDatabase>("AchievementDatabase");


        if (achievementDatabase == null)
            return;
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => GameManager.isPlayGame);


        if (!Load())
        {
            foreach (var achievement in achievementDatabase.Quests)
            {
                Register(achievement);
            }
        }
    }

    // 임시로 게임 종료시 저장
    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
        Save();
    }

    /// <summary>
    /// 퀘스트 등록하는 함수
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public Quest Register(Quest quest)
    {
        var newQuest = quest.Clone();

        if(newQuest is Achievement)
        {
            newQuest.onCanceled += OnAchievementcompleted;

            activeAchievements.Add(newQuest);

            newQuest.OnRegister();
            onAchievementRegistered?.Invoke(newQuest);
        }
        else
        {
            newQuest.onCompleted += OnQuestCompleted;
            newQuest.onCanceled += OnQuestCanceled;

            activeQuests.Add(newQuest);

            newQuest.OnRegister();
            onQuestRegistered?.Invoke(newQuest);
        }

        return newQuest;
    }

    // 보고 받는 함수 내부, 외부
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(activeQuests, category, target, successCount);
        ReceiveReport(activeAchievements, category, target, successCount);
    }

    public void ReceiveReport(Category category, TaskTarget target, int successCount)
        => ReceiveReport(category.CodeName, target.Value, successCount);

    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        // ToArray로 List의 사본을 만들어서 for문 돌리는 이유는
        // for문이 돌아가는 와중에 quest가 완료되어 목록에서 빠질수도 있기 떄문
        foreach(var quest in quests.ToArray())
            quest.ReceiveReport(category, target, successCount);
    }

    public void CompleteWaitingQuests()
    {
        foreach (var quest in activeQuests.ToList())
        {
            if (quest.IsCompletable)
            {
                quest.Complete();
            }
        }
        /*
         * 다른곳에서 퀘스트 종료 하는법
         * QuestSystem.Instance.CompleteWaitingQuests();
         * QuestSystem.Instance.Save();
         */
    }

    // Quest가 목록에 있는지 확인하는 함수
    public bool ContainsInActiveQuests(Quest quest) => activeQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompleteQuests(Quest quest) => completedQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInActiveAchievement(Quest quest) => activeAchievements.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompleteAchievement(Quest quest) => completedAchievements.Any(x => x.CodeName == quest.CodeName);

    /*
     * save는 여러 상황에서 할 수 있는데 
     * Quest의 Event에 등록해서 Task의 성공 횟수가 변하면 저장하게 만들어도 되고
     * 똑같이 event에 등록해서 quest가 완료되면 저장하게 만들어도 되고 
     * 아니면 따로 어디서 save 함수 직접 사용해도 됨
     */
    public void Save()
    {
        var root = new JObject();
        root.Add(kActiveQuestsSavePath, CreateSaveDatas(activeQuests));
        root.Add(kCompletedQuestsSavePath, CreateSaveDatas(completedQuests));
        root.Add(kActiveAchievementsSavePath, CreateSaveDatas(activeAchievements));
        root.Add(kCompletedAchievementsSavePath, CreateSaveDatas(completedAchievements));

        PlayerPrefs.SetString(kSaveRootPath, root.ToString());
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        if (PlayerPrefs.HasKey(kSaveRootPath))
        {
            var root = JObject.Parse(PlayerPrefs.GetString(kSaveRootPath));

            LoadSaveDatas(root[kActiveQuestsSavePath], questDatabase, LoadActiveQuest);
            LoadSaveDatas(root[kCompletedQuestsSavePath], questDatabase, LoadCompletedQuest);

            LoadSaveDatas(root[kActiveAchievementsSavePath], questDatabase, LoadActiveQuest);
            LoadSaveDatas(root[kCompletedAchievementsSavePath], questDatabase, LoadCompletedQuest);

            return true;
        }
        else
            return false;
    }

    //save Data 만드는 함수
    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();
        foreach(var quest in quests)
        {
            if (quest.IsSavable)
            {
                // Save Data를 Json 형태로 변환시킨 후에 JSON Array에 넣어주는 코드
                saveDatas.Add(JObject.FromObject(quest.ToSaveData()));
            }
        }
        return saveDatas;
    }

    private void LoadSaveDatas(JToken datasToken, QuestDatabase database, System.Action<QuestSaveData, Quest> onSuccess)
    {
        var datas = datasToken as JArray;
        foreach(var data in datas)
        {
            var saveDatas = data.ToObject<QuestSaveData>();
            var quest = database.FindQuestBy(saveDatas.codeName);
            onSuccess.Invoke(saveDatas, quest);
        }
    }

    /// <summary>
    /// 불러온 퀘스트들을 등록해주고 등록한 퀘스트에 저장된 데이터를 넣어주는 함수
    /// </summary>
    /// <param name="saveData"></param>
    /// <param name="quest"></param>
    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        if(newQuest is Achievement)
        {
            completedAchievements.Add(newQuest);
        }
        else
        {
            completedQuests.Add(newQuest);
        }
    }

    // 앞서 만든 event 들을 quest event에 등록할수있게 콜백함수 제작
    #region Callback

    // 아래 함수들은 퀘스트가 끝나면 자동으로 퀘스트를 빼고 추가해줌
    private void OnQuestCompleted(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        activeQuests.Remove(quest);
        onQuestCompleted?.Invoke(quest);

        Destroy(quest, Time.deltaTime);
    }

    private void OnAchievementcompleted(Quest achievement)
    {
        activeAchievements.Remove(achievement);
        completedAchievements.Add(achievement);

        onAchievementCompleted?.Invoke(achievement);
    }
    #endregion
}
