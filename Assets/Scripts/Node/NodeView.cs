using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Node node;
    public NodeView(Node node)
    {
        this.node = node;
        this.title = node.name;

        this.viewDataKey = node.guid;

        style.left = node.postion.x;
        style.top = node.postion.y;

    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.postion.x = newPos.xMin;
        node.postion.y = newPos.yMin;

    }
}
