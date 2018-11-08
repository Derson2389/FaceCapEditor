using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlateAnimationRecorder : SlateRecorder
{
    public bool IsActived
    {
        get
        {
            return SlateRecorderAnimationGroup.Current != null && SlateRecorderAnimationGroup.Current.CurrentRecorder == this;
        }
    }
}
