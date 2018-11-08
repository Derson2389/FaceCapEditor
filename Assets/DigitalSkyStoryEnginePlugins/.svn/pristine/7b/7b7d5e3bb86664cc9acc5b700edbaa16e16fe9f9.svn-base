using Slate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class SlateRecorderGroup
{
    protected List<SlateRecorder> m_RecorderList = new List<SlateRecorder>();
    protected SlateRecorder m_CurrentRecorder = null;
    protected bool m_NeedRecord = true;   
    public abstract string RecordGroupTypeInfo();
   
    public SlateRecorderGroup()
    {

    }

    public virtual bool NeedRecord
    {
        set { m_NeedRecord = value; }
        get { return m_NeedRecord;  }

    }

    public SlateRecorder CurrentRecorder
    {
        get
        {
            return m_CurrentRecorder;
        }

        set
        {            
            m_CurrentRecorder = value;
            string typeName = m_CurrentRecorder != null ? m_CurrentRecorder.GetType().ToString() : string.Empty;
            EditorPrefs.SetString(GetType().ToString(), typeName);            
        }
    }


    protected void InitRecorderList(Type InRecorderType, Type InGroupType)
    {
        m_RecorderList.Clear();
        m_CurrentRecorder = null;
        
        foreach (System.Type type in ReflectionTools.GetDerivedTypesOf(InRecorderType))
        {
            if (type.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).FirstOrDefault() != null)
            {
                continue;
            }

            m_RecorderList.Add((SlateRecorder)System.Activator.CreateInstance(type));
        }

        if (m_RecorderList.Count == 0)
        {
            return;
        }

        string defaultTypeName = EditorPrefs.GetString(InGroupType.ToString()); 
        if (string.IsNullOrEmpty(defaultTypeName))
        {
            return;
        }

        Type defaultType = Type.GetType(defaultTypeName);
        if (defaultType != null)
        {
            CurrentRecorder = m_RecorderList.Find(c => c.GetType() == defaultType);
        }
    }

    public void StartRecord(float startTime)
    {
        if (m_CurrentRecorder != null)
        {
            m_CurrentRecorder.StartRecord(startTime);
        }
    }
    public void StopRecord(float startTime, float endTime, int? recordIdx)
    {
        if (m_CurrentRecorder != null)
        {
            m_CurrentRecorder.StopRecord(startTime, endTime, recordIdx);
        }
    }
    public void Record(float currentTime, float totalTime)
    {
        if (m_CurrentRecorder != null)
        {
            m_CurrentRecorder.Record(currentTime, totalTime);
        }
    }

    public void UpdateRecord()
    {
        if (m_CurrentRecorder != null)
        {
            m_CurrentRecorder.UpdateRecord();
        }
    }

    public void PostRecord()
    {
        if (m_CurrentRecorder != null)
        {
            m_CurrentRecorder.PostRecord();
        }
    }

    public void Refresh()
    {
        if (m_CurrentRecorder != null)
        {
            m_CurrentRecorder.Refresh();
        }
    }

    public void Cleanup()
    {
        if (m_CurrentRecorder != null)
        {
            m_CurrentRecorder.Cleanup();
        }
    }

}
