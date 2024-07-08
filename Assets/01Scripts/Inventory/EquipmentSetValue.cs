using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentSetValue : Singleton<EquipmentSetValue>
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

    private void OnEnable()
    {
        UpdateStats();
    }

    public void setValue(string _name, float _hp, float _atk, float _def, float _speed)
    {
        name = _name;
        hp = _hp;
        atk = _atk;
        def = _def;
        speed = _speed;
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
            case "Sword_A":
                Debug.Log("sword_A");
                
                //atk += 10; // 무기 공격력 추가 (예)
                break;
            case "Sword_B":
                Debug.Log("sword_B");

                break;
            case "chest_A":
                Debug.Log("chest_A");

                break;
            case "chest_B":
                Debug.Log("chest_B");

                break;
            case "helm_A":
                Debug.Log("helm_A");

                break;
            case "helm_B":
                Debug.Log("helm_B");

                break;
            case "gloves_A":
                Debug.Log("gloves_A");

                break;
            case "gloves_B":
                Debug.Log("gloves_B");

                break;
            case "pants_A":
                Debug.Log("pants_A");

                break;
            case "pants_B":
                Debug.Log("pants_B");

                break;
            case "boots_A":
                Debug.Log("boots_A");

                break;
            case "boots_B":
                Debug.Log("boots_B");

                break;
            default:
                break;
        }
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
