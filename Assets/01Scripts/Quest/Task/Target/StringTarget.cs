using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Quest/Task/Target/String", fileName = "Target_")]
public class StringTarget : TaskTarget
{
    [SerializeField]
    private string value;

    public override object Value => value;

    // 내가 설정한 value가 Slime 이라는 문자열이고 
    // 들어온 target이 Slime이라는 문자열이라면 true가 리턴되어 
    // 이 Task가 목표로 하는 Target이 맞다는 것을 알려주는 것
    public override bool IsEqual(object target)
    {
        string targetAsString = target as string;
        if (targetAsString == null)
            return false;
        
        return value == targetAsString;
    }
}
