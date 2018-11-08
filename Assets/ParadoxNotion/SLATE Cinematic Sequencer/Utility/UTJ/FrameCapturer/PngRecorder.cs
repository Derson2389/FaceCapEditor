#if UNITY_EDITOR

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR


namespace Slate {

    public class PngRecorder : ImageSequenceRecorder {
        private Camera _camera = null;
        private fcAPI.fcPNGContext m_ctx;

        void EraseCallbacks() {
            if (m_callbacks_fb != null) {
                for (int i = 0; i < m_callbacks_fb.Length; ++i) {
                    fcAPI.fcEraseDeferredCall(m_callbacks_fb[i]);
                }
                m_callbacks_fb = null;
            }

            if (m_callbacks_gb != null) {
                for (int i = 0; i < m_callbacks_gb.Length; ++i) {
                    fcAPI.fcEraseDeferredCall(m_callbacks_gb[i]);
                }
                m_callbacks_gb = null;
            }
        }

        void AddCommandBuffers() {
            //var cam = GetComponent<Camera>();
            if (_camera != null)
            {
                if (captureFramebuffer)
                {
                    _camera.AddCommandBuffer(CameraEvent.AfterEverything, m_cb_copy_fb);
                }
                if (captureGBuffer)
                {
                    _camera.AddCommandBuffer(CameraEvent.BeforeLighting, m_cb_copy_gb);
                }
            }
        }

