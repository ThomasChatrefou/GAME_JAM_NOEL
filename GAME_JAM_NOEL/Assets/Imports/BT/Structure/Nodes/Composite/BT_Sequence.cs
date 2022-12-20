using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Sequence", menuName = "ScriptableObjects/BT/Base/Node/ControlFlow/Sequence")]
public class BT_Sequence : BT_Composite
{
    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        var clone = CreateInstance<BT_Sequence>();
        clone.SetBlackBoard( blackboard);
        clone.SetBlackBoard(blackboard);
        if (_children != null)
        {
            clone._children = new BT_Node[_children.Length];
            for (int i = 0; i < clone._children.Length; i++)
            {
                clone._children[i] = _children[i].Clone(blackboard);
            }

        }

        return clone;
    }

    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        await base.Execute(ct);
        
        bool anyChildIsRunning = false;

        foreach (BT_Node child in _children)
        {
            Debug.Log($"{child} started");
            switch (child.Execute(ct).Result)
            {
                case BT_Status.Failure:
                    _btStatus = BT_Status.Failure;
                    Debug.Log($"{child} failed");
                    return _btStatus;
                case BT_Status.Success:
                    //Debug.Log($"{child} successed");
                    continue;
                case BT_Status.Running:
                    anyChildIsRunning = true;
                    //Debug.Log($"{child} is running");
                    continue;
            }
        }

        _btStatus = anyChildIsRunning ? BT_Status.Running : BT_Status.Success;
        return _btStatus;
    }
}
