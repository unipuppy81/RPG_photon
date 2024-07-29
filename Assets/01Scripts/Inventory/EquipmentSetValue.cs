using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentSetValue : SingletonPhoton<EquipmentSetValue>
{
    [Header("EquipmentSlot")]
    [SerializeField]
    private GameObject weaponItem;
    [SerializeField]
    private GameObject armorItem;
    [SerializeField]
    private GameObject helmetItem;
    [SerializeField]
    private GameObject glovesItem;
    [SerializeField]
    private GameObject pantItem;
    [SerializeField]
    private GameObject bootsItem;

    [Header("Stats Value")]
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI hpText;
    [SerializeField]
    private TextMeshProUGUI atkText;
    [SerializeField]
    private TextMeshProUGUI defText;
    [SerializeField]
    private TextMeshProUGUI speedText;

    private string name;
    private float hp;
    private float atk;
    private float def;
    private float speed;
    private float runSpeed;


    //base 값
    float tmpAtk;
    float tmpDef;
    float tmpSpeed;
    float tmpRunSpeed;

    public event Action OnStatsUpdated;

    [Header("Player")]
    PhotonView playerPhotonView;
    Player player;
    Character_Warrior cw;

    private void OnEnable()
    {
        OnStatsUpdated += UpdateStats;
    }

    public void setValue(int pv, string _name, float _hp, float _atk, float _def, float _speed, float _runSpeed)
    {
        playerPhotonView = PhotonView.Find(pv);
        if(playerPhotonView != null)
        {
            GameObject player = playerPhotonView.gameObject;
            cw = player.GetComponent<Character_Warrior>();
        }

        name = _name;
        hp = _hp;
        atk = _atk;
        def = _def;
        speed = _speed;
        runSpeed = _runSpeed;

        //base 값
        tmpAtk = atk;
        tmpDef = def;
        tmpSpeed = speed;
        tmpRunSpeed = runSpeed;
    }

    public void CheckEquipItem(string _itemName)
    {
        EquipItem(_itemName);

        UpdateStats();
    }

    private void EquipItem(string itemName)
    {
        // 임의로 장착 아이템의 스탯을 설정합니다. 실제 게임 로직에 따라 달라질 수 있습니다.
        // 예: 아이템 이름에 따라 공격력, 방어력 등을 부여합니다.
        Debug.Log("EquipeItem Name : " + itemName);

        switch (itemName)
        {
            case "sword_A":
                atk = tmpAtk + 20;
                break;

            case "sword_B":
                atk = tmpAtk + 40;
                break;

            case "chest_A":
                def = tmpDef + 20;
                cw.chest_a.SetActive(true);
                cw.chest_b.SetActive(false);
                break;

            case "chest_B":
                def = tmpDef + 40;
                cw.chest_a.SetActive(false);
                cw.chest_b.SetActive(true);
                break;

            case "helm_A":
                def = tmpDef + 10;
                break;

            case "helm_B":
                def = tmpDef + 20;
                break;

            case "gloves_A":
                def = tmpDef + 6;
                break;

            case "gloves_B":
                def = tmpDef + 12;
                break;

            case "pants_A":
                def = tmpDef + 10;
                break;

            case "pants_B":
                def = tmpDef + 20;
                break;

            case "boots_A":
                speed = tmpSpeed * 1.1f;
                runSpeed = tmpRunSpeed * 1.1f;
                cw.shoes_a.SetActive(true);
                cw.shoes_b.SetActive(false);

                break;
            case "boots_B":
                speed = tmpSpeed * 1.3f;
                runSpeed = tmpRunSpeed * 1.3f;
                cw.shoes_a.SetActive(false);
                cw.shoes_b.SetActive(true);
                break;
            default:
                break;
        }
        Debug.Log("PhotonNetwork LocalPlayer : " + PhotonNetwork.LocalPlayer);
        cw.UpdateLocalStats(atk, def, speed, runSpeed);
        OnStatsUpdated?.Invoke();

    }

    private void UpdateStats()
    {
        nameText.text = "이름 : " + name;
        hpText.text = "Hp : " + hp.ToString();
        atkText.text = "Atk : " + atk.ToString();
        defText.text = "Def : " + def.ToString();
        speedText.text = "Speed : " + speed.ToString();
    }
    void OnDestroy()
    {
        OnStatsUpdated -= UpdateStats;
    }
}
