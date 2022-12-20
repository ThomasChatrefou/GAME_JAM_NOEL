using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BT_Parallele : BT_Composite
{
    [SerializeField] private BT_Task _task;

    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        var clone = CreateInstance<BT_Parallele>();
        clone.SetBlackBoard(blackboard);
        clone._task = _task;
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
        BT_Status status = await _task.Execute(ct);
        
        //List<BT_Status>

        //foreach (BT_Node child in _children)
        
        return status;
    }
}