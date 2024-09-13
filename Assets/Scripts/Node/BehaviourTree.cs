using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;
    public List<Node> nodes = new List<Node>();

    public Node.State Update()
    {
        if (rootNode.state == Node.State.Running)
        {
            treeState = rootNode.Update();
        }
        return treeState;
    }

    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();

        return node;

    }

    public void DestroyNode(Node node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        switch (parent)
        {
            case DecoratorNode:
                var tempParent_D = parent as DecoratorNode;
                tempParent_D.child = child;
                break;
            case CompositeNode:
                var tempParent_C = parent as CompositeNode;
                tempParent_C.children.Add(child);
                break;
            case RootNode:
                var tempParent_R = parent as RootNode;
                tempParent_R.child = child;
                break;
        }
    }
    public void RemoveChild(Node parent, Node child)
    {
        switch (parent)
        {
            case DecoratorNode:
                var tempParent_D = parent as DecoratorNode;
                tempParent_D.child = null;
                break;
            case CompositeNode:
                var tempParent_C = parent as CompositeNode;
                tempParent_C.children.Remove(child);
                break;
            case RootNode:
                var tempParent_R = parent as RootNode;
                tempParent_R.child = null;
                break;
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        switch (parent)
        {
            case DecoratorNode:
                var tempParent_D = parent as DecoratorNode;
                children.Add(tempParent_D.child);
                break;
            case CompositeNode:
                var tempParent_C = parent as CompositeNode;
                return tempParent_C.children;
            case RootNode:
                var tempParent_R = parent as RootNode;
                children.Add(tempParent_R.child);
                break;
        }
        return children;
    }

}
