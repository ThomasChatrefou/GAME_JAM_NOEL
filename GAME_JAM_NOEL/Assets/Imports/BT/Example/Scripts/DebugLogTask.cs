using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//[CreateAssetMenu(fileName = "TaskName", menuName = "ScriptableObjects/BT/AIControlledObjectName/Node/Task/TaskName")]
[CreateAssetMenu(fileName = "DebugLogTask", menuName = "ScriptableObjects/BT/Example/Node/Task/DebugLogTask")]
public class DebugLogTask : BT_Task
{
    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        Debug.Log((string)_blackBoard.GetObject("StuffToSay"));
        return BT_Status.Success;
    }

    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        DebugLogTask clone = CreateInstance<DebugLogTask>();
        clone._blackBoard = blackboard;
        return clone;
    }
}
