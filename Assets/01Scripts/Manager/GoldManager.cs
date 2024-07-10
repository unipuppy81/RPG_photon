using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldManager : Singleton<GoldManager>
{
    [SerializeField]
    private TextMeshProUGUI goldText;

    public void SetGoldText(int gold)
    {
        goldText.text = "Gold : " + gold.ToString();
    }
    public void SetGoldText()
    {
        goldText.text = "Gold : " + GameManager.Instance.Gold;
    }
}
