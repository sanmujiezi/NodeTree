using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspeactorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspeactorView, VisualElement.UxmlTraits> { }

    Editor editor;

    public InspeactorView() { }

    public void UpdateSelection(NodeView nodeview)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeview.node);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        Add(container);
    }
}
