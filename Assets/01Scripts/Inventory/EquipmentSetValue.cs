using Photon.Pun;
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

    [Header("Player")]
    PhotonView playerPhotonView;
    Character_Warrior cw;

    private void OnEnable()
    {
        UpdateStats();
    }

    public void setValue(int pv, string _name, float _hp, float _atk, float _def, float _speed, float _runSpeed)
    {
        playerPhotonView = PhotonView.Find(pv);
        if(playerPhotonView != null)
        {
            GameObject player = playerPhotonView.gameObject;
            cw = player.GetComponent<Character_Warrior>();

            Debug.Log("cw 찾았습니다.");
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
                break;

            case "chest_B":
                def = tmpDef + 40;
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

                break;
            case "boots_B":
                speed = tmpSpeed * 1.3f;
                runSpeed = tmpRunSpeed * 1.3f;

                break;
            default:
                break;
        }
        cw.StateUpdate(atk, tmpDef, tmpSpeed, tmpRunSpeed);
        UpdateStats();

    }

    private void UpdateStats()
    {
        nameText.text = "이름 : " + name;
        hpText.text = "Hp : " + hp.ToString();
        atkText.text = "Atk : " + atk.ToString();
        defText.text = "Def : " + def.ToString();
        speedText.text = "Speed : " + speed.ToString();
    }

}
