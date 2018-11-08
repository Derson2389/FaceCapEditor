using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SlateRecorderCameraGroup : SlateRecorderGroup, INeedStartTime
{
    public override string RecordGroupTypeInfo()
    {
        return "虚拟像机录制";
    } 

    public bool NeedStartTime()
    {
        return true;
    }

    public override bool NeedRecord
    {
        get
        {
            return EditorPrefs.GetBool("CameraGroupRecord");
        }

        set
        {
            if (value)
            {
                string defaultTypeName = EditorPrefs.GetString(this.GetType().ToString());
                if (string.IsNullOrEmpty(defaultTypeName))
                {
                    SlateRecorderCameraGroup.Current.CurrentRecorder = DirectorRoomSlateCameraRecorder.Current;
                }
                else
                {
                    Type defaultType = Type.GetType(defaultTypeName);
                    if (defaultType != null)
                    {
                        SlateRecorderCameraGroup.Current.CurrentRecorder = m_RecorderList.Find(c => c.GetType() == defaultType);
                    }
                }
            }
            else
            {
                SlateRecorderCameraGroup.Current.CurrentRecorder = null;
            }
            EditorPrefs.SetBool("CameraGroupRecord", value);
            base.NeedRecord = value;
        }
    }


    private static SlateRecorderCameraGroup m_Current = null;

    public static SlateRecorderCameraGroup Current
    {
        get
        {
            return m_Current;
        }
    }

    public SlateRecorderCameraGroup()
    {
        m_Current = this;
        InitRecorderList(typeof(SlateCameraRecorder), typeof(SlateRecorderCameraGroup));
    }
}
