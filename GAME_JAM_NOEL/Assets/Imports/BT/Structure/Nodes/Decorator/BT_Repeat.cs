using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BT_Repeat : BT_Decorator
{
    [SerializeField] protected int delay;
    
    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        await base.Execute(ct);
        
        while (!ConditionTrue())
        {
            _btStatus = await child.Execute(ct);
        }

        return _btStatus;
    }

    protected abstract bool ConditionTrue();
}
