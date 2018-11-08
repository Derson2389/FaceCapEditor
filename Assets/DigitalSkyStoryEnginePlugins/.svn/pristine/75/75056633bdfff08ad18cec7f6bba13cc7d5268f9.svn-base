using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Slate;
using System;

public class CutsceneBrowerView2 : EditorWindow 
{
    private string mCurrentProjectName = string.Empty;

    [SerializeField]
    private List<CutsceneClip> mCutsceneClips = new List<CutsceneClip>();
    [SerializeField]
    private List<CutsceneClip> mActCutsceneClips = new List<CutsceneClip>(); 
    private CutsceneClip mCurrentCutsceneClip = null;
    private CutsceneClip mDummyClip = null;
    private CinemaClip mCurrentCinemaClip = null;
    private Vector2 mScrollviewPos = Vector2.zero;
    private string mCurrentActName = string.Empty;
    private System.Action<CutsceneClip> mAction;
    private Camera myCamera = null;  //获取摄像机 
    private RenderTexture renderTexture;
    private Camera m_PreviewCamera;
    private string desc = null;

    public static CutsceneBrowerView2 ShowWindow(string projectName, string actName,System.Action<CutsceneClip> callback, string desc = null)
    {
        CutsceneBrowerView2 window = EditorWindow.GetWindow<CutsceneBrowerView2>("镜头预览");
        window.desc = desc;
        if (callback != null)
        {
            window.mAction = callback;
        }
        else
        {
            window.mAction = window.OpenAndEditCutsence;
        }
        
        window.SetCurrentActName(actName);
        window.SetProject(projectName);
        window.Show();
     
        return window;
    }

    public void SetCurrentActName(string ActName)
    {
        mCurrentActName = ActName;
    }

    private void SysDescWithUnitySence()
    {
        //update .desc
        List<string> cutscenes = CutsceneAssetsManagerUtility.GetCutsceneFilesByAct(mCurrentProjectName, mCurrentActName);
        List<CutsceneClip> mCutsceneClips = new List<CutsceneClip>();
        foreach (string cs in cutscenes)
        {
            CutsceneClip clip = new CutsceneClip();
            clip.fullname = cs;
            clip.name = System.IO.Path.GetFileNameWithoutExtension(cs);
            clip.image = CutsceneAssetsManagerUtility.LoadImage(clip.fullname);
            clip.belongActName = mCurrentActName;
            mCutsceneClips.Add(clip);
        }

        CinemaClip mCurrentCinemaClip = new CinemaClip();
        mCurrentCinemaClip.desc = desc;
        mCurrentCinemaClip.name = mCurrentActName;
        mCurrentCinemaClip.cutsceneClips = mCutsceneClips;
        CutsceneAssetsManagerUtility.UpdateDescFile(mCurrentProjectName, mCurrentCinemaClip);
        //
    }

    public void OnProjectChanged(Scene sn)
    {
        CutsceneAssetsManagerUtility.ChooseCutsence(sn);
        SetCurrentActName(mCurrentActName);
        SetProject(mCurrentProjectName);
    }

    private void SetProject(string projectName)
    {
        mCurrentProjectName = projectName;
       /// SysDescWithUnitySence();
        mActCutsceneClips.Clear();
        mCutsceneClips.Clear();
        List<string> cutscenes = CutsceneAssetsManagerUtility.GetCutsceneFilesByDesc(mCurrentProjectName, mCurrentActName);
        foreach (string cs in cutscenes)
        {
            CutsceneClip clip = new CutsceneClip();
            clip.fullname = cs;
            clip.name = System.IO.Path.GetFileNameWithoutExtension(cs);
            clip.image = CutsceneAssetsManagerUtility.LoadImage(clip.fullname);
            clip.belongActName = mCurrentActName;
            mCutsceneClips.Add(clip);
        }

        foreach (CutsceneClip cc in mCutsceneClips)
        {
            if (cc.belongActName == mCurrentActName && !string.IsNullOrEmpty(cc.belongActName) && !string.IsNullOrEmpty(mCurrentActName))
            {
                mActCutsceneClips.Add(cc);
            }
        }
        
        mScrollviewPos = Vector2.zero;
    }

    public void Awake()  //当跳出窗口时首先调用该方法  
    {
        renderTexture = new RenderTexture(1024, 576, 24);   //获取renderTexture  
        m_PreviewCamera = new GameObject("PreviewCamera").AddComponent<Camera>();

    }


