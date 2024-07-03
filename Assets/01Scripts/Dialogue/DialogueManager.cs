using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    public TalkManager talkManager;
    public QuestManager questManager;

    public TextMeshProUGUI objectName;
    public TextMeshProUGUI talkText;

    public GameObject talkPanel;
    public GameObject scanObject;
    public bool isAction;
    public int talkIndex;

    void Start()
    {
        //Debug.Log(questManager.CheckQuest());
    }


    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id, objData.isNpc, objData.objName);

        // Visible Talk for Action
        talkPanel.SetActive(isAction);
    }


    void Talk(int id, bool isNpc, string name)
    {
        // Set Talk Data
        int questTalkIndex = questManager.GetQuestTalkIndex();
        string talkData = talkManager.GetTalk(id + questTalkIndex, talkIndex);

        // End Talk
        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            questManager.CheckQuest(id);  // 퀘스트 상태 갱신

            return;
        }


        // Continue Talk
        if (isNpc)
        {
            talkText.text = talkData;
            objectName.text = name;
        }
        else
        {
            talkText.text = talkData;
        }

        isAction = true;
        talkIndex++;
    }
}
