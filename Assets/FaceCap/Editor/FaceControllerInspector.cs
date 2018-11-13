using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(FaceControllerComponent))]
public class FaceControllerInspector : Editor
{
   
    Rect rect;
    public override void OnInspectorGUI()
    {
        FaceControllerComponent Comp = (FaceControllerComponent)target;

        base.DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Config:", GUILayout.MaxWidth(90));
        //rect = EditorGUILayout.GetControlRect();
        //BlenderShapesManager.ConfigTxt = EditorGUI.tex(rect, BlenderShapesManager.ConfigTxt);
        //GUILayout.Space(20);
        //if ((Event.current.type == EventType.DragUpdated
        //  || Event.current.type == EventType.DragExited)
        //  && rect.Contains(Event.current.mousePosition))
        //{
        //    //改变鼠标的外表  
        //    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        //    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
        //    {
        //        BlenderShapesManager.ConfigTxt = DragAndDrop.paths[0];
        //    }
        //}


        //if (GUILayout.Button("加载配置"))
        //{
        //    BlenderShapesManager.LoadConfig((FaceControllerComponent)target);
        //    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //}

        if (GUILayout.Button("Bake To Animation"))
        {

            Debug.LogWarning("Bake To Animation To Impl Later");
            
        }

        //if (GUILayout.Button("增加控制器"))
        //{
        //    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //}

        GUILayout.EndHorizontal();   
    }
}
