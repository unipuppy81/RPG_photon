using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskAction : ScriptableObject
{
    /// <summary>
    /// successCount : 들어오는 값
    /// </summary>
    /// <param name="task"></param>
    /// <param name="currentSuccess"></param>
    /// <param name="successCount"></param>
    /// <returns></returns>
    public abstract int Run(Task task, int currentSuccess, int successCount);
}
