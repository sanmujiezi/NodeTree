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

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();

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
                    EditorUtility.SetDirty(parentView.node);

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

        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                NodeView view = n as NodeView;
                view.SortChildren();
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
                    RemoveEdges(nodeView);
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

    private void RemoveEdges(NodeView nodeView)
    {
        List<Edge> remove_edges = new List<Edge>();
        edges.ForEach(edge =>
        {
            if (edge.input.node == nodeView || edge.output.node == nodeView)
            {
                remove_edges.Add(edge);
            }
        });

        for (int i = 0; i < remove_edges.Count; i++)
        {
            NodeView parentView = remove_edges[i].output.node as NodeView;
            NodeView childView = remove_edges[i].input.node as NodeView;
            tree.RemoveChild(parentView.node, childView.node);
            remove_edges[i].input.Disconnect(remove_edges[i]);
            remove_edges[i].output.Disconnect(remove_edges[i]);
            RemoveElement(remove_edges[i]);

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

    public void updateNodeStates()
    {
        nodes.ForEach(n =>
        {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }


}
