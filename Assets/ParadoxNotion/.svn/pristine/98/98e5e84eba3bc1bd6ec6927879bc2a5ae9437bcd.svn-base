#if UNITY_EDITOR

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1

    #define AVPRO_MOVIECAPTURE_GLISSUEEVENT_52

#endif

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Slate
{
    public class AVICapturer
    {
        #region Data

        private Camera _camera = null;

        private RenderTexture _target = null;

        private CommandBuffer _commandBuffer = null;

        private bool _isRealTime = true;

        private int _targetWidth = 800;

        private int _targetHeight = 640;

        private int _frameRate = 30;

        //private int _codecIndex = 1;//MJPEG Compressor

        private static readonly string[] _videoCodecNames = {
                                                                "MJPEG Compressor",
                                                                "DV Video Encoder"
                                                            };

        private bool _hasAudio = true;

        private AudioCapturer _audioCapture = null;

        private int _audioCodecIndex = -1;

        private int _audioDeviceIndex = -1;

        private int _unityAudioSampleRate = -1;

        private int _unityAudioChannelCount = -1;

        private int _handle = -1;

        AVProMovieCapturePlugin.PixelFormat _pixelFormat = AVProMovieCapturePlugin.PixelFormat.RGBA32;

        private bool _isTopDown = false;

#if AVPRO_MOVIECAPTURE_GLISSUEEVENT_52

        protected System.IntPtr _renderEventFunction = System.IntPtr.Zero;

        protected System.IntPtr _freeEventFunction = System.IntPtr.Zero;

#endif

        private bool _isCapturing = false;

        private string _filename = string.Empty;

        private string _targetPath = string.Empty;

        private string _targetName = string.Empty;

        private int _resolutionWidth = 0;

        #endregion

        #region Public

        public void Init()
        {
            try
            {
                if (AVProMovieCapturePlugin.Init())
                {
                    Debug.Log("[AVICapturer] Init plugin version: " + AVProMovieCapturePlugin.GetPluginVersion().ToString("F2") + " with GPU " + SystemInfo.graphicsDeviceVersion);
#if AVPRO_MOVIECAPTURE_GLISSUEEVENT_52
                    _renderEventFunction = AVProMovieCapturePlugin.GetRenderEventFunc();
                    _freeEventFunction = AVProMovieCapturePlugin.GetFreeResourcesEventFunc();
#endif
                }
                else
                {
                    Debug.LogError("[AVICapturer] Failed to initialise plugin version: " + AVProMovieCapturePlugin.GetPluginVersion().ToString("F2") + " with GPU " + SystemInfo.graphicsDeviceVersion);
                }
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError("[AVICapturer] Unity couldn't find the DLL, did you move the 'Plugins' folder to the root of your project?");
                throw e;
            }

            if (_hasAudio)
            {
                _audioCapture = new AudioCapturer();
            }
        }

        public void Deinit()
        {
            if (_audioCapture != null)
            {
                _audioCapture.Deinit();
            }

            AVProMovieCapturePlugin.Deinit();
        }

        public bool StartCapture()
        {
            if (_camera == null)
            {
                Debug.LogError("[AVICapturer]Camera is invalid");
                return false;
            }

            if (!_isCapturing && PrepareCapture())
            {
                _camera.AddCommandBuffer(CameraEvent.AfterEverything, _commandBuffer);

                if (_audioCapture != null && _audioDeviceIndex < 0 && _hasAudio)
                {
                    _audioCapture.FlushBuffer();
                }

                AVProMovieCapturePlugin.Start(_handle);

                if (System.IO.File.Exists(_filename))
                {
                    _isCapturing = true;
                    Debug.Log("[AVICapturer]Starting capture");
                }
                else
                {
                    _isCapturing = false;
                    Debug.LogError("[AVICapturer]Failed to create the '.avi' file");
                }
            }

            return _isCapturing;
        }

        public void StopCapture()
        {
            AVProMovieCapturePlugin.SetTexturePointer(_handle, System.IntPtr.Zero);

            if (_isCapturing)
            {
                _camera.RemoveCommandBuffer(CameraEvent.AfterEverything, _commandBuffer);
                Debug.Log("[AVICapturer] Stopping capture");
            }

#if AVPRO_MOVIECAPTURE_GLISSUEEVENT_52
            GL.IssuePluginEvent(_freeEventFunction, AVProMovieCapturePlugin.PluginID | (int)AVProMovieCapturePlugin.PluginEvent.FreeResources);
#else
		    GL.IssuePluginEvent(AVProMovieCapturePlugin.PluginID | (int)AVProMovieCapturePlugin.PluginEvent.FreeResources);
#endif

            if (_handle >= 0)
            {
                AVProMovieCapturePlugin.Stop(_handle, false);
                AVProMovieCapturePlugin.FreeRecorder(_handle);
                _handle = -1;
            }

            if (_isCapturing)
            {
                CompressAVIFile();
            }

            _isCapturing = false;
        }

        public void UpdateFrame()
        {
            if (!_isCapturing || _camera == null)
                return;

            int dueAcc = 0;
            int dueMax = (1000 / _frameRate) - 5;
            while (_handle >= 0 && !AVProMovieCapturePlugin.IsNewFrameDue(_handle))
            {
                System.Threading.Thread.Sleep(1);

                //@modify slate sequencer
                //@by xieheng
                //@prevent the process to be blocked
                dueAcc++;
                if (dueAcc >= dueMax)
                {
                    Debug.LogWarning("[AVICapturer]The new frame is not due");
                    break; 
                }
                //@end
            }

            if (_handle >= 0)
            {
                if (_audioCapture != null && _audioDeviceIndex < 0 && _hasAudio && _isRealTime)
                {
                    int audioDataLength = 0;
                    System.IntPtr audioDataPtr = _audioCapture.ReadData(out audioDataLength);
                    if (audioDataPtr != IntPtr.Zero && audioDataLength > 0)
                    {
                        AVProMovieCapturePlugin.EncodeAudio(_handle, audioDataPtr, (uint)audioDataLength);
                    }
                }

                AVProMovieCapturePlugin.SetTexturePointer(_handle, _target.GetNativeTexturePtr());

#if AVPRO_MOVIECAPTURE_GLISSUEEVENT_52
                GL.IssuePluginEvent(_renderEventFunction, AVProMovieCapturePlugin.PluginID | (int)AVProMovieCapturePlugin.PluginEvent.CaptureFrameBuffer | _handle);
#else
		        GL.IssuePluginEvent(AVProMovieCapturePlugin.PluginID | (int)AVProMovieCapturePlugin.PluginEvent.CaptureFrameBuffer | _handle);
#endif
            }
        }

        public void AttachCamera(Camera camera)
        {
            if (!_isCapturing)
            {
                _camera = camera;
            }
        }

        public void OnAudioFilterRead(float[] data, int channels)
        {
            if (_hasAudio && _audioCapture != null && _isCapturing)
            {
                _audioCapture.OnAudioFilterRead(data, channels);
            }
        }

        public bool IsCapturing
        {
            get { return _isCapturing; }
        }

        public int FrameRate
        {
            set { _frameRate = Mathf.Max(15, Mathf.Min(60, value)); }//15 <= framerate <= 60
            get { return _frameRate; }
        }

        public string TargetFilePath
        {
            set { _targetPath = value; }
            get { return _targetPath; }
        }

        public string TargetFileName
        {
            set { _targetName = value; }
            get { return _targetName; }
        }

        public int ResolutionWidth
        {
            set { _resolutionWidth = value; }
            get { return _resolutionWidth; }
        }

        #endregion

        #region Private

        private string GenerateFilename()
        {
            string projectFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
            string fileFolder = System.IO.Path.Combine(projectFolder, _targetPath);
            string filename = System.IO.Path.Combine(fileFolder, _targetName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".avi");

            return filename;
        }

        private bool PrepareCapture()
        {
            _filename = GenerateFilename();

            _targetWidth = Mathf.FloorToInt(_camera.pixelRect.width);
            _targetHeight = Mathf.FloorToInt(_camera.pixelRect.height);

            if (_resolutionWidth > 0)
            {
                _targetHeight = (int)(_targetHeight * (_resolutionWidth / (float)_targetWidth));
                _targetWidth = _resolutionWidth;
            }

            if (_target == null)
            {
                _target = RenderTexture.GetTemporary(_targetWidth, _targetHeight, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
                _target.Create();
                _target.name = "Target";

                int tid = Shader.PropertyToID("_TmpFrameBuffer");
                Shader shader = FrameCapturerUtils.GetFrameBufferCopyShader();
                Mesh mesh = FrameCapturerUtils.CreateFullscreenQuad();
                Material material = new Material(shader);
                if (_camera.targetTexture != null)
                {
                    material.EnableKeyword("OFFSCREEN");
                }

                _commandBuffer = new CommandBuffer();
                _commandBuffer.name = "MovieRecorder: copy frame buffer";
                _commandBuffer.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Bilinear);
                _commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tid);
                _commandBuffer.SetRenderTarget(_target);
                _commandBuffer.DrawMesh(mesh, Matrix4x4.identity, material, 0, 0);
                _commandBuffer.ReleaseTemporaryRT(tid);
            }

            _targetWidth = NextMultipleOf4(_targetWidth);
            _targetHeight = NextMultipleOf4(_targetHeight);

            if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
            {
                _pixelFormat = AVProMovieCapturePlugin.PixelFormat.BGRA32;
                //@modify slate sequencer
                //@by xieheng
                //can not flip vertical on the Android platform
#if UNITY_STANDALONE
                _isTopDown = true;
#elif UNITY_ANDROID
                _isTopDown = false;
#endif
                //@end
            }
            else
            {
                //@modify slate sequencer
                //@by xieheng
                //I dont sure whether should flip vertical on the Android platform, comment
#if UNITY_STANDALONE
                _isTopDown = false;
#elif UNITY_ANDROID
                //_isTopDown = true;
#endif
                //@end
            }

            if (_hasAudio && _audioDeviceIndex < 0 && _audioCapture != null)
            {
                _audioCapture.Init();

                _unityAudioSampleRate = AudioSettings.outputSampleRate;
                _unityAudioChannelCount = _audioCapture.ChannelCount;
            }

            int codecIndex = GetVideoCodecIndex();
            _handle = AVProMovieCapturePlugin.CreateRecorderAVI(_filename,
                                                                (uint)_targetWidth,
                                                                (uint)_targetHeight,
                                                                _frameRate,
                                                                (int)(_pixelFormat),
                                                                _isTopDown,
                                                                codecIndex,
                                                                _hasAudio,
                                                                _unityAudioSampleRate,
                                                                _unityAudioChannelCount,
                                                                _audioDeviceIndex,
                                                                _audioCodecIndex,
                                                                _isRealTime,
                                                                false,
                                                                true);

            if (_handle < 0)
            {
                Debug.LogError("[AVICapturer] Failed to create recorder");
                StopCapture();
            }

            return (_handle >= 0);
        }

        private static int GetVideoCodecIndex()
        {
            int codecIndex = -1;//Uncompressed
            int codecCount = AVProMovieCapturePlugin.GetNumAVIVideoCodecs();

            for (int i = 0; i < _videoCodecNames.Length; i++)
            {
                for (int k = 0; k < codecCount; k++)
                {
                    string codecName = AVProMovieCapturePlugin.GetAVIVideoCodecName(k);
                    if (codecName == _videoCodecNames[i])
                    {
                        codecIndex = k;
                        Debug.LogFormat("[AVICapturer]Use '{0}' video codec", codecName);
                        return codecIndex;
                    }
                }
            }

            Debug.Log("[AVICapturer]Use 'Uncompressed' video codec");
            return codecIndex;
        }

        private static int NextMultipleOf4(int value)
        {
            return (value + 3) & ~0x03;
        }

        private void CompressAVIFile()
        {
            if (System.IO.File.Exists(_filename))
            {
#if SOTRYENGINE_SDK
                string ffmpegFilename = System.IO.Path.Combine(Application.dataPath, @"DigitalSkyStoryEngine\EditorPlugins\ffmpeg\ffmpeg.exe");
#else
                string ffmpegFilename = System.IO.Path.Combine(Application.dataPath, @"ParadoxNotion\SLATE Cinematic Sequencer\Utility\UTJ\FrameCapturer\MovieCapturePro\Plugins\ffmpeg\ffmpeg.exe");
#endif
                string sourceFilename = _filename;
                string targetFilename = sourceFilename.Replace(".avi", ".mov");

                string ffmpegCommand = string.Format("-threads 4 -i {0} -vcodec libx264 -preset fast -crf 18 -y -acodec libmp3lame -ab 128k {1}",
                                                     sourceFilename,
                                                     targetFilename);

                System.Diagnostics.Process p = new System.Diagnostics.Process();//建立外部调用线程
                p.StartInfo.FileName = ffmpegFilename;
                p.StartInfo.Arguments = ffmpegCommand;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
            }
        }

#endregion
    }
}

#endif