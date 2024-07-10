using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class TradeItemCountManager : MonoBehaviourPunCallbacks
{
    public static event Action OnCountPanelDeactivated;

    public GameObject TradePanel;
    private TradePanelController _controller;
    public Button countPlusBtn;
    public Button countMinusBtn;
    public Button setBtn;

    public TextMeshProUGUI countText;

    public string itemName;

    public int itemCount;
    public int itemMaxCount;

    private void Start()
    {
        _controller = TradePanel.GetComponent<TradePanelController>();

        countPlusBtn.onClick.AddListener(itemCountPlus);
        countMinusBtn.onClick.AddListener(itemCountMinus);
        setBtn.onClick.AddListener(registryButton);
    }

    private void OnEnable()
    {
        itemCount = 0;
        countText.text = "0";
    }

    public void itemCountPlus()
    {
        itemCount++;

        if(itemCount >= itemMaxCount)
        {
            itemCount = itemMaxCount;
        }

        countText.text = itemCount.ToString();
    }

    public void itemCountMinus()
    {
        itemCount--;

        if(itemCount <= 0)
        {
            itemCount = 0;
        }

        countText.text = itemCount.ToString();
    }

    public void registryButton()
    {
        _controller.itemNameList.Add(itemName);
        _controller.itemCountList.Add(itemCount);

        OnCountPanelDeactivated?.Invoke();

        gameObject.SetActive(false);
    }
}
