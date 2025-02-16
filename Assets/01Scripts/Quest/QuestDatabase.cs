using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif  

[CreateAssetMenu(menuName ="Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField]
    private List<Quest> quests;

    public IReadOnlyList<Quest> Quests => quests;

    public Quest FindQuestBy(string codeName) => quests.FirstOrDefault(x => x.CodeName == codeName);

#if UNITY_EDITOR
    [ContextMenu("FindQuets")]
    private void FindQuests()
    {
        FindQuestsBy<Quest>();
    }

    [ContextMenu("FindAchievements")]
    private void FindAchievements()
    {
        FindQuestsBy<Achievement>();
    }

    private void FindQuestsBy<T>() where T : Quest
    {
        quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach(var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if(quest.GetType() == typeof(T))
            {
                quests.Add(quest);
            }


            // QuestDatabase 객체가 가진 serialize 변수가 변화가 생겼으니 에셋 저장할때 반영
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
