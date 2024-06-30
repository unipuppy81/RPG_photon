using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/ContinousCount", fileName = "Continous Count")]
public class ContinousCount : TaskAction
{
    /// <summary>
    /// 양수가 들어오면 들어온 값 더하고, 음수가 들어오면 초기화
    /// </summary>
    /// <param name="task"></param>
    /// <param name="currentSuccess"></param>
    /// <param name="successCount"></param>
    /// <returns></returns>
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : 0;
    }
}
