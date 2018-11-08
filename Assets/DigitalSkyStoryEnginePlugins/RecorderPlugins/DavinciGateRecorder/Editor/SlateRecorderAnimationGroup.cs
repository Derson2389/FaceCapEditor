using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SlateRecorderAnimationGroup : SlateRecorderGroup
{

    public override string RecordGroupTypeInfo()
    {
        return "动作录制";
    }

    public override bool NeedRecord
    {
        get
        {
            return EditorPrefs.GetBool("AnimationGroupRecord");
        }

        set
        {
            if (value)
            {
                string defaultTypeName = EditorPrefs.GetString(this.GetType().ToString());
                if (string.IsNullOrEmpty(defaultTypeName))
                {
                    SlateRecorderAnimationGroup.Current.CurrentRecorder = GateSlateAnimationRecorder.Current;
                }
                else
                {
                    Type defaultType = Type.GetType(defaultTypeName);
                    if (defaultType != null) 
                    {
                        SlateRecorderAnimationGroup.Current.CurrentRecorder = m_RecorderList.Find(c => c.GetType() == defaultType);
                    }
                }
            }
            else
            {
                SlateRecorderAnimationGroup.Current.CurrentRecorder = null;
            }
            EditorPrefs.SetBool("AnimationGroupRecord", value);
            base.NeedRecord = value;
        }
    }

    static private SlateRecorderAnimationGroup m_Current = null;

    static public SlateRecorderAnimationGroup Current
    {
        get
        {
            if (m_Current == null)
                new SlateRecorderAnimationGroup();

            return m_Current;
        }
    }

    public SlateRecorderAnimationGroup()
    {
        m_Current = this;
        InitRecorderList(typeof(SlateAnimationRecorder), typeof(SlateRecorderAnimationGroup));
    }
    
}
