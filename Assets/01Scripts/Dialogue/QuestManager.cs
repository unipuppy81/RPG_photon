using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
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


    [SerializeField] GameObject questClearPanel;

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
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
        questList.Add(10, new QuestData("마을 한바퀴 돌기",      // 퀘스트 이름
                                new int[] { 1000, 1000 },  // 1000 : 1000번 npcid object
                                "0"));

        questList.Add(20, new QuestData("포션사기",
                                 new int[] { 2000, 100, 2000 },
                                 "0"));

        questList.Add(30, new QuestData("던전 들어가기",
                                new int[] { 3000 },
                                "0"));

        questList.Add(40, new QuestData("던전 클리어하기",
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
                    Debug.Log("quest ID : " + questId + "  questActionIndex : " + questActionIndex);
                }
                else if (questActionIndex == 2)
                {
                    Debug.Log("quest ID : " + questId + "  questActionIndex : " + questActionIndex);
                }
                break;


            case 20:
                if (questActionIndex == 1)
                {
                    Debug.Log("quest ID : " + questId + "  questActionIndex : " + questActionIndex);
                }
                else if (questActionIndex == 2)
                {
                    Debug.Log("quest ID : " + questId + "  questActionIndex : " + questActionIndex);
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

    // 게임 퀘스트





    // 마을


}
