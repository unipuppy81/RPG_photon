using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcQuest : MonoBehaviour
{
    [SerializeField]
    private Quest npcQuest;

    public void QuestSender()
    {
        GameObject obj = GameObject.Find("Quest Giver");
        if (obj != null)
        {
            QuestGiver questGiver = obj.GetComponent<QuestGiver>();
            if (questGiver != null)
            {
                questGiver.AddQuest(npcQuest);
                Debug.Log("퀘스트가 성공적으로 추가되었습니다.");
            }
            else
            {
                Debug.LogWarning("QuestGiver 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Quest Giver 오브젝트를 찾지 못했습니다.");
        }
    }

    public void QuestSender(Quest sender)
    {
        GameObject obj = GameObject.Find("Quest Giver");
        if (obj != null)
        {
            QuestGiver questGiver = obj.GetComponent<QuestGiver>();
            if (questGiver != null)
            {
                questGiver.AddQuest(sender);
                Debug.Log("퀘스트가 성공적으로 추가되었습니다.");
            }
            else
            {
                Debug.LogWarning("QuestGiver 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Quest Giver 오브젝트를 찾지 못했습니다.");
        }
    }
}
