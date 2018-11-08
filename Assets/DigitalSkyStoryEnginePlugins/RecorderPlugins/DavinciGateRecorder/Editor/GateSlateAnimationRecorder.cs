using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GateSlateAnimationRecorder : SlateAnimationRecorder
{
    private static GateSlateAnimationRecorder m_Current = null;

    public static GateSlateAnimationRecorder Current
    {
        get
        {
            return m_Current;
        }
    }

    public GateSlateAnimationRecorder()
    {
        m_Current = this;
    }

    public override void Refresh()
    {
        GateRecorderManager.Instance.RefreshObjectList();
    }
    public override void Cleanup()
    {
        GateRecorderManager.Instance.Cleanup();
    }
    public override void UpdateRecord()
    {
        GateRecorderManager.Instance.UpdateConnect();
    }

    public override void StartRecord(float startTime)
    {
        GateRecorderManager.Instance.StartRecord(startTime);
    }
    public override void StopRecord(float startTime, float endTime, int? recordIdx = null)
    {
        GateRecorderManager.Instance.StopRecord(Slate.CutsceneEditor.current.cutscene, startTime, endTime, recordIdx);
    }
    public override void Record(float currentTime, float totalTime)
    {
        GateRecorderManager.Instance.Record(currentTime, totalTime);
    }
}
