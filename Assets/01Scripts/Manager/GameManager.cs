using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool isPlayGame = false;
    public static bool isChatting = false;
    public static bool isTradeChatting = false;
    public static bool isTradeChatInput = false;

    public static bool isTown = true;
    public static bool isBattle = false;

    public string playerName;

    private int gold;

    public GameObject player;

    public int Gold
    {
        set => gold = value;
        get => gold;
    }

    private Dictionary<string, int> playerGold = new Dictionary<string, int>();

    /// <summary>
    /// 플레이어의 골드를 저장합니다.
    /// </summary>
    /// <param name="name">플레이어 이름</param>
    /// <param name="gold">저장할 골드 양</param>
    public void SaveGold(string name, int gold)
    {
        PlayerPrefs.SetInt(name + "_Gold", gold);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 플레이어의 골드를 변경합니다.
    /// </summary>
    /// <param name="name">플레이어 이름</param>
    /// <param name="gold">변경할 골드 양</param>
    public void ChangeGold(string name, int gold)
    {
        int currentGold = LoadGold(name);
        Gold = currentGold + gold;

        SaveGold(name, Gold); // 변경된 골드를 저장합니다.
        GoldManager.Instance.SetGoldText();

        Debug.Log("Gold for " + name + ": " + Gold);
    }

    /// <summary>
    /// 플레이어의 골드를 로드합니다.
    /// </summary>
    /// <param name="playerName">플레이어 이름</param>
    /// <returns>로드된 골드 값</returns>
    public int LoadGold(string playerName)
    {
        int loadedGold = PlayerPrefs.GetInt(playerName + "_Gold", 0); // 플레이어 이름을 키로 사용하여 저장된 골드를 불러옵니다.
        Debug.Log("Gold for " + playerName + ": " + loadedGold);
        Gold = loadedGold; // 현재 골드 값 업데이트

        GoldManager.Instance.SetGoldText();

        return Gold;
    }

    /// <summary>
    /// 플레이어의 골드를 가져옵니다.
    /// </summary>
    /// <param name="playerName">플레이어 이름</param>
    /// <returns>해당 플레이어의 골드</returns>
    public int GetPlayerGold(string playerName)
    {
        if (playerGold.ContainsKey(playerName))
        {
            return playerGold[playerName];
        }
        else
        {
            return 0; // 해당 플레이어의 골드가 없는 경우 0을 반환
        }
    }


    public void ChangeGold(int gold)
    {
        int currentGold = LoadGold(playerName);
        Gold = currentGold + gold;

        SaveGold(playerName, Gold); // 변경된 골드를 저장합니다.

        Debug.Log("Gold for " + playerName + ": " + Gold);
    }

    public void GameSave()
    {
        // 플레이어 위치
        PlayerPrefs.SetFloat("playerX", player.transform.position.x);
        PlayerPrefs.SetFloat("playerY", player.transform.position.y);
        PlayerPrefs.SetFloat("playerZ", player.transform.position.z);

        // 퀘스트
        PlayerPrefs.SetInt("QuestId", QuestManager.Instance.questId);
        PlayerPrefs.SetInt("QuestActionIndex", QuestManager.Instance.questActionIndex);

        PlayerPrefs.Save();
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("playerX"))
            return;

        float x = PlayerPrefs.GetFloat("playerX");
        float y = PlayerPrefs.GetFloat("playerY");
        float z = PlayerPrefs.GetFloat("plaeyrZ");
        int questId = PlayerPrefs.GetInt("QuestId");
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex");

        player.transform.position = new Vector3(x, y, z);
        QuestManager.Instance.questId = questId;
        QuestManager.Instance.questActionIndex = questActionIndex;
    }

    public Vector3 GameLoad2()
    {
        if (!PlayerPrefs.HasKey("playerX"))
            return new Vector3(0,0,0);

        float x = PlayerPrefs.GetFloat("playerX");
        float y = PlayerPrefs.GetFloat("playerY");
        float z = PlayerPrefs.GetFloat("plaeyrZ");
        int questId = PlayerPrefs.GetInt("QuestId");
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex");

        Vector3 pos = new Vector3(x, y, z);
        QuestManager.Instance.questId = questId;
        QuestManager.Instance.questActionIndex = questActionIndex;

        return pos;
    }

}
