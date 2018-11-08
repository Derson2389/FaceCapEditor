using UnityEngine;
using UnityEditor;

public class CutsceneClone : EditorWindow
{
    private string mCloneNameText = string.Empty;
    private string mPath = string.Empty;

    private System.Action<string, string> mAction;

    public static CutsceneClone ShowWindow(string path, string orignalName, System.Action<string, string> callback)
    {
        CutsceneClone window = EditorWindow.GetWindow<CutsceneClone>(true, "克隆");
        window.mAction = callback;
        window.minSize = window.maxSize = new Vector2(320, 150);
        window.Show();
        window.mCloneNameText = orignalName+"_Clone";
        window.mPath = path;
        return window;
    }

    private void OnGUI()
    {
        GUILayout.Space(24);
        GUILayout.Label("克隆项目名称：");

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical();
            mCloneNameText = EditorGUILayout.TextField(mCloneNameText);
            EditorGUILayout.Space();
            GUI.enabled = !string.IsNullOrEmpty(mCloneNameText) && !string.IsNullOrEmpty(mCloneNameText);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("确定", EditorStyles.miniButton, GUILayout.Width(48)))
            {
                if (mAction != null)
                {
                    mAction(mPath, mCloneNameText);
                }
                if (this != null)
                {
                    this.Close();
                }              
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnLostFocus()
    {
        this.Close();
    }
}
