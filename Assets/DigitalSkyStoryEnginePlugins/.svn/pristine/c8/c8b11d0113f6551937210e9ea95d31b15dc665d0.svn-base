using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynBinderCache
{
    public Dictionary<int, SynBinder> m_SynBinderCache = new Dictionary<int, SynBinder>();

    public static SynBinderCache m_Instance = null;
    public static SynBinderCache Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = new SynBinderCache();
            }

            return m_Instance;
        }
    }


    public SynBinder GetSynBinder(int suitIndex)
    {
        SynBinder binder = null;
        if (m_SynBinderCache.TryGetValue(suitIndex, out binder))
        {
            return binder;
        }

        return null;
    }

    public void RemoveSynBinder(int suitIndex)
    {
        SynBinder binder = GetSynBinder(suitIndex);
        if(binder != null)
        {
            binder.DisconnectSuit();
        }
        m_SynBinderCache.Remove(suitIndex);
    }

    public SynBinder CreateSynBinder(int suitIndex, string skelPath)
    {
        SynBinder binder = GetSynBinder(suitIndex);
        if(binder != null && binder.SuitIndex == suitIndex && binder.SkelPath == skelPath)
        {
            return binder;
        }
        
        RemoveSynBinder(suitIndex);

        binder = new SynBinder();
        if(binder.ConnectSuit(suitIndex, skelPath))
        {
            m_SynBinderCache.Add(suitIndex, binder);
            return binder;
        }
        else
        {
            return null;
        }
    }
}
