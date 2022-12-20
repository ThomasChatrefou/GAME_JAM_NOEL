using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BT_Decorator : BT_Node
{
    [SerializeField] protected BT_Node child;

    public override void Attach(BT_BlackBoard blackBoard)
    {
        base.Attach(blackBoard);
        
        child?.Attach(blackBoard);
    }
}
