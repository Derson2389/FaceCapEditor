using UnityEngine;
using UnityEditor;

public class CutsceneGroupNameInput : EditorWindow
{
    private string mInputText = string.Empty;
    private string mActDesc = string.Empty;///场次描述
    private System.Action<string, string> mAction;
    private int mActStep = 9;
    private string prefixPool = "ABCDEFGHIJKLMNOPQRSTUVWXZ";
    private string mActNamerPrefix = string.Empty;

    public static CutsceneGroupNameInput ShowWindow(System.Action<string, string> callback, int ActCount)
    {
        CutsceneGroupNameInput window = EditorWindow.GetWindow<CutsceneGroupNameInput>(true, "场次名称");
        int prefixDex = ActCount % window.mActStep;
        int prefixIdx = ActCount / window.mActStep;

        if (prefixIdx < 26)
        {
            var str_arr = window.prefixPool.ToCharArray();
            window.mActNamerPrefix = str_arr[prefixIdx].ToString();
            window.mInputText = string.Format(window.mActNamerPrefix + "{0:D2}",  (1+ prefixDex)*10);
        }
        else
        {
            window.mInputText = string.Empty;
        }
        
        window.mAction = callback;
        window.minSize = window.maxSize = new Vector2(320, 120);
        window.Show();

        return window;
    }

    private void OnGUI()
    {
        GUILayout.Space(14);
        GUILayout.Label("请输入场次名称：");
        EditorGUILayout.BeginVertical();
        mInputText = EditorGUILayout.TextField(mInputText);
        mActDesc = EditorGUILayout.TextArea(mActDesc, GUILayout.Height(40));
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUI.enabled = !string.IsNullOrEmpty(mInputText) && !string.IsNullOrEmpty(mActDesc);
            if (GUILayout.Button("确定", EditorStyles.miniButton, GUILayout.Width(48)))
            {
                if (mAction != null)
                {
                    mAction(mInputText, mActDesc);
                }
                this.Close();
            }
            GUI.enabled = true;
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void OnLostFocus()
    {
        this.Close();
    }
}
