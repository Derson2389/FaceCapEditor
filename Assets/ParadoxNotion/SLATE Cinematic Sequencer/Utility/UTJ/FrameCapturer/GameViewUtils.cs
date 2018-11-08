using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class GameViewUtils
{
    public enum GameViewResolutionType
    {
        AspectRatio, 
        FixedResolution
    }

    static object gameViewSizesInstance;
    static MethodInfo getGroup;

    static GameViewUtils()
    {
#if UNITY_EDITOR
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
#endif
    }
   
    public static void SetCurrentResolution(int width, int height)
    {
        int idx = FindResolution(width, height);
        if (idx == -1)
        {
            AddCustomResolution(GameViewResolutionType.FixedResolution, width, height);
            idx = FindResolution(width, height);
        }
#if UNITY_EDITOR
        SelectResolution(idx);
#endif
    }

    private static void AddCustomResolution(GameViewResolutionType viewResolutionType, int width, int height)
    {
#if UNITY_EDITOR
        var text = "";
        var group = GetGroup();
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { (int)viewResolutionType, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
#endif
    }
#if UNITY_EDITOR
    private static object GetGroup()
    {
#if UNITY_STANDALONE
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)GameViewSizeGroupType.Standalone });
#elif UNITY_ANDROID 
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)GameViewSizeGroupType.Android });
#endif
    }
#endif

#if UNITY_EDITOR
    private static void SelectResolution(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }
#endif

    private static int FindResolution(int width, int height)
    {
#if UNITY_EDITOR
        var group = GetGroup();
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
#endif
        return -1;

    }
}