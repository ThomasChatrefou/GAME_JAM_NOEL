using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Repeat", menuName = "ScriptableObjects/BT/Base/Node/Decorator/Repeat/RepeatForSeconds")]
public class RepeatForSeconds : BT_Repeat
{
    [SerializeField] private float _timePassed = 0f;
    [SerializeField] private float _repeatTime;

    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        var clone = CreateInstance<RepeatForSeconds>();
        clone.SetBlackBoard(blackboard);
        clone.SetTimers(_timePassed, _repeatTime);
        return clone;
    }
    public void SetTimers(float timePassed , float repeatTime) 
    {
        _timePassed = timePassed;
        _repeatTime = repeatTime;
    }

    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
        BT_Status result = await base.Execute(ct);

        _timePassed = 0f;

        return result;
    }

    protected override bool ConditionTrue()
    {
        _timePassed += Time.deltaTime;

        return _timePassed >= _repeatTime;
    }
}
