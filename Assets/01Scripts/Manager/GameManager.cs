using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public static bool isPlayGame = false;
    public static bool isChatting = false;
    public static bool isTradeChatting = false;
    public static bool isTradeChatInput = false;


    // 임시
    public int Gold = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 인스턴스를 유지
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 중복된 인스턴스가 생기지 않도록 파괴
        }
    }
    public void Start()
    {
        AddGold(PlayerPrefs.GetInt("BonusGold", 0));
    }

    public void AddGold(int gold)
    {
        Gold += gold;
        Debug.Log("Gold 는 " + Gold);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("On Left Room");

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Destroy(gameObject);
            SceneManager.LoadScene("TownScene");
        }
        else if (SceneManager.GetActiveScene().name == "TownScene")
        {
            Destroy(gameObject);
            SceneManager.LoadScene("GameScene");
        }
    }
}
