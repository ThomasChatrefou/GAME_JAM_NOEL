using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Selector", menuName = "ScriptableObjects/BT/Base/Node/ControlFlow/Selector")]
public class BT_Selector : BT_Composite
{
    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        var clone = CreateInstance<BT_Selector>();
        
        clone.SetBlackBoard(blackboard);
        if(_children != null) 
        {
            clone._children = new BT_Node[_children.Length];
            for (int i = 0; i < clone._children.Length; i++) 
            {
                clone._children[i] =_children[i].Clone(blackboard);
            }
        }

        return clone;
    }

    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        await base.Execute(ct);
        
        foreach (BT_Node child in _children)
        {
            Debug.Log($"{child} started");
            switch (child.Execute(ct).Result)
            {
                case BT_Status.Failure:
                    Debug.Log($"{child} failed");
                    continue;
                case BT_Status.Running:
                    _btStatus = BT_Status.Running;
                    //Debug.Log($"{child} is running");
                    return _btStatus;
                case BT_Status.Success:
                    _btStatus = BT_Status.Success;
                    //Debug.Log($"{child} successed");
                    return _btStatus;
            }
            
        }
        
        _btStatus = BT_Status.Failure;
        return _btStatus;
    }
}
