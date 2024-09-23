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
    public Blackborad blackboard = new Blackborad();

    public Node.State Update()
    {
        if (rootNode.state == Node.State.Running)
        {
            treeState = rootNode.Update();
        }
        return treeState;
    }

#if UNITY_EDITOR
    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

        AssetDatabase.SaveAssets();
        return node;

    }

    public void DestroyNode(Node node)
    {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        switch (parent)
        {
            case DecoratorNode:
                var tempParent_D = parent as DecoratorNode;
                Undo.RecordObject(tempParent_D, "Behaviour Tree(AddChild)");
                tempParent_D.child = child;
                EditorUtility.SetDirty(tempParent_D);
                break;
            case CompositeNode:
                var tempParent_C = parent as CompositeNode;
                Undo.RecordObject(tempParent_C, "Behaviour Tree(AddChild)");
                tempParent_C.children.Add(child);
                EditorUtility.SetDirty(tempParent_C);
                break;
            case RootNode:
                var tempParent_R = parent as RootNode;
                Undo.RecordObject(tempParent_R, "Behaviour Tree(AddChild)");
                tempParent_R.child = child;
                EditorUtility.SetDirty(tempParent_R);
                break;


        }
        AssetDatabase.SaveAssets();
    }
    public void RemoveChild(Node parent, Node child)
    {
        switch (parent)
        {
            case DecoratorNode:
                var tempParent_D = parent as DecoratorNode;
                Undo.RecordObject(tempParent_D, "Behaviour Tree(AddChild)");
                tempParent_D.child = null;
                EditorUtility.SetDirty(tempParent_D);
                break;
            case CompositeNode:
                var tempParent_C = parent as CompositeNode;
                Undo.RecordObject(tempParent_C, "Behaviour Tree(AddChild)");
                tempParent_C.children.Remove(child);
                EditorUtility.SetDirty(tempParent_C);
                break;
            case RootNode:
                var tempParent_R = parent as RootNode;
                Undo.RecordObject(tempParent_R, "Behaviour Tree(AddChild)");
                tempParent_R.child = null;
                EditorUtility.SetDirty(tempParent_R);
                break;
        }
        AssetDatabase.SaveAssets();
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
#endif

    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }

    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        tree.nodes = new List<Node>();

        Traverse(tree.rootNode, (n) =>
        {
            tree.nodes.Add(n);
        });

        return tree;
    }

    public void Bind(AiAgent aiAgent)
    {
        Traverse(rootNode, node =>
        {
            node.agent = aiAgent;
            node.blackboard = blackboard;
        });
    }

}
