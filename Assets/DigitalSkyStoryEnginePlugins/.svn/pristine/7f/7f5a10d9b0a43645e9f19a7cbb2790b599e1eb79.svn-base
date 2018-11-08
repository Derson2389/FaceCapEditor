using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateRecorderManagerBrage
{
    private  static string m_recordSavePath;
    public static string RecordSavePath
    {
        get
        {
            m_recordSavePath = PlayerPrefs.GetString("AnimationDir", "RecordAnimations/");
            return m_recordSavePath;
        }
    }

    private static Vector3 mSceneOffset = Vector3.zero;
    public static  Vector3 SceneOffset
    {
        get
        {
            mSceneOffset.x = PlayerPrefs.GetFloat("SceneOffsetX", 0.0f);
            mSceneOffset.y = PlayerPrefs.GetFloat("SceneOffsetY", 0.0f);
            mSceneOffset.z = PlayerPrefs.GetFloat("SceneOffsetZ", 0.0f);
            return mSceneOffset;
        }
    }

    private static float mAnimationFrameFPS = 30;
    public static float AnimationFrameFPS
    {
        get
        {
            mAnimationFrameFPS = PlayerPrefs.GetFloat("FrameInterval", 30.0f);
            return mAnimationFrameFPS;
        }        
    }
}
