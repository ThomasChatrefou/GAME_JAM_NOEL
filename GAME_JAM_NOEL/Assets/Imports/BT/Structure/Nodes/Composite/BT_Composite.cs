using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BT_Composite : BT_Node
{
    [SerializeField] protected BT_Node[] _children;

    public override void Attach(BT_BlackBoard blackBoard)
    {
        base.Attach(blackBoard);
        for (int i = 0; i < _children.Length; i++)
        {
            if (_children[i] == null)
                continue;
            _children[i] = Instantiate(_children[i]);
            _children[i]?.Attach(blackBoard);
        }
    }
    
}
