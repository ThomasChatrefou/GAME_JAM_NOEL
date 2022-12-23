using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "UpdateTargetTask", menuName = "ScriptableObjects/BT/BaseEnemy/Node/Task/UpdateTargetTask")]
public class UpdateTargetTask : BT_Task
{
    public override async Task<BT_Status> Execute(CancellationToken ct)
    {
		await base.Execute(ct);

        Debug.Log("locking target");
        _blackBoard.SetObject("Target", null);

        Enemy self = (_blackBoard.GetObject("GameObject") as GameObject)?.GetComponent<Enemy>();
        if (self == null) return BT_Status.Failure;

        List<PlayerController> players = _blackBoard.GetObject("Players") as List<PlayerController>;
        if (players == null || players.Count == 0) return BT_Status.Failure;

        float minDist = Mathf.Infinity;
        PlayerController closestPlayer = null;

        foreach (PlayerController player in players)
        {
            if (player == null || player.IsDead)
                continue;

            float currentDist = Vector3.Distance(player.transform.position, self.transform.position);

            if (minDist <= currentDist)
                continue;

            minDist = currentDist;
            closestPlayer = player;
        }

        if (closestPlayer == null)
            return BT_Status.Failure;

        _blackBoard.SetObject("Target", closestPlayer.gameObject);

        return BT_Status.Success;
	}

    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        UpdateTargetTask clone = CreateInstance<UpdateTargetTask>();
        clone._blackBoard = blackboard;
        return clone;
    }
}
