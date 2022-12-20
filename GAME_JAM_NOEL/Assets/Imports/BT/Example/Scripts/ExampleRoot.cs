using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "RootName", menuName = "ScriptableObjects/BT/AIControlledObjectName/Node/Root")]
[CreateAssetMenu(fileName = "ExampleRoot", menuName = "ScriptableObjects/BT/Example/Node/Root")]
public class ExampleRoot : BT_Root
{
    [SerializeField] private string stuffToSay;

    public override BT_Node Clone(BT_BlackBoard blackboard)
    {
        ExampleRoot clone = CreateInstance<ExampleRoot>();

        clone._blackBoard = blackboard;
        clone.child = child.Clone(blackboard);

        clone.stuffToSay = stuffToSay;

        return clone;
    }

    public override void Init()
    {
        _blackBoard.SetObject("StuffToSay", stuffToSay);
    }
}
