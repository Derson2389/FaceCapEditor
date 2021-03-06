﻿#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slate
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class MovieRecorder : MonoBehaviour
    {
        abstract public void Initialize();
        abstract public void Release();

        abstract public bool StartRecording();
        abstract public void StopRecording();

        abstract public bool IsCapturing
        {
            get;
        }

        abstract public int FrameRate
        {
            set;
            get;
        }

        abstract public string TargetFilePath
        {
            set;
            get;
        }

        abstract public string TargetFileName
        {
            set;
            get;
        }

        abstract public bool CaptureGBuffer
        {
            set;
            get;
        }

        abstract public bool CaptureFramebuffer
        {
            set;
            get;
        }

        //abstract public int ResolutionWidth
        //{
        //    set;
        //    get;
        //}

        abstract public bool IsRenderingWait
        {
            set;
            get;
        }

        abstract public bool IsUseRenderingWait
        {
            set;
            get;
        }

        abstract public bool IsSkipAudio
        {
            get;
            set;
        }

        void OnDestroy()
        {
            Release();
        }
    }
}

#endif