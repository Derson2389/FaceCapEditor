using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

public class CutsceneAssetsManager : EditorWindow
{
    private string[] mProjectArray = null;
    private int mCurrentProjectIndex = -1;
    private string mCurrentProjectName = string.Empty;
    private List<CinemaClip> mCinemaClips = new List<CinemaClip>();
    private int mCurrentCinemaClipIndex = -1;
    private Vector2 mScrollviewPos = Vector2.zero;

    [MenuItem("剧情工具/项目管理", false, 1)]
    private static void OpenNewProject()
    {
        ProjectListView window = EditorWindow.GetWindow<ProjectListView>(false, "项目列表", true); 
        window.Show();
    }

    private void OnEnable()
    {
        CutsceneAssetsManagerStyles.Init();

        List<string> projects = CutsceneAssetsManagerUtility.GetProjects();
        mProjectArray = new string[projects.Count];
        for (int i = 0; i < projects.Count; i++)
        {
            mProjectArray[i] = System.IO.Path.GetFileNameWithoutExtension(projects[i]);
        }

        if (mCurrentProjectIndex < 0 && mProjectArray.Length > 0)
        {
            mCurrentProjectIndex = 0;
        }
        OnProjectChanged();
    }

    private void OnDestroy()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            StopPlay();
        }
    }

    private void OnGUI()
    {
        Toolbar();
        CutsceneGroups();

        Repaint();
    }

    public void Update()
    {
        if (EditorApplication.isPlaying)
        {
           Slate.Cutscene.OnCutsceneStopped += OnCutsceneStoppedHandler;
        }
    }

    private void Toolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        {
            if (GUILayout.Button("新建项目", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                CutsceneCreateProje.ShowWindow(OnCreateProjectCallBack); 
            }

            EditorGUI.BeginChangeCheck();
            int index = EditorGUILayout.Popup(mCurrentProjectIndex, mProjectArray, EditorStyles.toolbarPopup);
            if (EditorGUI.EndChangeCheck())
            {
                mCurrentProjectIndex = index;
                OnProjectChanged();
            }

            if (GUILayout.Button("新建场", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                CutsceneGroupNameInput.ShowWindow(OnCutsceneGroupNameInput, 0);
            }

            if (GUILayout.Button("新建镜头", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                CutsceneBuilder.ShowWindow(mCurrentProjectName);
            }

            if (!EditorApplication.isPlaying)
            {
                if (GUILayout.Button("播放", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    StartPlay();
                }
            }
            else
            {
                if (GUILayout.Button("停止", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    StopPlay();
                }
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnCutsceneGroupNameInput(string text, string desc = null)
    {
        CinemaClip clip = CreateCinemaClip(text);
        clip.desc = desc;
        mCurrentCinemaClipIndex = mCinemaClips.Count;
        mCinemaClips.Add(clip);
    }

    private void OnCreateProjectCallBack(string project, string chargerName)
    {
        ///Debug.LogError("project: " + project + "chargerName: " + chargerName);
        string projectPath = Path.Combine(CutsceneAssetsManagerUtility.workspace, project);
        if (!Directory.Exists(projectPath))
        {
            Directory.CreateDirectory(projectPath);
        }

    }

    private void CutsceneGroups()
    {
        if (EditorApplication.isPlaying)
        {
            if (mCinemaClips.Count == 0)
            {
                OnProjectChanged();
            }

            Slate.Cutscene.OnCutsceneStopped += OnCutsceneStoppedHandler;
        }

        mScrollviewPos = EditorGUILayout.BeginScrollView(mScrollviewPos);
        {
            float x = 4;
            float y = 4;
            float w = Screen.width - 8 - 12;
            float h = 16 + 76;
            float s = h + 8;

            for (int i = 0; i < mCinemaClips.Count; i++)
            {
                CinemaClip clip = mCinemaClips[i];

                Rect cutsceneGroupHeaderRect = new Rect(x - 1, y, w + 2, 16);
                Rect cutsceneGroupClientRect = new Rect(x, y + 17, w, 76);

                Rect delButtonRect = new Rect(cutsceneGroupHeaderRect.width - 16, cutsceneGroupHeaderRect.y, 16, 16);
                if (Event.current.type == EventType.MouseDown && delButtonRect.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    DeleteCinemaClip(clip);

                    break;
                }

                Rect addButtonRect = new Rect(cutsceneGroupHeaderRect.width - 32, cutsceneGroupHeaderRect.y, 16, 16);
                if (Event.current.type == EventType.MouseDown && addButtonRect.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    mCurrentCinemaClipIndex = i;
                    CutsceneBrower.ShowWindow(mCurrentProjectName, OnCutsceneAdd);
                }

 
                float cx = cutsceneGroupClientRect.x + 4;
                float cy = cutsceneGroupClientRect.y + 5;
                float cw = (cutsceneGroupClientRect.width - 8 - (Mathf.Max(0, clip.cutsceneClips.Count - 1) * 2)) / Mathf.Max(1, clip.cutsceneClips.Count);

                foreach (CutsceneClip cc in clip.cutsceneClips)
                {
                    string assetName = Path.GetFileName(cc.fullname);
                    
                    if (cc.image == null)
                    {
                        cc.image = CutsceneAssetsManagerUtility.LoadImage(cc.fullname);
                    }

                    Rect groupRect = new Rect(cx, cy, 64, 64);
                    CutsceneAssetsManagerGUI.CutsceneGroupItem(groupRect, cc.image, cc.name, string.Empty, new Color(165 / 255f, 165 / 255f, 165 / 255f), Color.black);
                    cx += cw + 2;

                    GUI.color = Color.red;
                    if (GUI.Button(new Rect(cx - 22, cy, 22, 22), "×", "box"))
                    {
                        clip.cutsceneClips.Remove(cc);
                        SaveCinemaClip(clip);
                        break;
                    }
                    GUI.color = Color.white;
                }

                if (Event.current.type == EventType.MouseDown && 
                    (cutsceneGroupHeaderRect.Contains(Event.current.mousePosition) || cutsceneGroupClientRect.Contains(Event.current.mousePosition)))
                {
                    Event.current.Use();
                    mCurrentCinemaClipIndex = i;
                    this.AddCursorRect(addButtonRect, MouseCursor.SlideArrow);
                }

                if (cutsceneGroupHeaderRect.Contains(Event.current.mousePosition))
                {
                    this.AddCursorRect(addButtonRect, MouseCursor.SlideArrow);
                }

                GUI.backgroundColor = (currentCinemaClip == clip) ? new Color(0, 1, 1) : new Color(0.65f, 0.65f, 1);
                GUI.Toggle(cutsceneGroupHeaderRect, true, "", "dragtab");
                GUI.Label(new Rect(cutsceneGroupHeaderRect.x + 4, cutsceneGroupHeaderRect.y, 200, cutsceneGroupHeaderRect.height), clip.name);
                GUI.Box(cutsceneGroupClientRect, "");
                GUI.backgroundColor = Color.white;

                GUI.color = Color.green;
                GUI.Label(addButtonRect, "+");
                GUI.color = Color.white;

                //GUI.color = new Color(0.9f, 0.4f, 0.4f);
                //GUI.Label(delButtonRect, "×");
                //GUI.color = Color.white;

                y = y + s;
                GUILayout.Space(s);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void AddCursorRect(Rect rect, MouseCursor type)
    {
        EditorGUIUtility.AddCursorRect(rect, type);      
    }

    void OnCutsceneAdd(CutsceneClip cc)
    {
        currentCinemaClip.cutsceneClips.Add(cc);
        SaveCinemaClip(currentCinemaClip);
    }

    private void OnProjectChanged()
    {
        if (mCurrentProjectIndex < 0)
            return;

        mCinemaClips.Clear();
        mCurrentProjectName = mProjectArray[mCurrentProjectIndex];

        List<string> files = CutsceneAssetsManagerUtility.GetCinemaClipFiles(mCurrentProjectName);
        foreach (string file in files)
        {
            CinemaClip clip = LoadCinemaClip(file);
            mCinemaClips.Add(clip);
        }

        if (mCurrentCinemaClipIndex == -1 && mCinemaClips.Count > 0)
        {
            mCurrentCinemaClipIndex = 0;
        }
    }

    private CinemaClip currentCinemaClip
    {
        get
        {
            if (mCurrentCinemaClipIndex >= 0)
            {
                return mCinemaClips[mCurrentCinemaClipIndex];
            }

            return null;
        }
    }

    private int mCutsceneClipIndex = 0;
    private bool cutscenePlaying = false;

    private void StartPlay()
    {
        mCutsceneClipIndex = 0;
        string cutscenePath = currentCinemaClip.cutsceneClips[mCutsceneClipIndex].fullname;
        PlayCutscene(cutscenePath);
    }


    private void StopPlay()
    {
        mCutsceneClipIndex = 0;
        Slate.Cutscene.OnCutsceneStopped -= OnCutsceneStoppedHandler;
        CutsceneAssetsManagerUtility.StopInEditorMode();
    }

    private void PlayCutscene(string cutscenePath)
    {
        int idx = cutscenePath.IndexOf("Assets");
        cutscenePath = cutscenePath.Substring(idx);
        
        CutsceneAssetsManagerUtility.OpenCutscene(cutscenePath, OnCutsceneOpendHandler);
    }

    private void OnCutsceneOpendHandler(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        foreach (GameObject go in roots)
        {
            Slate.Cutscene cutscene = go.GetComponent<Slate.Cutscene>();
            if (cutscene != null)
            {
                if (!Application.isPlaying)
                {
                    CutsceneAssetsManagerUtility.PlayInEditorMode();
                }
                cutscene.defaultWrapMode = Slate.Cutscene.WrapMode.Once;
                //cutscene.Play(OnCutsceneStoppedHandler);
                cutscenePlaying = true;
                break;
            }
        }
    }

    void OnCutsceneStoppedHandler(Slate.Cutscene cutscene)
    {
        if (cutscenePlaying)
        {
            cutscenePlaying = false;
            mCutsceneClipIndex++;
            
            if (mCutsceneClipIndex < currentCinemaClip.cutsceneClips.Count)
            {
                Debug.Log("OnCutsceneStoppedHandler: " + mCutsceneClipIndex);
                string cutscenePath = currentCinemaClip.cutsceneClips[mCutsceneClipIndex].fullname;
                PlayCutscene(cutscenePath);
            }
            else
            {
                Debug.Log("OnCutsceneStoppedHandler: play to end!");
                StopPlay();
            }
        }
    }

    private CinemaClip CreateCinemaClip(string clipName)
    {
        CinemaClip clip = new CinemaClip();
        clip.name = clipName;

        try
        {
            string currentProjectPath = Path.Combine(Application.dataPath, "Workspace/" + mCurrentProjectName);
            string filename = Path.Combine(currentProjectPath, clip.name + ".cinemaclip");

            StreamWriter sw = new StreamWriter(filename);
            sw.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        return clip;
    }

    private CinemaClip LoadCinemaClip(string filename)
    {
        CinemaClip clip = null;

        try
        {
            if (File.Exists(filename))
            {
                clip = new CinemaClip();
                clip.name = Path.GetFileNameWithoutExtension(filename);
                clip.cutsceneClips = new List<CutsceneClip>();

                StreamReader sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    string text = sr.ReadLine();

                    if (!string.IsNullOrEmpty(text))
                    {
                        CutsceneClip cc = new CutsceneClip();
                        cc.fullname = text;
                        cc.name = Path.GetFileNameWithoutExtension(cc.fullname);
                        cc.image = CutsceneAssetsManagerUtility.LoadImage(cc.fullname);

                        clip.cutsceneClips.Add(cc);
                    }
                }
                sr.Close();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.StackTrace);
        }

        return clip;
    }

    private void SaveCinemaClip(CinemaClip clip)
    {
        try
        {
            string currentProjectPath = Path.Combine(Application.dataPath, "Workspace/" + mCurrentProjectName);
            string filename = Path.Combine(currentProjectPath, clip.name + ".cinemaclip");

            StreamWriter sw = new StreamWriter(filename);
            foreach (CutsceneClip cc in clip.cutsceneClips)
            {
                sw.WriteLine(cc.fullname);
            }
            sw.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void DeleteCinemaClip(CinemaClip clip)
    {
        try
        {
            string currentProjectPath = Path.Combine(Application.dataPath, "Workspace/" + mCurrentProjectName);
            string filename = Path.Combine(currentProjectPath, clip.name + ".cinemaclip");

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            mCinemaClips.Remove(clip);

            if (mCinemaClips.Count == 0)
            {
                mCurrentCinemaClipIndex = -1;
            }

            if (mCurrentCinemaClipIndex >= mCinemaClips.Count)
            {
                mCurrentCinemaClipIndex = mCinemaClips.Count - 1; 
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}
