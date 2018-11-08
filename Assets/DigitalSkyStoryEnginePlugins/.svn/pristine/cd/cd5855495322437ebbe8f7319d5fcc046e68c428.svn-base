using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DigitalSky.Tracker;
///using dxyz; 

#if USE_SLATE
public class SlateFaceTrackerRecorder : SlateTrackerRecorder
{  
    private static SlateFaceTrackerRecorder _current = null;
    public static SlateFaceTrackerRecorder Current
    {
        get
        {
            return _current;
        }
    }

    public SlateFaceTrackerRecorder()
    {
        _current = this;
    }

    public override void Refresh()
    {
        foreach (var obj in TrackerRecordManager.Instance.recordObjectList)
        {
            if (obj.emotionTracker == null)
                continue;

            obj.EnableTracking();
        }           
    }

    public override void Cleanup()
    {
        foreach (var obj in TrackerRecordManager.Instance.recordObjectList)
        {
            obj.DisableTracking();
        }
    }

    public override void UpdateRecord()
    {
        foreach (var obj in TrackerRecordManager.Instance.recordObjectList)
        {
            if (obj.emotionTracker == null)
                continue;
            obj.emotionTracker.OnUpdate();

            if (obj.emotionRetargeter == null)
                continue;
            UnityMainThreadDispatcher dispatcher = obj.emotionRetargeter.GetComponent<UnityMainThreadDispatcher>();

            if (dispatcher != null)
                dispatcher.Update();
        }
    }

    public override void StartRecord(float startTime)
    {
        TrackerRecordManager.Instance.StartRecord(startTime);
    }

    public override void StopRecord(float startTime, float endTime, int? recordIdx)
    {
        Slate.Cutscene cutscene = Slate.CutsceneEditor.current.cutscene;
        TrackerRecordManager.Instance.StopRecord(cutscene, startTime, endTime, recordIdx);
    }

    public override void Record(float currentTime, float totalTime)
    {
        TrackerRecordManager.Instance.Record(currentTime, totalTime);
    }
}
#endif
