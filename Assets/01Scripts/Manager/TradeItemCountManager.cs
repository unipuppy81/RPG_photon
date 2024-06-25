using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class TradeItemCountManager : MonoBehaviourPunCallbacks
{
    public GameObject TradePanel;
    private TradePanelController _controller;
    public Button countPlusBtn;
    public Button countMinusBtn;
    public Button setBtn;

    public TextMeshProUGUI countText;

    public string itemName;

    public int itemCount;
    public int itemMaxCount;

    public List<string> itemNameArray;
    public List<int> itemCountArray;

    private void Start()
    {
        _controller = TradePanel.GetComponent<TradePanelController>();

        countPlusBtn.onClick.AddListener(itemCountPlus);
        countMinusBtn.onClick.AddListener(itemCountMinus);
        setBtn.onClick.AddListener(registryButton);

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

        itemCount = 0;
        countText.text = "0";

        gameObject.SetActive(false);
    }
}
