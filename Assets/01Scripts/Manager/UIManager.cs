using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class UIManager : SingletonPhoton<UIManager>
{
    [SerializeField] private GameObject _InventoryPanel;
    [SerializeField] private GameObject _EquipmentPanel;
    [SerializeField] private GameObject _ShopPanel;
    [SerializeField] private GameObject _RealShopPanel;
    [SerializeField] private GameObject characterListUI;
    [SerializeField] private GameObject _QuestPanel;
    [SerializeField] private GameObject _AchievementPanel;
    [SerializeField] private Image _chatPanel;


    bool activeInventory = true;
    bool activeEquipment = true;
    bool activeShop = false;
    bool activeRealShop = false; 
    bool activeQuest = false;
    bool activeAchievement = true;


    private void Start()
    {
        _InventoryPanel.SetActive(activeInventory);
        _EquipmentPanel.SetActive(activeEquipment);
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
                _EquipmentPanel.SetActive(activeEquipment);
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

    public void CloseEquip()
    {
        activeEquipment = false;
        activeInventory = false;
        _EquipmentPanel.SetActive(activeEquipment);
        _InventoryPanel.SetActive(activeInventory);
    }

    public void CloseUIPanel()
    {
        characterListUI.SetActive(false);
    }
}
