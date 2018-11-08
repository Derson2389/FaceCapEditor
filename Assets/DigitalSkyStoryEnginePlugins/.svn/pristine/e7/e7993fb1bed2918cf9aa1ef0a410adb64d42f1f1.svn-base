using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

public class ActDetailView : EditorWindow
{
    ///private string[] mProjectArray = null;
    private List<ProjectDetail> projectList = null;
    private ProjectDetail mCurrentProject = null;
    private int mCurrentProjectIndex = -1;
    private string mCurrentProjectName = string.Empty;
    private List<CinemaClip> mCinemaClips = new List<CinemaClip>();
    private int mCurrentCinemaClipIndex = -1;
    private Vector2 mScrollviewPos = Vector2.zero;
    [System.NonSerialized]
    private string searchString = string.Empty;

    private int totalActsCount = 0;
    public static void OpenWindow(int ProjectIdx)
    {
        ActDetailView window = EditorWindow.GetWindow<ActDetailView>(false, "项目信息", true);
        window.minSize = new Vector2(360,100);
        window.mCurrentProjectIndex = ProjectIdx;
        window.Show();
        window.InitProjectData();
    }

    private void OnEnable()
    {
        InitProjectData();
    }


    private void InitProjectData()
    {
        CutsceneAssetsManagerStyles.Init();

        List<string> projects = CutsceneAssetsManagerUtility.GetProjects();
        ///mProjectArray = new string[projects.Count];
        projectList = new List<ProjectDetail>();
        for (int i = 0; i < projects.Count; i++)
        {
            ProjectDetail tempProj = new ProjectDetail();
            string projectName = System.IO.Path.GetFileNameWithoutExtension(projects[i]);
            tempProj.name = projectName;
            string projectPath = Path.Combine(CutsceneAssetsManagerUtility.workspace, projectName);
            string projFile = Path.Combine(projectPath, projectName + ".proj");
            if (File.Exists(projFile))
            {
                StreamReader sr = new StreamReader(projFile);
                string timeStr = sr.ReadLine();
                tempProj.timesnap = timeStr;
                string projDetail = sr.ReadLine();
                string[] projDetails = projDetail.Split('|');
                tempProj.projectId = projDetails[(int)CutsceneAssetsManagerUtility.ProjectDetailIdx.s_proj_projectId];
                tempProj.charger = projDetails[(int)CutsceneAssetsManagerUtility.ProjectDetailIdx.s_proj_chager];
                tempProj.status = projDetails[(int)CutsceneAssetsManagerUtility.ProjectDetailIdx.s_proj_status];
                sr.Close();
            }
            List<string> files = CutsceneAssetsManagerUtility.GetCinemaClipFilesByActName(projectName);

            if (files != null)
            {
                foreach (string file in files)
                {
                    CinemaClip clip = CutsceneAssetsManagerUtility.LoadCinemaClipByFolder(file);
                    tempProj.CinemaClips.Add(clip);
                }
            }
            projectList.Add(tempProj);
        }
        InitProjectActs();

    }

    private void OnGUI()
    {
        Toolbar();
        ShowSearchAndAdd();
        CutsceneGroups();

        Repaint();
    }

