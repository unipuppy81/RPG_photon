using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DataBase : ScriptableObject
{
	public List<CharacterStatsDBEntity> Character; // 엑셀 시트 이름
    public List<EnemyStatsDBEntity> Enemy;

    // 특정 타입의 캐릭터 스탯을 업데이트하는 메서드
    public void UpdateCharacterStats(string type, float maxHp, float atk, float def, float wspeed, float rspeed)
    {
        // 리스트를 순회하면서 타입에 맞는 캐릭터 스탯을 찾아 업데이트합니다.
        for (int i = 0; i < Character.Count; ++i)
        {
            if (Character[i].Type == type)
            {
                Character[i].Maxhp = maxHp;
                Character[i].Atk = atk;
                Character[i].Def = def;
                Character[i].Wspeed = wspeed;
                Character[i].Rspeed = rspeed;

                return;
            }
        }
    }

    // 특정 타입의 적 스탯을 업데이트하는 메서드
    public void UpdateEnemyStats(float maxHp, float spd, float atk, float def, float healAmount, float Radius)
    {
        for (int i = 0; i < Enemy.Count; ++i)
        {
            Enemy[i].Maxhp = maxHp;
            Enemy[i].Speed = spd;
            Enemy[i].Atk = atk;
            Enemy[i].Def = def;
            Enemy[i].Healamount = healAmount;
            Enemy[i].Radius = Radius;

            return;

        }
    }
}
