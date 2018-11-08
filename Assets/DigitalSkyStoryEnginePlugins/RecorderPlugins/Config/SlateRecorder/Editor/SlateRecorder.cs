using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlateRecorder
{
    
    public virtual void Refresh()
    {

    }
    public virtual void Cleanup()
    {

    }
    public virtual void UpdateRecord()
    {

    }
    public virtual void PostRecord()
    {

    }
    public virtual void StartRecord(float startTime)
    {

    }
    public virtual void StopRecord(float startTime, float endTime, int? recordIdx = null)
    {
    }
    public virtual void Record(float currentTime, float totalTime)
    {

    }
}