    private void Toolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        {
            GUILayout.Label(mCurrentProjectName, GUILayout.Width(160));
            GUILayout.FlexibleSpace();
            totalActsCount = mCinemaClips.Count;
            GUILayout.Label(string.Format("总场次：{0}", totalActsCount), GUILayout.Width(120));
            EditorGUI.BeginChangeCheck();
            if (mCurrentProject != null)
            {
                int index = EditorGUILayout.Popup((int)CutsceneAssetsManagerUtility.GetStatusByStr(mCurrentProject.status), CutsceneAssetsManagerUtility.mProjectStatusArr, EditorStyles.toolbarPopup);
                if (EditorGUI.EndChangeCheck())
                {
                    mCurrentProject.status = CutsceneAssetsManagerUtility.mProjectStatusArr[index];
                    CutsceneAssetsManagerUtility.UpdateProjFile(mCurrentProject);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    private void OnCreateCutsenceCallback(Scene sn)
    {
        CutsceneAssetsManagerUtility.ChooseCutsence(sn);
        InitProjectData();
        this.Close();
    }

    private void OnCutsceneGroupNameInput(string text, string desc)
    {
        CinemaClip clip = CreateCinemaClip(text, desc);

        mCurrentCinemaClipIndex = mCinemaClips.Count;
        mCinemaClips.Add(clip);
    }

    private void ShowSearchAndAdd()
    {
        var searchRect = new Rect(80, 20, 260, 40);
        var searchCancelRect = new Rect(340, 20, 40, 40);
        EditorGUILayout.BeginHorizontal("GroupBox");
        GUILayout.Label("场次搜索：");
        GUIStyle dummyStyle = (GUIStyle)"ToolbarSeachTextField";
        dummyStyle.fixedHeight = 14;

        CutsceneAssetsManagerGUI.DrawInputTextField(ref searchString);

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("新建场次", GUILayout.Width(80), GUILayout.Height(30)))
        {
            CutsceneGroupNameInput.ShowWindow(OnCutsceneGroupNameInput, totalActsCount);
        }

        EditorGUILayout.EndHorizontal();

    }


    private void InitProjectActs()
    {
        if (mCurrentProjectIndex < 0)
            return;

        mCinemaClips.Clear();
        mCinemaClips = projectList[mCurrentProjectIndex].CinemaClips;
        mCurrentProjectName = projectList[mCurrentProjectIndex].name;
        mCurrentProject = projectList[mCurrentProjectIndex];
        if (mCurrentCinemaClipIndex == -1 && mCinemaClips.Count > 0)
        {
            mCurrentCinemaClipIndex = 0;
        }
    }


    private void CutsceneGroups()
    {

        GUILayout.BeginHorizontal("GroupBox");
        mScrollviewPos = EditorGUILayout.BeginScrollView(mScrollviewPos);
        {
            float x = 0;
            float y = 4;
            float w = Screen.width - 8 - 50;
            float h = 16 + 76;
            float s = h + 8;
             
            for (int i = 0; i < mCinemaClips.Count; i++) 
            {
                CinemaClip clip = mCinemaClips[i];

                if (searchString != string.Empty && !clip.name.ToLower().Contains(searchString.ToLower()))
                {
                    GUI.backgroundColor = Color.white;
                    continue;
                }

                Rect cutsceneGroupHeaderRect = new Rect(x - 1, y, w + 2, 16);
                Rect cutsceneGroupClientRect = new Rect(x, y + 17, w, 76);

                GUI.backgroundColor = (currentCinemaClip == clip) ? new Color(0, 1, 1) : new Color(0.65f, 0.65f, 1);
                GUI.Toggle(cutsceneGroupHeaderRect, true, "", "dragtab");
                GUI.Label(new Rect(cutsceneGroupHeaderRect.x + 4, cutsceneGroupHeaderRect.y, 200, cutsceneGroupHeaderRect.height), clip.name);
                GUI.Box(cutsceneGroupClientRect, "");
                GUI.backgroundColor = Color.white;
  
                int totalCount = 4;
                float cx = cutsceneGroupClientRect.x ;
                float cy = cutsceneGroupClientRect.y + 5;
                float cw = (cutsceneGroupClientRect.width - 8 - (Mathf.Max(0, /*clip.cutsceneClips.Count*/totalCount - 1) * 2)) / Mathf.Max(1, /*clip.cutsceneClips.Count*/totalCount);

                for(int j =0; j<3; j++)
                {
                    if (clip.cutsceneClips == null)
                    {
                        break;
                    }
                    if (clip.cutsceneClips.Count == 0)
                    {
                        cw = 115;
                        Rect imageRect = new Rect(cx, cy, cw, 64);
                        CutsceneAssetsManagerGUI.CutsceneGroupItem(imageRect, null, null, clip.desc, !EditorGUIUtility.isProSkin ? (currentCinemaClip == clip) ? new Color(0, 1, 1) : new Color(0.65f, 0.65f, 1) : new Color(165 / 255f, 165 / 255f, 165 / 255f), Color.black, 0);
                        cx += cw + 2;
                        cw = (cutsceneGroupClientRect.width - 8 - 80 - 115); 
                        Rect groupRect1 = new Rect(cx, cy, cw, 64);
                        groupRect1.width = groupRect1.width * 4;
                        CutsceneAssetsManagerGUI.CutsceneGroupDesc(groupRect1, string.Empty, clip.desc, /*new Color(165 / 255f, 165 / 255f, 165 / 255f)*/!EditorGUIUtility.isProSkin ? Color.white : Color.gray, Color.white);
                        cx += cw + 2;
                        cw = 80;
                        groupRect1 = new Rect(cx, cy, cw, 64);
                        
                        CutsceneAssetsManagerGUI.CutsceneGroupCtrPanel(groupRect1, clip.desc, /*new Color(165 / 255f, 165 / 255f, 165 / 255f)*/!EditorGUIUtility.isProSkin ? Color.white : Color.gray, Color.white, 0, mCurrentProjectName, clip.name);
                        cx += cw + 2;
                        break;
                    }

                    var cc = clip.cutsceneClips[0];
                    if (cc == null)
                    {
                        break;
                    }
                    string assetName = Path.GetFileName(cc.fullname);
                    
                    if (cc.image == null)
                    {
                        cc.image = CutsceneAssetsManagerUtility.LoadImage(cc.fullname);
                    }

                    Rect groupRect = Rect.zero;
                    if (j == 0)
                    {
                        cw = 115;
                        groupRect = new Rect(cx, cy, cw, 64);
                        int clipsCount = clip.cutsceneClips.Count;
                        CutsceneAssetsManagerGUI.CutsceneGroupItem(groupRect, cc.image, cc.name, clip.desc, !EditorGUIUtility.isProSkin ? (currentCinemaClip == clip) ? new Color(0, 1, 1) : new Color(0.65f, 0.65f, 1) : new Color(165 / 255f, 165 / 255f, 165 / 255f), Color.black, clipsCount);
                        cx += cw + 2;
                    }
                    else if (j == 1)
                    {
                        cw = (cutsceneGroupClientRect.width - 8 - 80 - 115);
                        groupRect = new Rect(cx, cy, cw, 64);
                        CutsceneAssetsManagerGUI.CutsceneGroupDesc(groupRect, cc.name, clip.desc, /*new Color(165 / 255f, 165 / 255f, 165 / 255f)*/!EditorGUIUtility.isProSkin ? Color.white : Color.gray, Color.white);
                        cx += cw + 2;
                    }
                    else
                    {
                        cw = 80;
                        groupRect = new Rect(cx, cy, cw, 64);
                        int clipsCount = clip.cutsceneClips.Count;
                        CutsceneAssetsManagerGUI.CutsceneGroupCtrPanel(groupRect,  clip.desc, /*new Color(165 / 255f, 165 / 255f, 165 / 255f)*/!EditorGUIUtility.isProSkin ? Color.white : Color.gray, Color.white, clipsCount, mCurrentProjectName, clip.name);
                        cx += cw + 2;
                    }
                    
                    //GUI.color = Color.red;
                    //if (GUI.Button(new Rect(cx - 22, cy, 22, 22), "×", "box"))
                    //{
                    //    clip.cutsceneClips.Remove(cc);
                    //    SaveCinemaClip(clip);
                    //    break;
                    //}
                    //GUI.color = Color.white;
                }

                EditorGUIUtility.AddCursorRect(cutsceneGroupHeaderRect, MouseCursor.Link);
                EditorGUIUtility.AddCursorRect(cutsceneGroupClientRect, MouseCursor.Link);


                Rect addButtonRect = new Rect(cutsceneGroupHeaderRect.width - 16, cutsceneGroupHeaderRect.y, 16, 16);

                if (Event.current.type == EventType.Used && addButtonRect.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    mCurrentCinemaClipIndex = i;
                    CutsceneBuilderView.ShowWindow(mCurrentProjectName, clip.name, clip.desc, OnCreateCutsenceCallback);

                }


                if (Event.current.type == EventType.MouseDown && 
                    (cutsceneGroupHeaderRect.Contains(Event.current.mousePosition) || cutsceneGroupClientRect.Contains(Event.current.mousePosition)))
                {
                    Event.current.Use();
                    mCurrentCinemaClipIndex = i;
                    if (Event.current.clickCount >= 2)
                    {
                        if (mCurrentProjectName != null && clip.cutsceneClips.Count != 0)
                        {
                            CutsceneBrowerView2.ShowWindow(mCurrentProjectName, clip.name, null, clip.desc);
                        }
                    }
                }

                

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
        GUILayout.EndHorizontal();
    }



    void OnCutsceneAdd(CutsceneClip cc)
    {
        currentCinemaClip.cutsceneClips.Add(cc);
        SaveCinemaClip(currentCinemaClip);
    }

    private CinemaClip currentCinemaClip
    {
        get
        {
            if (mCurrentCinemaClipIndex >= 0 && mCurrentCinemaClipIndex < mCinemaClips.Count)
            {
                return mCinemaClips[mCurrentCinemaClipIndex];
            }

            return null;
        }
    }

    
    private CinemaClip CreateCinemaClip(string clipName, string desc = null)
    {
        CinemaClip clip = new CinemaClip();
        clip.name = clipName;
        clip.desc = desc;
        try
        {
            string currentProjectPath = Path.Combine(Application.dataPath, "Workspace/" + mCurrentProjectName);
            string combinePath = Path.Combine(CutsceneAssetsManagerUtility._storyStr, clip.name);
            string DsPath = Path.Combine(currentProjectPath, combinePath);
            string actDescPath = Path.Combine(DsPath, "desc");
            if (!Directory.Exists(actDescPath))
            {
                Directory.CreateDirectory(actDescPath);
            }
            
            string filename = Path.Combine(actDescPath, clip.name + ".desc");

            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine(clip.desc);
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
