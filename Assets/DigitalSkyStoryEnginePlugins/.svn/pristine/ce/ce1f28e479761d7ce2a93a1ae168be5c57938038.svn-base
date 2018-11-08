using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlateSoundRecorder : SlateRecorder
{
    public bool IsActived
    {
        get
        {
            return SlateRecorderSoundGroup.Current != null && SlateRecorderSoundGroup.Current.CurrentRecorder == this;
        }
    }
}
