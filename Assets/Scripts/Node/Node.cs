using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }

    public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 postion;

    public State Update()
    {
        if (!started)
        {
            Onstart();
            started = true;
        }

        state = OnUpdate();

        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }

        return state;
    }

    protected abstract void Onstart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
