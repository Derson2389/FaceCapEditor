using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_SLATE
public abstract class SlateTrackerRecorder : SlateRecorder
{
    public bool IsActived
    {
        get
        {
            return SlateTrackerRecorderGroup.Current != null && SlateTrackerRecorderGroup.Current.CurrentRecorder == this;
        }
    }
}
#endif
