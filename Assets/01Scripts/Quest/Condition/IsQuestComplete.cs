using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditon/IsQuestComplete", fileName = "IsQuestComplete_")]
public class IsQuestComplete : Condition
{
    [SerializeField]
    private Quest target;

    public override bool IsPass(Quest quest)
    {
        return QuestSystem.Instance.ContainsInCompleteQuests(target);
    }
}
