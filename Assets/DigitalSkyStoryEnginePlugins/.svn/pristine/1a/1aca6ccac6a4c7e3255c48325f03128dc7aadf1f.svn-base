using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LinkColliderChecker), true)]
public class LinkColliderCheckerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("搜索所有子节点"))
        {
            LinkColliderChecker checker = (LinkColliderChecker)target;
            checker.LinkedCheckerList = checker.GetComponentsInChildren<SphereColliderChecker>();
            Repaint();
        }
    }

}
