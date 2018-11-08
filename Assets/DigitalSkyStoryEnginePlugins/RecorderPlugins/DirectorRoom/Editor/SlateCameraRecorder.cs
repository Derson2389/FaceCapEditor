using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlateCameraRecorder : SlateRecorder
{
    public bool IsActived
    {
        get
        {
            return SlateRecorderCameraGroup.Current != null && SlateRecorderCameraGroup.Current.CurrentRecorder == this;
        }
    }
}
