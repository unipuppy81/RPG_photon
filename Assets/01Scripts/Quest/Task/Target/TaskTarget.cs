using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class TaskTarget : ScriptableObject
{
    /// <summary>
    /// 실제 Target의 자료형은 이 class를 상속받는 자식에서 구현할 것이므로 object 타입
    /// </summary>
    public abstract object Value { get; }

    /// <summary>
    /// QuestSystem에 보고된 Target이 Task에 설정한 Target과 같은지 확인하는 함수
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool isEqual(object target);
}