    public void Update()  //跳出窗口后每帧调用该方法  
    {
        if (EditorApplication.isPlaying)
        {
            if (mActCutsceneClips.Count == 0)
            {
                OnProjectChanged();
            }

            Slate.Cutscene.OnCutsceneStopped += OnCutsceneStoppedHandler; 
        }

        if (m_PreviewCamera == null)
        {
            m_PreviewCamera = new GameObject("PreviewCamera").AddComponent<Camera>();
        }
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(1024, 576, 24);
        }

        if ( Camera.main != null)
        {
            m_PreviewCamera.CopyFrom(Camera.main);
            m_PreviewCamera.targetTexture = renderTexture;
        }
    }

    private void OnProjectChanged()
    {
        SetCurrentActName(mCurrentActName);
        SetProject(mCurrentProjectName);
    }

    public void OnDestroy()
    {
        if (m_PreviewCamera != null)
        {
            m_PreviewCamera.targetTexture = null;
            GameObject.DestroyImmediate(m_PreviewCamera.gameObject);
            m_PreviewCamera = null;
        }
        renderTexture = null;
        mActCutsceneClips.Clear();
        mCutsceneClips.Clear();

        if (EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            StopPlay();
        }
    }

    public void AddFunctionCallback()
    {
        CutsceneBuilderView.ShowWindow(mCurrentProjectName, mCurrentActName, null, OnProjectChanged);
    }

    private void Toolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        {
            if (GUILayout.Button("新分镜", EditorStyles.toolbarButton, GUILayout.Width(168)))
            {
                CutsceneBuilderView.ShowWindow(mCurrentProjectName, mCurrentActName, null, OnProjectChanged);
            };
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("刷新缩略图", EditorStyles.toolbarButton, GUILayout.Width(168)))
            {
                CutsceneAssetsManagerUtility.refreshSenceThumbnails(mActCutsceneClips);
            };

            GUILayout.FlexibleSpace();
            if (!EditorApplication.isPlaying)
            {
                if (GUILayout.Button("预览", EditorStyles.toolbarButton, GUILayout.Width(176)))
                { 
                    StartPlay();
                }
            }
            else
            {
                if (GUILayout.Button("停止", EditorStyles.toolbarButton, GUILayout.Width(176)))
                {
                    StopPlay();
                }
            }        
        }
        EditorGUILayout.EndHorizontal();
    }

    private int mCutsceneClipIndex = 0;
    private bool cutscenePlaying = false;
    private void StartPlay()
    {
        mCutsceneClipIndex = 0;
        string cutscenePath = mActCutsceneClips[mCutsceneClipIndex].fullname;
        List<string> allCutPaths = CutsceneAssetsManagerUtility.GetCutsceneFilesByDesc(mCurrentProjectName, mCurrentActName);

        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[allCutPaths.Count];
        for (int j = 0; j < allCutPaths.Count; ++j)
        {
            scenes[j] = new EditorBuildSettingsScene(allCutPaths[j], true);
        }
      
        EditorBuildSettings.scenes = scenes;

        PlayCutscene(cutscenePath);
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

            if (mCutsceneClipIndex < mActCutsceneClips.Count)
            {
                Debug.Log("OnCutsceneStoppedHandler: " + mCutsceneClipIndex);
                string cutscenePath = mActCutsceneClips[mCutsceneClipIndex].fullname;
                PlayCutscene(cutscenePath);
            }
            else
            {
                Debug.Log("OnCutsceneStoppedHandler: play to end!");
                StopPlay();
            }
        }
    }

    private void OpenAndEditCutsence(CutsceneClip  cc)
    {
        string ccPath = cc.fullname;
        int idx = ccPath.IndexOf("Assets");
        ccPath = ccPath.Substring(idx);

        CutsceneAssetsManagerUtility.OpenCutscene(ccPath, CutsceneAssetsManagerUtility.ChooseCutsence);
    }

    private void StopPlay()
    {
        mCutsceneClipIndex = 0;
        Slate.Cutscene.OnCutsceneStopped -= OnCutsceneStoppedHandler;
        CutsceneAssetsManagerUtility.StopInEditorMode();
    }

    private void RenderCutsence()
    {
        Rect renderRC = new Rect(0 , 10, Screen.width-2, Screen.height-210);
        GUILayout.BeginArea(renderRC);
        GUILayout.BeginHorizontal();

        Rect whiteRC = new Rect(renderRC.x +10, renderRC.y+10, renderRC.width - 20, renderRC.height - 20);
        GUI.DrawTexture(whiteRC, Texture2D.whiteTexture);

        if (renderTexture != null && m_PreviewCamera != null && m_PreviewCamera.targetTexture != null && EditorApplication.isPlaying)
        {
            m_PreviewCamera.Render();
            GUI.DrawTexture(whiteRC, renderTexture, ScaleMode.ScaleToFit, false);

            Rect stopRect = new Rect(renderRC.x + renderRC.width - 100, renderRC.y + renderRC.height - 100, 60, 60);
            if (GUI.Button(stopRect, "停止"))
            {
                StopPlay();
            }

        }
        else
        {
            
            GUIStyle bb = new GUIStyle();
            bb.normal.background = null;    //这是设置背景填充的
            bb.normal.textColor = Color.green;
            bb.fontSize = 30;       //当然，这是字体颜色
            Rect textRect2 = new Rect((whiteRC.x + whiteRC.width) / 2 - 55, whiteRC.y + whiteRC.height/2 -30, whiteRC.width, whiteRC.height);
            GUI.Label(textRect2, "预览显示区域",bb);
            GUIStyle bs = new GUIStyle();
            bs.normal.background = Texture2D.blackTexture;     //这是设置背景填充的
            bs.normal.textColor = Color.green;
            bs.fontSize = 50;       //当然，这是字体颜色


            if (!EditorApplication.isPlaying)
            {               
                Rect startRect = new Rect(textRect2.x + 60 , textRect2.y - 80, 60, 60);
                EditorGUIUtility.AddCursorRect(startRect, MouseCursor.Link);
                if (GUI.Button(startRect, "▶", bs))
                {
                    StartPlay();
                }

                Rect stopRect = new Rect(renderRC.x + renderRC.width - 100, renderRC.y + renderRC.height - 100, 60, 60);
                if (GUI.Button(stopRect, "刷新\n缩略图"))
                {                  
                    CutsceneAssetsManagerUtility.refreshSenceThumbnails(mActCutsceneClips);
                }

                if (mCurrentCutsceneClip != null)
                {
                    Rect cloneRect = new Rect(renderRC.x + renderRC.width - 160, renderRC.y + renderRC.height - 100, 60, 60);
                    if (GUI.Button(cloneRect, "克隆"))
                    {
                        CutsceneClone.ShowWindow(mCurrentCutsceneClip.fullname, mCurrentCutsceneClip.name, CloneCutsence);
                    }
                }             
            }            
        }
        

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void CloneCutsence(string path, string name)
    {
        string finalPath = path.Replace('\\', '/');
        Scene sc = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByPath(finalPath);

        sc = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(finalPath);
        string newSceneDic = System.IO.Path.GetDirectoryName(finalPath);
        string newScenePath = newSceneDic + "/" + name + ".unity";
        bool success = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(sc, newScenePath, true);
        if (success)
        {
            SysDescWithUnitySence();
            OnProjectChanged();
        }
    }


    private void OnGUI()
    {
        float width = Screen.width;
        float height = Screen.height;
       /// Toolbar();
        RenderCutsence();

        GUILayout.BeginArea(new Rect(2, Screen.height - 210 , Screen.width-2, 200));
        GUILayout.BeginHorizontal("GroupBox");
        if (mActCutsceneClips.Count != 0)
        {
            mScrollviewPos = EditorGUILayout.BeginScrollView(mScrollviewPos, GUILayout.ExpandHeight(true));
            {
                GUILayout.Space(4);

                int columns = Mathf.Max(1, Mathf.FloorToInt((width - 16) / 182f));
                int dummyCount = mActCutsceneClips.Count + 1;

                int rows = dummyCount / columns;
                if (dummyCount % columns > 0)
                {
                    rows++;
                }

                for (int r = 0; r < rows; r++)
                {
                    float y = 4 + r * 162;

                    int lc = Mathf.Min(columns, dummyCount - (r * columns));
                    for (int c = 0; c < lc; c++)
                    {
                        float x = 2 + c * 182;
                        int idx = r * columns + c;
                        //if (mActCutsceneClips.Count == 0)
                        //{
                        //    break;
                        //}

                        if (idx >= mActCutsceneClips.Count)
                        {
                            Color backgroundColor = Color.white;
                            Color textColor = Color.white;
                            Rect itemRect = new Rect(x, y, 180, 160);

                            CutsceneAssetsManagerGUI.ListviewItem(itemRect, null, "", backgroundColor, textColor, null, null, idx, true, AddFunctionCallback);
                        }
                        else
                        {

                            CutsceneClip cc = mActCutsceneClips[idx];

                            bool selected = (mCurrentCutsceneClip == cc);
                            Color backgroundColor = selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;
                            Color textColor = selected ? new Color(0, 192 / 255f, 255 / 255f) : Color.white;

                            Rect itemRect = new Rect(x, y, 180, 160);

                            if (idx == 0)
                            {
                                CutsceneAssetsManagerGUI.ListviewItem(itemRect, cc.IMGAGE, cc.name, backgroundColor, textColor, null, null, idx);
                            }

                            else if (idx == mActCutsceneClips.Count - 1)
                            {
                                CutsceneAssetsManagerGUI.ListviewItem(itemRect, cc.IMGAGE, cc.name, backgroundColor, textColor, null, null, idx);
                            }
                            else
                            {
                                CutsceneAssetsManagerGUI.ListviewItem(itemRect, cc.IMGAGE, cc.name, backgroundColor, textColor, null, null, idx);
                            }


                            AddCursorRect(itemRect, mDummyClip == null ? MouseCursor.Link : MouseCursor.MoveArrow);
                            if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
                            {
                                Event.current.Use();
                                mCurrentCutsceneClip = cc;
                                mDummyClip = mCurrentCutsceneClip;
                                if (Event.current.clickCount >= 2)
                                {
                                    if (mAction != null)
                                    {
                                        mAction(mCurrentCutsceneClip);
                                    }
                                    this.Close();
                                }
                            }

                           
                            if (mDummyClip != null && mDummyClip!=cc)
                            {
                                if (itemRect.Contains(Event.current.mousePosition))
                                {
                                    var markRect = new Rect((mActCutsceneClips.IndexOf(mDummyClip) < idx) ? itemRect.xMax - 3 : itemRect.x, itemRect.y , 3, itemRect.height);
                                    GUI.color = Color.green;
                                    GUI.DrawTexture(markRect, Styles.whiteTexture);
                                    GUI.color = Color.white;
                                }

                                if (Event.current.rawType == EventType.MouseUp && Event.current.button == 0 && itemRect.Contains(Event.current.mousePosition))
                                {
                                    mActCutsceneClips.Remove(mDummyClip);
                                    mActCutsceneClips.Insert(idx, mDummyClip);
                                    mDummyClip = null;
                                    OnUpdateDescFile();
                                    Event.current.Use();
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
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //clear picks
        if (Event.current.rawType == EventType.MouseUp)
        {
            mDummyClip = null;
        }

        Repaint();

    }

    private void AddCursorRect(Rect rect, MouseCursor type)
    {
        
         EditorGUIUtility.AddCursorRect(rect, type);
            
    }

    private void MoveFront(int idx)
    {
        var temp = mActCutsceneClips[idx];
        mActCutsceneClips[idx] = mActCutsceneClips[idx-1];
        mActCutsceneClips[idx - 1] = temp;
        CinemaClip mCurrentCinemaClip = new CinemaClip();
        mCurrentCinemaClip.desc = desc;
        mCurrentCinemaClip.name = mCurrentActName;
        mCurrentCinemaClip.cutsceneClips = mActCutsceneClips;
        CutsceneAssetsManagerUtility.UpdateDescFile(mCurrentProjectName, mCurrentCinemaClip);
    }

    private void OnUpdateDescFile()
    {
        CinemaClip mCurrentCinemaClip = new CinemaClip();
        mCurrentCinemaClip.desc = desc;
        mCurrentCinemaClip.name = mCurrentActName;
        mCurrentCinemaClip.cutsceneClips = mActCutsceneClips;
        CutsceneAssetsManagerUtility.UpdateDescFile(mCurrentProjectName, mCurrentCinemaClip);
    }

    private void MoveNext(int idx)
    {
        var temp = mActCutsceneClips[idx];
        mActCutsceneClips[idx] = mActCutsceneClips[idx + 1];
        mActCutsceneClips[idx + 1] = temp;
        CinemaClip mCurrentCinemaClip = new CinemaClip();
        mCurrentCinemaClip.desc = desc;
        mCurrentCinemaClip.name = mCurrentActName;
        mCurrentCinemaClip.cutsceneClips = mActCutsceneClips;
        CutsceneAssetsManagerUtility.UpdateDescFile(mCurrentProjectName, mCurrentCinemaClip);
    }

    //private void OnLostFocus()
    //{
    //    this.Close();
    //}
}
