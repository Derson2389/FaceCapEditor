#if UNITY_EDITOR

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR


namespace Slate {

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class ImageSequenceRecorder : MonoBehaviour{

        public bool captureFramebuffer = true;
        public bool captureGBuffer = true;
        public string dirName;
        public string fileName;
        public Shader shCopy;        

        protected Material m_mat_copy;
        protected Mesh m_quad;
        protected CommandBuffer m_cb_copy_fb;
        protected CommandBuffer m_cb_copy_gb;
        protected RenderTexture m_frame_buffer;
        protected RenderTexture[] m_gbuffer;
        protected int[] m_callbacks_fb;
        protected int[] m_callbacks_gb;
        protected int currentFrame;
        
        public bool IsUseRenderingWait = false;
        public bool IsRenderingWait = false;

        abstract public void Initialize();
        abstract public void Release();
    }
}

#endif