#if UNITY_EDITOR && !NO_UTJ

using UnityEditor;
using UnityEngine;
using System.Collections;
using RenderFormat = Slate.Prefs.RenderSettings.RenderFormat;

namespace Slate{

    public class RenderWindow : EditorWindow
    {
        enum VideoStep
        {
            None = -1,
            Audio = 0,
            Video = 1
        }

        private Prefs.RenderSettings settings;
        private ImageSequenceRecorder recorder;
        // @modify slate sequencer
        // @xieheng
        private MovieRecorder movieRecorder;
        private WAVRecorder wavRecorder;
        private VideoStep videoStep = VideoStep.None;
        private bool errorRecorder = false;
        private Camera sceneCamera = null;
        //@end
        private FixDeltaTime deltaTimeFixer;
        private bool isRendering;

        public bool IsUseRenderingWait = false;
        public bool IsRenderingWait = false;
        public bool IsStartAudioRecord = false;

        private Cutscene cutscene
        {
            get { return CutsceneEditor.current != null ? CutsceneEditor.current.cutscene : null; }
        }

        public static void Open()
        {
            var window = CreateInstance<RenderWindow>();
            window.ShowUtility();
        }

        void OnEnable()
        {
            titleContent = new GUIContent("Slate Render Utility");
            settings = Prefs.renderSettings;
            minSize = new Vector2(410, 250);
        }

        void OnDisable()
        {
            Prefs.renderSettings = settings;
            Done();
        }

        void Update()
        {

            if (!isRendering || cutscene == null)
            {
                return;
            }

            if (recorder != null)
            {
                if (recorder.IsUseRenderingWait && !recorder.IsRenderingWait)
                {
                    recorder.IsRenderingWait = true;
                    cutscene.currentTime += 1f / settings.framerate;
                    if (cutscene.cameraTrack!= null && cutscene.currentTime >= cutscene.cameraTrack.endTime)
                    {
                        Done();
                    }
                }
            }
            else if (movieRecorder != null)
            {
                if (movieRecorder.IsUseRenderingWait && !movieRecorder.IsRenderingWait)
                {
                    movieRecorder.IsRenderingWait = true;
                    cutscene.currentTime += 1f / settings.framerate;
                    if (cutscene.currentTime >= cutscene.cameraTrackCustom.endTime)
                    {
                        Done();
                    }
                }
            }
            else
            {
                

                //cutscene.currentTime += 1f / settings.framerate;
                cutscene.currentTime = Time.realtimeSinceStartup - audioTimestamp;
                if (cutscene.currentTime >= cutscene.startPlayTime && !IsStartAudioRecord)
                {
                    IsStartAudioRecord = true;
                    bool started = wavRecorder.StartRecording();
                    if (!started)
                    {
                        videoStep = VideoStep.None;
                        movieRecorder.StopRecording();
                        errorRecorder = true;
                        Done();
                    }
                }

                if (cutscene.currentTime >= cutscene.cameraTrackCustom.endTime)
                {
                    Done();
                }
            }
        }

