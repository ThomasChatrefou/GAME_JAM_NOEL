using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RusherRoot", menuName = "ScriptableObjects/BT/Rusher/Node/Root")]
public class RusherRoot : BT_Root
{
    [SerializeField] private float speed;

    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        RusherRoot clone = CreateInstance<RusherRoot>();

        clone._blackBoard = blackboard;
        clone.child = child.Clone(blackboard);

        clone.speed = speed;

        return clone;
    }

    public override void Init()
    {
        _blackBoard.SetFloat("Speed", speed);
    }
}
