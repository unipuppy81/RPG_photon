using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //public static GameManager Instance { get; private set; }

    public static bool isPlayGame = false;
    public static bool isChatting = false;
    public static bool isTradeChatting = false;
    public static bool isTradeChatInput = false;

    public string playerName;

    private int gold;

    public int Gold
    {
        set => gold = value;
        get => gold;
    }

    private Dictionary<string, int> playerGold = new Dictionary<string, int>();

    private void Awake()
    {
        /*
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 인스턴스를 유지
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 중복된 인스턴스가 생기지 않도록 파괴
        }
        */
    }

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
        GoldManager.Instance.SetGoldText(Gold);

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

        GoldManager.Instance.SetGoldText(Gold);

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
}
