using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Invert", menuName = "ScriptableObjects/BT/Base/Node/Decorator/Invert")]
public class BT_Invert : BT_Decorator
{
    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        var clone = CreateInstance<BT_Invert>();
        clone.SetBlackBoard(blackboard);
        if (child != null)
        {
            clone.child = child.Clone(blackboard);
        }
        return clone;
    }

    internal void AddChild(BT_Node child)
    {
        throw new NotImplementedException();
    }

    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        switch (await child.Execute(ct))
        {
            case BT_Status.Failure:
                return BT_Status.Success;
            case BT_Status.Running:
                return BT_Status.Running;
            case BT_Status.Success:
                return BT_Status.Failure;
        }

        return BT_Status.Failure;
    }
}
