using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{

    BehaviourTree tree;

    // Start is called before the first frame update
    void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var parallel = ScriptableObject.CreateInstance<ParallelNode>();
        
        var log = ScriptableObject.CreateInstance<DebugLogNode>();
        log.message = "我好困啊！！！！";

        WaitNode wait1 = ScriptableObject.CreateInstance<WaitNode>();

        var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
        log1.message = "我好累啊！！！！";

        WaitNode wait2 = ScriptableObject.CreateInstance<WaitNode>();

        var log2 = ScriptableObject.CreateInstance<DebugLogNode>();
        log2.message = "我好饿啊！！！！";

        WaitNode wait3 = ScriptableObject.CreateInstance<WaitNode>();

        parallel.children.Add(log);
        parallel.children.Add(wait1);
        parallel.children.Add(log1);
        parallel.children.Add(wait2);
        parallel.children.Add(log2);
        parallel.children.Add(wait3);

        var loop = ScriptableObject.CreateInstance<RepeatNode>();
        loop.child = parallel;

        tree.rootNode = parallel;
    }

    // Update is called once per frame
    void Update()
    {
       tree.Update();
    }
}
