using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if USE_SLATE
public class SlateTrackerRecorderGroup : SlateRecorderGroup
{
    public override string RecordGroupTypeInfo()
    {
        return "表情录制";
    }

    public override bool NeedRecord
    {
        get
        {
            return EditorPrefs.GetBool("FaceGroupRecord");
        }

        set
        {
            if (value)
            {
                if (SlateTrackerRecorderGroup.Current.CurrentRecorder == null)
                {
                    SlateTrackerRecorderGroup.Current.CurrentRecorder = SlateFaceTrackerRecorder.Current;
                }
            }
            else
            {
                SlateTrackerRecorderGroup.Current.CurrentRecorder = null;
            }
            EditorPrefs.SetBool("FaceGroupRecord", value);
            base.NeedRecord = value;
        }
    }

    private static SlateTrackerRecorderGroup _current = null;

    public static SlateTrackerRecorderGroup Current
    {
        get
        {
            if (_current == null)
                new SlateTrackerRecorderGroup();

            return _current;
        }
    }

    public SlateTrackerRecorderGroup()
    {
        _current = this;
        InitRecorderList(typeof(SlateTrackerRecorder), typeof(SlateTrackerRecorderGroup));
    }

}
#endif