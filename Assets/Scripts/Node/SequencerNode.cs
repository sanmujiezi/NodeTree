using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    int current;
    protected override void Onstart()
    {
        current = 0;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        var child = children[current];
        switch (child.Update())
        {
            case State.Success:
                current++;
                break;
            case State.Failure:
                return State.Failure;
            case State.Running:
                return State.Running;
        }

        return current == children.Count ? State.Success : State.Running;

    }

    public override Node Clone()
    {
        SequencerNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}
