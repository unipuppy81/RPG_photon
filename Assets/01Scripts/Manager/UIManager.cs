using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField]
    private TextMeshProUGUI[] text;

    private void Start()
    {
        InitializeUIPanels();

        TextUpdate();
    }
    
    public void TextUpdate()
    {
        for (int i = 0; i < text.Length; i++)
        {
            text[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }
    

    void Update()
    {
        if (GameManager.isPlayGame && !GameManager.isChatting)
        {
            TextUpdate();

            TogglePanel(KeySetting.keys[KeyAction.Inventory], ref activeInventory, _InventoryPanel);
            TogglePanel(KeySetting.keys[KeyAction.Equip], ref activeEquipment, _EquipmentPanel);
            TogglePanel(KeySetting.keys[KeyAction.Quest], ref activeQuest, _QuestPanel);
            TogglePanel(KeySetting.keys[KeyAction.Achievement], ref activeAchievement, _AchievementPanel);
        }

        /*
        if (GameManager.isPlayGame && !GameManager.isChatting)
        {
            if (Input.GetKeyDown(toggleInventoryKey))
            {
                activeInventory = !activeInventory;
                _InventoryPanel.SetActive(activeInventory);
            }
            else if (Input.GetKeyDown(toggleEquipmentKey))
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
            else if (Input.GetKeyDown(toggleQuestKey))
            {
                activeQuest = !activeQuest;
                _QuestPanel.SetActive(activeQuest);
            }
            else if (Input.GetKeyDown(toggleAchievementKey))
            {
                activeAchievement = !activeAchievement;
                _AchievementPanel.SetActive(activeAchievement);
            }
        }
        */
    }

    private void InitializeUIPanels()
    {
        _InventoryPanel.SetActive(activeInventory);
        _EquipmentPanel.SetActive(activeEquipment);
        _ShopPanel.SetActive(activeShop);
        _QuestPanel.SetActive(activeQuest);
        _AchievementPanel.SetActive(activeAchievement);
    }

    private void TogglePanel(KeyCode key, ref bool isActive, GameObject panel)
    {
        if (Input.GetKeyDown(key))
        {
            isActive = !isActive;
            panel.SetActive(isActive);
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
