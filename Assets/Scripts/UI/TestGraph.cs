using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TestGraph : GraphView
{
    public new class UxmlFactory : UxmlFactory<TestGraph, GraphView.UxmlTraits> { }
}
