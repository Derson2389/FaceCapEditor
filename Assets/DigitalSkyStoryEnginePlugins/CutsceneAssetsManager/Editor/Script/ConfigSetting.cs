using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[SerializeField]
public class ConfigSetting : ScriptableObject
{
    #region Editor Settings Variables
    [SerializeField]
    public List<string> makeBunldeFilePaths = new List<string>();

    [SerializeField]
    public List<string> makeCopyFilePaths = new List<string>();

    [SerializeField]
    public List<string> makeIgnoreFilePaths = new List<string>();
    #endregion

#if UNITY_EDITOR
    public static ConfigSetting EditorLoadConfigSetting(string name, string sencenPath)
    {
        string[] guids = AssetDatabase.FindAssets(name+"Config t:ConfigSetting");
        string path = "";

        if (guids.Length > 0)
        {
            path = AssetDatabase.GUIDToAssetPath(guids[0]);
            if (guids.Length > 1)
                Debug.LogWarning("ConfigSetting: Multiple ConfigSetting files found. Only one will be used.");
        }

        ConfigSetting settings = (ConfigSetting)AssetDatabase.LoadAssetAtPath(path, typeof(ConfigSetting));

        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<ConfigSetting>();

            ConfigSetting newSettings = ScriptableObject.CreateInstance<ConfigSetting>();

            EditorUtility.CopySerialized(newSettings, settings);
            string dirPath = sencenPath;
            // create Directory if not exist
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            AssetDatabase.CreateAsset(settings, path+ name + ".asset");
            AssetDatabase.Refresh();
            DestroyImmediate(newSettings);
        }

        return settings;
    }
#endif
}
