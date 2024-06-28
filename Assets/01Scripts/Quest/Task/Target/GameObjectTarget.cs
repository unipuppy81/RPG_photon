using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/GameObject", fileName = "Target_")]
public class GameObjectTarget : TaskTarget
{
    [SerializeField]
    private GameObject value;

    public override object Value => value;


    // value에 들어갈 GameObject는 prefab인데 
    // isEqual의 인자로 들어오는 값은 프리팹일수도 있고 게임씬에 존재하는 오브젝트일수도 있음
    // Equal 연산시 같은 오브젝트지만 prefab 유무로 false 반환할수도 있음
    public override bool IsEqual(object target)
    {
        var targetAsGameObject = target as GameObject;
        if (targetAsGameObject == null)
            return false;

        return targetAsGameObject.name.Contains(value.name);
    }
}
