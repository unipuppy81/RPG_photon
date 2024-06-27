public class QuestData
{
    public string questName;
    public int[] npcId;
    public string npcName;

    public QuestData(string name, int[] npc, string _npcName)
    {
        questName = name;
        npcId = npc;
        npcName = _npcName;
    }
}
