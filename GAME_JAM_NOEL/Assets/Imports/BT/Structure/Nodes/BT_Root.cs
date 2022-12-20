using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//[CreateAssetMenu(fileName = "RootName", menuName = "ScriptableObjects/BT/ObjectName/Node/Root")]
public abstract class BT_Root : BT_Node
{
    [SerializeField] protected BT_Node child;

    public override void Attach(BT_BlackBoard blackBoard)
    {
        base.Attach(blackBoard);
        
        child?.Attach(blackBoard);
    }

    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        await base.Execute(ct);
        Debug.Log($"{child} started");
        return await child.Execute(ct);
    }
    
    public abstract void Init(); 
}
