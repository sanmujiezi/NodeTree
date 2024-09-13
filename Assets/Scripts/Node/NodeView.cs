using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public Node node;
    public Port input;
    public Port output;


    public NodeView(Node node)
    {
        this.node = node;
        this.title = node.name;

        this.viewDataKey = node.guid;

        style.left = node.postion.x;
        style.top = node.postion.y;

        CreateInputPorts();
        CreateOutputPorts();
    }

    private void CreateInputPorts()
    {
        switch (node)
        {
            case ActionNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case CompositeNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case DecoratorNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }

        if (input != null)
        {
            input.portName = "in";
            inputContainer.Add(input);
        }

    }

    private void CreateOutputPorts()
    {
        switch (node)
        {
            case CompositeNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case DecoratorNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }

        if(output != null)
        {
            output.portName = "out";
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.postion.x = newPos.xMin;
        node.postion.y = newPos.yMin;

    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }
}
