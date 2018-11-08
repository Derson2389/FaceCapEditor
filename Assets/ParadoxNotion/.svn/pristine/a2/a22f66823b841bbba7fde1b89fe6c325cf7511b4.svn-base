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


namespace Slate
{

    public class DepthRecorder : ImageSequenceRecorder
    {
        private Camera _camera = null;
        private fcAPI.fcEXRContext m_ctx;

        void EraseCallbacks()
        {
            if (m_callbacks_fb != null)
            {
                for (int i = 0; i < m_callbacks_fb.Length; ++i)
                {
                    fcAPI.fcEraseDeferredCall(m_callbacks_fb[i]);
                }
                m_callbacks_fb = null;
            }

            if (m_callbacks_gb != null)
            {
                for (int i = 0; i < m_callbacks_gb.Length; ++i)
                {
                    fcAPI.fcEraseDeferredCall(m_callbacks_gb[i]);
                }
                m_callbacks_gb = null;
            }
        }

        void AddCommandBuffers()
        {
            if (_camera != null)
            {
                if (captureFramebuffer)
                {
                    _camera.AddCommandBuffer(CameraEvent.AfterEverything, m_cb_copy_fb);
                }
            }
        }

        void RemoveCommandBuffers()
        {
            if (_camera != null)
            {
                if (captureFramebuffer)
                {
                    _camera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_cb_copy_fb);
                }
            }
        }

        void DoExport()
        {

            Debug.Log("DepthRecorder: exporting frame " + currentFrame);

            string dir = dirName;
            string ext = fileName + "_" + currentFrame.ToString("0000") + ".exr";

            //Texture2D tex = new Texture2D(m_frame_buffer.width, m_frame_buffer.height, TextureFormat.RGBAFloat, false);
            //RenderTexture.active = m_frame_buffer;
            //tex.ReadPixels(new Rect(0, 0, m_frame_buffer.width, m_frame_buffer.height), 0, 0);
            //tex.Apply();

            //byte[] colorList = tex.GetRawTextureData();

            //float v;
            //for (int i = 0; i < colorList.Length; i += 4)
            //{
            //    v = BitConverter.ToSingle(colorList, i);
            //}

            if (captureFramebuffer)
            {
                // callback for frame buffer
                if (m_callbacks_fb == null)
                {
                    m_callbacks_fb = new int[3];
                }
                {
                    string path = dir + "/Depth_" + ext;
                    var rt = m_frame_buffer;
                    m_callbacks_fb[0] = fcAPI.fcExrBeginFrame(m_ctx, path, rt.width, rt.height, m_callbacks_fb[0]);
                    m_callbacks_fb[1] = fcAPI.fcExrAddLayerTexture(m_ctx, rt, 0, "Z", m_callbacks_fb[1]);
                    m_callbacks_fb[2] = fcAPI.fcExrEndFrame(m_ctx, m_callbacks_fb[2]);
                }
                for (int i = 0; i < m_callbacks_fb.Length; ++i)
                {
                    GL.IssuePluginEvent(fcAPI.fcGetRenderEventFunc(), m_callbacks_fb[i]);
                }
            }
        }

        public override void Initialize()
        {
            _camera = GetComponent<Camera>();
            IsUseRenderingWait = true;

            shCopy = FrameCapturerUtils.GetFrameBufferCopyShader();

            System.IO.Directory.CreateDirectory(dirName);
            m_quad = FrameCapturerUtils.CreateFullscreenQuad();
            m_mat_copy = new Material(shCopy);
            
            if (_camera.targetTexture != null)
            {
                m_mat_copy.EnableKeyword("OFFSCREEN");
            }
            
            
            fcAPI.fcExrConfig conf = fcAPI.fcExrConfig.default_value;
            m_ctx = fcAPI.fcExrCreateContext(ref conf);

            // initialize render targets
            {
                m_frame_buffer = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.ARGBFloat);
                m_frame_buffer.wrapMode = TextureWrapMode.Repeat;
                m_frame_buffer.Create();
            }

            // initialize command buffers
            {

                m_cb_copy_fb = new CommandBuffer();
                m_cb_copy_fb.name = "DepthRecorder: Copy FrameBuffer";
                m_cb_copy_fb.SetRenderTarget(m_frame_buffer);
                m_cb_copy_fb.DrawMesh(m_quad, Matrix4x4.identity, m_mat_copy, 0, 6);
            }

            AddCommandBuffers();
        }

        void OnDestroy() { Release(); }
        public override void Release()
        {

            RemoveCommandBuffers();
            _camera = null;

            if (m_cb_copy_fb != null)
            {
                m_cb_copy_fb.Release();
                m_cb_copy_fb = null;
            }

            if (m_frame_buffer != null)
            {
                m_frame_buffer.Release();
                m_frame_buffer = null;
            }

            fcAPI.fcGuard(() =>
            {
                EraseCallbacks();
                fcAPI.fcExrDestroyContext(m_ctx);
                m_ctx.ptr = System.IntPtr.Zero;
            });
        }

        IEnumerator OnPostRender()
        {
            yield return new WaitForEndOfFrame()/*Application.isPlaying? new WaitForEndOfFrame() : null*/;
            DoExport();
            currentFrame++;
            IsRenderingWait = false;
        }
    }
}

#endif