using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CutsceneBrower : EditorWindow 
{
    private string mCurrentProjectName = string.Empty;
    private List<CutsceneClip> mCutsceneClips = new List<CutsceneClip>();
    private CutsceneClip mCurrentCutsceneClip = null;
    private Vector2 mScrollviewPos = Vector2.zero;

    private System.Action<CutsceneClip> mAction;

    public static CutsceneBrower ShowWindow(string projectName, System.Action<CutsceneClip> callback)
    {
        CutsceneBrower window = EditorWindow.GetWindow<CutsceneBrower>("镜头");

        window.mAction = callback;
        window.SetProject(projectName);
        window.Show();

        return window;
    }

    private void SetProject(string projectName)
    {
        mCurrentProjectName = projectName;

        List<string> cutscenes = CutsceneAssetsManagerUtility.GetCutsceneFiles(mCurrentProjectName);
        foreach (string cs in cutscenes)
        {
            CutsceneClip clip = new CutsceneClip();
            clip.fullname = cs;
            clip.name = Path.GetFileNameWithoutExtension(cs);
            clip.image = CutsceneAssetsManagerUtility.LoadImage(clip.fullname);

            mCutsceneClips.Add(clip);
        }

        mScrollviewPos = Vector2.zero;
    }

    private void OnGUI()
    {
        float width = Screen.width;
        float height = Screen.height;

        mScrollviewPos = EditorGUILayout.BeginScrollView(mScrollviewPos, GUILayout.ExpandHeight(true));
        {
            GUILayout.Space(4);

            int columns = Mathf.Max(1, Mathf.FloorToInt((width - 16) / 182f));
            
            int rows = mCutsceneClips.Count / columns;
            int dummyRowPlus = rows + 1;
            if (dummyRowPlus % columns > 0)
            {
                rows++;
            }

            for (int r = 0; r < rows; r++)
            {
                float y = 4 + r * 162;

                int lc = Mathf.Min(columns, dummyRowPlus - (r * columns));
                for (int c = 0; c < lc; c++)
                {
                    float x = 2 + c * 182;
                    int tempInt = r * columns + c;
                    if (tempInt > mCutsceneClips.Count)
                    {
                        //Color backgroundColor = selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;
                        //Color textColor = selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;
                        //Rect itemRect = new Rect(x, y, 180, 160);
                        //CutsceneAssetsManagerGUI.ListviewItem(itemRect, null, "", backgroundColor, textColor);
                    }
                    else
                    {
                        CutsceneClip cc = mCutsceneClips[tempInt];

                        bool selected = (mCurrentCutsceneClip == cc);
                        Color backgroundColor = selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;
                        Color textColor = selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;

                        Rect itemRect = new Rect(x, y, 180, 160);
                        CutsceneAssetsManagerGUI.ListviewItem(itemRect, cc.image, cc.name, backgroundColor, textColor);

                        if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
                        {
                            Event.current.Use();
                            mCurrentCutsceneClip = cc;

                            if (Event.current.clickCount >= 2)
                            {
                                if (mAction != null)
                                {
                                    mAction(mCurrentCutsceneClip);
                                }

                                this.Close();
                            }
                        }
                    }
                }

                GUILayout.Space(162);
            }

            GUILayout.Space(2);
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnLostFocus()
    {
        this.Close();
    }
}