        void OnGUI()
        {
            if (isRendering)
            {
                Repaint();
            }

            settings.renderFormat = (RenderFormat)EditorGUILayout.EnumPopup("Render Format", settings.renderFormat);
            GUILayout.BeginHorizontal();
            settings.folderName = EditorGUILayout.TextField("SubFolder In Project", settings.folderName);
            if (GUILayout.Button("F", GUILayout.Width(20), GUILayout.Height(14)))
            {
                OpenTargetFolder();
            }
            GUILayout.EndHorizontal();
            settings.fileName = EditorGUILayout.TextField("Filename", settings.fileName);

            GUILayout.BeginVertical("box");

            if (settings.renderFormat == RenderFormat.PNGImageSequence || settings.renderFormat == RenderFormat.EXRImageSequence)
            {
                settings.splitRenderBuffer = EditorGUILayout.Toggle("Render Passes", settings.splitRenderBuffer);
            }
            else if (settings.renderFormat != RenderFormat.DepthImageSequence)
            {
                settings.resolutionWidth  = Mathf.Clamp(EditorGUILayout.IntField("Resolution Width",  settings.resolutionWidth),  64, 1920);
                settings.resolutionHeight = Mathf.Clamp(EditorGUILayout.IntField("Resolution Height", settings.resolutionHeight), 64, 1080);

                settings.resolutionWidth  = settings.resolutionWidth  / 2 * 2;
                settings.resolutionHeight = settings.resolutionHeight / 2 * 2;
            }

            settings.framerate = Mathf.Clamp(EditorGUILayout.IntField("Frame Rate", settings.framerate), 2, 60);
            EditorGUILayout.LabelField("Resolution", EditorTools.GetGameViewSize().ToString("0"));
            if(settings.renderFormat == RenderFormat.AVIVideo)
            {
                settings.skipAudio = EditorGUILayout.Toggle("Skip Audio", settings.skipAudio);
            }

            EditorGUILayout.HelpBox("Rendering Resolution is taken from the Game Window.\nYou can create custom resolutions with the '+' button through the second dropdown in the Game window toolbar (where it usually reads 'Free Aspect').", MessageType.None);

            GUILayout.EndVertical();

            if (cutscene == null)
            {
                EditorGUILayout.HelpBox("Cutscene is null or the Cutscene Editor is not open", MessageType.Error);
            }

            if (cutscene != null && cutscene.cameraTrack == null)
            {
                EditorGUILayout.HelpBox("Cutscene has no Camera Track", MessageType.Warning);
            }

            GUI.enabled = cutscene != null && cutscene.cameraTrack != null && !isRendering;
            if (GUILayout.Button(isRendering ? "RENDERING..." : "RENDER", GUILayout.Height(50)))
            {
                Begin();
            }
            GUI.enabled = true;

            if (isRendering && GUILayout.Button("CANCEL"))
            {
                Done();
                videoStep = VideoStep.None;
            }

            // @modify slate sequencer
            // @xieheng
            // show the error on ui
            if (errorRecorder)
            {
                EditorGUILayout.HelpBox("Some error here! Please try again", MessageType.Error);
            }
            //@end
        }

        void Begin()
        {
            // @modify slate sequencer
            // @xieheng
            // show the error on ui
            errorRecorder = false;
            //@end

            if (isRendering)
            {
                return;
            }

            cutscene.Rewind();
            CutsceneEditor.current.DisableAllAudioTrack(true);
            //EditorApplication.ExecuteMenuItem("Window/Game");
            isRendering = true;
            cutscene.currentTime = cutscene.startPlayTime;//cutscene.cameraTrack.startTime;
            cutscene.Sample();
            CutsceneEditor.OnStopInEditor += Done;

            if (settings.renderFormat == RenderFormat.PNGImageSequence || settings.renderFormat == RenderFormat.EXRImageSequence
                || settings.renderFormat == RenderFormat.DepthImageSequence)
            {

                if (Application.isPlaying)
                {
                    deltaTimeFixer = DirectorCamera.current.cam.GetAddComponent<FixDeltaTime>();
                    deltaTimeFixer.targetFramerate = settings.framerate;
                }

                if (settings.renderFormat == RenderFormat.PNGImageSequence)
                {
                    recorder = DirectorCamera.current.cam.GetAddComponent<PngRecorder>();
                }

                if (settings.renderFormat == RenderFormat.EXRImageSequence)
                {
                    recorder = DirectorCamera.current.cam.GetAddComponent<ExrRecorder>();
                }

                if (settings.renderFormat == RenderFormat.DepthImageSequence)
                {
                    recorder = DirectorCamera.current.cam.GetAddComponent<DepthRecorder>();
                }

                recorder.dirName = settings.folderName;
                recorder.fileName = settings.fileName;
                recorder.captureGBuffer = settings.splitRenderBuffer;
                recorder.captureFramebuffer = true;
                recorder.Initialize();
            }

            // @modify slate sequencer
            // @xieheng
            // support movie recording
            if (settings.renderFormat == RenderFormat.AVIVideo)
            {
                GameViewUtils.SetCurrentResolution(settings.resolutionWidth, settings.resolutionHeight);
                if(settings.skipAudio)
                {
                    BeginVideo();
                }
                else
                {
                    BeginAudio();
                }
            }
            //@end
        }

