using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ProjectListView : EditorWindow
{
    private string[] mProjectArray = null;
    private List<ProjectDetail> projectList = null;
    private Vector2 mScrollviewPos = Vector2.zero;
    private Rect ProjectBgRect = Rect.zero;
    private int mCurrentProjectIndex = 2;
    private static CutsceneCreateProje createWindow = null;
    [System.NonSerialized]
    private string searchString = string.Empty;
    public static ProjectListView ShowWindow(System.Action<string> callback)
    {
        ProjectListView window = EditorWindow.GetWindow<ProjectListView>(false, "资源管理器", true);
        window.Show();

        return window;
    }


    private void OnEnable()
    {
        CutsceneAssetsManagerStyles.Init();

        List<string> projects = CutsceneAssetsManagerUtility.GetProjects();
        mProjectArray = new string[projects.Count];
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

        if (mCurrentProjectIndex < 0 && mProjectArray.Length > 0)
        {
            mCurrentProjectIndex = 0;
        }
        //OnProjectChanged();
    }


    private void OnGUI()
    {    
        Toolbar();
        SearchContent();
        ShowProjectDetail();
    }


    private void Toolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        {       
            GUILayout.Box("项目编号", EditorStyles.toolbarButton,GUILayout.Width(168));        
            GUILayout.Box("项目名称", EditorStyles.toolbarButton, GUILayout.Width(176));
            GUILayout.Box("总场次", EditorStyles.toolbarButton, GUILayout.Width(176));
            GUILayout.Box("更新时间", EditorStyles.toolbarButton, GUILayout.Width(176));
            GUILayout.Box("状态", EditorStyles.toolbarButton, GUILayout.Width(176));
            GUILayout.Box("负责人", EditorStyles.toolbarButton);

        }
        EditorGUILayout.EndHorizontal();
    }

    private void SearchContent()
    {
        EditorGUILayout.BeginHorizontal("GroupBox");
        GUILayout.FlexibleSpace();
        GUILayout.Label("项目搜索");
        //searchString = GUILayout.TextField(searchString, (GUIStyle)"SeachTextField", GUILayout.Width(360));
        //if (GUILayout.Button("Close", (GUIStyle)"SeachCancelButton"))
        //{
        //    searchString = string.Empty;
        //    GUIUtility.keyboardControl = 0;
        //}
        CutsceneAssetsManagerGUI.DrawInputTextField(ref searchString);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
    }

    private void ShowProjectDetail()
    {
        Rect listRect = new Rect(8,80, Screen.width-18, Screen.height-120);
        GUILayout.BeginArea(listRect);
        EditorGUILayout.BeginVertical("GroupBox");
        mScrollviewPos = EditorGUILayout.BeginScrollView(mScrollviewPos, false, false);
        
        float x = 0;
        float y = 2;
        float w = Screen.width ;
        float h = 20;
        float s = h + 2;
        for (int i =0; i< projectList.Count; i++)
        {           
            ProjectBgRect = new Rect(x, y+i*s, w + 2, h);
            GUI.backgroundColor = (i == mCurrentProjectIndex) ? Color.green : new Color(0, 1, 1);
            if (searchString != string.Empty && !projectList[i].name.ToLower().Contains(searchString.ToLower()))
            {
                GUI.backgroundColor = Color.white;
                continue;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.textArea, GUILayout.ExpandWidth(true), GUILayout.Height(16));
           
            GUILayout.Box(projectList[i].projectId, EditorStyles.label, GUILayout.Width(160));
            GUILayout.Box("|", EditorStyles.label, GUILayout.Width(8));
            GUILayout.Box(projectList[i].name, EditorStyles.label, GUILayout.Width(160));
            GUILayout.Box("|", EditorStyles.label, GUILayout.Width(8));
            int ActCount = projectList[i].CinemaClips.Count;
            GUILayout.Box("总场次:"+ ActCount, EditorStyles.label, GUILayout.Width(160));
            GUILayout.Box("|", EditorStyles.label, GUILayout.Width(8));
            GUILayout.Box(projectList[i].timesnap, EditorStyles.label, GUILayout.Width(160));
            GUILayout.Box("|", EditorStyles.label, GUILayout.Width(8));
            GUILayout.Box(projectList[i].status, EditorStyles.label, GUILayout.Width(160));
            GUILayout.Box("|", EditorStyles.label, GUILayout.Width(8));
            GUILayout.Box(projectList[i].charger, EditorStyles.label);

            EditorGUILayout.EndHorizontal();
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor = Color.red;
                GUI.Box(ProjectBgRect, "");
                GUI.backgroundColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = Color.white;
            }
      
            if (Event.current.type == EventType.MouseDown && ProjectBgRect.Contains(Event.current.mousePosition)&& Event.current.clickCount == 1)
            {
                Event.current.Use();
                mCurrentProjectIndex = i;
                ///Debug.LogError("mCurrentProjectIndex : " + mCurrentProjectIndex); 
            }

            if (Event.current.type == EventType.MouseDown && ProjectBgRect.Contains(Event.current.mousePosition)&& Event.current.clickCount == 2)
            {
                Event.current.Use();
                ActDetailView.OpenWindow(mCurrentProjectIndex); 
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("创建", GUILayout.Width(48)))
        {
            createWindow = CutsceneCreateProje.ShowWindow(OnCreateProjectCallBack);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }

  
    private void OnCreateProjectCallBack(string project, string chargerName)
    {
        string projectPath = Path.Combine(CutsceneAssetsManagerUtility.workspace, project);
        if (!Directory.Exists(projectPath))
        {
            Directory.CreateDirectory(projectPath);
            string projFile = Path.Combine(projectPath, project + ".proj");
            StreamWriter sw = new StreamWriter(projFile);
            sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            sw.WriteLine(CutsceneAssetsManagerUtility.mProjectStatusArr[0]+ "|" + chargerName + "|" + project+"|"+ CutsceneAssetsManagerUtility.AutoGentProjectID());
            sw.Close();
        }
        
        string resPath = Path.Combine(projectPath, "_Resources");
        if (!Directory.Exists(resPath))
        {
            Directory.CreateDirectory(resPath);
        }

        string animPath = Path.Combine(resPath, "Animation");
        if (!Directory.Exists(animPath))
        {
            Directory.CreateDirectory(animPath);
        }

        string charactersPath = Path.Combine(resPath, "Characters");
        if (!Directory.Exists(charactersPath))
        {
            Directory.CreateDirectory(charactersPath);
        }

        string LipAnimationPath = Path.Combine(resPath, "LipAnimation");
        if (!Directory.Exists(LipAnimationPath))
        {
            Directory.CreateDirectory(LipAnimationPath);
        }

        string MaterialsPath = Path.Combine(resPath, "Materials");
        if (!Directory.Exists(MaterialsPath))
        {
            Directory.CreateDirectory(MaterialsPath);
        }


        string ScenesPath = Path.Combine(resPath, "Scenes");
        if (!Directory.Exists(ScenesPath))
        {
            Directory.CreateDirectory(ScenesPath);
        }

        string SoundPath = Path.Combine(resPath, "Sound");
        if (!Directory.Exists(SoundPath))
        {
            Directory.CreateDirectory(SoundPath);
        }


        string EffectPath = Path.Combine(resPath, "Effect");
        {
            if (!Directory.Exists(EffectPath))
            {
                Directory.CreateDirectory(EffectPath);
            }
        }

        string PresetsPath = Path.Combine(resPath, "Presets");
        {
            if (!Directory.Exists(PresetsPath))
            {
                Directory.CreateDirectory(PresetsPath);
            }
        }

        string PropsPath = Path.Combine(resPath, "Props");
        {
            if (!Directory.Exists(PropsPath))
            {
                Directory.CreateDirectory(PropsPath);
            }
        }

        string storyPath = Path.Combine(projectPath, "_Story");
        if (!Directory.Exists(storyPath))
        {
            Directory.CreateDirectory(storyPath);
        }
        AssetDatabase.Refresh();

        //svn add 并上传
        SvnForUnity.SvnAddFolderAndFile(project);     
        SvnForUnity.SvnCommitFolderAndFile(project);
        OnEnable();

    }
}
