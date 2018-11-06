using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(FaceControllerSet))]
public class FaceControllerSetEdit : Editor
{
#pragma warning disable 618

    private FaceControllerSet lsTarget;
    private SerializedObject serializedTarget;

    void OnEnable()
    {
        serializedTarget = new SerializedObject(target);
        lsTarget = (FaceControllerSet)target;
        
    }

    void OnDisable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        if (serializedTarget == null)
        {
            OnEnable();
        }

        serializedTarget.Update();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("ddd"))
        {
            FaceControllerSet.FaceController aa = new FaceControllerSet.FaceController();
            aa.ctrName = "addddd";
            lsTarget.ctrls.Add(aa);
            EditorUtility.SetDirty(lsTarget);
            serializedTarget.SetIsDifferentCacheDirty();
        }
        for (int i = 0; i < lsTarget.ctrls.Count; i++)
        {
            GUILayout.Label(lsTarget.ctrls[i].ctrName);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        serializedTarget.ApplyModifiedProperties();
    }

    
}
