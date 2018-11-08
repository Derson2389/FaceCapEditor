using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, DisallowMultipleComponent]
public class CutsceneCountDown : MonoBehaviour
{
    public delegate void OnCountDownGUI();
    public OnCountDownGUI OnCountDownGUICallBack;

    private void OnGUI()
    {
        if(OnCountDownGUICallBack != null)
        {
            OnCountDownGUICallBack();
#if UNITY_EDITOR
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
        }
    }
}
