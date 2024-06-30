using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _InventoryPanel;
    [SerializeField] private GameObject _EqipmentPanel;
    [SerializeField] private GameObject _ShopPanel;
    [SerializeField] private GameObject _RealShopPanel;
    [SerializeField] private GameObject characterListUI;
    [SerializeField] private GameObject _QuestPanel;
    [SerializeField] private GameObject _AchievementPanel;

    bool activeInventory = false;
    bool activeEquipment = false;
    bool activeShop = false;
    bool activeRealShop = false; 
    bool activeQuest = false;
    bool activeAchievement = false;


    private void Start()
    {
        _InventoryPanel.SetActive(activeInventory);
        _EqipmentPanel.SetActive(activeEquipment);
        _ShopPanel.SetActive(activeShop);
        _QuestPanel.SetActive(activeQuest);
        _AchievementPanel.SetActive(activeAchievement);
    }

    void Update()
    {
        if(GameManager.isPlayGame && !GameManager.isChatting)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                activeInventory = !activeInventory;
                _InventoryPanel.SetActive(activeInventory);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                activeEquipment = !activeEquipment;
                _EqipmentPanel.SetActive(activeEquipment);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                activeShop = !activeShop;
                _ShopPanel.SetActive(activeShop);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                activeRealShop = !activeRealShop;
                _RealShopPanel.SetActive(activeRealShop);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                activeQuest = !activeQuest;
                _QuestPanel.SetActive(activeQuest);
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                activeAchievement = !activeAchievement;
                _AchievementPanel.SetActive(activeAchievement);
            }
        }
    }

    public void CloseUIPanel()
    {
        characterListUI.SetActive(false);
    }
}
