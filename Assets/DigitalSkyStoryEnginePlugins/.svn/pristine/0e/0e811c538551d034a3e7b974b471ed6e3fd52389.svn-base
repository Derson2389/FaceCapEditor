#region Copyright (c) TQ.

//达文西扩展方法

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Extension Unity's function, to be more convinient
/// </summary>
public static class DavinciExtensions
{
    public static void SetWidth(this RectTransform rectTrans, float width)
    {
        var size = rectTrans.sizeDelta;
        size.x = width;
        rectTrans.sizeDelta = size;
    }

    public static void SetHeight(this RectTransform rectTrans, float height)
    {
        var size = rectTrans.sizeDelta;
        size.y = height;
        rectTrans.sizeDelta = size;
    }

    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }

    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = new Vector3(t.position.x, newY, t.position.z);
    }

    public static void SetLocalPositionX(this Transform t, float newX)
    {
        t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
    }

    public static void SetLocalPositionY(this Transform t, float newY)
    {
        t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
    }

    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = new Vector3(t.position.x, t.position.y, newZ);
    }

    public static void SetLocalPositionZ(this Transform t, float newZ)
    {
        t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
    }

    public static void SetLocalScale(this Transform t, Vector3 newScale)
    {
        t.localScale = newScale;
    }

    public static void SetLocalScaleZero(this Transform t)
    {
        t.localScale = Vector3.zero;
    }

    public static float GetPositionX(this Transform t)
    {
        return t.position.x;
    }

    public static float GetPositionY(this Transform t)
    {
        return t.position.y;
    }

    public static float GetPositionZ(this Transform t)
    {
        return t.position.z;
    }

    public static float GetLocalPositionX(this Transform t)
    {
        return t.localPosition.x;
    }

    public static float GetLocalPositionY(this Transform t)
    {
        return t.localPosition.y;
    }

    public static float GetLocalPositionZ(this Transform t)
    {
        return t.localPosition.z;
    }

    public static bool HasRigidbody(this GameObject gobj)
    {
        return (gobj.GetComponent<Rigidbody>() != null);
    }

    public static bool HasAnimation(this GameObject gobj)
    {
        return (gobj.GetComponent<Animation>() != null);
    }

    public static void SetSpeed(this Animation anim, float newSpeed)
    {
        anim[anim.clip.name].speed = newSpeed;
    }

    public static Vector2 ToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static byte ToByte(this string val)
    {
        return string.IsNullOrEmpty(val) ? (byte)0 : Convert.ToByte(val);
    }

    public static int ToInt32(this string val)
    {
        return string.IsNullOrEmpty(val) ? 0 : Convert.ToInt32(val);
    }

    public static long ToInt64(this string val)
    {
        return string.IsNullOrEmpty(val) ? 0 : Convert.ToInt64(val);
    }

    public static float ToFloat(this string val)
    {
        return string.IsNullOrEmpty(val) ? 0f : Convert.ToSingle(val);
    }

    /// <summary>
    /// Get from object Array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="openArgs"></param>
    /// <param name="offset"></param>
    /// <param name="isLog"></param>
    /// <returns></returns>
    public static T Get<T>(this object[] openArgs, int offset, bool isLog = true)
    {
        T ret;
        if ((openArgs.Length - 1) >= offset)
        {
            var arrElement = openArgs[offset];
            if (arrElement == null)
                ret = default(T);
            else
            {
                try
                {
                    ret = (T)Convert.ChangeType(arrElement, typeof(T));
                }
                catch (Exception)
                {
                    if (arrElement is string && string.IsNullOrEmpty(arrElement as string))
                        ret = default(T);
                    else
                    {
                        Debug.LogError(string.Format("[Error get from object[],  '{0}' change to type {1}", arrElement, typeof(T)));
                        ret = default(T);
                    }
                }
            }
        }
        else
        {
            ret = default(T);
            if (isLog)
                Debug.LogError(string.Format("[GetArg] {0} args - offset: {1}", openArgs, offset));
        }

        return ret;
    }


    public static string ReplaceFileNameDot(string inputStr)
    {
#if USE_BUNDLE_BROWER
        int lastDot = inputStr.LastIndexOf('.');
        string ext = inputStr.Substring(0, lastDot);
        string filename = inputStr.Substring(lastDot + 1);
        return ext + "_" + filename;
#else
        return inputStr;
#endif
    }
    public static string BackFileNameWithDot(string inputStr)
    {
        int lastDot = inputStr.LastIndexOf('_');
        string ext = inputStr.Substring(0, lastDot);
        return ext;//+ "." + filename;
    }
    



    public static string GetRelativeURL(string inputStr, string substr)
    {
        
        int FirstLine = inputStr.IndexOf(substr);
        string ext = inputStr.Substring(FirstLine+substr.Length);
        return ext;
    }

    public static string AppendRelativeURL(string inputStr, string substr)
    {      
        string ext = substr + inputStr;
        return ext;
    }

}

