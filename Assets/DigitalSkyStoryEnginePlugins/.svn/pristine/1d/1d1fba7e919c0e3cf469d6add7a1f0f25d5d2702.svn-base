/**
 * 临时场景加载器
 * 为方便剧情场景管理开发
 * 不用在正式资源加载中
 **/

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[ExecuteInEditMode]
public class TempLoadScene : MonoBehaviour
{
    
#if UNITY_EDITOR
    //[ReadOnly]
#endif
    public string currentScenePath = null;

///#if UNITY_EDITOR
    public string projectDir = "";
    public string sceneName = "";

    public Slate.Cutscene cutscene;

    void Awake()
    {
        if (!Application.isPlaying) return;

        if (!string.IsNullOrEmpty(projectDir))
        {
            return;
        }
        string path = SceneManager.GetActiveScene().path;
        string baseDir = "Assets/Workspace/";
        int baseDirIndex = path.IndexOf(baseDir);
        Debug.LogError("a:" + baseDirIndex);
        if (baseDirIndex >= 0)
        {
            int dirNameIndex = path.IndexOf("/", baseDirIndex + baseDir.Length);
            //Debug.LogError("b:" + dirNameIndex);
            if (dirNameIndex >= 0)
            {
                projectDir = path.Substring(baseDirIndex + baseDir.Length, dirNameIndex - baseDirIndex - baseDir.Length);
                //Debug.LogError("c:" + projectDir);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        if (string.IsNullOrEmpty(sceneName)) return;

#if UNITY_EDITOR
        bool existedInBuildSettings = false;
        foreach (EditorBuildSettingsScene ebss in EditorBuildSettings.scenes)
        {
            if (ebss.path == currentScenePath && ebss.enabled)
            {
                existedInBuildSettings = true;
                break;
            }
        }

        if (!existedInBuildSettings)
        {
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                scenes[i] = EditorBuildSettings.scenes[i];
            }
            scenes[EditorBuildSettings.scenes.Length] = new EditorBuildSettingsScene(currentScenePath, true);

            EditorBuildSettings.scenes = scenes;
        }
#endif
        
        string workspaceRootPath = Path.Combine(Application.dataPath, "Workspace");
        string[] scenePaths = Directory.GetFiles(workspaceRootPath, "*.unity", SearchOption.AllDirectories);
        string findScenePath = "";
        foreach (string scenePath in scenePaths)
        {
            int projectDirIndex = scenePath.IndexOf(projectDir, workspaceRootPath.Length);
            //Debug.LogError("a:" + projectDirIndex);
            if (projectDirIndex >= 0)
            {
                int sceneNameIndex = scenePath.IndexOf(sceneName, projectDirIndex + projectDir.Length); //sceneName 空串会返回startIndex
                //Debug.LogError("b:" + sceneNameIndex);
                if (sceneNameIndex >= 0)
                {
                    findScenePath = scenePath.Remove(0, Application.dataPath.Length - "Assets".Length);
                    //Debug.LogError(findScenePath);
                }
            }
        }
        if (findScenePath == string.Empty)
        {
            return;
        }
        this.currentScenePath = findScenePath.Replace('\\', '/');
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Scene scene = EditorSceneManager.GetSceneByPath(currentScenePath);
            if (!scene.IsValid())
            {
                SceneManager.LoadScene(currentScenePath, LoadSceneMode.Additive);
            }
            if (cutscene == null)
            {
                Debug.LogError("Cutscene is not provided TempLoadScene", gameObject);
                return;
            }
            cutscene.defaultStopMode = Slate.Cutscene.StopMode.Rewind;
            cutscene.defaultWrapMode = Slate.Cutscene.WrapMode.Once;
            cutscene.Play();
        }
        else
        {
            Scene scene = EditorSceneManager.GetSceneByPath(currentScenePath);
            if (!scene.IsValid())
            {
                var dummyScene = EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Additive);
                EditorSceneManager.SetActiveScene(dummyScene);
            }
            if (cutscene == null)
            {
                Debug.LogError("Cutscene is not provided TempLoadScene", gameObject);
                return;
            }           
        }
#endif
    }


    void Update()
    {
    }
///#endif

    public static Slate.Cutscene FindCutscene()
    {
        Slate.Cutscene result = null;
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            GameObject[] gos = scene.GetRootGameObjects();
            foreach (GameObject go in gos)
            {
                //if(go.activeSelf)
                if (go.activeSelf && go.name.IndexOf("Cutscene") >= 0)
                {
                    result = go.GetComponent<Slate.Cutscene>();
                }
                if (result != null) break;
            }
            if (result != null) break;
        }
        return result;
    }


#if UNITY_EDITOR
    public static Slate.Cutscene static_cutscene;
    public static bool isPause = false;
    void OnGUI()
    {
//         if (cutscene == null) return;
//         static_cutscene = cutscene;
//         if (GUI.Button(new Rect(30, 400, 400, 200), "PAUSE"))
//         {
//             if (cutscene.isPaused)
//             {
//                 isPause = false;
//                 cutscene.Resume();
//             }
//             else
//             {
//                 isPause = true;
//                 cutscene.Pause();
//             }
//         }
    }
#endif

}
