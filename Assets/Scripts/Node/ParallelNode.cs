using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ParallelNode : CompositeNode
{
    
    protected override void Onstart()
    {
        
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        int successCount = 0;

        foreach(var item in children)
        {
            var state = item.Update();
            if(state == State.Failure)
            {
                return State.Failure;
            }else if (state == State.Success)
            {
                successCount++;
            }else if(state == State.Running)
            {
                continue;
            }
        }

        return  successCount == children.Count ? State.Success : State.Failure;
    }

    
}
   

