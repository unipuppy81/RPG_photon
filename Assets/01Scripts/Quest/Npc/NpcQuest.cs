using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcQuest : MonoBehaviour
{
    [SerializeField]
    private List<Quest> npcQuest;
    public void QuestSender()
    {
        GameObject obj = GameObject.Find("Quest Giver");
        if (obj != null)
        {
            QuestGiver questGiver = obj.GetComponent<QuestGiver>();

            Quest quest = popFirstQuest();
            if (quest != null)
            {
                questGiver.AddQuest(quest);
                Debug.Log("퀘스트가 성공적으로 추가되었습니다.");
            }
            else
            {
                Debug.LogWarning("추가할 퀘스트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Quest Giver 오브젝트를 찾지 못했습니다.");
        }
    }

    public Quest popFirstQuest()
    {
        if (npcQuest != null && npcQuest.Count > 0)
        {
            Quest firstQuest = npcQuest[0];
            npcQuest.RemoveAt(0);
            return firstQuest;
        }
        return null; // 리스트가 비어있거나 null인 경우
    }
}
