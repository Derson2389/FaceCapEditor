using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SphereColliderChecker), true)]
public class SphereColliderCheckerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("复制给所有子节点"))
        {
            SphereColliderChecker checker = (SphereColliderChecker)target;
            CopyToChildren(checker.transform);
        }
    }

    private void CopyToChildren(Transform parent)
    {
        SphereColliderChecker checker = parent.GetComponent<SphereColliderChecker>();
        if(checker == null)
        {
            checker = parent.gameObject.AddComponent<SphereColliderChecker>();
        }

        checker.Radius = ((SphereColliderChecker)target).Radius;

        for (int i=0; i<parent.childCount; i++)
        {
            CopyToChildren(parent.GetChild(i));
        }
    }
}