        void Done()
        {
            if (!isRendering)
            {
                return;
            }

            // @modify slate sequencer
            // @xieheng
            // support movie recording
            if (settings.renderFormat == RenderFormat.AVIVideo)
            {
                if (wavRecorder != null)
                {
                    wavRecorder.StopRecording();
                    DestroyImmediate(wavRecorder, true);
                    wavRecorder = null;
                }
                if (videoStep == VideoStep.Audio)
                {
                    BeginVideo();
                    return;
                }

                if (videoStep == VideoStep.Video)
                {
                    if (movieRecorder != null)
                    {
                        movieRecorder.StopRecording();
                        DestroyImmediate(movieRecorder, true);
                        movieRecorder = null;
                    }
                }
            }
            //@end

            if (recorder != null)
            {
                recorder.IsRenderingWait = false;
            }

            if (movieRecorder != null)
            {
                movieRecorder.IsRenderingWait = false;
            }

            CutsceneEditor.OnStopInEditor -= Done;
            isRendering = false;

            if (recorder != null)
            {
                DestroyImmediate(recorder, true);
            }

            if (deltaTimeFixer != null)
            {
                DestroyImmediate(deltaTimeFixer, true);
            }

            SetSceneCameraEnabled(true);

            cutscene.Rewind();
            OpenTargetFolder();
        }

        void OpenTargetFolder()
        {
            // @modify slate sequencer
            // @xieheng
            // support movie recording

            if (!errorRecorder)
            {
                var path = Application.dataPath.Replace("/Assets", "/" + settings.folderName + "/");
                System.IO.Directory.CreateDirectory(path); //ensure folder exists
                Application.OpenURL(path);
            }

            //@end
        }

        private float audioTimestamp = 0;

        void BeginAudio()
        {
            audioTimestamp = Time.realtimeSinceStartup ;
            IsStartAudioRecord = false;
            cutscene.currentTime = 0;
            cutscene.Sample();

            SetSceneCameraEnabled(false);
            videoStep = VideoStep.Audio;

            wavRecorder = DirectorCamera.current.cam.GetAddComponent<WAVRecorder>();

            wavRecorder.FrameRate = settings.framerate;
            wavRecorder.TargetFilePath = settings.folderName;
            wavRecorder.TargetFileName = settings.fileName;

            
        }

        void BeginVideo()
        {
            cutscene.currentTime = cutscene.startPlayTime;
            cutscene.Sample();

            SetSceneCameraEnabled(true);
            videoStep = VideoStep.Video;

            movieRecorder = DirectorCamera.current.cam.GetAddComponent<AVIRecorder>();
            movieRecorder.Initialize();

            movieRecorder.FrameRate = settings.framerate;
            movieRecorder.TargetFilePath = settings.folderName;
            movieRecorder.TargetFileName = settings.fileName;
            movieRecorder.CaptureGBuffer = settings.splitRenderBuffer;
            movieRecorder.CaptureFramebuffer = true;
            movieRecorder.IsSkipAudio = settings.skipAudio;

            bool started = movieRecorder.StartRecording();
            if (!started)
            {
                videoStep = VideoStep.None;
                movieRecorder.StopRecording();
                errorRecorder = true;
                Done();
            }
        }

        private void SetSceneCameraEnabled(bool enabled)
        {
            if (sceneCamera == null)
            {
                sceneCamera = Camera.main;
            }

            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(enabled);
            }

            DirectorCamera.current.cam.enabled = enabled;
        }
    }
}

#endif