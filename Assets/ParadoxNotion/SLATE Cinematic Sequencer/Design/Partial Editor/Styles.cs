#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Slate{

	///Images and GUIStyles for the editor
	[InitializeOnLoad]
	public static class Styles {

		public static Texture2D expressionIcon;
		public static Texture2D stripes;
		public static Texture2D magnetIcon;
/// @modify slate sequencer
/// @TQ
        public static Texture2D magnetIcon1;
        public static Texture2D connect1;
        public static Texture2D connect2;
        public static Texture2D keyIcon2;
        public static Texture2D clip1;
        public static Texture2D clip2;
        public static Texture2D cam1;
        public static Texture2D cam2;
        public static Texture2D recordStop;
        public static Texture2D showIcon;
        public static Texture2D upload;
        public static Texture2D showIcon3;
        public static Texture2D showIcon4;
        public static Texture2D curveIcon1;
        public static Texture2D buttonBG;
        public static Texture2D nextCutIcon;
        public static Texture2D hideAllIcon;
        public static Texture2D DisplayAllIcon;
        public static Texture2D HideTrackIcon;
        public static Texture2D DisplayTrackIcon;
        public static Texture2D NextFrameIcon;
        public static Texture2D PrevFrameIcon;
        public static Texture2D StartPointIcon;
        public static Texture2D EndPointIcon;
        public static Texture2D DisplayMIcon;
        public static Texture2D HideMIcon;
        public static Texture2D HideAIcon;
        public static Texture2D DisplaySIcon;
        public static Texture2D HideSIcon;
        public static Texture2D CutReturnIcon;
        /// end

        public static Texture2D lockIcon;
		public static Texture2D hiddenIcon;
		public static Texture2D clockIcon;
		public static Texture2D keyIcon;
		public static Texture2D nextKeyIcon;
		public static Texture2D previousKeyIcon;
		public static Texture2D recordIcon;
		
		public static Texture2D playIcon; 
		public static Texture2D playReverseIcon;
		public static Texture2D stepIcon;
		public static Texture2D stepReverseIcon;
		public static Texture2D stopIcon;
		public static Texture2D pauseIcon;
		public static Texture2D loopIcon;
		public static Texture2D pingPongIcon;

		public static Texture2D carretIcon;
		public static Texture2D cutsceneIconOpen;
		public static Texture2D cutsceneIconClose;

		public static Texture2D cutsceneIcon;
		public static Texture2D slateIcon;
        // @modify slate sequencer
        // @rongxia
        public static Texture2D editorLogoIcon;
        // @end

        public static Texture2D borderShadowsImage;

		public static Texture2D gearIcon;
		public static Texture2D plusIcon;
		public static Texture2D trashIcon;
		public static Texture2D curveIcon;

		public static Texture2D alembicIcon;

		public static Texture2D dopeKey;
		public static Texture2D dopeKeySmooth;
		public static Texture2D dopeKeyLinear;
		public static Texture2D dopeKeyConstant;
		
		public static Texture2D dopeKeyIconBig = EditorGUIUtility.FindTexture("blendKey");
		
		public static Texture2D cameraIcon     = EditorGUIUtility.FindTexture("Camera Icon");
		public static Texture2D audioIcon      = EditorGUIUtility.FindTexture("AudioClip Icon");
		public static Texture2D animationIcon  = EditorGUIUtility.FindTexture("NavMeshAgent Icon");
		public static Texture2D animatorIcon   = EditorGUIUtility.FindTexture("Animator Icon");
		public static Texture2D actionIcon     = EditorGUIUtility.FindTexture("CircleCollider2D Icon");
		public static Texture2D sceneIcon      = EditorGUIUtility.FindTexture("SceneAsset Icon");
		public static Texture2D errorIconSmall = EditorGUIUtility.FindTexture("CollabError");

		public static Color recordingColor = new Color(1,0.5f,0.5f);

		private static GUISkin styleSheet;

		static Styles(){
			Load();
		}

		[InitializeOnLoadMethod]
		public static void Load(){
            // @modify slate sequencer
            // @rongxia

            // 			dopeKey            = (Texture2D)Resources.Load("DopeKey");
            // 			dopeKeySmooth      = (Texture2D)Resources.Load("DopeKeySmooth");
            // 			dopeKeyLinear      = (Texture2D)Resources.Load("DopeKeyLinear");
            // 			dopeKeyConstant    = (Texture2D)Resources.Load("DopeKeyConstant");
            // 			
            // 			keyIcon            = (Texture2D)Resources.Load("KeyIcon");
            // 			nextKeyIcon        = (Texture2D)Resources.Load("NextKeyIcon");
            // 			previousKeyIcon    = (Texture2D)Resources.Load("PreviousKeyIcon");
            // 			
            // 			expressionIcon     = (Texture2D)Resources.Load("ExpressionIcon");
            // 			stripes            = (Texture2D)Resources.Load("Stripes");
            // 			magnetIcon         = (Texture2D)Resources.Load("MagnetIcon");
            // 			lockIcon           = (Texture2D)Resources.Load("LockIcon");
            // 			hiddenIcon         = (Texture2D)Resources.Load("HiddenIcon");
            // 			clockIcon          = (Texture2D)Resources.Load("ClockIcon");
            // 			recordIcon         = (Texture2D)Resources.Load("RecordIcon");
            // 			playIcon           = (Texture2D)Resources.Load("PlayIcon");
            // 			playReverseIcon    = (Texture2D)Resources.Load("PlayReverseIcon");
            // 			stepIcon           = (Texture2D)Resources.Load("StepIcon");
            // 			stepReverseIcon    = (Texture2D)Resources.Load("StepReverseIcon");
            // 			loopIcon           = (Texture2D)Resources.Load("LoopIcon");
            // 			pingPongIcon       = (Texture2D)Resources.Load("PingPongIcon");
            // 			stopIcon           = (Texture2D)Resources.Load("StopIcon");
            // 			pauseIcon          = (Texture2D)Resources.Load("PauseIcon");
            // 			carretIcon         = (Texture2D)Resources.Load("CarretIcon");
            // 			cutsceneIconOpen   = (Texture2D)Resources.Load("CutsceneIconOpen");
            // 			cutsceneIconClose  = (Texture2D)Resources.Load("CutsceneIconClose");
            // 			cutsceneIcon       = (Texture2D)Resources.Load("Cutscene Icon");
            // 			slateIcon          = (Texture2D)Resources.Load("SLATEIcon");
            // 			borderShadowsImage = (Texture2D)Resources.Load("BorderShadows");
            // 			gearIcon           = (Texture2D)Resources.Load("GearIcon");
            // 			plusIcon           = (Texture2D)Resources.Load("PlusIcon");
            // 			trashIcon          = (Texture2D)Resources.Load("TrashIcon");
            // 			curveIcon          = (Texture2D)Resources.Load("CurveIcon");
            // 			alembicIcon        = (Texture2D)Resources.Load("AlembicIcon");

#if SOTRYENGINE_SDK
            const string assetDir = "Assets/DigitalSkyStoryEngine/Editor/EditorResources/";
#else
            const string assetDir = "Assets/ParadoxNotion/SLATE Cinematic Sequencer/Design/Editor/EditorResources/";
#endif

            dopeKey = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "DopeKey.png");
            dopeKeySmooth = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "DopeKeySmooth.png");
            dopeKeyLinear = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "DopeKeyLinear.png");
            dopeKeyConstant = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "DopeKeyConstant.png");

            keyIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "KeyIcon.png");
            nextKeyIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "NextKeyIcon.png");
            previousKeyIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "PreviousKeyIcon.png");

            expressionIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "ExpressionIcon.png");
            stripes = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Stripes.png");
            magnetIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "MagnetIcon.png");
