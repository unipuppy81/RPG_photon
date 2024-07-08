using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    Dictionary<int, System.Action> questActions; // 호출할 메서드를 매핑하기 위한 사전
    Dictionary<int, QuestReporter> questReporters; // QuestReporter 매핑을 위한 사전 추가
    QuestManager q;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        questActions = new Dictionary<int, System.Action>(); // 사전 초기화
        questReporters = new Dictionary<int, QuestReporter>(); // 사전 초기화

        GenerateData();
        GenerateQuestReporters();
        GenerateQuestActions();
    }

    private void Start()
    {
        q = GameObject.Find("QuestManager").GetComponent<QuestManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            q.NextQuest();
        }
    }
    void GenerateQuestReporters()
    {
        // 각 대화 ID에 대응하는 QuestReporter를 설정합니다.
        // 예시: Npc_Nun 해당하는 QuestReporter를 찾습니다.
        GameObject npcNun = GameObject.Find("Npc_Nun");
        if (npcNun != null)
        {
            QuestReporter nunReporter = npcNun.GetComponent<QuestReporter>();
            if (nunReporter != null)
            {
                questReporters.Add(10 + 1000, nunReporter);
                questReporters.Add(11 + 1000, nunReporter);
            }
        }

        GameObject npcBuilder = GameObject.Find("Npc_Builder");
        if (npcBuilder != null)
        {
            QuestReporter builderReporter = npcBuilder.GetComponent<QuestReporter>();
            if (builderReporter != null)
            {
                questReporters.Add(20 + 2000, builderReporter);
                questReporters.Add(22 + 2000, builderReporter);
            }
        }
    }

    void GenerateQuestActions()
    {
        // Npc_Chief 오브젝트에 있는 NpcQuest의 QuestSender() 메서드를 설정합니다.
        GameObject npcNun = GameObject.Find("Npc_Nun");
        if (npcNun != null)
        {
            NpcQuest npcQuest = npcNun.GetComponent<NpcQuest>();
            if (npcQuest != null)
            {
                // 특정 대화 ID와 NpcQuest의 QuestSender() 메서드를 매핑합니다.
                questActions.Add(10 + 1000, npcQuest.QuestSender);
                questActions.Add(11 + 1000, npcQuest.QuestSender);
            }
        }

        // Npc_Builder 오브젝트에 있는 NpcQuest의 QuestSender() 메서드를 설정합니다.
        GameObject npcBuilder = GameObject.Find("Npc_Builder");
        if (npcBuilder != null)
        {
            NpcQuest npcQuest = npcBuilder.GetComponent<NpcQuest>();
            if (npcQuest != null)
            {
                // 특정 대화 ID와 NpcQuest의 QuestSender() 메서드를 매핑합니다.
                questActions.Add(20 + 2000, npcQuest.QuestSender);
                questActions.Add(22 + 2000, npcQuest.QuestSender);
            }
        }


    }
    void GenerateData()
    {
        // Talk Data
        // Nun : 1000, Builder : 2000
        // 헬멧 : 100

        talkData.Add(100, new string[]
        {
            "헬멧이다"
        });

        talkData.Add(1000, new string[]
        { 
            "아이들이 무럭무럭 자랐으면 좋겠어요."
        });

        talkData.Add(2000, new string[]
        {
            "얼른 공사를 마무리해야하는데.."
        });

        talkData.Add(3000, new string[]
        {
            "포탈 Npc 입니다.",
            "돈 내놔라.."
        });


        // Quest Talk
        talkData.Add(10 + 1000, new string[]
        {
            "요즘 마을에 유령들이 많이 나타나서 아이들이 자유롭게 뛰어다닐수가 없어요..",
            "용사님께서 도와주신다고요?",
            "그럼 유령을 5마리 정도만 잡아주시겠어요?"
        });

        talkData.Add(11 + 1000, new string[]{
            "감사합니다. 덕분에 아이들이 안심하고 귀가할수 있겠어요.",
            "여기 별 거 아니지만 답례에요."
        });

        talkData.Add(20 + 2000, new string[]
        {
            "공사를 시작해야하는데 안전모가 없어서 공사를 할수가 없어.",
            "너가 찾아주겠다고?",
            "다른 마을로 넘어가는 다리쪽에서 잃어버린거 같아."
        });

        talkData.Add(22 + 2000, new string[]
        {
            "헬멧을 찾았다고? 내가 찾을때는 못 봤는데",
            "어찌됐든 고마워 얼른 일하러 가야겠어"
        });
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (!talkData.ContainsKey(id))
        {
            // 해당 퀘스트 진행 순서 대사가 없을 때.
            // 퀘스트 맨 처음 대사를 가지고 온다.
            if (!talkData.ContainsKey(id - id % 10))
            {
                return GetTalk(id - id % 100, talkIndex);

            }
            else
            {
                // 퀘스트 맨 처음 대사마저 없을 때 ( 물건 )
                // 기본 대사를 가지고 온다.
                return GetTalk(id - id % 10, talkIndex);
            }
        }


        // 대사가 끝남
        if (talkIndex == talkData[id].Length)
        {
            if (questReporters.ContainsKey(id))
            {
                questReporters[id].Report();
            }

            if (questActions.ContainsKey(id))
            {
                questActions[id].Invoke();
            }

            // QuestManager에서 해당 Quest가 완료되었는지 체크
            if (q.IsQuestComplete(id - 1000))
            {
                q.NextQuest();
            }

            return null;
        }
        else
        {
            return talkData[id][talkIndex];
        }
    }
}