        void RemoveCommandBuffers() {
            //var cam = GetComponent<Camera>();
            if (_camera != null)
            {
                if (captureFramebuffer)
                {
                    _camera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_cb_copy_fb);
                }
                if (captureGBuffer)
                {
                    _camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, m_cb_copy_gb);
                }
            }
        }

        void DoExport(){

            Debug.Log("PngRecorder: exporting frame " + currentFrame);
            
            string dir = dirName;
            string ext = fileName + "_" + currentFrame.ToString("0000") + ".png";

#if UNITY_2017_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(dir + "/" + ext);
#else
            Application.CaptureScreenshot(dir + "/" + ext); //to also capture GUI
#endif
          
            /*
                        // callback for frame buffer
                        if (captureFramebuffer)
                        {
                            string path = dir + "/Final_" + ext;
                            if(m_callbacks_fb == null)
                            {
                                m_callbacks_fb = new int[1];
                            }
                            m_callbacks_fb[0] = fcAPI.fcPngExportTexture(m_ctx, path, m_frame_buffer, m_callbacks_fb[0]);
                            GL.IssuePluginEvent(fcAPI.fcGetRenderEventFunc(), m_callbacks_fb[0]);
                        }
            */
            // callbacks for gbuffer
            if (captureGBuffer)
            {
                string[] path = new string[] {
                    dir + "/Albedo_" + ext,
                    dir + "/Occlusion_" + ext,
                    dir + "/Specular_" + ext,
                    dir + "/Smoothness_" + ext,
                    dir + "/Normal_" + ext,
                    dir + "/Emission_" + ext,
                    dir + "/Depth_" + ext,
                };
                if (m_callbacks_gb == null) {
                    m_callbacks_gb = new int[m_gbuffer.Length];
                }
                for (int i = 0; i < m_callbacks_gb.Length; ++i){
                    m_callbacks_gb[i] = fcAPI.fcPngExportTexture(m_ctx, path[i], m_gbuffer[i], m_callbacks_gb[i]);
                    GL.IssuePluginEvent(fcAPI.fcGetRenderEventFunc(), m_callbacks_gb[i]);
                }
            }
        }

        public override void Initialize() {
            _camera = GetComponent<Camera>();
            IsUseRenderingWait = true;

            shCopy = FrameCapturerUtils.GetFrameBufferCopyShader();
            
            System.IO.Directory.CreateDirectory(dirName);
            m_quad = FrameCapturerUtils.CreateFullscreenQuad();
            m_mat_copy = new Material(shCopy);

            //var cam = GetComponent<Camera>();
            if (_camera.targetTexture != null)
            {
                m_mat_copy.EnableKeyword("OFFSCREEN");
            }

#if UNITY_EDITOR
            if (captureGBuffer && !FrameCapturerUtils.IsRenderingPathDeferred(_camera))
            {
                Debug.LogWarning("PngRecorder: Rendering Path must be deferred to use Capture GBuffer mode.");
                captureGBuffer = false;
            }
#endif // UNITY_EDITOR

            // initialize png context
            fcAPI.fcPngConfig conf = fcAPI.fcPngConfig.default_value;
            m_ctx = fcAPI.fcPngCreateContext(ref conf);

            // initialize render targets
            {
                m_frame_buffer = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);
                m_frame_buffer.wrapMode = TextureWrapMode.Repeat;
                m_frame_buffer.Create();

                var formats = new RenderTextureFormat[7] {
                    RenderTextureFormat.ARGBHalf,   // albedo (RGB)
                    RenderTextureFormat.RHalf,      // occlusion (R)
                    RenderTextureFormat.ARGBHalf,   // specular (RGB)
                    RenderTextureFormat.RHalf,      // smoothness (R)
                    RenderTextureFormat.ARGBHalf,   // normal (RGB)
                    RenderTextureFormat.ARGBHalf,   // emission (RGB)
                    RenderTextureFormat.RHalf,      // depth (R)
                };
                m_gbuffer = new RenderTexture[7];
                for (int i = 0; i < m_gbuffer.Length; ++i) {
                    // last one is depth (1 channel)
                    m_gbuffer[i] = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, formats[i]);
                    m_gbuffer[i].filterMode = FilterMode.Point;
                    m_gbuffer[i].Create();
                }
            }

            // initialize command buffers
            {
                int tid = Shader.PropertyToID("_TmpFrameBuffer");

                m_cb_copy_fb = new CommandBuffer();
                m_cb_copy_fb.name = "PngRecorder: Copy FrameBuffer";
                m_cb_copy_fb.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Point);
                m_cb_copy_fb.Blit(BuiltinRenderTextureType.CurrentActive, tid);
                m_cb_copy_fb.SetRenderTarget(m_frame_buffer);
                m_cb_copy_fb.DrawMesh(m_quad, Matrix4x4.identity, m_mat_copy, 0, 0);
                m_cb_copy_fb.ReleaseTemporaryRT(tid);

                m_cb_copy_gb = new CommandBuffer();
                m_cb_copy_gb.name = "PngRecorder: Copy G-Buffer";
                m_cb_copy_gb.SetRenderTarget(
                    new RenderTargetIdentifier[] { m_gbuffer[0], m_gbuffer[1], m_gbuffer[2], m_gbuffer[3] }, m_gbuffer[0]);
                m_cb_copy_gb.DrawMesh(m_quad, Matrix4x4.identity, m_mat_copy, 0, 4);
                m_cb_copy_gb.SetRenderTarget(
                    new RenderTargetIdentifier[] { m_gbuffer[4], m_gbuffer[5], m_gbuffer[6], m_gbuffer[3] }, m_gbuffer[0]);
                m_cb_copy_gb.DrawMesh(m_quad, Matrix4x4.identity, m_mat_copy, 0, 5);
            }

            AddCommandBuffers();
        }

        void OnDestroy(){ Release(); }
        public override void Release() {

            RemoveCommandBuffers();
            _camera = null;

            if (m_cb_copy_gb != null){
                m_cb_copy_gb.Release();
                m_cb_copy_gb = null;
            }

            if (m_cb_copy_fb != null){
                m_cb_copy_fb.Release();
                m_cb_copy_fb = null;
            }

            if (m_frame_buffer != null){
                m_frame_buffer.Release();
                m_frame_buffer = null;
            }
            if (m_gbuffer != null){
                for (int i = 0; i < m_gbuffer.Length; ++i) {
                    m_gbuffer[i].Release();
                }
                m_gbuffer = null;
            }

            fcAPI.fcGuard(() =>
            {
                EraseCallbacks();
                fcAPI.fcPngDestroyContext(m_ctx);
                m_ctx.ptr = System.IntPtr.Zero;
            });
        }

        IEnumerator OnPostRender() {
            yield return new WaitForEndOfFrame()/*Application.isPlaying? new WaitForEndOfFrame() : null*/;
            DoExport();
            currentFrame++;
            IsRenderingWait = false;
        }
    }
}

#endif