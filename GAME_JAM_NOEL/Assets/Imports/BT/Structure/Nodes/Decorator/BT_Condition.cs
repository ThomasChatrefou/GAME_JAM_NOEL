using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BT_Condition : BT_Node
{
	public override async Task<BT_Status> Execute(CancellationToken ct)
	{
		await base.Execute(ct);
		
		if (ConditionTrue())
			return BT_Status.Success;

		return BT_Status.Failure;
	}

	protected abstract bool ConditionTrue();
}
