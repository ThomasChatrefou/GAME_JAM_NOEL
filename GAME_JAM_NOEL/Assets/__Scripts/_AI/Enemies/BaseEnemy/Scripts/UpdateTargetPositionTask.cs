using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "UpdateTargetPositionTask", menuName = "ScriptableObjects/BT/BaseEnemy/Node/Task/UpdateTargetPositionTask")]
public class UpdateTargetPositionTask : BT_Task
{
    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        await base.Execute(ct);

        GameObject targetGO = _blackBoard.GetObject("Target") as GameObject;
        if (targetGO == null) return BT_Status.Failure;

        PlayerController target = targetGO.GetComponent<PlayerController>();
        if (target == null || target.IsDead) return BT_Status.Failure;

        Transform targetTransform = targetGO.transform;

        GameObject go = _blackBoard.GetObject("GameObject") as GameObject;
        if (go == null) return BT_Status.Failure;

        Enemy self = go.GetComponent<Enemy>();
        if (self == null) return BT_Status.Failure;

        self.SetTarget(targetTransform);
        return BT_Status.Success;
    }

    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        UpdateTargetPositionTask clone = CreateInstance<UpdateTargetPositionTask>();
        clone._blackBoard = blackboard;
        return clone;
    }
}
