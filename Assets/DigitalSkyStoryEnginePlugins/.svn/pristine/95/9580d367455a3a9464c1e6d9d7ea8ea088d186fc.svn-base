using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class SlateRecorderCache
{
    private static SlateRecorderCache m_Instance = null;
    public static SlateRecorderCache Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SlateRecorderCache();
            }

            return m_Instance;
        }
    }

    public class CloneCache
    {
        public int RefCount;
        public GameObject CloneObject;

        public CloneCache(GameObject clone)
        {
            RefCount = 1;
            CloneObject = clone;
        }
    }

    public Dictionary<GameObject, CloneCache> m_CacheDictionary = new Dictionary<GameObject, CloneCache>();

    public GameObject FindCloneObject(GameObject original)
    {
        CloneCache cache = null;
        m_CacheDictionary.TryGetValue(original, out cache);
        if (cache != null)
        {
            return cache.CloneObject;
        }
        else
        {
            return null;
        }
    }

    public void DestroyAll()
    {
        foreach (CloneCache cache in m_CacheDictionary.Values)
        {
            GameObject.Destroy(cache.CloneObject);
        }
        m_CacheDictionary.Clear();
    }

    public void DestoryFromClone(GameObject clone)
    {
        foreach (var item in m_CacheDictionary)
        {
            if(item.Value.CloneObject == clone)
            {
                item.Value.RefCount--;
                if(item.Value.RefCount == 0)
                {
                    GameObject.DestroyImmediate(item.Value.CloneObject);
                    m_CacheDictionary.Remove(item.Key);
                }
                break;
            }
        }
    }

    public void DestroyFromOriginal(GameObject original)
    {
        CloneCache cache = null;
        m_CacheDictionary.TryGetValue(original, out cache);
        if (cache != null)
        {
            cache.RefCount--;
            if (cache.RefCount == 0)
            {
                GameObject.DestroyImmediate(cache.CloneObject);
            }
            m_CacheDictionary.Remove(original);
        }
    }

    public bool IsCloneObject(GameObject clone)
    {
        if (m_CacheDictionary == null)
            return false;

        foreach(var cache in m_CacheDictionary)
        {
            if (cache.Value.CloneObject == clone)
                return true;
        }

        return false;
    }

    public GameObject CreateClone(GameObject original)
    {
        CloneCache cache = null;
        m_CacheDictionary.TryGetValue(original, out cache);
        if (cache != null)
        {
            cache.RefCount++;
            return cache.CloneObject;
        }

        GameObject clonePrefabObj = null;
        if (PrefabUtility.GetPrefabType(original) == PrefabType.PrefabInstance)
        {
            clonePrefabObj = PrefabUtility.GetCorrespondingObjectFromSource(original) as GameObject;
        }

        if (clonePrefabObj == null)
        {
            return null;
        }

        GameObject clone = GameObject.Instantiate<GameObject>(clonePrefabObj);

        m_CacheDictionary.Add(original, new CloneCache(clone));

        return clone;
    }
}

#endif