// C# 扩展, 扩充C#类的功能
public static class KEngineToolExtensions
{
    // 扩展List/  
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T KFirstOrDefault<T>(this IEnumerable<T> source)
    {
        if (source != null)
        {
            foreach (T item in source)
            {
                return item;
            }
        }

        return default(T);
    }

    public static List<T> KFirst<T>(this IEnumerable<T> source, int num)
    {
        var count = 0;
        var items = new List<T>();
        if (source != null)
        {
            foreach (T item in source)
            {
                if (++count > num)
                {
                    break;
                }
                items.Add(item);
            }
        }

        return items;
    }

    public delegate bool KFilterAction<T>(T t);

    public static List<T> KFilter<T>(this IEnumerable<T> source, KFilterAction<T> testAction)
    {
        var items = new List<T>();
        if (source != null)
        {
            foreach (T item in source)
            {
                if (testAction(item))
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    public delegate bool KFilterAction<T, K>(T t, K k);

    public static Dictionary<T, K> KFilter<T, K>(this IEnumerable<KeyValuePair<T, K>> source,
        KFilterAction<T, K> testAction)
    {
        var items = new Dictionary<T, K>();
        if (source != null)
        {
            foreach (KeyValuePair<T, K> pair in source)
            {
                if (testAction(pair.Key, pair.Value))
                {
                    items.Add(pair.Key, pair.Value);
                }
            }
        }

        return items;
    }

    public static T KLastOrDefault<T>(this IEnumerable<T> source)
    {
        var result = default(T);
        foreach (T item in source)
        {
            result = item;
        }
        return result;
    }

    /// <summary>
    /// == Linq Last
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static List<T> KLast<T>(this IEnumerable<T> source, int num)
    {
        // 开始读取的位置
        var startIndex = Math.Max(0, source.KToList().Count - num);
        var index = 0;
        var items = new List<T>();
        if (source != null)
        {
            foreach (T item in source)
            {
                if (index < startIndex)
                {
                    continue;
                }
                items.Add(item);
            }
        }

        return items;
    }

    /// <summary>
    /// HashSet AddRange
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
    {
        bool allAdded = true;
        foreach (T item in items)
        {
            allAdded &= @this.Add(item);
        }
        return allAdded;
    }

    public static T[] KToArray<T>(this IEnumerable<T> source)
    {
        var list = new List<T>();
        foreach (T item in source)
        {
            list.Add(item);
        }
        return list.ToArray();
    }

    public static List<T> KToList<T>(this IEnumerable<T> source)
    {
        var list = new List<T>();
        foreach (T item in source)
        {
            list.Add(item);
        }
        return list;
    }

    public static List<T> KUnion<T>(this List<T> first, List<T> second, IEqualityComparer<T> comparer)
    {
        var results = new List<T>();
        var list = first.KToList();
        list.AddRange(second);
        foreach (T item in list)
        {
            var include = false;
            foreach (T result in results)
            {
                if (comparer.Equals(result, item))
                {
                    include = true;
                    break;
                }
            }
            if (!include)
            {
                results.Add(item);
            }
        }
        return results;
    }

    public static string KJoin<T>(this IEnumerable<T> source, string sp)
    {
        var result = new StringBuilder();
        foreach (T item in source)
        {
            if (result.Length == 0)
            {
                result.Append(item);
            }
            else
            {
                result.Append(sp).Append(item);
            }
        }
        return result.ToString();
    }

    public static bool KContains<TSource>(this IEnumerable<TSource> source, TSource value)
    {
        foreach (TSource item in source)
        {
            if (Equals(item, value))
            {
                return true;
            }
        }
        return false;
    }

    // by KK, 获取自动判断JSONObject的str，n
    //public static object Value(this JSONObject jsonObj)
    //{
    //    switch (jsonObj.type)
    //    {
    //        case JSONObject.Type.NUMBER:  // 暂时返回整形！不管浮点了, lua目前少用浮点
    //            return (int)jsonObj.n;
    //        case JSONObject.Type.STRING:
    //            return jsonObj.str;
    //        case JSONObject.Type.NULL:
    //            return null;
    //        case JSONObject.Type.ARRAY:
    //        case JSONObject.Type.OBJECT:
    //            return jsonObj;
    //        case JSONObject.Type.BOOL:
    //            return jsonObj.b;
    //    }

    //    return null;
    //}
}