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
                                new int[] { 1000, 3000 },  // 1000 : 1000번 npcid object
                                "0"));

        questList.Add(20, new QuestData("포션사기",
                                 new int[] { 2000, 1000 },
                                 "0"));

        questList.Add(30, new QuestData("던전 들어가기",
                                new int[] { 3000 },
                                "0"));


        questList.Add(40, new QuestData("던전 클리어하기",
                               new int[] { 3000, 3000 },
                               "0"));
    }





    public int GetQuestTalkIndex(int id)
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


        //Debug.Log(questList[questId].npcId.Length);
        // Talk Complete & Next Quest
        if (questActionIndex == questList[questId].npcId.Length)
        {
            NextQuest();
        }

        // 현재 퀘스트명 반환
        return questList[questId].questName;
    }

    void NextQuest()
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
                    //ClearQuestPanelOn();
                    //Debug.Log("이장 냥이에게 말 걸기 클리어");
                }
                else if (questActionIndex == 2)
                {
                    //Debug.Log("왜 자꾸 뜨는거지?");
                    //questObject[0].SetActive(true);
                }
                break;


            case 20:
                if (questActionIndex == 1)
                {
                   // Debug.Log("포션 구매하기 클리어");
                }
                else if (questActionIndex == 2)
                {

                }
                break;

            case 30:
                if (questActionIndex == 1)
                {
                    //questObject[1].SetActive(true);
                }
                break;

            case 40:
                if (questActionIndex == 2)
                {
                   // Debug.Log("던전 클리ㅣㅣ어");
                }
                break;
        }
    }

    // 게임 퀘스트





    // 마을


}
