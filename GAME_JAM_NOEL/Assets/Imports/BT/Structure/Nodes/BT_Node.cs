using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BT_Node : ScriptableObject
{
    protected BT_Status _btStatus;
    protected BT_BlackBoard _blackBoard;

    public virtual void Attach(BT_BlackBoard blackBoard) => _blackBoard = blackBoard;

    public virtual async Task<BT_Status> Execute(CancellationToken ct)
    {
        _btStatus = BT_Status.Running;
        return _btStatus;
    }

    public async Task<BT_Status> Evaluate() => _btStatus;

    public abstract BT_Node Clone(BT_BlackBoard blackboard);
    
    public virtual void SetBlackBoard(BT_BlackBoard blackBoard) 
    {
        this._blackBoard = blackBoard;
    }
}
