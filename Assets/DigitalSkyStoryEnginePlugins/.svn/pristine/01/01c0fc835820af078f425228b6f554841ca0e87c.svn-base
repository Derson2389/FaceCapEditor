using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class CutsceneBuilder : EditorWindow
{
    class SceneListviewItem
    {
        public bool selected;
        public Texture2D image;
        public string fullname;
        public string name;
    }

    class CharacterListviewItem
    {
        public bool selected;
        public Texture2D image;
        public string fullname;
        public string name;
        public int number;
    }

    private string mCurrentProject = string.Empty;
    private Vector2 mSceneScrollviewPos = Vector2.zero;
    private Vector2 mCharacterScrollviewPos = Vector2.zero;
    private List<SceneListviewItem> mSceneItemList = new List<SceneListviewItem>();
    private List<CharacterListviewItem> mCharacterItemList = new List<CharacterListviewItem>();
    private string mNewCutsceneName = string.Empty;

    public static CutsceneBuilder ShowWindow(string project)
    {
        CutsceneBuilder window = EditorWindow.GetWindow<CutsceneBuilder>("创建镜头");
        window.SetProject(project);
        window.Show();

        return window;
    }

    private void SetProject(string project)
    {
        mCurrentProject = project;

        List<string> scenes = CutsceneAssetsManagerUtility.GetSceneFiles(project);
        List<string> characters = CutsceneAssetsManagerUtility.GetCharacterFiles(project);

        mSceneItemList.Clear();
        mCharacterItemList.Clear();

        foreach (string s in scenes)
        {
            SceneListviewItem item = new SceneListviewItem();
            item.selected = false;
            item.fullname = s;
            item.name = System.IO.Path.GetFileNameWithoutExtension(s);
            item.image = CutsceneAssetsManagerUtility.LoadImage(item.fullname);

            mSceneItemList.Add(item);
        }

        foreach (string c in characters)
        {
            CharacterListviewItem item = new CharacterListviewItem();
            item.selected = false;
            item.fullname = c;
            item.name = System.IO.Path.GetFileNameWithoutExtension(c);
            item.image = CutsceneAssetsManagerUtility.LoadImage(item.fullname);

            mCharacterItemList.Add(item);
        }
    }

    private void OnGUI()
    {
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        float clientY = 20;
        float clientH = Screen.height - clientY;

        bool creating = false;
        float scrollviewHeight = (clientH - (22 + 22 + 32)) * 0.5f;

        GUILayout.Space(22);
        GUI.Toggle(new Rect(0, 4, Screen.width, 16), true, "", "dragtab");
        GUI.Label(new Rect(4, 4, Screen.width * 0.5f, 16), "场景");
        //mSceneSearchText = CutsceneAssetsManagerGUI.SearchField(new Rect(Screen.width - 224, 4, 200, 20), mSceneSearchText);

        mSceneScrollviewPos = EditorGUILayout.BeginScrollView(mSceneScrollviewPos, GUILayout.Height(scrollviewHeight));
        {
            int column = Mathf.Max(1, Mathf.FloorToInt((Screen.width - 16) / 182f));
            int row = mSceneItemList.Count / column;
            if (mSceneItemList.Count % column > 0)
            {
                row++;
            }

            for (int r = 0; r < row; r++)
            {
                GUILayout.Space(164);

                int cc = Mathf.Min(column, mSceneItemList.Count - (r * column));
                for (int c = 0; c < cc; c++)
                {
                    SceneListviewItem item = mSceneItemList[r * column + c];
                    float x = 2 + (c * 180);
                    float y = 2 + (r * 162);

                    if (item.image == null)
                    {
                        item.image = CutsceneAssetsManagerUtility.LoadImage(item.fullname);
                    }

                    Color backgroundColor = item.selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;
                    Color textColor = item.selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;

                    Rect itemRect = new Rect(x, y, 178, 160);
                    CutsceneAssetsManagerGUI.ListviewItem(itemRect, item.image, item.name, backgroundColor, textColor);

                    if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
                    {
                        foreach (SceneListviewItem s in mSceneItemList)
                        {
                            s.selected = false;
                        }

                        item.selected = !item.selected;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(22);
        GUI.Toggle(new Rect(0, scrollviewHeight + 24, Screen.width, 16), true, "", "dragtab");
        GUI.Label(new Rect(4, scrollviewHeight + 24, Screen.width * 0.5f, 16), "角色");
        //mCharacterSearchText = CutsceneAssetsManagerGUI.SearchField(new Rect(Screen.width - 224, (Screen.height - CutsceneAssetsManagerGUI.LISTVIEW_TITLE_HEIGHT) * 0.5f + 5, 200, 20), mCharacterSearchText);

        mCharacterScrollviewPos = EditorGUILayout.BeginScrollView(mCharacterScrollviewPos, GUILayout.Height(scrollviewHeight));
        {
            int column = Mathf.Max(1, Mathf.FloorToInt((Screen.width - 16) / 182f));
            int row = mCharacterItemList.Count / column;
            if (mCharacterItemList.Count % column > 0)
            {
                row++;
            }

            for (int r = 0; r < row; r++)
            {
                GUILayout.Space(164);

                int cc = Mathf.Min(column, mCharacterItemList.Count - (r * column));
                for (int c = 0; c < cc; c++)
                {
                    CharacterListviewItem item = mCharacterItemList[r * column + c];

                    if (item.image == null)
                    {
                        item.image = CutsceneAssetsManagerUtility.LoadImage(item.fullname);
                    }

                    float x = 2 + (c * 180);
                    float y = 2 + (r * 162);

                    Color backgroundColor = item.selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;
                    Color textColor = item.selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;

                    Rect itemRect = new Rect(x, y, 178, 160);
                    CutsceneAssetsManagerGUI.ListviewItem(itemRect, item.image, item.name, backgroundColor, textColor);
                    
                    if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
                    {
                        item.selected = !item.selected;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        float nameHeight = 32;
        EditorGUILayout.BeginHorizontal(GUILayout.Height(nameHeight));
        {
            float x = 0;
            float y = clientH - (nameHeight + 16) * 0.5f;

            GUI.Label(new Rect(x, y, 60, 16), "镜头名称：");
            x = x + 60;
            mNewCutsceneName = GUI.TextField(new Rect(x, y, Screen.width - 106, 16), mNewCutsceneName);

            GUI.enabled = !string.IsNullOrEmpty(mNewCutsceneName);
            if (GUI.Button(new Rect(Screen.width - 42, y - 1, 40, 16), "创建"))
            {
                creating = true;
            }
            GUI.enabled = true;

            GUILayout.Space(46);
        }
        EditorGUILayout.EndHorizontal();

        if (creating)
        {
            List<string> scenes = new List<string>();
            foreach (SceneListviewItem item in mSceneItemList)
            {
                if (item.selected)
                {
                    scenes.Add(item.fullname);
                }
            }

            List<CutsceneAssetsManagerUtility.CharactorInfo> characters = new List<CutsceneAssetsManagerUtility.CharactorInfo>();
            foreach (CharacterListviewItem item in mCharacterItemList)
            {
                CutsceneAssetsManagerUtility.CharactorInfo chInof = new CutsceneAssetsManagerUtility.CharactorInfo();
                chInof.charactorName = item.fullname;
                chInof.number = item.number;
                if (item.selected)
                {
                    characters.Add(chInof);
                }
            }

            this.Close();
            CutsceneAssetsManagerUtility.CreateCutscene(mCurrentProject, scenes, characters, mNewCutsceneName);
        }

        //DrawGridLines();
        Repaint();
    }

    //private void DrawGridLines()
    //{
    //    Handles.color = Color.black;

    //    float y = CutsceneAssetsManagerGUI.LISTVIEW_TITLE_HEIGHT;
    //    Handles.DrawLine(new Vector3(0, y, 0), new Vector3(Screen.width, y, 0));
    //    y = (Screen.height - CutsceneAssetsManagerGUI.LISTVIEW_TITLE_HEIGHT) * 0.5f;
    //    Handles.DrawLine(new Vector3(0, y, 0), new Vector3(Screen.width, y, 0));
    //    y = y + CutsceneAssetsManagerGUI.LISTVIEW_TITLE_HEIGHT;
    //    Handles.DrawLine(new Vector3(0, y, 0), new Vector3(Screen.width, y, 0));
    //    y = Screen.height - CutsceneAssetsManagerGUI.LISTVIEW_TITLE_HEIGHT;
    //    Handles.DrawLine(new Vector3(0, y, 0), new Vector3(Screen.width, y, 0));

    //    Handles.color = Color.white;
    //}

    private void OnLostFocus()
    {
        this.Close();
    }
}
