using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDirectorMarker : MonoBehaviour
{
    public int Order = 0;
    public Slate.Cutscene TriggerSubCutscene = null;
    [HideInInspector]
    public bool IsDynamic = false;
}
