using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();

        GenerateData();
    }

    void GenerateData()
    {
        // Talk Data
        // 이장 : 1000, 상점 : 2000, 부두 관리자 : 3000
        // 학생(마르코) : 10000, 스님(홉스) : 4000
        // ShopPotal : 100, OtherPotal : 200, 


        talkData.Add(1000, new string[]
        { 
            " ",
            "...",
            "인사 했잖아.."
        });

        talkData.Add(2000, new string[]
        {
            "이곳은 상점입니다",
            "상점이라고.."
        });

        talkData.Add(3000, new string[]
        {
            "포탈 Npc 입니다.",
            "돈 내놔라.."
        });

        talkData.Add(100, new string[]
        {
            "상점입니다 ㅎ"
        });

        talkData.Add(200, new string[] {
            "던전에 입장합니다"
        });

        // Quest Talk
        talkData.Add(10 + 1000, new string[]
        {
            "어서 와",
            "저기 뒤에 있는 상점에 가봐",
            "돈이없다고? ... 어.. 돈 줄게 .."
        });

        talkData.Add(11 + 2000, new string[]
        {
            "어 그래 여긴 상점이라고.",
            "체력 포션이랑 마나 포션을 사봐",
            "어디서 사냐고? 다 알게되있어 임마"
        });


        talkData.Add(20 + 2000, new string[]
        {
            "어 그래 샀으면 다시 돌아가봐",
            "어떻게 쓰냐고?",
            "R : 체력 포션, T : 마나 포션",
            "이제 좀 가"
        });

        talkData.Add(20 + 1000, new string[]
        {
            "왔니?",
            "포션은 체력이랑 마나를 10 올려주니 잘 써먹도록",
            "사냥한번 가볼까?",
            "던전 입구에 가서 말을 걸어봐",
            "그리고 던전에는 스켈레톤이 있어",
            "스켈레톤 가까이에 가면 널 쫓아올거야",
            "조심하도록 해"
        });

        talkData.Add(30 + 3000, new string[]
        {
            "잠시만, 스킬 설명을 해줄게",
            "Q 스킬은 하늘에서 폭탄이 떨어지는 스킬이야",
            "W 스킬은 바라보는 방향으로 불을 발사하는 스킬이고",
            "E 스킬은 지정한 곳에 적을 모으는 스킬이야",
            "이제 던전을 클리어 할 수 있을거라 믿어"
        });

        talkData.Add(40 + 3000, new string[]
        {
            "고생했어",
            "성공할 줄 몰랐는데 대단한걸",
            "저기 게시판 앞에 있는 친구한테 가봐"
        });

        // Object
        talkData.Add(30 + 300, new string[]
        {
            "준비됐어?",
            "클리어하기 전엔 못 돌아와",
            "그래 가보자"
        });

        talkData.Add(40 + 500, new string[]
        {

        });

        //Name Data
        //nameData.Add(1000, "이장 냥이");
        //nameData.Add(2000, "상점 냥이");
        //nameData.Add(3000, "던전 냥이");

    }

    public string GetTalk(int id, int talkIndex)
    {
        // Exception
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


        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }

    public string GetObjName(int id, string name)
    {
        return name;
    }
}
