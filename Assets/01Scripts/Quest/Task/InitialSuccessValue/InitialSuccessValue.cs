using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Level40 을찍어야 한다거나, 힘 스탯이 일정 수준이 되어야하는
/// Task들 설정해주는 것
/// </summary>
public abstract class InitialSuccessValue : ScriptableObject
{
    public abstract int GetValue(Task task);
}
