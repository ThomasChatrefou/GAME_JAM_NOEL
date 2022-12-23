using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseRoot", menuName = "ScriptableObjects/BT/BaseEnemy/Node/Root")]
public class BaseRoot : BT_Root
{
    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        BaseRoot clone = CreateInstance<BaseRoot>();

        clone._blackBoard = blackboard;
        clone.child = child.Clone(blackboard);

        return clone;
    }

    public override void Init()
    {
        _blackBoard.SetObject("Players", new List<PlayerController>(GameManager.Instance.PlayerList));
    }
}