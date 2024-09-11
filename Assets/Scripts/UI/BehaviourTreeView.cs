using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    BehaviourTree tree;

    public BehaviourTreeView()
    {

        Insert(0,new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UI/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        DeleteElements(graphElements);
        tree.nodes.ForEach(n => CreateNodeView(n));

    }

    


    private void CreateNodeView(Node n)
    {
        NodeView nodeView = new NodeView(n);
        AddElement(nodeView);
    }
}
