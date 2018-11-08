using Slate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, DisallowMultipleComponent]
public class CutsceneRecordTime : MonoBehaviour
{
    public float CurrentTime;

#if UNITY_EDITOR
    public static string HumanizeTimeString(float seconds)
    {
        System.TimeSpan ts = System.TimeSpan.FromSeconds(seconds);
        string timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Minutes, ts.Seconds, Mathf.RoundToInt((float)ts.Milliseconds / 10));
        return timeStr;
    }
#endif

    private void OnGUI()
    {
#if UNITY_EDITOR
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 65;

        if (Prefs.timeStepMode == Prefs.TimeStepMode.Seconds)
        {
            GUILayout.Label(HumanizeTimeString(Mathf.Round(CurrentTime / Prefs.snapInterval) * Prefs.snapInterval));
        }
        else
        {
            GUILayout.Label(((int)(CurrentTime * Prefs.frameRate)).ToString());
        }

        GUI.color = Color.white;
#endif
    }

}
