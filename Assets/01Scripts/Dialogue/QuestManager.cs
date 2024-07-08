using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public int questId;
    public int questActionIndex;

    public GameObject[] questObject; // portal

    [SerializeField] GameObject questListManagerObject;
    QuestListManager qListManager;


    public Dictionary<int, QuestData> questList;



    [Header("GameQuest")]
    public int killCount;
    public int hpGlobeCount;
    public int mpGlobeCount;

    [Header("BuildereQuest")]
    [SerializeField]
    private GameObject helmetBuilder;
    [SerializeField]
    private GameObject helmet;


    [SerializeField]
    private GameObject questClearPanel;
    [SerializeField] 
    private TextMeshProUGUI nextQuestText;
    private Image panelImage;
    private Color initialColor; // 초기 색상 저장용 변수

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();

        panelImage = questClearPanel.GetComponent<Image>();
        initialColor = panelImage.color;
    }

    void Start()
    {
        //qListManager = questListManagerObject.GetComponent<QuestListManager>();
    }


    // 퀘스트 성고 패널

    void ClearQuestPanelOn()
    {
        //questClearPanel.SetActive(true);
    }

    // 대화 퀘스트 
    void GenerateData()
    {
        questList.Add(10, new QuestData("퀘스트 1",  
                                new int[] { 1000, 1000 }, 
                                "0"));

        questList.Add(20, new QuestData("퀘스트 2",
                                 new int[] { 2000, 100, 2000 },
                                 "0"));

        questList.Add(30, new QuestData("퀘스트 3",
                                new int[] { 3000 },
                                "0"));

        questList.Add(40, new QuestData("퀘스트 4",
                               new int[] { 3000, 3000 },
                               "0"));
    }


    // Quest 완료 여부 확인
    public bool IsQuestComplete(int questIdToCheck)
    {
        if (questList.ContainsKey(questIdToCheck))
        {
            return questActionIndex >= questList[questIdToCheck].npcId.Length;
        }
        return false;
    }


    public int GetQuestTalkIndex()
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        // 다음 NPC 확인
        if (id == questList[questId].npcId[questActionIndex])
        {
            questActionIndex++;
        }

        // Control Quest Object
        ControlObject();

        // Talk Complete & Next Quest
        if (questActionIndex == questList[questId].npcId.Length)
        {
            NextQuest();
        }

        // 현재 퀘스트명 반환
        return questList[questId].questName;
    }

    public void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
    }

    void ControlObject()
    {
        switch (questId)
        {
            case 10:
                if (questActionIndex == 1)
                {

                }
                else if (questActionIndex == 2)
                {
                    ShowQuestClearPanel("새로운 퀘스트 : 인부의 부탁");
                }
                break;


            case 20:
                if (questActionIndex == 1)
                {

                }
                else if (questActionIndex == 2)
                {

                }
                else if(questActionIndex == 3)
                {
                    ShowQuestClearPanel("새로운 퀘스트 : 다음 마을로 가자");

                    helmet.SetActive(false);
                    helmetBuilder.SetActive(true);
                }
                break;

            case 30:
                if (questActionIndex == 1)
                {
                    Debug.Log("quest ID : " + questId + "  questActionIndex : " + questActionIndex);
                }
                break;

            case 40:
                if (questActionIndex == 2)
                {
                    Debug.Log("quest ID : " + questId + "  questActionIndex : " + questActionIndex);
                }
                break;
        }
    }

    public void ShowQuestClearPanel(string questText)
    {
        // 새로운 퀘스트 텍스트 설정
        nextQuestText.text = questText;

        // 패널을 활성화
        questClearPanel.SetActive(true);
        
        // 투명도 초기화
        ResetAlpha();

        // 2초 후에 페이드아웃 코루틴 실행
        StartCoroutine(FadeOutPanel(2.0f));
    }

    private IEnumerator FadeOutPanel(float fadeOutTime)
    {
        // 대기
        yield return new WaitForSeconds(fadeOutTime);

        // 시작 투명도
        float startAlpha = panelImage.color.a;
        float rate = 1.0f / fadeOutTime;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color color = panelImage.color;
            color.a = Mathf.Lerp(startAlpha, 0, progress);
            panelImage.color = color;

            progress += rate * Time.deltaTime;
            yield return null;
        }

        // 완전히 투명하게 만들고 비활성화
        Color finalColor = panelImage.color;
        finalColor.a = 0.0f;
        panelImage.color = finalColor;

        questClearPanel.SetActive(false);
    }

    private void ResetAlpha()
    {
        // 초기 색상으로 설정하여 투명도 초기화
        panelImage.color = initialColor;
    }
}