/// @modify slate sequencer
/// @TQ
            magnetIcon1 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "MagnetIcon2.png");
            connect1 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "connect1.png");
            connect2 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "connect2.png");
            keyIcon2 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "KeyIcon2.png");
            clip1 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "clip1.png");
            clip2 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "clip2.png");
            cam1 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "cam1.png");
            cam2 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "cam2.png");
            recordStop = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "RecordStop.png");
            showIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "showIcon.png");
            upload = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "upload.png");
            showIcon3 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "show3.png");
            showIcon4 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "show4.png");
            curveIcon1 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "CurveIcon1.png");
            buttonBG = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "buttonBG.png");
            nextCutIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "nextCut.png");
            hideAllIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Hide_all.png");

            DisplayAllIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Display_all.png");
            HideTrackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Hide_track.png");
            DisplayTrackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Display_track.png");
            NextFrameIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Next_frame.png");
            PrevFrameIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Previous_frame.png");
            StartPointIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Start_point.png");
            EndPointIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "End_point.png");
            DisplayMIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Display_m.png");
            HideMIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Hide_m.png");
            HideAIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Hide_a.png");
            DisplaySIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Display_s.png");
            HideSIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Hide_s.png");
            CutReturnIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Cut_Return.png");
            /// end

            lockIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "LockIcon.png");
            hiddenIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "HiddenIcon.png");
            clockIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "ClockIcon.png");
            recordIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "RecordIcon.png");
            playIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "PlayIcon.png");
            playReverseIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "PlayReverseIcon.png");
            stepIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "StepIcon.png");
            stepReverseIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "StepReverseIcon.png");
            loopIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "LoopIcon.png");
            pingPongIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "PingPongIcon.png");
            stopIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "StopIcon.png");
            pauseIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "PauseIcon.png");
            carretIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "CarretIcon.png");
            cutsceneIconOpen = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "CutsceneIconOpen.png");
            cutsceneIconClose = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "CutsceneIconClose.png");
            cutsceneIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "Cutscene Icon.png");
            slateIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "SLATEIcon.png");
            borderShadowsImage = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "BorderShadows.png");
            gearIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "GearIcon.png");
            plusIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "PlusIcon.png");
            trashIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "TrashIcon.png");
            curveIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "CurveIcon.png");
            alembicIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "AlembicIcon.png");

            editorLogoIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDir + "DigitalSky.png");

            styleSheet = AssetDatabase.LoadAssetAtPath<GUISkin>(assetDir + "StyleSheet.guiskin");

            // @end
        }

        ///Get a white 1x1 texture
        public static Texture2D whiteTexture{
			get {return EditorGUIUtility.whiteTexture;}
		}


		private static GUIStyle _shadowBorderStyle;
		public static GUIStyle shadowBorderStyle{
			get {return _shadowBorderStyle != null? _shadowBorderStyle : _shadowBorderStyle = styleSheet.GetStyle("ShadowBorder");}
		}

		private static GUIStyle _clipBoxStyle;
		public static GUIStyle clipBoxStyle{
			get {return _clipBoxStyle != null? _clipBoxStyle : _clipBoxStyle = styleSheet.GetStyle("ClipBox");}
		}

		private static GUIStyle _clipBoxFooterStyle;
		public static GUIStyle clipBoxFooterStyle{
			get {return _clipBoxFooterStyle != null? _clipBoxFooterStyle : _clipBoxFooterStyle = styleSheet.GetStyle("ClipBoxFooter");}
		}

		private static GUIStyle _clipBoxHorizontalStyle;
		public static GUIStyle clipBoxHorizontalStyle{
			get {return _clipBoxHorizontalStyle != null? _clipBoxHorizontalStyle : _clipBoxHorizontalStyle = styleSheet.GetStyle("ClipBoxHorizontal");}
		}

		private static GUIStyle _timeBoxStyle;
		public static GUIStyle timeBoxStyle{
			get {return _timeBoxStyle != null? _timeBoxStyle : _timeBoxStyle = styleSheet.GetStyle("TimeBox");}
		}

		private static GUIStyle _headerBoxStyle;
		public static GUIStyle headerBoxStyle{
			get {return _headerBoxStyle != null? _headerBoxStyle : _headerBoxStyle = styleSheet.GetStyle("HeaderBox");}
		}

		private static GUIStyle _hollowFrameStyle;
		public static GUIStyle hollowFrameStyle{
			get {return _hollowFrameStyle != null? _hollowFrameStyle : _hollowFrameStyle = styleSheet.GetStyle("HollowFrame");}
		}

		private static GUIStyle _hollowFrameHorizontalStyle;
		public static GUIStyle hollowFrameHorizontalStyle{
			get {return _hollowFrameHorizontalStyle != null? _hollowFrameHorizontalStyle : _hollowFrameHorizontalStyle = styleSheet.GetStyle("HollowFrameHorizontal");}
		}

		private static GUIStyle _leftLabel;
		public static GUIStyle leftLabel{
			get
			{
				if (_leftLabel != null){
					return _leftLabel;
				}
				_leftLabel = new GUIStyle("label");
				_leftLabel.alignment = TextAnchor.MiddleLeft;
				return _leftLabel;
			}
		}

		private static GUIStyle _centerLabel;
		public static GUIStyle centerLabel{
			get
			{
				if (_centerLabel != null){
					return _centerLabel;
				}
				_centerLabel = new GUIStyle("label");
				_centerLabel.alignment = TextAnchor.MiddleCenter;
				return _centerLabel;
			}
		}
/// @modify slate sequencer
/// @TQ
        private static GUIStyle _bottomLabel;
        public static GUIStyle bottomLabel
        {
            get
            {
                if (_bottomLabel != null)
                {
                    return _bottomLabel;
                }
                _bottomLabel = new GUIStyle("label");
                _bottomLabel.alignment = TextAnchor.LowerCenter;
                return _bottomLabel;
            }
        }
/// end
    }
}

#endif