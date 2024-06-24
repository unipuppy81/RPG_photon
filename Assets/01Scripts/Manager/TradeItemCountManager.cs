using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class TradeItemCountManager : MonoBehaviourPunCallbacks
{
    public Button countPlusBtn;
    public Button countMinusBtn;

    public TextMeshProUGUI countText;

    private int itemCount;
    private int itemMaxCount;

    private void Start()
    {
        countPlusBtn.onClick.AddListener(itemCountPlus);
        countMinusBtn.onClick.AddListener(itemCountMinus);
    }

    public void itemCountPlus()
    {
        itemCount++;
        countText.text = itemCount.ToString();
    }

    public void itemCountMinus()
    {
        itemCount--;
        countText.text = itemCount.ToString();
    }
}
