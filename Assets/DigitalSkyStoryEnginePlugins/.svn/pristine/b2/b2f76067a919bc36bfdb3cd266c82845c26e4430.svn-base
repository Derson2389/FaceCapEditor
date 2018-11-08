using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SlateRecorderSoundGroup : SlateRecorderGroup
{
    public override string RecordGroupTypeInfo()
    {
        return "声音录制";
    }

    public override bool NeedRecord
    {
        get
        {
            return EditorPrefs.GetBool("SoundGroupRecord");
        }
        set
        {

            if (value)
            {
                string defaultTypeName = EditorPrefs.GetString(this.GetType().ToString());
                if (string.IsNullOrEmpty(defaultTypeName))
                {
                    SlateRecorderSoundGroup.Current.CurrentRecorder = ViconSlateSoundRecorder.Current;
                }
                else
                {
                    Type defaultType = Type.GetType(defaultTypeName);
                    if (defaultType != null)
                    {
                        SlateRecorderSoundGroup.Current.CurrentRecorder = m_RecorderList.Find(c => c.GetType() == defaultType);
                    }
                }                
            }
            else
            {
                SlateRecorderSoundGroup.Current.CurrentRecorder = null;
            }
            EditorPrefs.SetBool("SoundGroupRecord", value);
            base.NeedRecord = value;
        }
    }

    private static SlateRecorderSoundGroup m_Current = null;
    public static SlateRecorderSoundGroup Current
    {
        get
        {
            return m_Current;
        }
    }

    public SlateRecorderSoundGroup()
    {
        m_Current = this;
        InitRecorderList(typeof(SlateSoundRecorder), typeof(SlateRecorderSoundGroup));
    }
}
