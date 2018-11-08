using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CutsceneAssetsManagerStyles
{
    public static Texture2D texAdd = null;
    public static Texture2D texTrash = null;

    private static Dictionary<string, GUIStyle> mStyles = new Dictionary<string, GUIStyle>();

    public static void Init()
    {
        GUISkin skin = AssetDatabase.LoadAssetAtPath<GUISkin>(@"Assets\DigitalSkyStoryEnginePlugins_Post\CutsceneAssetsManager\Editor\GUIStyle\CutsceneAssetsManagerSkin.guiskin");
        if(skin == null)
        {
            skin = AssetDatabase.LoadAssetAtPath<GUISkin>(@"Assets\DigitalSkyStoryEnginePlugins\CutsceneAssetsManager\Editor\GUIStyle\CutsceneAssetsManagerSkin.guiskin");
        }

        foreach (GUIStyle style in skin.customStyles)
        {
            if (mStyles.ContainsKey(style.name))
                continue;
            mStyles.Add(style.name, style);
        }

        texAdd = AssetDatabase.LoadAssetAtPath<Texture2D>(@"Assets\DigitalSkyStoryEnginePlugins\CutsceneAssetsManager\Editor\GUIStyle\Icons\add.png");
        texTrash = AssetDatabase.LoadAssetAtPath<Texture2D>(@"Assets\DigitalSkyStoryEnginePlugins\CutsceneAssetsManager\Editor\GUIStyle\Icons\trash.png");
    }

    public static GUIStyle titleText
    {
        get { return mStyles["titleText"]; }
    }

    public static GUIStyle itemText
    {
        get { return mStyles["itemText"]; }
    }

    public static GUIStyle flatButton
    {
        get { return mStyles["flatButton"]; }
    }
}
