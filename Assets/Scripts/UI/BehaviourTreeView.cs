using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    BehaviourTree tree;

    public BehaviourTreeView()
    {

        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UI/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endport => endport.direction != startPort.direction && endport.node != startPort.node).ToList();
    }

    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        //Create node view
        tree.nodes.ForEach(n => CreateNodeView(n));

        //Create edges
        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(child =>
            {
                if (child != null)
                {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);

                }

            });


        });

    }

    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach((elem) =>
            {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    tree.DestroyNode(nodeView.node);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach((edge) =>
            {
                NodeView parentNode = edge.output.node as NodeView;
                NodeView childNode = edge.input.node as NodeView;
                tree.AddChild(parentNode.node, childNode.node);

            });
        }



        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }



        {
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Delete", (a) => DeleteSelectionNode());
        }
    }

    private void DeleteSelectionNode()
    {

        if (selection.Count > 0)
        {
            var nodeviews = new List<ISelectable>(selection);

            for (int i = 0; i < nodeviews.Count; i++)
            {
                NodeView nodeView = nodeviews[i] as NodeView;
                if (nodeView != null)
                {
                    RemoveElement(nodeView);
                    tree.DestroyNode(nodeView.node);
                }


                Edge edge = nodeviews[i] as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                    RemoveElement(edge);
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                }
            }
        }
    }

    private void CreateNode(System.Type type)
    {
        Node node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    private void CreateNodeView(Node n)
    {
        NodeView nodeView = new NodeView(n);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }


}
