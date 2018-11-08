#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Slate.ActionClips;

namespace Slate{

	public class CutsceneEditor : EditorWindow{

		enum EditorPlayback{
			Stoped,
			PlayingForwards,
			PlayingBackwards
		}

		public static CutsceneEditor current;
		public static event System.Action OnStopInEditor;

		private Cutscene _cutscene;
		private int _cutsceneID;
        
		public float length{
			get {return cutscene.length;}
			set {cutscene.length = value;}
		}

/// @modify slate sequencer
/// add by TQ
        public float startPlayTime
        {
            get
            {
                return cutscene.startPlayTime;
            }
            set
            {
                cutscene.startPlayTime = value;
            }
        }
/// end
        public float viewTimeMin{
			get {return cutscene!= null ? cutscene.viewTimeMin : 0;}
			set {cutscene.viewTimeMin = value;}
		}

        public float viewCurveBoxHeightMax
        {
            get { return cutscene.viewCurveBoxHeightMax; }
            set
            {
                cutscene.viewCurveBoxHeightMax = value;
            }
        }

		public float viewTimeMax{
			get {return cutscene!= null? cutscene.viewTimeMax:0;}
			set {cutscene.viewTimeMax = value;}
		}

		public float maxTime{
			get {return Mathf.Max(viewTimeMax, length); }
		}

		public float viewTime{
			get {return viewTimeMax - viewTimeMin;}
		}


		//Layout variables
		private float leftMargin{ //margin on the left side. The width of the group/tracks list.
			get {return Prefs.trackListLeftMargin;}
			set {Prefs.trackListLeftMargin = Mathf.Clamp(value, 230, 400);}
		}
		private const float RIGHT_MARGIN           = 16; //margin on the right side
		private const float TOOLBAR_HEIGHT         = 18; //the height of the toolbar
/// @modify slate sequencer
/// @TQ
        private const float TOOLBAR_HEIGHT1        = 36;  
		private const float TOP_MARGIN             = 75; //top margin AFTER the toolbar
/// end
        private const float GROUP_HEIGHT           = 21; //height of group headers
		private const float TRACK_MARGINS          = 4;  //margin between tracks of same group (top/bottom)
		private const float GROUP_RIGHT_MARGIN     = 4;  //margin at the right side of groups
		private const float TRACK_RIGHT_MARGIN     = 4;  //margin at the right side of tracks
		private const float FIRST_GROUP_TOP_MARGIN = 20; //initial top margin
/// @modify slate sequencer
/// @TQ
        private const float TOP_MARGIN1 = 42;
/// end
        //

		//Layout Rects
		private Rect topLeftRect;	//for playback controls
		private Rect topMiddleRect;	//for time info
		private Rect leftRect;		//for group/track list
		private Rect centerRect;    //for timeline
                                    //private Rect topRightRect;
                                    //private Rect rightRect;
                                    //
/// @modify slate sequencer
/// @TQ
        private Rect topLeftRect1;  //for playback controls1
/// end


        private static readonly Color listSelectionColor = new Color(0.5f, 0.5f, 1, 0.3f);
		private static readonly Color groupColor = new Color(0f, 0f, 0f, 0.2f);
        /// @modify slate sequencer
        /// @TQ
        private static readonly Color mutiSelectColor = new Color(0.5f, 0.5f, 1, 0.3f);
        /// end
		private Color highlighColor{
			get {return isProSkin? new Color(0.65f, 0.65f, 1) : new Color(0.1f, 0.1f, 0.1f);}
		}
		private float magnetSnapInterval{
			get {return viewTime * 0.01f;}
		}

		[System.NonSerialized] private Dictionary<int, ActionClipWrapper> clipWrappers;
		[System.NonSerialized] private EditorPlayback editorPlayback            = EditorPlayback.Stoped;
		[System.NonSerialized] private Cutscene.WrapMode editorPlaybackWrapMode = Cutscene.WrapMode.Loop;	
		[System.NonSerialized] private bool anyClipDragging                     = false;
		[System.NonSerialized] private Vector2 scrollPos                        = Vector2.zero;
		[System.NonSerialized] private float totalHeight                        = 0;
		[System.NonSerialized] private bool movingScrubCarret                   = false;
		[System.NonSerialized] private bool movingEndCarret                     = false;
/// @modify slate sequencer
/// add by TQ
        [System.NonSerialized] private bool movingStartCarret                   = false;
/// end
        [System.NonSerialized] private CutsceneTrack pickedTrack                = null;
		[System.NonSerialized] private CutsceneGroup pickedGroup                = null;
		[System.NonSerialized] private bool mouseButton2Down                    = false;
		[System.NonSerialized] private float lastStartPlayTime                  = 0;
		[System.NonSerialized] private float editorPreviousTime                 = 0;

		[System.NonSerialized] private Vector2? multiSelectStartPos           = null;
		[System.NonSerialized] private List<ActionClipWrapper> multiSelection = null;
		[System.NonSerialized] private Rect preMultiSelectionRetimeMinMax     = default(Rect);
		[System.NonSerialized] private int multiSelectionScaleDirection       = 0;

		[System.NonSerialized] private Vector2 mousePosition       = Vector2.zero;
		[System.NonSerialized] private Section draggedSection      = null;
		[System.NonSerialized] private bool willRepaint            = true;
		[System.NonSerialized] private bool willDirty              = false;
		[System.NonSerialized] private bool willResample           = false;
		[System.NonSerialized] private int repaintCooldown         = 0;
		[System.NonSerialized] private System.Action onDoPopup     = null;
		[System.NonSerialized] private float? clipScalingGuideTime = null;
		[System.NonSerialized] private bool isResizingLeftMargin   = false;
		[System.NonSerialized] private bool helpButtonPressed      = false;
		[System.NonSerialized] private string searchString         = null;
		[System.NonSerialized] private bool showDragDropInfo       = true;
        [System.NonSerialized]
        private string frameInputString = string.Empty;
        private string frameOldString = string.Empty;
        // @modify slate sequencer
        // @rongxia
        private List<ICutsceneTrackDropFile> dropFileList = new List<ICutsceneTrackDropFile>();
        // @end

        /// @modify slate sequencer
        /// @TQ
        [System.NonSerialized]
        private List<CutsceneGroup> pickedGroupList = new List<CutsceneGroup>();

        [System.NonSerialized]
        private List<CutsceneTrack> pickedTrackList = new List<CutsceneTrack>();
        /// @end
        public Cutscene cutscene{
			get
			{
				if (_cutscene == null){
					_cutscene = EditorUtility.InstanceIDToObject(_cutsceneID) as Cutscene;
				}
				return _cutscene;
			}
			 set
			{
				_cutscene = value;
                Cutscene.Current = _cutscene;
				if (value != null){
					_cutsceneID = value.GetInstanceID();
				}
			}
		}

		//SHORTCUTS//
		private static bool isProSkin{
			get {return EditorGUIUtility.isProSkin;}
		}

		private static Texture2D whiteTexture{
			get {return Slate.Styles.whiteTexture;}
		}

		private bool isPrefab{
			get {return cutscene != null && PrefabUtility.GetPrefabType(cutscene) == PrefabType.Prefab;}
		}

		private float screenWidth{
			get {return Screen.width;}
		}

		private float screenHeight{
			get {return Screen.height;}
		}


		//UTILITY FUNCS//
		float TimeToPos(float time){
			return (time - viewTimeMin) / viewTime * centerRect.width;
		}

		float PosToTime(float pos){
			return (pos - leftMargin) / centerRect.width * viewTime + viewTimeMin;
		}

		float SnapTime(float time){
			return (Mathf.Round(time / Prefs.snapInterval) * Prefs.snapInterval);
		}

		void SafeDoAction(System.Action call){
			var time = cutscene.currentTime;
			Stop(true);
			call();
			cutscene.currentTime = time;
		}

		bool FilteredOutBySearch(IDirectable directable, string search){
			if (string.IsNullOrEmpty(search)){ return false; }
			if (string.IsNullOrEmpty(directable.name)){ return true; }
			return !directable.name.ToLower().Contains(search.ToLower());
		}

		void DrawGuideLine(float xPos, Color color){
			if (xPos > 0 && xPos < centerRect.xMax - leftRect.width){
				var guideRect = new Rect(xPos + centerRect.x, centerRect.y, 1, centerRect.height);
				GUI.color = color;
				GUI.DrawTexture(guideRect, whiteTexture);
				GUI.color = Color.white;
			}
		}

		void AddCursorRect(Rect rect, MouseCursor type ){
			EditorGUIUtility.AddCursorRect(rect, type);
			willRepaint = true;
		}

		void DoPopup(System.Action call){
			onDoPopup = call;
		}
		///////////


		///Opens the editor :)
		public static void ShowWindow(){ ShowWindow(null); }

        public static void ShowWindow(Cutscene newCutscene)
        {
			var window = EditorWindow.GetWindow(typeof(CutsceneEditor)) as CutsceneEditor;
            window.InitializeAll(newCutscene);
			window.Show();

            OnLoadCutscene(newCutscene);
        }

        private static void OnLoadCutscene(Cutscene cutscene)
        {
            if (cutscene != null)
            {
                foreach (var group in cutscene.groups)
                {
                    foreach (var track in group.tracks)
                    {
                        foreach (var action in track.actions)
                        {
                            if (action as ActionClips.SubCutscene)
                            {
                                ActionClips.SubCutscene sub = (ActionClips.SubCutscene)action;
                                OnLoadCutscene(sub.cutscene);
                            }
                            else if (action as ActionClips.PlayAnimationClip)
                            {
                                ActionClips.PlayAnimationClip clip = (ActionClips.PlayAnimationClip)action;
                                if (clip.animationClip == null && !string.IsNullOrEmpty(clip.animationClipPath))
                                {
                                    clip.animationClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(clip.animationClipPath);
                                }
                            }
                        }
                    }
                }
            }
        }

        void OnEnable(){
			Styles.Load();
            // @modify slate sequencer
            // @TQ
            ShortcutManager.instance.Init(
                new List<DGEditorShortcut.Action> {
                new DGEditorShortcut.Action(ShortcutText.ClipCut, null),
                new DGEditorShortcut.Action(ShortcutText.ClipSelectBehind, null),
                new DGEditorShortcut.Action(ShortcutText.ClipDelete, null),
                new DGEditorShortcut.Action(ShortcutText.ClipSelectRevert, null),
                new DGEditorShortcut.Action(ShortcutText.EditKeyFrame, null),
                new DGEditorShortcut.Action(ShortcutText.EditPreFrame, null),
                new DGEditorShortcut.Action(ShortcutText.EditNextFrame, null),
                new DGEditorShortcut.Action(ShortcutText.EditPreClip, null),
                new DGEditorShortcut.Action(ShortcutText.EditNextClip, null),
                new DGEditorShortcut.Action(ShortcutText.EditStopOrPlay, null),
                new DGEditorShortcut.Action(ShortcutText.ClipLeftCut, null),
                new DGEditorShortcut.Action(ShortcutText.ClipRightCut, null),
                new DGEditorShortcut.Action(ShortcutText.DopeSheetSelectBehind, null),
                new DGEditorShortcut.Action(ShortcutText.EditPause, null),
                new DGEditorShortcut.Action(ShortcutText.PropertyFramePre, null),
                new DGEditorShortcut.Action(ShortcutText.PropertyFrameNext, null),
                new DGEditorShortcut.Action(ShortcutText.PropertKeyFrame, null),
                new DGEditorShortcut.Action(ShortcutText.MoveStartTimeToCurrent, null),
                new DGEditorShortcut.Action(ShortcutText.MoveEndTimeToCurrent, null)
            });
            ShortcutManager.SetDefaultKeyboardShortcuts(new DGEditorShortcut[] {
                new DGEditorShortcut(1, KeyCode.A ,EventModifiers.Control),
                new DGEditorShortcut(2, KeyCode.Delete, EventModifiers.FunctionKey),
                new DGEditorShortcut(3, KeyCode.R, EventModifiers.Control),
                new DGEditorShortcut(4, KeyCode.S, EventModifiers.None),
                new DGEditorShortcut(5, KeyCode.Comma, EventModifiers.None),
                new DGEditorShortcut(6, KeyCode.Period, EventModifiers.None),
                new DGEditorShortcut(7, KeyCode.LeftArrow, EventModifiers.FunctionKey),
                new DGEditorShortcut(8, KeyCode.RightArrow, EventModifiers.FunctionKey),
                new DGEditorShortcut(9, KeyCode.Space, EventModifiers.None),
                new DGEditorShortcut(10, KeyCode.LeftBracket, EventModifiers.None),
                new DGEditorShortcut(11, KeyCode.RightBracket, EventModifiers.None),
                new DGEditorShortcut(12, KeyCode.A, EventModifiers.Control),
                new DGEditorShortcut(13, KeyCode.P, EventModifiers.None),
                new DGEditorShortcut(14, KeyCode.F1, EventModifiers.None),
                new DGEditorShortcut(15, KeyCode.F2, EventModifiers.None),
                new DGEditorShortcut(16, KeyCode.F3, EventModifiers.None),
                new DGEditorShortcut(17, KeyCode.LeftBracket, EventModifiers.Control),
                new DGEditorShortcut(18, KeyCode.RightBracket, EventModifiers.Control)
            });
			//end

#if UNITY_5_6_OR_NEWER
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaving -= OnWillSaveScene;
			UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += OnWillSaveScene;
			#endif

#pragma warning disable 618
			EditorApplication.playmodeStateChanged += delegate { repaintCooldown = 4; };
			EditorApplication.playmodeStateChanged -= InitializeAll;
			EditorApplication.playmodeStateChanged += InitializeAll;
#pragma warning restore
			EditorApplication.update -= OnEditorUpdate;
			EditorApplication.update += OnEditorUpdate;
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			Tools.hidden = false;
			current = this;

            // @modify slate sequencer
            // @rongxia
            titleContent = new GUIContent("达芬奇录制器", Styles.cutsceneIconOpen);
            // @end
			wantsMouseMove = true;
			autoRepaintOnSceneChange = true;
			minSize = new Vector2(500, 250);

            // @modify slate sequencer
            // @rongxia
            dropFileList.Clear();
            foreach (System.Type type in ReflectionTools.GetDerivedTypesOf(typeof(ICutsceneTrackDropFile)))
            {
                if (type.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).FirstOrDefault() != null)
                {
                    continue;
                }

                dropFileList.Add((ICutsceneTrackDropFile)System.Activator.CreateInstance(type));
            }
            // @end
			willRepaint = true;

			InitializeAll();
            // @modify slate sequencer
            // @TQ
            DopeSheetEditor.onDragRectDelegate += DragMoveClipWithProperty;
            //End
        }

        // @modify slate sequencer
        // @TQ
       
        private void DragMoveClipWithProperty(float deltaTime, IKeyable _keyAble)
        {
            var delta = deltaTime;

            if (multiSelection != null && multiSelection.Count > 0)
            {
                CutsceneUtility.selectedObject = null;
                var boundMin = Mathf.Min(multiSelection.Select(b => b.action.startTime).ToArray());
                // var boundMax = Mathf.Max( multiSelection.Select(b => b.action.endTime).ToArray() );
                if (boundMin + delta < 0)
                {
                    delta = 0;
                }
                foreach (var cw in multiSelection)
                {
                    var _cwKeyAble = cw.action as IKeyable;
                    if (_cwKeyAble != null && _cwKeyAble == _keyAble)
                        continue;
                    cw.action.startTime += delta;
                }
            }
            if (CutsceneUtility.selectedObject == null)
            {
                return;
            }

            var dir = CutsceneUtility.selectedObject;
            //var sKeyAble = dir as IKeyable;
            if (dir == _keyAble)
                return;
            ActionClip aclip = dir as ActionClip;
            bool isCameraClipe = aclip is CameraShot;
            bool isAudioClip = aclip is PlayAudio;
            if (aclip != null && !isAudioClip && !isCameraClipe && (multiSelection == null || multiSelection.Count == 0))
            {
                aclip.startTime += delta;
            }
        }
        //End

		void OnDisable(){
			#if UNITY_5_6_OR_NEWER
			UnityEditor.SceneManagement.EditorSceneManager.sceneSaving -= OnWillSaveScene;
			#endif
#pragma warning disable 618
			EditorApplication.playmodeStateChanged -= InitializeAll;
#pragma warning restore
			EditorApplication.update -= OnEditorUpdate;
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			Tools.hidden = false;
			if (cutscene != null && !Application.isPlaying){
				Stop(true);
			}
			current = null;

            // @modify slate sequencer
            // @rongxia
            SlateExtensions.Instance.RecordUtility.Cleanup();
            // @end
        }


        //Set a new view when a script is selected in Unity's tab
        void OnSelectionChange(){
			if (Selection.activeGameObject != null){
				var cut = Selection.activeGameObject.GetComponent<Cutscene>();
				if (cut != null && cutscene != cut){
					InitializeAll(cut);
				}
			}
		}

		//Before scene is saved we need to stop so that cutscene changes are reverted.
		void OnWillSaveScene(UnityEngine.SceneManagement.Scene scene, string path){
			if (cutscene != null && cutscene.currentTime > 0){
				Stop(true);
				Debug.LogWarning("Scene Saved while a cutscene was in preview mode. Cutscene was reverted before saving the scene along with changes it affected.");
			}
		}

		///Initialize everything
		void InitializeAll(){InitializeAll(cutscene);}
		void InitializeAll(Cutscene newCutscene){

            //first stop current cut if any
            if (cutscene != null){
                if (!Application.isPlaying)
                {
                    Stop(true);
                }
            }
            // @modify slate sequencer
            // @rongxia
            if (newCutscene != null && PrefabUtility.GetPrefabType(newCutscene) != PrefabType.Prefab)
            // @end
            {
                cutscene = newCutscene;
				CutsceneUtility.selectedObject = null;
				multiSelection = null;
				InitClipWrappers();
				if (!Application.isPlaying){
					Stop(true);
				}
			}

			Repaint();
		}

        // @modify slate sequencer
        // @rongxia
        void CleanCutscne()
        {
            if (cutscene != null)
            {
                if (!Application.isPlaying)
                {
                    Stop(true);
                }
            }

            _cutsceneID = -1;
            cutscene = null;
            CutsceneUtility.selectedObject = null;
            multiSelection = null;

            Repaint();
        }
        // @end

        //initialize the action clip wrappers
        void InitClipWrappers(bool needStop = true){ 

			if (cutscene == null){
				return;
			}

			multiSelection = null;
			var lastTime = cutscene.currentTime;

        // @modify slate sequencer
        // @rongxia
			if (!Application.isPlaying && (Slate.SlateExtensions.Instance != null
                && Slate.SlateExtensions.Instance.RecordUtility != null
                && Slate.SlateExtensions.Instance.RecordUtility.IsRecording == false)
                && needStop)
            {
				Stop(true);
			}
		// @end

			cutscene.Validate();
			clipWrappers = new Dictionary<int, ActionClipWrapper>();
			for (int g = 0; g < cutscene.groups.Count; g++){
				for (int t = 0; t < cutscene.groups[g].tracks.Count; t++){
					for (int a = 0; a < cutscene.groups[g].tracks[t].actions.Count; a++){
						var id = UID(g, t, a);
						if (clipWrappers.ContainsKey(id)){
							Debug.LogError("Collided UIDs. This should really not happen but it did!");
							continue;
						}
						clipWrappers[id] = new ActionClipWrapper( cutscene.groups[g].tracks[t].actions[a]	);
					}
				}
			}

			if (lastTime > 0){
				cutscene.currentTime = lastTime;
			}
		}
/// @modify slate sequencer
/// add by TQ
        public void OutInitClipWrappers()
        {
            InitClipWrappers();
        }
/// end
        //An integer UID out of list indeces.
        int UID(int g, int t, int a){
			var A = g.ToString("D3");
			var B = t.ToString("D3");
			var C = a.ToString("D4");
			return int.Parse(A+B+C);
		}

        // @modify slate sequencer
        // @rongxia

        public void StartRecord()
        {
            if (editorPlayback == EditorPlayback.Stoped)
            {
                SlateExtensions.Instance.RecordUtility.IsRecordWhenPlay = true;
            }
            else if (editorPlayback == EditorPlayback.PlayingForwards)
            {
                SlateExtensions.Instance.RecordUtility.StartCountDown(_cutscene);
            }
        }

        public void StopRecord()
        {
            if (editorPlayback == EditorPlayback.PlayingForwards || SlateExtensions.Instance.RecordUtility.IsRecording)
            {
                Stop(true);
            }
            else if (editorPlayback == EditorPlayback.Stoped)
            {
                SlateExtensions.Instance.RecordUtility.IsRecordWhenPlay = false;
            }
        }

        // @end

        //Play button pressed or otherwise started
        public void Play(Cutscene.WrapMode wrapMode = Cutscene.WrapMode.Loop, System.Action callback = null){

            // @modify slate sequencer
            // @rongxia
            titleContent = new GUIContent("时间轴录制", Styles.cutsceneIconClose);
            // @end
			
			if (Application.isPlaying){
				var temp = cutscene.currentTime == length? 0 : cutscene.currentTime;
/// @modify slate sequencer
/// add by TQ
				cutscene.Play(startPlayTime, length, cutscene.defaultWrapMode, callback, Cutscene.PlayingDirection.Forwards);
				cutscene.currentTime = startPlayTime;
/// end
				return;
			}

			editorPlaybackWrapMode = wrapMode;
			editorPlayback = EditorPlayback.PlayingForwards;
			editorPreviousTime = Time.realtimeSinceStartup;
/// @modify slate sequencer
/// add by TQ
            if (cutscene.currentTime == 0)
            {
                cutscene.currentTime = cutscene.startPlayTime;
            }
/// end
			lastStartPlayTime = cutscene.currentTime;
			OnStopInEditor = callback != null? callback : OnStopInEditor;

            // @modify slate sequencer
            // @rongxia
            if (SlateExtensions.Instance.RecordUtility.IsRecordWhenPlay)
            {
                SlateExtensions.Instance.RecordUtility.StartCountDown(_cutscene);
            }
            // @end
        }

        //Play reverse button pressed
        public void PlayReverse(){

            // @modify slate sequencer
            // @rongxia
            titleContent = new GUIContent("时间轴", Styles.cutsceneIconClose);
            // @end

			if (Application.isPlaying){
/// @modify slate sequencer
/// add by TQ
				var temp = cutscene.currentTime == 0? length : cutscene.currentTime;               
                cutscene.Play(startPlayTime, length, cutscene.defaultWrapMode, null, Cutscene.PlayingDirection.Backwards);
/// end
				cutscene.currentTime = temp;
				return;
			}

			editorPlayback = EditorPlayback.PlayingBackwards;
			editorPreviousTime = Time.realtimeSinceStartup; 
            if (cutscene.currentTime == 0){
				cutscene.currentTime = length;
				lastStartPlayTime = 0;
			} else {
				lastStartPlayTime = cutscene.currentTime;			
			}
		}

		//Pause button pressed
		public void Pause(){

            // @modify slate sequencer
            // @rongxia
            titleContent = new GUIContent("时间轴", Styles.cutsceneIconOpen);
            // @end

            if (Application.isPlaying){
				if (cutscene.isActive){
					cutscene.Pause();
					return;
				}
			}

			editorPlayback = EditorPlayback.Stoped;
			if (OnStopInEditor != null){
				OnStopInEditor();
				OnStopInEditor = null;
			}
		}

		//Stop button pressed or otherwise reset the scrubbing/previewing
		public void Stop(bool forceRewind){

            // @modify slate sequencer
            // @rongxia
            titleContent = new GUIContent("时间轴", Styles.cutsceneIconOpen);
            // @end

            if (Application.isPlaying){
				if (cutscene.isActive){
					cutscene.Stop();
					return;
				}
			}

            // @modify slate sequencer
            // @rongxia
            if (SlateExtensions.Instance != null && SlateExtensions.Instance.RecordUtility != null)
            {
                SlateExtensions.Instance.RecordUtility.StopRecord(_cutscene);
            }
            // @end


            if (OnStopInEditor != null){
				OnStopInEditor();
				OnStopInEditor = null;
			}

			//Super important to Sample instead of setting time here, so that we rewind correct if need be. 0 rewinds.
			cutscene.Sample( editorPlayback != EditorPlayback.Stoped && !forceRewind? lastStartPlayTime : 0 );
			editorPlayback = EditorPlayback.Stoped;
			willRepaint = true;
		}

        void StepForwardNextFrame()
        {
            if (cutscene.currentTime == cutscene.length)
            {
                cutscene.currentTime = 0;
                return;
            }

            cutscene.currentTime = cutscene.currentTime + Prefs.snapInterval;

        }

        void StepBackwardPreFrame()
        {

            if (cutscene.currentTime == cutscene.length)
            {
                cutscene.currentTime = 0;
                return;
            }

            cutscene.currentTime = cutscene.currentTime - Prefs.snapInterval;
        }

		///Steps time forward to the next key time
		void StepForward(){
			var keyable = CutsceneUtility.selectedObject as IKeyable;
			if (keyable != null){
				var time = keyable.animationData.GetKeyNext( cutscene.currentTime - keyable.startTime );
				cutscene.currentTime = time + keyable.startTime;
				return;
			}
			if (cutscene.currentTime == cutscene.length){
				cutscene.currentTime = 0;
				return;
			}
			cutscene.currentTime = cutscene.GetKeyTimes().FirstOrDefault(t => t > cutscene.currentTime + 0.01f);
		}

		///Steps time backwards to the previous key time
		void StepBackward(){
			var keyable = CutsceneUtility.selectedObject as IKeyable;
			if (keyable != null){
				var time = keyable.animationData.GetKeyPrevious( cutscene.currentTime - keyable.startTime );
				cutscene.currentTime = time + keyable.startTime;
				return;
			}
			if (cutscene.currentTime == 0){
				cutscene.currentTime = cutscene.length;
				return;
			}
			cutscene.currentTime = cutscene.GetKeyTimes().LastOrDefault(t => t < cutscene.currentTime - 0.01f);
		}
//add by TQ 20140920
        void SelectRevertClip()
        {
            ActionClip aClip = CutsceneUtility.selectedObject as ActionClip;

            if (multiSelection == null)
            {
                if (aClip != null)
                {
                    var parent = aClip.root;
                    CutsceneTrack cTrack = aClip.parent as CutsceneTrack;
                    if (cTrack == null)
                        return;

                    foreach (var cw in clipWrappers.Values.Where(c => c.action != aClip))
                    {
                        if (cw.action.isLocked)
                        {
                            continue;
                        }
                        if (multiSelection == null)
                        {
                            multiSelection = new List<ActionClipWrapper>();
                        }
                        CutsceneTrack pTrack = cw.action.parent as CutsceneTrack;
                        if (pTrack == cTrack)
                            multiSelection.Add(cw);
                    }
                }
            }
            else
            {
                ActionClip mClip = multiSelection[0].action as ActionClip;
                if (mClip == null)
                {
                    return;
                }

                CutsceneTrack cTrack = mClip.parent as CutsceneTrack;
                if (cTrack == null)
                {
                    return;
                }

                foreach (var cw in clipWrappers.Values.Where(c => c.action.startTime >= 0))
                {
                    if (cw.action.isLocked)
                    {
                        continue;
                    }

                    CutsceneTrack pTrack = cw.action.parent as CutsceneTrack;
                    if (pTrack != cTrack)
                    {
                        continue;
                    }
                    if (multiSelection.Contains(cw))
                    {
                        multiSelection.Remove(cw);
                    }
                    else
                    {
                        multiSelection.Add(cw);
                    }
                }
            }        
        }
        

        void SelectAllBehindClip()
        {
            ActionClip aClip = CutsceneUtility.selectedObject as ActionClip;
            if (aClip == null)
            {
                return;
            }
            var parent = aClip.root;
            if (aClip != null)
            {
                CutsceneTrack cTrack = aClip.parent as CutsceneTrack;
                if (cTrack == null)
                    return;

                foreach (var cw in clipWrappers.Values.Where(c => c.action.startTime >= aClip.startTime))
                {
                    if (cw.action.isLocked)
                    {
                        continue;
                    }
                    if (multiSelection == null)
                    {
                        multiSelection =  new List<ActionClipWrapper>();
                    }
                    CutsceneTrack pTrack = cw.action.parent as CutsceneTrack;
                    if (pTrack == cTrack)
                        multiSelection.Add(cw);
                }                 
            }       
        }//end

		void OnEditorUpdate(){

			//if cutscene playmode active, it will sample and update itself.
			if (cutscene == null || cutscene.isActive){
				return;
			}
			
			if (EditorApplication.isCompiling){
				Stop(true);
				return;
			}

			//Sample at it's current time.
			cutscene.Sample();

            // @modify slate sequencer
            // @rongxia
            SlateExtensions.Instance.RecordUtility.Update();
            // @end

            //Nothing.
            if (editorPlayback == EditorPlayback.Stoped){
				return;
			}

            // @modify slate sequencer
            // @rongxia
            if (SlateExtensions.Instance.RecordUtility.IsCountDownRecordState)
            {
                editorPreviousTime = Time.realtimeSinceStartup;
                return;
            }
            // @end

            //Playback.
            if (cutscene.currentTime >= length && editorPlayback == EditorPlayback.PlayingForwards){
				if (editorPlaybackWrapMode == Cutscene.WrapMode.Once){
					Stop(true);
					return;
				}

                // @modify slate sequencer
                // @rongxia
                if (SlateExtensions.Instance.RecordUtility.IsRecording)
                {
                    Stop(true);
                    return;
                }
                //@end

                if (editorPlaybackWrapMode == Cutscene.WrapMode.Loop){
/// @modify slate sequencer
/// add by TQ
					cutscene.currentTime = cutscene.startPlayTime;
/// end
					return;
				}
			}
/// @modify slate sequencer
/// add by TQ
			if (cutscene.currentTime <= cutscene.startPlayTime && editorPlayback == EditorPlayback.PlayingBackwards){
/// end
				Stop(true);
				return;
			}


			var delta = (Time.realtimeSinceStartup - editorPreviousTime) * Time.timeScale;
			delta *= cutscene.playbackSpeed;
			cutscene.currentTime += editorPlayback == EditorPlayback.PlayingForwards? delta : -delta;
			editorPreviousTime = Time.realtimeSinceStartup;
		}


		//...
		void OnSceneGUI(SceneView sceneView){

            // @modify slate sequencer
            // @rongxia
            if (SlateExtensions.Instance.ProcessScreenEvent())
            {
                return;
            }
            // @end

            if (cutscene == null){
				return;
			}

			//Shortcuts for scene gui only
			var e = Event.current;
			if (e.type == EventType.KeyUp){

                DGEditorShortcut[] tempShortcut = ShortcutManager.GetKeyboardShortcuts("StoryEngine", null);
                for (int i = 0; i < tempShortcut.Length; i++)
                {
                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditStopOrPlay)
                    {
                        GUIUtility.keyboardControl = 0;
                        if (editorPlayback != EditorPlayback.Stoped) { Stop(false); }
                        else { Play(); }
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditPreFrame)
                    {
                        GUIUtility.keyboardControl = 0;
                        StepBackwardPreFrame();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.MoveStartTimeToCurrent)
                    {
                        GUIUtility.keyboardControl = 0;
                        current.cutscene.startPlayTime = current.cutscene.currentTime;
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.MoveEndTimeToCurrent)
                    {
                        GUIUtility.keyboardControl = 0;
                        current.cutscene.length = current.cutscene.currentTime;
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditNextFrame)
                    {
                        GUIUtility.keyboardControl = 0;
                        StepForwardNextFrame();
                        e.Use();
                    }


                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditPreClip)
                    {
                        GUIUtility.keyboardControl = 0;
                        StepBackward();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditNextClip)
                    {
                        GUIUtility.keyboardControl = 0;
                        StepForward();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditPause)
                    {
                        GUIUtility.keyboardControl = 0;
                        if (editorPlayback != EditorPlayback.Stoped) { Pause(); }
                        else {
                            Play();
                            DisableAllAudioTrack(true);
                        }
                        e.Use();
                    }
                }
               
            }

			///Forward OnSceneGUI
			if (cutscene.directables != null){
				for (var i = 0; i < cutscene.directables.Count; i++){
					var directable = cutscene.directables[i];
                    /// @modify slate sequencer
                    /// @TQ
                    if (Prefs.ShowAllGizmoLine)
                        directable.SceneGUI(CutsceneUtility.selectedObject == directable);
                    else
                    {
                        if(CutsceneUtility.selectedObject == directable )
                            directable.SceneGUI(CutsceneUtility.selectedObject == directable);
                    }
                    ///end
				}
			}
			///

			///No need to show tools of cutscene object, plus handles are shown per clip when required
			Tools.hidden = (Selection.activeObject == cutscene || Selection.activeGameObject == cutscene.gameObject) && CutsceneUtility.selectedObject != null;
			
			///Cutscene Root info and gizmos
			Handles.color = Prefs.gizmosColor;
			Handles.Label(cutscene.transform.position + new Vector3(0,0.4f,0), "Cutscene Root");
			Handles.DrawLine(cutscene.transform.position + cutscene.transform.forward, cutscene.transform.position + cutscene.transform.forward * -1);
			Handles.DrawLine(cutscene.transform.position + cutscene.transform.right, cutscene.transform.position + cutscene.transform.right * -1);
			Handles.color = Color.white;

			Handles.BeginGUI();

			if (cutscene.currentTime > 0 && (cutscene.currentTime < cutscene.length || !Application.isPlaying) ){
				///view frame. Red = scrubbing, yellow = active in playmode
				var cam       = sceneView.camera;
				var lineWidth = 3f;
				var top       = new Rect(0, 0, cam.pixelWidth, lineWidth);
				var bottom    = new Rect(0, cam.pixelHeight - lineWidth - 10, cam.pixelWidth, lineWidth + 10 );
				var left      = new Rect(0, 0, lineWidth, cam.pixelHeight);
				var right     = new Rect(cam.pixelWidth-lineWidth, 0, lineWidth, cam.pixelHeight);
				var texture   = whiteTexture;
				GUI.color = cutscene.isActive? Color.green : Color.red;
				GUI.DrawTexture(top, texture);
				GUI.DrawTexture(bottom, texture);
				GUI.DrawTexture(left, texture);
				GUI.DrawTexture(right, texture);
				//

				//Info
				GUI.color = new Color(0,0,0,0.7f);
				if (cutscene.isActive){
					GUI.Label(bottom, string.Format(" Active '{0}'", cutscene.name), GUIStyle.none);
				} else {
					GUI.Label(bottom, string.Format(" Previewing '{0}'. Non animatable changes made to actor components will be reverted.", cutscene.name), GUIStyle.none);
				}
			}

			GUI.color = Color.white;
			Handles.EndGUI();
		}

        //...
        void OnGUI(){

            GUI.skin.label.richText         = true;
			GUI.skin.label.alignment        = TextAnchor.UpperLeft;
			EditorStyles.label.richText     = true;
			EditorStyles.textField.wordWrap = true;
			EditorStyles.foldout.richText   = true;
			var e         = Event.current;
			mousePosition = e.mousePosition;
			current = this;

			if (cutscene == null || helpButtonPressed){
                // @modify slate sequencer
                // @rongxia
                if(SlateExtensions.Instance.ProcessScreenEvent())
                {
                    return;
                }
                ShowStoryWelcome();
                // @end
				return;
			}

           
            //avoid edit when compiling
            if (EditorApplication.isCompiling){
				Stop(true);
				ShowNotification(new GUIContent("Compiling\n...Please wait..."));
				return;
			}

			//this is basicaly a bad hack to avoid unwanted behaviour when exiting playmode
			if (repaintCooldown > 0){
				repaintCooldown --;
				ShowNotification(new GUIContent("...PlayMode Changed..."));
				Repaint();
				return;
			}

			//handle undo/redo shortcuts
			if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed"){
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                multiSelection = null;
                cutscene.Validate();
                InitClipWrappers();
                e.Use();
				return;
			}

			//prefab editing is not allowed
			if (isPrefab){
				ShowNotification(new GUIContent("Editing Prefab Assets is not allowed for safety\nPlease add an instance in the scene"));
				if (e.isMouse || e.isKey){
					e.Use();
				}
			}

			//remove notifications quickly
			if (e.type == EventType.MouseDown){
				RemoveNotification();
			}


			//Record Undo and dirty? This is an overal fallback. Certain actions register undo as well.
			if (e.rawType == EventType.MouseDown && e.button == 0){
				Undo.RegisterFullObjectHierarchyUndo(cutscene.groupsRoot.gameObject, "Cutscene Change");
				Undo.RecordObject(cutscene, "Cutscene Change");
				willDirty = true;
			}

			//button 2 seems buggy
			if (e.button == 2 && e.type == EventType.MouseDown){ mouseButton2Down = true; }
			if (e.button == 2 && e.rawType == EventType.MouseUp){ mouseButton2Down = false; }


			//make the layout rects
/// @modify slate sequencer
/// @TQ
			topLeftRect       = new Rect(0, TOOLBAR_HEIGHT-6, screenWidth, TOP_MARGIN1+6);
            topLeftRect1      = new Rect(0, TOOLBAR_HEIGHT+TOOLBAR_HEIGHT1-1, leftMargin, TOP_MARGIN - TOP_MARGIN1);
			topMiddleRect     = new Rect(leftMargin, TOOLBAR_HEIGHT+TOOLBAR_HEIGHT1, screenWidth - leftMargin - RIGHT_MARGIN, TOP_MARGIN1);
/// end
			leftRect          = new Rect(0, TOOLBAR_HEIGHT + TOP_MARGIN, leftMargin, screenHeight - TOOLBAR_HEIGHT - TOP_MARGIN + scrollPos.y);
			centerRect        = new Rect(leftMargin, TOP_MARGIN + TOOLBAR_HEIGHT, screenWidth - leftMargin - RIGHT_MARGIN, screenHeight - TOOLBAR_HEIGHT - TOP_MARGIN + scrollPos.y);
			//topRightRect    = new Rect(screenWidth - RIGHT_MARGIN, TOOLBAR_HEIGHT, RIGHT_MARGIN, TOP_MARGIN);
			//rightRect       = new Rect(screenWidth - RIGHT_MARGIN, TOP_MARGIN, RIGHT_MARGIN, totalHeight);


			//reorder action lists for better UI. This is strictly a UI thing.
			if (!anyClipDragging && e.type == EventType.Layout){
				foreach(var group in cutscene.groups){
					foreach(var track in group.tracks){
						track.actions = track.actions.OrderBy(a => a.startTime).ToList();
					}
				}
			}				

			//just an icon watermark at bottom right
			var r = new Rect(0,0,128,128);
			r.center = new Vector2(screenWidth-80, screenHeight-80);
			GUI.color = new Color(1,1,1,0.15f);
			GUI.DrawTexture(r, Styles.slateIcon);
			GUI.color = Color.white;
            ///

            // @modify slate sequencer
            // @rongxia
            if(SlateExtensions.Instance != null && SlateExtensions.Instance.RecordUtility != null)
            {
                SlateExtensions.Instance.RecordUtility.InterceptInput();
            }
            // @end

            //...
            DoKeyboardShortcuts();
            if (cutscene == null)
                return;
            //call respective function for each rect
            /// @modify slate sequencer
            /// @TQ
            ShowEditHelpControls(topLeftRect);
            ShowPlaybackControls(topLeftRect);
/// end
            ShowTimeInfo(topMiddleRect);

			//Other functions
			ShowToolbar();
			DoScrubControls();
			DoZoomAndPan();


			//Dirty and Resample flags?
			if (e.rawType == EventType.MouseUp && e.button == 0){
				willDirty = true;
				willResample = true;
			}


			//Timelines
			var scrollRect1 = Rect.MinMaxRect(0, centerRect.yMin, screenWidth, screenHeight - 5);
			var scrollRect2 = Rect.MinMaxRect(0, centerRect.yMin, screenWidth, totalHeight + 150);
			scrollPos = GUI.BeginScrollView(scrollRect1, scrollPos, scrollRect2);
            ShowGroupsAndTracksList(leftRect);
			ShowTimeLines(centerRect);
			GUI.EndScrollView();
			////////////////////////////////////////

			///etc
			DrawGuides();
			AcceptDrops();

			///Final stuff...///
			//enforce reset interaction since rawType does not work from within GUI.Window
			if (e.rawType == EventType.MouseUp){
				foreach(var cw in clipWrappers.Values){
					cw.ResetInteraction();
				}
                /// @modify slate sequencer
                /// add by TQ
                //subcutsence cam sys
                //if (e.button == 0)
                //    CutsceneEditor.current.SubCutsenceSysCamFunc(false);
                /// end

            }

            //clean selection and hotcontrols
            if (e.type == EventType.MouseDown && e.button == 0 && GUIUtility.hotControl == 0){
				if (centerRect.Contains(mousePosition) && !e.control){
					CutsceneUtility.selectedObject = null;
					multiSelection = null;
				}
				GUIUtility.keyboardControl = 0;
				showDragDropInfo = false;
			}
		
			//just some info for the user to drag/drop gameobject in editor
			if (showDragDropInfo && cutscene != null && cutscene.groups.Find(g => g.GetType() == typeof(ActorGroup)) == null){
				var label = "Drag & Drop GameObjects or Prefabs in this window to create Actor Groups";
				var size = new GUIStyle("label").CalcSize(new GUIContent(label));
				var notificationRect = new Rect(0, 0, size.x, size.y);
				notificationRect.center = new Vector2((screenWidth/2) + (leftMargin/2), (screenHeight/2) + TOP_MARGIN);
				GUI.Label(notificationRect, label);
			}

			//repaint?
			if (e.type == EventType.MouseDrag || e.type == EventType.MouseUp || GUI.changed){
				willRepaint = true;
			}


			//set dirty?
			if (willDirty){
				willDirty = false;
				EditorUtility.SetDirty(cutscene);
				foreach(var o in cutscene.GetComponentsInChildren(typeof(IDirectable), true).Cast<Object>() ){
					EditorUtility.SetDirty(o);
				}
			}

			//Resample flag
			if (willResample){
				willResample = false;
				//delaycall so that other gui controls are finalized before resample.
				EditorApplication.delayCall += ()=>{ if (cutscene != null) cutscene.ReSample(); };
			}

			//Repaint flag
			if (willRepaint){
				willRepaint = false;
				Repaint();
			}


			//uber hack to show modal popup windows
			if (onDoPopup != null){
				var temp = onDoPopup;
				onDoPopup = null;
				QuickPopup.Show(temp);
			}


			//if preafb darken
			if (isPrefab){
				GUI.color = new Color(0,0,0,0.5f);
				GUI.DrawTexture(new Rect(0,0,screenWidth, screenHeight), whiteTexture);
				GUI.color = Color.white;
			}

			//cheap ver/hor seperators
			Handles.color = Color.black;
			Handles.DrawLine(new Vector2(0, centerRect.y+1), new Vector2(centerRect.xMax, centerRect.y+1));
			Handles.DrawLine(new Vector2(centerRect.x, centerRect.y+1), new Vector2(centerRect.x, centerRect.yMax));
			Handles.color = Color.white;

            // @modify slate sequencer
            // @rongxia
            if (SlateExtensions.Instance != null
                && SlateExtensions.Instance.RecordUtility != null)
            {
            SlateExtensions.Instance.RecordUtility.ShowRecordInfo();
            }
            // @end

            //cleanup
            GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			GUI.skin = null;

        }



        void DoKeyboardShortcuts(){
			
			var e = Event.current;

            if (e.type == EventType.KeyUp /*&& GUIUtility.keyboardControl == 0*/){
            
                DGEditorShortcut[] tempShortcut = ShortcutManager.GetKeyboardShortcuts("StoryEngine", null);
                for (int i = 0; i < tempShortcut.Length; i++)
                {
                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.ClipDelete)
                    {
                        if (multiSelection != null)
                        {
                            SafeDoAction(() =>
                            {
                                foreach (var act in multiSelection.Select(b => b.action).ToArray())
                                {
                                    (act.parent as CutsceneTrack).DeleteAction(act);
                                }
                                InitClipWrappers();
                                multiSelection = null;
                            });
                            e.Use();
                        }
                        else
                        {
                            var clip = CutsceneUtility.selectedObject as ActionClip;
                            if (clip != null)
                            {
/// @modify slate sequencer
/// add by TQ
                                SafeDoAction(() => { (clip.parent as CutsceneTrack).WrapDeleteAction(clip); InitClipWrappers(); });
/// end
                                e.Use();
                            }
                        }
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.ClipSelectRevert)
                    {

                        SelectRevertClip();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditKeyFrame)
                    {
                        var keyable = CutsceneUtility.selectedObject as IKeyable;//k 帧 快捷键
                        if (keyable != null)
                        {
                            var time = cutscene.currentTime - CutsceneUtility.selectedObject.startTime;
                            time = Mathf.Clamp(time, 0, keyable.endTime - keyable.startTime);
                            if (keyable.animationData != null && keyable.animationData.isValid)
                            {
                                keyable.animationData.TryKeyIdentity(time);
                                e.Use();
                            }
                        }
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditPreFrame)
                    {
                        StepBackwardPreFrame();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditNextFrame)
                    {
                        StepForwardNextFrame();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.MoveStartTimeToCurrent)
                    {
                        GUIUtility.keyboardControl = 0;
                        current.cutscene.startPlayTime = current.cutscene.currentTime;
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.MoveEndTimeToCurrent)
                    {
                        GUIUtility.keyboardControl = 0;
                        current.cutscene.length = current.cutscene.currentTime;
                        e.Use();
                    }


                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditPreClip)
                    {
                        StepBackward();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditNextClip)
                    {
                        StepForward();
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditStopOrPlay)
                    {
                        if (editorPlayback != EditorPlayback.Stoped) { Stop(false); }
                        else
                        {
                            Play();
                            DisableAllAudioTrack(true);
                        } 
                        e.Use();
                    }

                    if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.EditPause)
                    {
                        GUIUtility.keyboardControl = 0;
                        if (editorPlayback != EditorPlayback.Stoped) { Pause(); }
                        else
                        {
                            Play();
                            DisableAllAudioTrack(true);
                        }
                        e.Use();
                    }


                    if (CutsceneUtility.selectedObject is ActionClip)
                    {
                        var action = (ActionClip)CutsceneUtility.selectedObject;
                        var time = PosToTime(mousePosition.x);
                        if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.ClipLeftCut && time < action.endTime)
                        {
                            var temp = action.endTime;
                            action.startTime = time;
                            action.endTime += temp - action.endTime;
                            e.Use();
                        }

                        if (e.keyCode == tempShortcut[i].key && e.modifiers == tempShortcut[i].modifiers && ShortcutManager.shortcutActions[tempShortcut[i].action] == ShortcutText.ClipRightCut && time > action.startTime)
                        {
                            action.endTime = time;
                            e.Use();
                        }
                    }
       
                }

			}
		}
		
		void DrawGuides(){
            if (cutscene == null)
                return;
            //draw a vertical line at 0 time
            DrawGuideLine(TimeToPos(0), isProSkin? Color.white : Color.black);

			//draw a vertical line at length time
			DrawGuideLine(TimeToPos(length), isProSkin? Color.white : Color.black);

/// @modify slate sequencer
/// add by TQ
            //draw a vertical line at play start time 
            DrawGuideLine(TimeToPos(startPlayTime), isProSkin ? Color.yellow : Color.yellow);
/// end
            //draw a vertical line at current time
            if (cutscene.currentTime > 0){
				DrawGuideLine(TimeToPos(cutscene.currentTime), cutscene.isActive? Color.yellow : new Color(1,0.3f,0.3f));
			}

			//draw a vertical line at dragging clip start/end time
			if (CutsceneUtility.selectedObject != null && anyClipDragging){
				DrawGuideLine(TimeToPos(CutsceneUtility.selectedObject.startTime), new Color(1,1,1,0.05f));
				DrawGuideLine(TimeToPos(CutsceneUtility.selectedObject.endTime), new Color(1,1,1,0.05f));
			}

			if (clipScalingGuideTime != null){
				DrawGuideLine(TimeToPos(clipScalingGuideTime.Value), new Color(1,1,1,0.05f));
			}

			//draw a vertical line at dragging section
			if (draggedSection != null){
				DrawGuideLine( TimeToPos(draggedSection.time), draggedSection.color );
			}

			if (cutscene.isActive){
				if (cutscene.playTimeStart > 0){
					DrawGuideLine(TimeToPos(cutscene.playTimeStart), Color.red);
				}
				if (cutscene.playTimeEnd < length){
					DrawGuideLine(TimeToPos(cutscene.playTimeEnd), Color.red);
				}
			}
		}

        // @modify slate sequencer
        // @rongxia

        void ShowStoryWelcome()
        {
            if (cutscene == null)
            {
                helpButtonPressed = false;
            }

            var label = string.Format("<size=30><b>{0}</b></size>", "Welcome to Super Story Engine!");
            var size = new GUIStyle("label").CalcSize(new GUIContent(label));
            var titleRect = new Rect(0, 0, size.x, size.y);
            titleRect.center = new Vector2(screenWidth / 2, (screenHeight / 2) - size.y - 10);
            GUI.Label(titleRect, label);

            var iconRect = new Rect(0, 0, 401, 53);
            iconRect.center = new Vector2(screenWidth / 2, titleRect.yMin - 60);
            GUI.DrawTexture(iconRect, Styles.editorLogoIcon);

            var buttonRect = new Rect(0, 0, size.x, size.y);
            var next = 0;
            
            {
                GUI.backgroundColor = new Color(0.8f, 0.8f, 1, 1f);
                buttonRect.center = new Vector2(screenWidth / 2, (screenHeight / 2) + (size.y + 2) * next);
                next++;
                if (GUI.Button(buttonRect, "创建新时间轴"))
                {
                    InitializeAll(Commands.CreateCutscene());
                }
                GUI.backgroundColor = Color.white;
            }

            if(helpButtonPressed)
            {
                buttonRect.center = new Vector2(screenWidth / 2, (screenHeight / 2) + (size.y + 2) * next);
                next++;
                if (GUI.Button(buttonRect, "清空时间轴"))
                {
                    if(cutscene != null)
                    {
                        if(Selection.Contains(cutscene.gameObject))
                        {
                            Selection.selectionChanged = null;
                        }
                        CleanCutscne();
                    }
                }
            }

            if (helpButtonPressed && cutscene != null)
            {
                var backRect = new Rect(0, 0, 400, 20);
                backRect.center = new Vector2(screenWidth / 2, 20);
                GUI.backgroundColor = new Color(0.8f, 0.8f, 1, 1f);
                if (GUI.Button(backRect, "退出帮助面板"))
                {
                    helpButtonPressed = false;
                }
                GUI.backgroundColor = Color.white;
            }
        }

        // @end

        void ShowWelcome(){
			
			if (cutscene == null){
				helpButtonPressed = false;
			}

			var label = string.Format("<size=30><b>{0}</b></size>", helpButtonPressed? "Important and Helpful Links" : "Welcome to Super Story Engine!");
			var size = new GUIStyle("label").CalcSize(new GUIContent(label));
			var titleRect = new Rect(0,0,size.x,size.y);
			titleRect.center = new Vector2(screenWidth/2, (screenHeight/2) - size.y );
			GUI.Label(titleRect, label);

			var iconRect = new Rect(0, 0, 401, 53);
			iconRect.center = new Vector2(screenWidth/2, titleRect.yMin - 60);
			GUI.DrawTexture(iconRect, Styles.slateIcon, ScaleMode.ScaleToFit);

			var buttonRect = new Rect(0,0,size.x,size.y);
			var next = 0;

			if (!helpButtonPressed){
				GUI.backgroundColor = new Color(0.8f, 0.8f, 1, 1f);
				buttonRect.center = new Vector2(screenWidth/2, (screenHeight/2) + (size.y + 2) * next );
				next++;
				if (GUI.Button(buttonRect, "Create New Cutscene")){
					InitializeAll( Commands.CreateCutscene() );
				}
				GUI.backgroundColor = Color.white;
			}

			buttonRect.center = new Vector2(screenWidth/2, (screenHeight/2) + (size.y + 2) * next );
			next++;
			if (GUI.Button(buttonRect, "Visit The Website")){
				Help.BrowseURL("http://slate.paradoxnotion.com");
			}

			buttonRect.center = new Vector2(screenWidth/2, (screenHeight/2) + (size.y + 2) * next );
			next++;
			if (GUI.Button(buttonRect, "Read The Documentation")){
				Help.BrowseURL("http://slate.paradoxnotion.com/documentation");
			}

			buttonRect.center = new Vector2(screenWidth/2, (screenHeight/2) + (size.y + 2) * next );
			next++;
			if (GUI.Button(buttonRect, "Download Extensions")){
				Help.BrowseURL("http://slate.paradoxnotion.com/downloads");
			}

			buttonRect.center = new Vector2(screenWidth/2, (screenHeight/2) + (size.y + 2) * next );
			next++;
			if (GUI.Button(buttonRect, "Join The Forums")){
				Help.BrowseURL("http://slate.paradoxnotion.com/forums-page");
			}

			if (!helpButtonPressed){
				buttonRect.center = new Vector2(screenWidth/2, (screenHeight/2) + (size.y + 2) * next );
				next++;
				if (GUI.Button(buttonRect, "Leave a Review")){
					Help.BrowseURL("http://u3d.as/ozt");
				}
			}


			if (helpButtonPressed && cutscene != null){
				var backRect = new Rect(0,0,400, 20);
				backRect.center = new Vector2(screenWidth/2, 20);
				GUI.backgroundColor = new Color(0.8f, 0.8f, 1, 1f);
				if (GUI.Button(backRect, "Close Help Panel")){
					helpButtonPressed = false;
				}
				GUI.backgroundColor = Color.white;
			}
		}
		 // @modify slate sequencer
         // @TQ

        void DealCameraEffectDrops(Object obj)
        {
            bool isComponet = obj is Component;
            if (!isComponet)
            {
                return;
            }
            Component objComponent = obj as Component;
            bool isRenderCamera = objComponent.gameObject.GetComponent<Camera>();
            bool go = objComponent.gameObject.name == "Render Camera";
            isRenderCamera = isRenderCamera && go;
            if (!isRenderCamera)
            {
                return;
            }
            Debug.LogWarning("DealCameraEffectDrops :" + objComponent.GetType().Name);
            PropertiesTrack _postEffectProperty = null;
            for (int i = 0; i < cutscene.directorGroup.tracks.Count; i++)
            {
                if (cutscene.directorGroup.tracks[i].name == "Post Effect Track")
                {
                    _postEffectProperty = cutscene.directorGroup.tracks[i] as DirectorPropertiesTrack;
                    break;
                }
            }

            if (cutscene && _postEffectProperty == null)
            {
                _postEffectProperty = cutscene.directorGroup.AddTrack<DirectorPropertiesTrack>("Post Effect Track");
            }
            var go1 = _postEffectProperty.animatedParametersTarget as GameObject;
            EditorTools.ShowAnimatedPropertySelectionMenuForEffect(objComponent.GetType().Name, go1, AnimatedParameter.supportedTypes, (prop, comp) =>
            {
                _postEffectProperty.animationData.TryAddParameter(prop, _postEffectProperty, comp.transform, go1.transform);
            });
        }
		// end

		void AcceptDrops(){
            if (cutscene == null)
                return;

            if (cutscene.currentTime > 0){
                return;
            }
 
            var e = Event.current;
            if (e.type == EventType.DragUpdated){
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }
 
            if (e.type == EventType.DragPerform){
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++) {
                    var o = DragAndDrop.objectReferences[i];
		            // @modify slate sequencer
		            // @TQ
                    DealCameraEffectDrops(o);
					// end
                    if (o is GameObject){
                        var go = (GameObject)o;
                        if ( go.GetComponent<DirectorCamera>() != null ){
                            ShowNotification(new GUIContent("The 'DIRECTOR' group is already used for the 'DirectorCamera' object"));
                            continue;
                        }
 
                        if ( cutscene.GetAffectedActors().Contains(go) ){
                            ShowNotification(new GUIContent(string.Format("GameObject '{0}' is already in the cutscene", o.name)));
                            continue;
                        }
 
                        DragAndDrop.AcceptDrag();
                        var newGroup = cutscene.AddGroup<ActorGroup>(go);
                        // @modify slate sequencer
                        // @rongxia
                        newGroup.name = go.name;
                        // @end
                        newGroup.AddTrack<ActorActionTrack>("Action Track");
                        CutsceneUtility.selectedObject = newGroup;
                    }      
                }
            }
        }


		//The toolbar...
		void ShowToolbar(){
            if (cutscene == null)
                return;

            GUI.enabled = cutscene.currentTime <= 0;

			var e = Event.current;
		
			GUI.backgroundColor = isProSkin? new Color(1f,1f,1f,0.5f) : Color.white;
			GUI.color = Color.white;
			GUILayout.BeginHorizontal(EditorStyles.toolbar);

			if (GUILayout.Button(string.Format("[{0}]", cutscene.name), EditorStyles.toolbarDropDown, GUILayout.Width(100))){
				GenericMenu.MenuFunction2 SelectSequencer = (object cut) => {
					Selection.activeObject = (Cutscene)cut;
					EditorGUIUtility.PingObject((Cutscene)cut);
				};

				var cutscenes = FindObjectsOfType<Cutscene>();
				var menu = new GenericMenu();
				foreach (Cutscene cut in cutscenes){
					menu.AddItem(new GUIContent(string.Format("[{0}]", cut.name) ), cut == cutscene, SelectSequencer, cut);
				}
				menu.ShowAsContext();
				e.Use();
			}


			if (GUILayout.Button("Select", EditorStyles.toolbarButton, GUILayout.Width(60))){
				Selection.activeObject = cutscene;
				EditorGUIUtility.PingObject(cutscene);
			}

#if !NO_UTJ
			if (GUILayout.Button("Render", EditorStyles.toolbarButton, GUILayout.Width(60))){
				RenderWindow.Open();
			}
#endif

			if (GUILayout.Button("Snap: " + Prefs.snapInterval.ToString(), EditorStyles.toolbarDropDown, GUILayout.Width(80))){
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("0.001"), false, ()=>{ Prefs.timeStepMode = Prefs.TimeStepMode.Seconds; Prefs.frameRate = 1000; });
				menu.AddItem(new GUIContent("0.01"), false, ()=>{ Prefs.timeStepMode = Prefs.TimeStepMode.Seconds; Prefs.frameRate = 100; });
				menu.AddItem(new GUIContent("0.1"), false, ()=>{ Prefs.timeStepMode = Prefs.TimeStepMode.Seconds; Prefs.frameRate = 10; });
				menu.AddItem(new GUIContent("30 FPS"), false, ()=>{ Prefs.timeStepMode = Prefs.TimeStepMode.Frames; Prefs.frameRate = 30; });
				menu.AddItem(new GUIContent("60 FPS"), false, ()=>{ Prefs.timeStepMode = Prefs.TimeStepMode.Frames; Prefs.frameRate = 60; });
				menu.ShowAsContext();
				e.Use();
			}

			GUILayout.Space(10);

            /// @modify slate sequencer
            /// @TQ
            GUI.enabled = cutscene.currentTime <= 0;
            /// end
            GUILayout.FlexibleSpace();
			if (!Prefs.autoKey){
				var wasEnabled = GUI.enabled;
				GUI.enabled = true;
				var changedParams = CutsceneUtility.changedParameterCallbacks;
				var hasChangedParams = changedParams != null && changedParams.Count > 0;
				GUI.color = hasChangedParams? Color.white : Color.clear;
				GUILayout.BeginHorizontal(EditorStyles.toolbarButton);
				if (hasChangedParams){
					GUI.backgroundColor = Color.clear;
					GUI.color = Color.green;
					var b1 = GUILayout.Button(Styles.keyIcon, EditorStyles.toolbarButton);
					GUI.color = Color.white;
					var b2 = GUILayout.Button(string.Format("Key ({0}) Changed Parameters", changedParams.Count), EditorStyles.toolbarButton );
					GUI.backgroundColor = Color.white;
					if (b1 || b2){
						foreach(var pair in changedParams){
							pair.Value.Commit();
						}						
					}
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal();
				GUI.enabled = wasEnabled;
			}

            // @modify slate sequencer
            // @TQ

            if (GUILayout.Button("配置窗口", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                var window = GetWindow<DigitalSky.Tracker.TrackerEditorWindow>(false);
                window.titleContent = new GUIContent("Tracker Editor Window");
                window.minSize = new Vector2(800, 500);

                // OnInit is called after OnPanelEnable
                window.Init();

            }

            if (GUILayout.Button("导播台", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                var window = EditorWindow.GetWindow<DirectorRoomWindow>("导播台");
                window.Show();
            }

            if (GUILayout.Button(string.Format("Record[{0}]", cutscene.CutRecord), EditorStyles.toolbarDropDown, GUILayout.Width(100)))
            {
                GenericMenu.MenuFunction2 SelectSequencer = (object time) =>
                {
                    cutscene.CutRecord = (int)time;                
                };
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(string.Format("Record Current")), (int)cutscene.CutRecord == 0, SelectSequencer, cutscene.RecordCount+1);
                for (int i = 1; i < cutscene.RecordCount+1; i++)
                {
                   menu.AddItem(new GUIContent(string.Format("Record[{0}]", i)), (int)cutscene.CutRecord == i, SelectSequencer, i);
                }
                menu.ShowAsContext();
                e.Use();
            }

            if (GUILayout.Button("Reset Recod", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                if (EditorUtility.DisplayDialog("重置!", "是否重置录制计数?", "是", "否!"))
                {
                    cutscene.CurrentRecordTime = 0;
                    cutscene.RecordCount = 0;
                    cutscene.CutRecord = 1;
                }
            }
            GUILayout.FlexibleSpace();
           

            GUI.color = new Color(1,1,1,0.3f);
			GUILayout.Label(string.Format("<size=9>Version {0}</size>", Cutscene.VERSION_NUMBER.ToString("0.00")));
			GUI.color = Color.white;

			if (GUILayout.Button(Slate.Styles.gearIcon, EditorStyles.toolbarButton, GUILayout.Width(26))){
				PreferencesWindow.Show(new Rect(screenWidth - 5 - 400, TOOLBAR_HEIGHT + 5, 400, screenHeight - TOOLBAR_HEIGHT - 50));
			}

			helpButtonPressed = GUILayout.Toggle(helpButtonPressed, "Help", EditorStyles.toolbarButton);

			GUI.backgroundColor = new Color(1, 0.8f, 0.8f, 1);
			if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50))){
                if (EditorUtility.DisplayDialog("Clear All", "You are about to delete everything in this cutscene and start a new!\nAre you sure?", "YES", "NO!"))
                {
                    Stop(true);
                    cutscene.ClearAll();
                    InitializeAll();
                }
            }

			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			GUI.enabled = true;
		}

        // 删除Direction的camera
         public void DisableDirctionCam()
        {
            Stop(true);
            int grouCount = cutscene.groups.Count;

            for (int i = 0; i < grouCount; i++)
            {
                var group = cutscene.groups[i] as DirectorGroup;
                if (group == null)
                {
                    continue;
                }
                int trackCount = group.tracks.Count;
                for (int j = 0; j < trackCount; j++)
                {
                    var thisTrack = group.tracks[j];
                    var track = thisTrack as CameraTrack;
                    if (track == null)
                    {
                        continue;
                    }
                    track.isActive = false;
                }
            }
            InitializeAll();
        }

        //删除空内容的track
        public void ClearAllEmptyClips()
        {
            if (EditorUtility.DisplayDialog("清除!", "是否清除所有空轨道?", "是", "否!"))
            {
                Stop(true);
                int grouCount = cutscene.groups.Count;
                List<CutsceneTrack> tracksDummy = new List<CutsceneTrack>();
                tracksDummy.Clear();
                for (int i = 0; i < grouCount; i++)
                {
                    var group = cutscene.groups[i] as CutsceneGroup;
                    int trackCount = group.tracks.Count;
                    for (int j = 0; j < trackCount; j++)
                    {
                        tracksDummy.Add(group.tracks[j]);
                    }
                }

                for (int i = 0; i < grouCount; i++)
                {
                    var group = cutscene.groups[i] as CutsceneGroup;
                    int trackCount = tracksDummy.Count;
                    for (int j = 0; j < trackCount; j++)
                    {
                        var track = tracksDummy[j] as CutsceneTrack;
                        bool isProperty = tracksDummy[j] is PropertiesTrack;
                        if (track.actions.Count == 0 && !isProperty && tracksDummy[j]!= null)
                        {
                            group.DeleteTrack(tracksDummy[j]);
                            continue;
                        }
                    }
                }
                InitializeAll();
            }
        }
        // 删除动作轨和声音轨的数据
        public  void ClearAnimAndAudioClips()
        {
            if (EditorUtility.DisplayDialog("清除!", "是否清除旧的录制数据?", "是", "否!"))
            {
                Stop(true);
                int grouCount = cutscene.groups.Count;
                List<CutsceneTrack> tracksDummy = new List<CutsceneTrack>();
                tracksDummy.Clear();
                for (int i = 0; i < grouCount; i++)
                {
                    var group = cutscene.groups[i] as CutsceneGroup;
                    int trackCount = group.tracks.Count;
                    for (int j = 0; j < trackCount; j++)
                    {
                        tracksDummy.Add(group.tracks[j]);
                    }
                }

                for (int i = 0; i < grouCount; i++)
                {                   
                    var group = cutscene.groups[i] as CutsceneGroup;            
                    int trackCount = tracksDummy.Count;
                    for (int j = 0; j < trackCount; j++)
                    {
                        var track = tracksDummy[j] as CutsceneTrack;
                        if (track == null)
                        {
                            continue;
                        }

                        var dummyTrack = track as ActorActionTrack;
                        if ((dummyTrack != null && dummyTrack.name.Contains("Camera:")))
                        {
                            group.DeleteTrack(tracksDummy[j]);
                            continue;
                        }

                        if (!(track is AudioTrack ) && !(track is AnimationTrack))
                        {
                            continue;
                        }
                        group.DeleteTrack(tracksDummy[j]);
                    }
                }
                InitializeAll();
            }
        }
        //按照录制Time来执行隐藏显示逻辑
        public void DisableRecordTrackByTimes(int recordTimes)
        {
            if (recordTimes == 0)
            {
                DisableAnimAndAudioClips();
                return;
            }
                
            Stop(true);
            int grouCount = cutscene.groups.Count;
            for (int i = 0; i < grouCount; i++)
            {
                var group = cutscene.groups[i] as CutsceneGroup;
                int trackCount = group.tracks.Count;
                for (int j = 0; j < trackCount; j++)
                {
                    var track = group.tracks[j] as CutsceneTrack;
                    if (track == null)
                    {
                        continue;
                    }
                    if (track.RecordIdx == null)
                    {
                        continue;
                    }
                    if (track.RecordIdx == recordTimes)
                    {
                        //track.isActive = true;
                    }
                    else
                    {
                        track.isActive = false;
                    }
                }
            }
            InitializeAll();
        }

        public void DisableAllAudioTrack(bool enable = false)
        {
            int grouCount = cutscene.groups.Count;

            for (int i = 0; i < grouCount; i++)
            {
                var group = cutscene.groups[i] as CutsceneGroup;
                int trackCount = group.tracks.Count;
                for (int j = 0; j < trackCount; j++)
                {
                    var track = group.tracks[j] as CutsceneTrack;
                    if (track == null)
                    {
                        continue;
                    }
                    if (track is AudioTrack)
                    {
                        var dummyTrackA = track as AudioTrack;
                        dummyTrackA.IsNeedSound = enable;
                    }
                }
            }
        }

        // disable动作轨和声音轨的数据
        public void DisableAnimAndAudioClips()
        {
            Stop(true);
            int grouCount = cutscene.groups.Count;

            for (int i = 0; i < grouCount; i++)
            {
                var group = cutscene.groups[i] as CutsceneGroup;
                int trackCount = group.tracks.Count;
                for (int j = 0; j < trackCount; j++)
                {
                    var track = group.tracks[j] as CutsceneTrack;
                    if (track == null)
                    {
                        continue;
                    }
                    var dummyTrack = track as ActorActionTrack;
                    if ((dummyTrack != null && dummyTrack.name.Contains("Camera:")))
                    {
                        track.isActive = false;
                        continue;
                    }

                    if (!(track is AudioTrack) && !(track is AnimationTrack))
                    {
                        continue;
                    }

                    if (track is AudioTrack)
                    {
                        var dummyTrackA = track as AudioTrack;
                        if (!dummyTrackA.IsAutoRecord)
                        {
                            continue;
                        }
                    }


                    if (!track.isActive)
                    {
                        continue;
                    }
                    track.isActive = false;
                }
            }
            InitializeAll();
        }

		//Scrubing....
		void DoScrubControls(){


            if (cutscene == null)
                return;

            if (cutscene.isActive){ //no scrubbing if playing in runtime
				return;
			}

			///////
			var e = Event.current;
            Rect dummyRect = new Rect(topMiddleRect.x, topMiddleRect.y + 16, topMiddleRect.width, topMiddleRect.height - 16);
            if (e.type == EventType.MouseDown && dummyRect.Contains(mousePosition) ){
				var carretPos = TimeToPos(length) + leftRect.width;
/// @modify slate sequencer
/// add by TQ
                var carretStartPlayPos = TimeToPos(startPlayTime) + leftRect.width;
				var isEndCarret = Mathf.Abs(mousePosition.x - carretPos) < 10 && !e.control;
                var isStartCarret = Mathf.Abs(mousePosition.x - carretStartPlayPos) < 10 && e.control/*|| e.control*/;
				
				if (e.button == 0){
					movingEndCarret = isEndCarret;
                    movingStartCarret = isStartCarret;
                    movingScrubCarret = !movingEndCarret && !movingStartCarret;
                    DisableAllAudioTrack(false);

                    Pause();
				}
/// end               

				if (e.button == 1){
					if (isEndCarret){
						if (cutscene.directables != null){
							var menu = new GenericMenu();
							menu.AddItem(new GUIContent("Set To Last Clip Time"), false, ()=>
								{
									var lastClip = cutscene.directables.Where(d => d is ActionClip).OrderBy(d => d.endTime).LastOrDefault();
									if (lastClip != null){
										length = lastClip.endTime;
									}
								});
							menu.ShowAsContext();
						}
					}
/// @modify slate sequencer
/// add by TQ
                    if (isStartCarret)
                    {
                        if (cutscene.directables != null)
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Set To First Clip Time"), false, () =>
                            {
                                var firstClip = cutscene.directables.Where(d => d is ActionClip).OrderBy(d => d.startTime).FirstOrDefault();
                                if (firstClip != null)
                                {
                                    startPlayTime = firstClip.startTime;
                                }
                            });
                            menu.ShowAsContext();
                        }

                    }

				}
/// end
				e.Use();
			}

			if (e.button == 0 && e.rawType == EventType.MouseUp){
				movingScrubCarret = false;
				movingEndCarret = false;
/// @modify slate sequencer
/// add by TQ
                movingStartCarret = false;
/// end
                //DisableAllAudioTrack(true);
			}

			var pointerTime = PosToTime(mousePosition.x);
			if (movingScrubCarret){
				cutscene.currentTime = SnapTime(pointerTime);
				cutscene.currentTime = Mathf.Clamp(cutscene.currentTime, Mathf.Max(viewTimeMin, startPlayTime) + float.Epsilon, viewTimeMax - float.Epsilon);
/// @modify slate sequencer
/// add by TQ
                float MagnetNearTime1 = cutscene.GetKeyTimes().LastOrDefault(t => t < cutscene.currentTime - 0.01f);
                float MagnetNearTime2 = cutscene.GetKeyTimes().FirstOrDefault(t => t > cutscene.currentTime + 0.01f);

                if (Prefs.magnetSnapping && e.shift)
                { //magnet snap
                    if (Mathf.Abs(cutscene.currentTime - MagnetNearTime1) <= magnetSnapInterval)
                    {
                        cutscene.currentTime = MagnetNearTime1;
                        return;
                    }
                    if (Mathf.Abs(cutscene.currentTime - MagnetNearTime2) <= magnetSnapInterval)
                    {
                        cutscene.currentTime = MagnetNearTime2;
                        return;
                    }
                }
/// end
            }

			if (movingEndCarret){
				length = SnapTime(pointerTime);
				length = Mathf.Clamp(length, viewTimeMin + float.Epsilon, viewTimeMax - float.Epsilon);
/// @modify slate sequencer
/// add by TQ
                float MagnetNearTime1 = cutscene.GetKeyTimes().LastOrDefault(t => t < length - 0.01f);
                float MagnetNearTime2 = cutscene.GetKeyTimes().FirstOrDefault(t => t > length + 0.01f);
                if (Prefs.magnetSnapping && !e.control)
                {
                    if (Mathf.Abs(length - MagnetNearTime1) <= magnetSnapInterval)
                    {
                        length = MagnetNearTime1;
                        return;
                    }
                    if (Mathf.Abs(length - MagnetNearTime2) <= magnetSnapInterval)
                    {
                        length = MagnetNearTime2;
                        return;
                    }

                    if (Mathf.Abs(length - cutscene.currentTime) <= magnetSnapInterval)
                    {
                        length = cutscene.currentTime;
                        return;
                    }
                }
/// end
            }
/// @modify slate sequencer
/// add by TQ
            if (movingStartCarret)
            {
                startPlayTime = SnapTime(pointerTime);
                startPlayTime = Mathf.Clamp(startPlayTime, viewTimeMin + float.Epsilon, viewTimeMax - float.Epsilon);

                float MagnetNearTime1 = cutscene.GetKeyTimes().LastOrDefault(t => t < cutscene.startPlayTime - 0.01f);
                float MagnetNearTime2 = cutscene.GetKeyTimes().FirstOrDefault(t => t > cutscene.startPlayTime + 0.01f);
              
                if (Prefs.magnetSnapping && !e.control)
                { //magnet snap
                    if (Mathf.Abs(startPlayTime - MagnetNearTime1) <= magnetSnapInterval)
                    {
                        startPlayTime = MagnetNearTime1;
                        return;
                    }
                    if (Mathf.Abs(startPlayTime - MagnetNearTime2) <= magnetSnapInterval)
                    {
                        startPlayTime = MagnetNearTime2;
                        return;
                    }
                    if (Mathf.Abs(startPlayTime - cutscene.currentTime) <= magnetSnapInterval)
                    {
                        startPlayTime = cutscene.currentTime;
                        return;
                    }
                }                 
            }
/// end
		}

		void DoZoomAndPan(){
			
			if (!centerRect.Contains(mousePosition)){
				return;
			}

			var e = Event.current;
			//Zoom or scroll down/up if prefs is set to scrollwheel
			if ( (e.type == EventType.ScrollWheel && Prefs.scrollWheelZooms ) || (e.alt && !e.shift && e.button == 1) ){
				this.AddCursorRect(centerRect, MouseCursor.Zoom);
				if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp || e.type == EventType.ScrollWheel){
					var pointerTimeA = PosToTime( mousePosition.x );
					var delta = e.alt? -e.delta.x * 0.1f : e.delta.y;
					var t = (Mathf.Abs(delta * 25) / centerRect.width ) * viewTime;
                    if (!e.control)
                    {
                        viewTimeMin += delta > 0 ? -t : t;
                        viewTimeMax += delta > 0 ? t : -t;
                    }
                    var pointerTimeB = PosToTime( mousePosition.x + e.delta.x );
					var diff = pointerTimeA - pointerTimeB;
                    if (!e.control)
                    {
                        viewTimeMin += diff;
                        viewTimeMax += diff;
                    }
                    if (e.control)
                    {
                        viewCurveBoxHeightMax += (delta > 0 ? -t : t) * 20f;
                        scrollPos.y -= e.delta.y;
                    }
					e.Use();
				}
			}

            //pan left/right, up/down
            if (mouseButton2Down || (e.alt && !e.shift && e.button == 0))
            {
                this.AddCursorRect(centerRect, MouseCursor.Pan);
                if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp)
                {
                    var t = (Mathf.Abs(e.delta.x) / centerRect.width) * viewTime;
                    viewTimeMin += e.delta.x > 0 ? -t : t;
                    viewTimeMax += e.delta.x > 0 ? -t : t;
                    scrollPos.y -= e.delta.y;
                    e.Use();
                }
            }
        }

        /// @modify slate sequencer
        /// @TQ
        //top left controls 1
        void ShowEditHelpControls(Rect topLeftRect)
        {
            if (!(SlateExtensions.Instance != null && SlateExtensions.Instance.RecordUtility != null && SlateExtensions.Instance.RecordUtility.IsWaittingRecordState))
            {
                GUI.enabled = false;
            }
            Rect temp = new Rect(topLeftRect.x, topLeftRect.y, topLeftRect.width, topLeftRect.height);
            GUI.Box(topLeftRect, ""); 
            var connectRect = new Rect(topLeftRect.xMin + 10, topLeftRect.yMin + 8, 52, 30);
            AddCursorRect(connectRect, MouseCursor.Link);
            GUI.backgroundColor = Prefs.autoKey ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.0f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);

            if (GUI.Button(connectRect, SlateExtensions.Instance.RecordUtility.IsRecordActive ? Styles.connect1 : Styles.connect2, (GUIStyle)"box"))
            {
#if UNITY_EDITOR
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
                SlateExtensions.Instance.RecordUtility.SetRecordActive(!SlateExtensions.Instance.RecordUtility.IsRecordActive, cutscene);
            }
            var connectLabelRect = connectRect;
            connectLabelRect.yMin += 15;
            connectLabelRect.height = connectLabelRect.height + 10;
            GUI.Label(connectLabelRect, isProSkin ? "<color=#AAAAAA>Preview</color>" : "<color=#333333>Preview</color>", Styles.centerLabel);
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;

           // var autoCutRect = new Rect(topLeftRect.xMin + 50, topLeftRect.yMin + 4, 30, 30);
           // AddCursorRect(autoCutRect, MouseCursor.Link);
           // GUI.backgroundColor = Prefs.autoCut ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.0f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
           // //GUI.Box(autoCutRect, "", Styles.clipBoxStyle);
           // //GUI.color = Prefs.autoCut ? new Color(1, 0.4f, 0.4f) : Color.white;
           // if (GUI.Button(autoCutRect, Prefs.autoCut? Styles.clip2 : Styles.clip1, (GUIStyle)"box"))
           // {
           //     Prefs.autoCut = !Prefs.autoCut;
           //     ShowNotification(new GUIContent(string.Format("Clip AutoCut {0}", Prefs.autoCut ? "Enabled" : "Disabled"), Styles.keyIcon));
           // }
           // var autoCutLabelRect = autoCutRect;
           // autoCutLabelRect.yMin += 20;
           // autoCutLabelRect.height = autoCutLabelRect.height + 10;
           // GUI.Label(autoCutLabelRect, isProSkin ? "<color=#AAAAAA>Clip</color>" : "<color=#333333>Clip</color>", Styles.centerLabel);
           // GUI.backgroundColor = Color.white;
           // GUI.color = Color.white;

           // var autoCutCamerRect = new Rect(topLeftRect.xMin + 90, topLeftRect.yMin + 4, 30, 30);
           // AddCursorRect(autoCutCamerRect, MouseCursor.Link);
           // GUI.backgroundColor = Prefs.autoCutCam ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.0f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
           // //GUI.Box(autoCutCamerRect, "", Styles.clipBoxStyle);
           // //GUI.color = Prefs.autoCutCam ? new Color(1, 0.4f, 0.4f) : Color.white;
           // if (GUI.Button(autoCutCamerRect, Prefs.autoCutCam ? Styles.cam2 : Styles.cam1, (GUIStyle)"box"))
           // {
           //     Prefs.autoCutCam = !Prefs.autoCutCam;
           //     ShowNotification(new GUIContent(string.Format("Camera AutoCut {0}", Prefs.autoCutCam ? "Enabled" : "Disabled"), Styles.keyIcon));
           // }
           // var autoCutCamLabelRect = autoCutCamerRect;
           // autoCutCamLabelRect.yMin += 20;
           // autoCutCamLabelRect.height = autoCutCamLabelRect.height + 10;
           // GUI.Label(autoCutCamLabelRect, isProSkin ? "<color=#AAAAAA>Cam</color>" : "<color=#333333>Cam</color>", Styles.centerLabel);
           // GUI.backgroundColor = Color.white;
           // GUI.color = Color.white;

           // var autoCutCamerRect1 = new Rect(topLeftRect.xMin + 130, topLeftRect.yMin + 4, 30, 30);
           // AddCursorRect(autoCutCamerRect1, MouseCursor.Link);
           // GUI.backgroundColor = Prefs.magnetSnapping ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.0f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
           //// GUI.Box(autoCutCamerRect1, "", Styles.clipBoxStyle);
           // //GUI.color = Prefs.magnetSnapping ? new Color(1, 0.4f, 0.4f) : Color.white;
           // if (GUI.Button(autoCutCamerRect1, Prefs.magnetSnapping ? Styles.magnetIcon1:Styles.magnetIcon, (GUIStyle)"box"))
           // {
           //     Prefs.magnetSnapping =!Prefs.magnetSnapping;
           //     ShowNotification(new GUIContent(string.Format("Magnet Snapping {0}", Prefs.magnetSnapping ? "Enabled" : "Disabled"), Styles.magnetIcon));
           // }
           // var autoCutCamLabelRect1 = autoCutCamerRect1;
           // autoCutCamLabelRect1.yMin += 20;
           // autoCutCamLabelRect1.height = autoCutCamLabelRect1.height + 10;
           // GUI.Label(autoCutCamLabelRect1, isProSkin ? "<color=#AAAAAA>Mag</color>" : "<color=#333333>Mag</color>", Styles.centerLabel);
           // GUI.backgroundColor = Color.white;
           // GUI.color = Color.white;

           // var autoCutCamerRect2 = new Rect(topLeftRect.xMin + 170, topLeftRect.yMin + 4, 30, 30);
           // AddCursorRect(autoCutCamerRect2, MouseCursor.Link);
           // GUI.backgroundColor = !Prefs.showChoosenTrack ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.0f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
           // //GUI.Box(autoCutCamerRect2, "", Styles.clipBoxStyle);
           // //GUI.color = !Prefs.showChoosenTrack ? new Color(1, 0.4f, 0.4f) : Color.white;
           // GUI.color = Color.white;
           // if (GUI.Button(autoCutCamerRect2, Prefs.showChoosenTrack ? Styles.DisplayMIcon : Styles.HideAIcon, (GUIStyle)"box"))
           // {
           //     bool hasCheckedTrack = false;
           //     //GROUPS
           //     for (int g = 0; g < cutscene.groups.Count; g++)
           //     {
           //         var group = cutscene.groups[g];
           //         if (group.HideCheceked)
           //         {
           //             group.ShowInTimeLine = !group.ShowInTimeLine;
           //             hasCheckedTrack = true;
           //             continue;
           //         }

           //         //TRACKS
           //         for (int t = 0; t < group.tracks.Count; t++)
           //         {
           //             var track = group.tracks[t];
           //             if (track.isHideChecked)
           //             {
           //                 track.isShowInTimeline = !track.isShowInTimeline;
           //                 hasCheckedTrack = true;
           //             }
           //             if ((track is PropertiesTrack) && (track is IKeyable))
           //             {
           //                 var pTrack = track as PropertiesTrack;
           //                 var keyAble = track as IKeyable;
           //                 if (keyAble.animationData != null && keyAble.animationData.animatedParameters != null)
           //                 {
           //                     var paramsCount = keyAble.animationData.animatedParameters.Count;
           //                     for (var i = 0; i < paramsCount; i++)
           //                     {
           //                         var animParam = keyAble.animationData.animatedParameters[i];
           //                         if (animParam.hideChecked)
           //                         {
           //                             if (pTrack.InspectedParameterIndex == i)
           //                             {
           //                                 pTrack.InspectedParameterIndex = -1;
           //                             }
           //                             hasCheckedTrack = true;
           //                         }
           //                     }
           //                 }
           //             }                          
           //         }
           //     }
           //     if (hasCheckedTrack)
           //     {
           //         Prefs.showChoosenTrack = !Prefs.showChoosenTrack;
           //     }
           //     else
           //     {
           //         Prefs.showChoosenTrack = !hasCheckedTrack;
           //     }
           //     ShowNotification(new GUIContent(Prefs.showChoosenTrack ? string.Format("Show The Choosen Clips!") : string.Format("Hide The Choosen Clips!")));
           // }
           // var autoCutCamLabelRect2 = autoCutCamerRect2;
           // autoCutCamLabelRect2.yMin += 20;
           // autoCutCamLabelRect2.height = autoCutCamLabelRect2.height + 10;
           // GUI.Label(autoCutCamLabelRect2, isProSkin ? "<color=#AAAAAA>Hide</color>" : "<color=#333333>Hide</color>", Styles.centerLabel);
           // GUI.backgroundColor = Color.white;
           // GUI.color = Color.white;

           // var autoCutCamerRect3 = new Rect(topLeftRect.xMin + 210, topLeftRect.yMin + 4, 30, 30);
           // AddCursorRect(autoCutCamerRect3, MouseCursor.Link);
           // GUI.backgroundColor = Prefs.ShowAllGizmoLine ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
           // //GUI.Box(autoCutCamerRect3, "", Styles.clipBoxStyle);
           // //GUI.color = Prefs.ShowAllGizmoLine ? new Color(1, 0.4f, 0.4f) : Color.white;
           // if (GUI.Button(autoCutCamerRect3, Prefs.ShowAllGizmoLine ? Styles.DisplayTrackIcon : Styles.HideTrackIcon, (GUIStyle)"box"))
           // {
           //     Prefs.ShowAllGizmoLine = !Prefs.ShowAllGizmoLine;
           //     ShowNotification(new GUIContent(string.Format("Show All Gizmo Line {0}", Prefs.ShowAllGizmoLine ? "Enabled" : "Disabled")));
           // }
           // var autoCutCamLabelRect3 = autoCutCamerRect3;
           // autoCutCamLabelRect3.yMin += 20;
           // autoCutCamLabelRect3.height = autoCutCamLabelRect3.height + 10;
           // GUI.Label(autoCutCamLabelRect3, isProSkin? "<color=#AAAAAA>Line</color>" : "<color=#333333>Line</color>", Styles.centerLabel);
           // GUI.backgroundColor = Color.white;
           // GUI.color = Color.white;

            //var autoCutCamerRect4 = new Rect(topLeftRect.xMax - 50, topLeftRect.yMin + 2, 30, 30);
            //AddCursorRect(autoCutCamerRect4, MouseCursor.Link);
            //GUI.backgroundColor = Prefs.ShowAllGizmoLine ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
            ////GUI.Box(autoCutCamerRect3, "", Styles.clipBoxStyle);
            ////GUI.color = Prefs.ShowAllGizmoLine ? new Color(1, 0.4f, 0.4f) : Color.white;
            //if (GUI.Button(autoCutCamerRect4, Styles.upload, (GUIStyle)"box"))
            //{
            //    //Prefs.ShowAllGizmoLine = !Prefs.ShowAllGizmoLine;
            //    //ShowNotification(new GUIContent(string.Format("Show All Gizmo Line {0}", Prefs.ShowAllGizmoLine ? "Enabled" : "Disabled")));
            //    ///BuildAndUpload();              
            //}
            //var autoCutCamLabelRect4 = autoCutCamerRect4;
            //autoCutCamLabelRect4.xMin = autoCutCamLabelRect4.xMin - 4;
            //autoCutCamLabelRect4.yMin += 20;
            //autoCutCamLabelRect4.width = autoCutCamLabelRect4.width + 10;
            //autoCutCamLabelRect4.height = autoCutCamLabelRect4.height + 13;
            //GUI.Label(autoCutCamLabelRect4, isProSkin ? "<color=#AAAAAA>U/L</color>" : "<color=#333333>U/L</color>", Styles.centerLabel);
            //GUI.backgroundColor = Color.white;
            //GUI.color = Color.white;

            /// hide all
            ///  
            //var autoCutCamerRectHideAll = new Rect(topLeftRect.xMin + 250, topLeftRect.yMin + 2, 30, 30);
            //AddCursorRect(autoCutCamerRectHideAll, MouseCursor.Link);
            //GUI.backgroundColor = Prefs.ShowAllGizmoLine ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
            ////GUI.Box(autoCutCamerRect3, "", Styles.clipBoxStyle);
            ////GUI.color = Prefs.ShowAllGizmoLine ? new Color(1, 0.4f, 0.4f) : Color.white;
            //if (GUI.Button(autoCutCamerRectHideAll, Prefs.HideAllTrack ? Styles.DisplayAllIcon : Styles.hideAllIcon, (GUIStyle)"box"))
            //{
            //    Prefs.HideAllTrack = !Prefs.HideAllTrack;
            //    ShowNotification(new GUIContent(string.Format("Hide All Tracks {0}", Prefs.HideAllTrack ? "Enabled" : "Disabled")));
            //}
            //var autoCutCamLabelRectHideAll = autoCutCamerRectHideAll;
            //autoCutCamLabelRectHideAll.xMin = autoCutCamerRectHideAll.xMin - 8;
            //autoCutCamLabelRectHideAll.yMin += 20;
            //autoCutCamLabelRectHideAll.width = autoCutCamLabelRectHideAll.width + 10;
            //autoCutCamLabelRectHideAll.height = autoCutCamLabelRectHideAll.height + 13;
            //GUI.Label(autoCutCamLabelRectHideAll, isProSkin ? "<color=#AAAAAA>All</color>" : "<color=#333333>All</color>", Styles.centerLabel);
            //GUI.backgroundColor = Color.white;
            //GUI.color = Color.white; 

            var autoCutCamerRect5 = new Rect(topLeftRect.xMin + 430, topLeftRect.yMin + 6, 30, 30);
            AddCursorRect(autoCutCamerRect5, MouseCursor.Link);
            GUI.backgroundColor = Prefs.ShowAllGizmoLine ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
            //GUI.Box(autoCutCamerRect3, "", Styles.clipBoxStyle);
            //GUI.color = Prefs.ShowAllGizmoLine ? new Color(1, 0.4f, 0.4f) : Color.white;
            if (GUI.Button(autoCutCamerRect5, Styles.nextCutIcon, (GUIStyle)"box"))
            {
                bool isSubCutRoot = cutscene.PrevCutscene != null && cutscene.name.Contains("Take") && cutscene.name.Contains("_Cut_");

                var autoTrackName = "SubCut_Take" + cutscene.CutRecord;
                if (isSubCutRoot)
                {
                    autoTrackName = "SubSubCut_Take" + cutscene.CutRecord;
                }
                var curCutTrack = cutscene.GetTrackByName(autoTrackName);
                if (curCutTrack == null)
                {
                    var curCutTrackPre = cutscene.GetTrackByName("SubCut_Take" + (cutscene.CutRecord - 1));
                    if (isSubCutRoot)
                    {
                        curCutTrackPre = cutscene.GetTrackByName("SubSubCut_Take" + (cutscene.CutRecord - 1));
                    }
                    var lastClipPre = curCutTrackPre == null ? null : curCutTrackPre.actions.Where(d => d is ActionClip).OrderBy(d => d.endTime).LastOrDefault();
                    if (lastClipPre != null)
                    {
                        cutscene.currentTime = lastClipPre.endTime;
                    }
                    return;
                }
                var lastClip = curCutTrack.actions.Where(d => d is ActionClip).OrderBy(d => d.endTime).LastOrDefault();
                if (lastClip == null)
                    return;
                cutscene.currentTime = lastClip.endTime;
                cutscene.CutRecord = cutscene.CutRecord + 1;
                cutscene.RecordCount = cutscene.RecordCount + 1;
            }
            var autoCutCamLabelRect5 = autoCutCamerRect5;
            autoCutCamLabelRect5.xMin = autoCutCamerRect5.xMin - 14;
            autoCutCamLabelRect5.yMin += 20;
            autoCutCamLabelRect5.width = autoCutCamerRect5.width + 25;
            autoCutCamLabelRect5.height = autoCutCamerRect5.height - 5;
            GUI.Label(autoCutCamLabelRect5, isProSkin ? "<color=#AAAAAA>Next Cut</color>" : "<color=#333333>Next Cut</color>", Styles.centerLabel);
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
            if (current.cutscene.PrevCutscene != null)
            {
                //back parent cutsence
                var autoCutCamerRectBack = new Rect(topLeftRect.xMax - 50, topLeftRect.yMin + 4, 30, 30);
                AddCursorRect(autoCutCamerRectBack, MouseCursor.Link);
                GUI.backgroundColor = Prefs.ShowAllGizmoLine ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);

                if (GUI.Button(autoCutCamerRectBack, Styles.CutReturnIcon, (GUIStyle)"box"))
                {
                    if (current.cutscene.PrevCutscene != null)
                    {
                        Selection.activeObject = current.cutscene.PrevCutscene;
                    }
                    if (current.cutscene.IsSubEdite)
                    {
                        Cutscene.DestroyImmediate(current.cutscene.gameObject);
                    }

                }
                var autoCutCamLabelRectBack = autoCutCamerRectBack;
                //autoCutCamLabelRectBack.xMin = autoCutCamLabelRectBack.xMin - 4;
                autoCutCamLabelRectBack.yMin += 20;
                autoCutCamLabelRectBack.width = autoCutCamLabelRectBack.width + 10;
                autoCutCamLabelRectBack.height = autoCutCamLabelRectBack.height + 13;
                GUI.Label(autoCutCamLabelRectBack, isProSkin ? "<color=#AAAAAA>Back</color>" : "<color=#333333>Back</color>", Styles.centerLabel);
                GUI.backgroundColor = Color.white;
                GUI.color = Color.white;
            }


            if (current.cutscene!= null && current.cutscene.PrevCutscene != null && current.cutscene.IsSubEdite)
            {
                //back parent cutsence
                var autoCutCamerRectSaveEdit = new Rect(topLeftRect.xMax - 90, topLeftRect.yMin + 4, 30, 30);
                AddCursorRect(autoCutCamerRectSaveEdit, MouseCursor.Link);
                GUI.backgroundColor = Prefs.ShowAllGizmoLine ? (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.0f);
                //GUI.Box(autoCutCamerRect3, "", Styles.clipBoxStyle);
                //GUI.color = Prefs.ShowAllGizmoLine ? new Color(1, 0.4f, 0.4f) : Color.white;
                if (GUI.Button(autoCutCamerRectSaveEdit, Styles.CutReturnIcon, (GUIStyle)"box"))
                {
                    if (EditorUtility.DisplayDialog("删除并保存!", "是否保存当前编辑的录制文件而删除其他?", "是", "否!"))
                    {
                        List<CutsceneTrack> templist = cutscene.directorGroup.tracks.FindAll(t => t.name == "Composition Track");
                        for (int i = 0; i < templist.Count; i++)
                        {
                            CutsceneTrack t = templist[i];
                            if (!t.isActive)
                            {
                                if (t.actions.Count > 0)
                                {
                                    var subcutAction = t.actions[0] as SubCutscene;                              
                                    t.DeleteAction(subcutAction);
                                    Object.DestroyImmediate(subcutAction.cutscene.gameObject);
                                }
                            }
                            else
                            {
                                if (t.actions.Count > 0)
                                {
                                    var subcutAction = t.actions[0] as SubCutscene;
                                    var cp = cutscene.EditFromCutClip as SubCutscene;
                                    cp.length = subcutAction.cutscene.length;
                                    cp.cutscene = subcutAction.cutscene;                                    
                                }
                            }                       
                        }

                        var preCut = cutscene.PrevCutscene as Cutscene;
                        if (preCut != null)
                        {
                            var cp = cutscene.EditFromCutClip as SubCutscene;

                            var invalidCamClip = preCut.cameraTrackCustom.actions.FindAll(g => g.isActive == false || g.startTime < cp.startTime + cp.length);
                            foreach (var incc in invalidCamClip)
                            {
                                preCut.cameraTrackCustom.DeleteAction(incc);
                            }
                            ////CutsceneEditor.current.SubCutsenceSysCamFunc(editor.anyClipDragging);
                        }
                    }
                }
                var autoCutCamLabelRectSaveEdit = autoCutCamerRectSaveEdit;
                //autoCutCamLabelRectBack.xMin = autoCutCamLabelRectBack.xMin - 4;
                autoCutCamLabelRectSaveEdit.yMin += 20;
                autoCutCamLabelRectSaveEdit.width = autoCutCamLabelRectSaveEdit.width + 10;
                autoCutCamLabelRectSaveEdit.height = autoCutCamLabelRectSaveEdit.height + 13;
                GUI.Label(autoCutCamLabelRectSaveEdit, isProSkin ? "<color=#AAAAAA>Save</color>" : "<color=#333333>Save</color>", Styles.centerLabel);
                GUI.backgroundColor = Color.white;
                GUI.color = Color.white;
            }

            GUI.enabled = true;
        }
        /// end

        /// @modify slate sequencer
        /// @TQ
        //top left controls
        void ShowPlaybackControls(Rect topLeftRect)
        {
            
            //Cutscene shows the gui
            Rect dummyRect = new Rect(topLeftRect.x, topLeftRect.y + 6, topLeftRect.width, topLeftRect.height);
            topLeftRect = dummyRect;
            GUI.Box(dummyRect, "");

            GUILayout.BeginArea(topLeftRect);
            topLeftRect.y = topLeftRect.y - topLeftRect.height;
            topLeftRect.xMin = topLeftRect.xMin;

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Space(50);

            GUI.backgroundColor = (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.0f));
            Rect lastRect;

            if (!(SlateExtensions.Instance != null && SlateExtensions.Instance.RecordUtility != null && SlateExtensions.Instance.RecordUtility.IsWaittingRecordState))
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(Styles.StartPointIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                current.cutscene.startPlayTime = current.cutscene.currentTime;
                Event.current.Use();
            }

            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }
            GUILayout.Space(-10);
            if (GUILayout.Button(Styles.stepReverseIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                StepBackward();
                Event.current.Use();
            }
            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }
            GUILayout.Space(-10);
            if (GUILayout.Button(Styles.PrevFrameIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                StepBackwardPreFrame();
                Event.current.Use();
            }
            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }
            GUILayout.Space(-10);

            var isStoped = Application.isPlaying ? (cutscene.isPaused || !cutscene.isActive) : editorPlayback == EditorPlayback.Stoped;
            if (GUILayout.Button(!isStoped && editorPlayback == EditorPlayback.PlayingBackwards ? Styles.pauseIcon : Styles.playReverseIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                if (!isStoped && editorPlayback == EditorPlayback.PlayingBackwards)
                {
                    Pause();
                }
                else
                {
                    PlayReverse();
                    DisableAllAudioTrack(true);
                }
                Event.current.Use();
            }

            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }
            GUILayout.Space(-10);
            if (GUILayout.Button(!isStoped && editorPlayback == EditorPlayback.PlayingForwards ? Styles.pauseIcon : Styles.playIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                if (!isStoped && editorPlayback == EditorPlayback.PlayingForwards)
                {
                    Pause();
                }
                else
                {
                    Play();
                    DisableAllAudioTrack(true);
                }
                Event.current.Use();
            }
            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }

            GUILayout.Space(-10);
            if (GUILayout.Button(Styles.NextFrameIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                StepForwardNextFrame();
                Event.current.Use();
            }

            GUILayout.Space(-10);
            if (GUILayout.Button(Styles.stepIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                StepForward();
                Event.current.Use();
            }
            GUI.enabled = true;

            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }

            if (SlateExtensions.Instance != null && SlateExtensions.Instance.RecordUtility != null && SlateExtensions.Instance.RecordUtility.IsWaittingRecordState)
            {
                GUILayout.Space(-10);
                if (GUILayout.Button(Styles.stopIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
                {
                    Stop(false);
                    Event.current.Use();
                }
            }

            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }

            GUILayout.Space(-10);
            if (GUILayout.Button(Styles.EndPointIcon, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                current.cutscene.length = current.cutscene.currentTime;
                Event.current.Use();
            }
            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }

            if (GUILayout.Button(SlateExtensions.Instance.RecordUtility.IsWaittingRecordState ? Styles.recordIcon : Styles.recordStop, (GUIStyle)"box", GUILayout.Width(30), GUILayout.Height(30)))
            {
                if (!SlateExtensions.Instance.RecordUtility.IsRecordEnable())
                {
                    SlateExtensions.Instance.RecordUtility.SetRecordActive(true, cutscene);
                }
                if (SlateExtensions.Instance.RecordUtility.IsWaittingRecordState)
                {

                    SetRecordCurrentStartTime();
                    StartRecord();
                    DisableAllAudioTrack(true);
                    Play();
                }
                else
                {
                    StopRecord();
                }
#if UNITY_EDITOR
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
                Event.current.Use();
            }

            Debug.unityLogger.logEnabled = false;
            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }

            Rect dummyTime = new Rect(lastRect.x + 28, lastRect.y + 6, lastRect.width + 78, lastRect.height);
            Rect dummyTimeRect = dummyTime;
            Rect dummyFrameRect = dummyTime;
            dummyTimeRect.y = dummyTimeRect.y - 6;
            dummyFrameRect.y = dummyFrameRect.y - 13;
            GUI.backgroundColor = Color.black;
            GUI.Box(dummyTimeRect, "");
            if (cutscene != null && !(SlateExtensions.Instance.RecordUtility.IsRecording || SlateExtensions.Instance.RecordUtility.IsCountDownRecordState))
            {
                GUI.Label(dummyTime, "<color=#00b7ee><size=9>" + HumanizeTimeString(cutscene.currentTime) + "(" + Prefs.frameRate + "fps) </size></color>", Styles.centerLabel);
            }
            else
            {
                if (cutscene != null)
                    GUI.Label(dummyTime, "<color=#00b7ee><size=9>" + HumanizeTimeString(cutscene.currentTime) + "(" + Prefs.frameRate + "fps)</size> </color>", Styles.centerLabel);
            }

            frameOldString = (cutscene != null ? Mathf.Round(cutscene.currentTime / Prefs.snapInterval) : 0).ToString();
            GUIStyle bb = new GUIStyle();

            bb.normal.textColor = new Color(0, 183, 238);   //设置字体颜色的 
            bb.alignment = TextAnchor.MiddleCenter;

            GUI.SetNextControlName("user");
            if (GUI.GetNameOfFocusedControl() == "user")
            {
                bb.normal.background = Styles.buttonBG;
                bb.normal.textColor = Color.white;
            }
            frameInputString = frameOldString;
            frameInputString = GUI.TextField(dummyFrameRect, frameInputString, bb);
            frameInputString = System.Text.RegularExpressions.Regex.Replace(frameInputString, "[^0-9]", "");


                int frameNumber = frameInputString.ToInt32();
                if (GUI.GetNameOfFocusedControl() == "user")
                {
                    cutscene.currentTime =  SnapTime(frameNumber * Prefs.snapInterval); 
                }

            GUI.backgroundColor = (isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.0f));
            GUI.color = Color.white;

            GUI.backgroundColor = Color.white;
            GUILayout.Space(6);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            Debug.unityLogger.logEnabled = true;
        }
        /// end
/// @modify slate sequencer
/// @TQ
        public static string HumanizeTimeString(float seconds)
        {
            System.TimeSpan ts = System.TimeSpan.FromSeconds(seconds);
            string timeStr = string.Format("{0:D2}:{1:D2}:{2:D3}", ts.Minutes, ts.Seconds, Mathf.RoundToInt((float)ts.Milliseconds ));
            return timeStr;
        }

        public void SetRecordCurrentStartTime()
        {

            if (!(cutscene.currentTime > 0))
            {
                bool isSubCutRoot = cutscene.PrevCutscene != null && cutscene.name.Contains("Take") && cutscene.name.Contains("_Cut_");
                var autoTrackName = "SubCut_Take" + (cutscene.CutRecord - 1);
                if (isSubCutRoot)
                {
                    autoTrackName = "SubSubCut_Take" + (cutscene.CutRecord - 1);
                }
                var curCutTrack = cutscene.GetTrackByName(autoTrackName);
                if (curCutTrack == null)
                {
                    var curCutTrackPre = cutscene.GetTrackByName("SubCut_Take" + (cutscene.CutRecord - 1));
                    if (isSubCutRoot)
                    {
                        curCutTrackPre = cutscene.GetTrackByName("SubSubCut_Take" + (cutscene.CutRecord - 1));
                    }
                    var lastClipPre = curCutTrackPre == null ? null : curCutTrackPre.actions.Where(d => d is ActionClip).OrderBy(d => d.endTime).LastOrDefault();
                    if (lastClipPre != null)
                    {
                        cutscene.currentTime = lastClipPre.endTime;
                    }
                }
                else
                {
                    var lastClip = curCutTrack.actions.Where(d => d is ActionClip).OrderBy(d => d.endTime).LastOrDefault();
                    if (lastClip != null)
                    {
                        cutscene.currentTime = lastClip.endTime;
                    }
                }
            }

        } 

/// end

        //top mid - viewTime selection and time info
        void ShowTimeInfo(Rect topMiddleRect){
            /// @modify slate sequencer
            /// @TQ     
            /// 
            if (cutscene == null)
                return;
            Rect dummyRect = new Rect(topMiddleRect.x, topMiddleRect.y + 16, topMiddleRect.width, topMiddleRect.height - 16);
            GUI.Box(dummyRect, "");
/// end
            GUI.color = new Color(1,1,1,0.2f);
			GUI.Box(topMiddleRect, "", EditorStyles.toolbarButton);
			GUI.color = new Color(0,0,0,0.2f);
			GUI.Box(topMiddleRect, "", Styles.timeBoxStyle);
			GUI.color = Color.white;

			var timeInterval = 1000000f;
			var highMod = timeInterval;
			var lowMod = 0.01f;
			var modulos = new float[]{ 0.1f, 0.5f, 1, 5, 10, 50, 100, 500, 1000, 5000, 10000, 50000, 100000, 250000, 500000 }; //... O.o
			for (var i = 0; i < modulos.Length; i++){
				var count = viewTime / modulos[i];
				if ( centerRect.width / count > 50){ //50 is approx width of label
					timeInterval = modulos[i];
					lowMod = i > 0? modulos[ i - 1 ] : lowMod;
					highMod = i < modulos.Length - 1? modulos[i + 1] : highMod;
					break;
				}
			}

			var doFrames = Prefs.timeStepMode == Prefs.TimeStepMode.Frames;
			var timeStep = doFrames? (1f/Prefs.frameRate) : lowMod;

			var start = (float)Mathf.FloorToInt(viewTimeMin / timeInterval) * timeInterval;
			var end = (float)Mathf.CeilToInt(viewTimeMax / timeInterval) * timeInterval;
			start = Mathf.Round(start * 10) / 10;
			end = Mathf.Round(end * 10) / 10;

			//draw vertical guide lines. Do this outside the BeginArea bellow.
			for (var _i = start; _i <= end; _i += timeInterval){
				var i = Mathf.Round(_i * 10) / 10;
				var linePos = TimeToPos(i);
				DrawGuideLine(linePos, new Color(0, 0, 0, 0.4f));
				if (i % highMod == 0){
					DrawGuideLine(linePos, new Color(0,0,0,0.5f));
				}
			}

			GUILayout.BeginArea(topMiddleRect);

			//the minMax slider
			var _timeMin = viewTimeMin;
			var _timeMax = viewTimeMax;
/// @modify slate sequencer
/// @TQ
			var sliderRect = new Rect(5, 1, topMiddleRect.width - 10, 15);
/// end
			EditorGUI.MinMaxSlider(sliderRect, ref _timeMin, ref _timeMax, 0, maxTime);
			viewTimeMin = _timeMin;
			viewTimeMax = _timeMax;
			if (sliderRect.Contains(Event.current.mousePosition) && Event.current.clickCount == 2){
				viewTimeMin = 0;
				viewTimeMax = length;
			}

			GUI.color = new Color(1,1,1,0.1f);
/// @modify slate sequencer
/// @TQ
			GUI.DrawTexture( Rect.MinMaxRect(0, TOP_MARGIN1 - 1, topMiddleRect.xMax, TOP_MARGIN1), Styles.whiteTexture);
/// end
			GUI.color = Color.white;

			//the step interval
			if (centerRect.width / (viewTime/timeStep) > 6){
				for (var i = start; i <= end; i += timeStep){
					var posX = TimeToPos(i);
/// @modify slate sequencer
/// @TQ
					var frameRect = Rect.MinMaxRect(posX-1, TOP_MARGIN1 - 2, posX+1, TOP_MARGIN1 - 1 );
/// end
					GUI.color = isProSkin? Color.white : Color.black;
					GUI.DrawTexture(frameRect, whiteTexture);
					GUI.color = Color.white;
				}
			}

			//the time interval
			for (var i = start; i <= end; i += timeInterval){

				var posX = TimeToPos(i);
				var rounded = Mathf.Round(i * 10) / 10;

				GUI.color = isProSkin? Color.white : Color.black;
/// @modify slate sequencer
/// @TQ
				var markRect = Rect.MinMaxRect(posX - 2, TOP_MARGIN1 - 3, posX + 2, TOP_MARGIN1 - 1);
/// end
				GUI.DrawTexture(markRect, whiteTexture);
				GUI.color = Color.white;

				var text = doFrames? (rounded * Prefs.frameRate).ToString("0") : rounded.ToString("0.00");
				var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(text));
				var stampRect = new Rect(0, 0, size.x, size.y);
/// @modify slate sequencer
/// @TQ
				stampRect.center = new Vector2(posX, TOP_MARGIN1 - size.y + 4);
/// end
				GUI.color = rounded % highMod == 0? Color.white : new Color(1,1,1,0.5f);
				GUI.Box(stampRect, text, (GUIStyle)"label");
				GUI.color = Color.white;
			}

			//the number showing current time when scubing
			if (cutscene.currentTime > 0){
				var label = doFrames? (cutscene.currentTime * Prefs.frameRate).ToString("0") : cutscene.currentTime.ToString("0.00");
				var text = "<b><size=17>" + label + "</size></b>";
				var size = Styles.headerBoxStyle.CalcSize(new GUIContent(text));
				var posX = TimeToPos(cutscene.currentTime);
				var stampRect = new Rect(0, 0, size.x, size.y);
/// @modify slate sequencer
/// @TQ
				stampRect.center = new Vector2(posX, TOP_MARGIN1 - size.y/2 - 3);
/// end
				
				GUI.backgroundColor = isProSkin? new Color(0,0,0,0.4f) : new Color(0,0,0,0.7f);
				GUI.color = cutscene.isActive? Color.yellow : new Color(1,0.2f,0.2f);
				GUI.Box(stampRect, text, Styles.headerBoxStyle);
			}

			//the length position carret texture and pre-exit length indication
			var lengthPos = TimeToPos(length);
			var lengthRect = new Rect(0, 0, 16, 16);
/// @modify slate sequencer
/// @TQ
			lengthRect.center = new Vector2(lengthPos, TOP_MARGIN1 - 3);
/// end
			GUI.color = isProSkin? Color.white : Color.black;
			GUI.DrawTexture(lengthRect, Styles.carretIcon);

/// @modify slate sequencer
/// add by TQ
            //the start play time position carret texture and pre-exit start play time indication
            var startTimePos = TimeToPos(startPlayTime);
            var startIimeRect = new Rect(0, 0, 16, 16);
            startIimeRect.center = new Vector2(startTimePos, TOP_MARGIN1 - 3);
            GUI.color = isProSkin ? Color.white : Color.black;
            GUI.DrawTexture(startIimeRect, Styles.carretIcon);
/// end

            GUILayout.EndArea();
		}




		//left - the groups and tracks info and option per group/track
		void ShowGroupsAndTracksList(Rect leftRect){
            if (cutscene == null)
                return;

            var e = Event.current;

			//allow resize list width
			var scaleRect = new Rect(leftRect.xMax - 4, leftRect.yMin, 4, leftRect.height);
			AddCursorRect(scaleRect, MouseCursor.ResizeHorizontal);
			if (e.type == EventType.MouseDown && e.button == 0 && scaleRect.Contains(e.mousePosition)){ isResizingLeftMargin = true; e.Use(); }
			if (isResizingLeftMargin){ leftMargin = e.mousePosition.x + 2; }
			if (e.rawType == EventType.MouseUp){ isResizingLeftMargin = false;}
            // @modify slate sequencer
            // @TQ
            // avoid context click in Play modle
            GUI.enabled = true;//cutscene.currentTime <= 0;
            //end

			//starting height && search.
			var nextYPos = FIRST_GROUP_TOP_MARGIN;
			var wasEnabled = GUI.enabled;
			GUI.enabled = true;
			var collapseAllRect = Rect.MinMaxRect(leftRect.x + 5, leftRect.y + 4, 20, leftRect.y + 20 - 1 );
			var searchRect = Rect.MinMaxRect(leftRect.x + 20, leftRect.y + 4, leftRect.xMax - 18, leftRect.y + 20 - 1);
			var searchCancelRect = Rect.MinMaxRect(searchRect.xMax, searchRect.y, leftRect.xMax - 4, searchRect.yMax);
			var anyExpanded = cutscene.groups.Any(g => !g.isCollapsed);
			AddCursorRect(collapseAllRect, MouseCursor.Link);
			GUI.color = new Color(1,1,1,0.5f);
			if (GUI.Button(collapseAllRect, anyExpanded? "▼" : "►", (GUIStyle)"label" )){
				foreach(var group in cutscene.groups){
					group.isCollapsed = anyExpanded;
				}
			}
			GUI.color = Color.white;
			searchString = EditorGUI.TextField(searchRect, searchString, (GUIStyle)"ToolbarSeachTextField");
			if ( GUI.Button(searchCancelRect, "", (GUIStyle)"ToolbarSeachCancelButton") ){
				searchString = string.Empty;
				GUIUtility.keyboardControl = 0;
			}
			GUI.enabled = wasEnabled;


			//begin area for left Rect
			GUI.BeginGroup(leftRect);
			ShowListGroups(e, ref nextYPos);
			GUI.EndGroup();

			totalHeight = nextYPos;


			//Simple button to add empty group for convenience
			var addButtonY = totalHeight + TOP_MARGIN + TOOLBAR_HEIGHT + 20;
			var addRect = Rect.MinMaxRect(leftRect.xMin + 10, addButtonY, leftRect.xMax - 10, addButtonY + 20);
			
            GUI.DrawTexture(addRect, Styles.buttonBG);
            GUI.color = new Color(1, 1, 1, 0.5f);
            if (GUI.Button(addRect, "Add Actor Group"))
            {
                var newGroup = cutscene.AddGroup<ActorGroup>(null).AddTrack<ActorActionTrack>();
                CutsceneUtility.selectedObject = newGroup;
            }

            //clear picks
            if (e.rawType == EventType.MouseUp){
				pickedGroup = null;
				pickedTrack = null;
			}

			GUI.enabled = true;
			GUI.color = Color.white;
		}
        /// @modify slate sequencer
        /// @TQ
        bool CheckGroupsSelected()
        {
            if (pickedGroupList.Count <= 1)
            {
                return false;
            }       
            return true;
        }

        bool CheckTracksSelected()
        {
            return pickedTrackList.Count > 1;
        }

        bool CheckTrackCanMerge()
        {
            if (pickedTrackList.Count == 0)
                return false;
            var trackDm = pickedTrackList[0];
            var typeDm = trackDm.GetType();
            var groupDm = trackDm.parent as CutsceneGroup;
            if (groupDm is CutsceneGroup)
            {
                var sameTypeCount = pickedTrackList.Where(k => k.GetType() != typeDm).ToList();
                if (sameTypeCount.Count == 0)
                {
                    return true;
                }
            }
            else
            {
                var exist1 = pickedTrackList.Where(k => k.parent.name != groupDm.name).ToList();
                var exist2 = pickedTrackList.Where(k => k.name != trackDm.name).ToList();
                if (exist1.Count == 0 && exist2.Count == 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

		/// end
		void ShowListGroups(Event e, ref float nextYPos){

			//GROUPS
			for (int g = 0; g < cutscene.groups.Count; g++){
				var group = cutscene.groups[g];
               /// @modify slate sequencer
               /// @TQ
                if (!Prefs.showChoosenTrack && group.HideCheceked)
                {
                    continue;
                }
                //if (Prefs.HideAllTrack)
                //{
                //    continue;
                //}
                ///end
				if ( FilteredOutBySearch(group, searchString) ){
					group.isCollapsed = true;
					continue;
				}

				var groupRect = new Rect(4, nextYPos, leftRect.width - GROUP_RIGHT_MARGIN - 4, GROUP_HEIGHT - 3);
				this.AddCursorRect(groupRect, pickedGroup == null? MouseCursor.Link : MouseCursor.MoveArrow);
				nextYPos += GROUP_HEIGHT;

				///highligh?
				var groupSelected = (!CheckGroupsSelected()&&( ReferenceEquals(group, CutsceneUtility.selectedObject) || group == pickedGroup));
                /// @modify slate sequencer
                /// @TQ
                var groupMutiSelected = (pickedGroupList.Contains(group)&& CheckGroupsSelected());
                groupSelected &= !groupMutiSelected;

                GUI.color = groupSelected? listSelectionColor : groupMutiSelected? mutiSelectColor : groupColor;
                /// end
				GUI.Box(groupRect, "", Styles.headerBoxStyle);
				GUI.color = Color.white;

				//GROUP CONTROLS
				var plusClicked = false;
				GUI.color = isProSkin? new Color(1,1,1,0.5f) : new Color(0.2f,0.2f,0.2f);
				var plusRect = new Rect(groupRect.xMax - 14, groupRect.y + 5, 8, 8);

                if (GUI.Button(plusRect, Slate.Styles.plusIcon, GUIStyle.none)){plusClicked = true;}
				if (!group.isActive){
					var disableIconRect = new Rect(plusRect.xMin - 20, groupRect.y + 1, 30, 30);
					if (GUI.Button(disableIconRect, Styles.hiddenIcon, GUIStyle.none)){ /*group.isActive = true;*/ }
				}
				if (group.isLocked){
					var lockIconRect = new Rect(plusRect.xMin - (group.isActive? 20 : 36), groupRect.y + 1, 16, 16);
					if (GUI.Button(lockIconRect, Styles.lockIcon, GUIStyle.none)){ /*group.isLocked = false;*/ }
				}

                var hideRect = new Rect(plusRect.x - 20, plusRect.y-6, 20, 20);
                GUI.color = !group.HideCheceked ? (isProSkin ? new Color(1, 1, 1, 0.5f) : new Color(0.2f, 0.2f, 0.2f)): new Color(1, 0.4f, 0.4f);
                if (GUI.Button(hideRect, group.HideCheceked? Slate.Styles.HideSIcon : Slate.Styles.DisplaySIcon, GUIStyle.none))
                {
                    //if (Prefs.showChoosenTrack)
                    // group.HideCheceked = !group.HideCheceked;
                    if (CheckGroupsSelected())
                    {
                        for (int idx = 0; idx < pickedGroupList.Count; idx++)
                        {
                            var groupDummy = pickedGroupList[idx];
                            groupDummy.HideCheceked = !groupDummy.HideCheceked;
                        }
                    }
                    else
                    {
                        group.HideCheceked = !group.HideCheceked;
                    }
                }

                GUI.color = isProSkin? Color.yellow : Color.white;
				GUI.color = group.isActive? GUI.color : Color.grey;
				var foldRect = new Rect(groupRect.x + 2, groupRect.y + 1, 20, groupRect.height);
				var isVirtual = group.referenceMode == CutsceneGroup.ActorReferenceMode.UseInstanceHideOriginal;
				group.isCollapsed = !EditorGUI.Foldout(foldRect, !group.isCollapsed, string.Format("<b>{0} {1}</b>", group.name, isVirtual? "(Ref)" : "" ));
				GUI.color = Color.white;
				//Actor Object Field
				if (group.actor == null){
					var oRect = Rect.MinMaxRect(groupRect.xMin + 20, groupRect.yMin + 1, groupRect.xMax - 20, groupRect.yMax - 1);
					group.actor = (GameObject)UnityEditor.EditorGUI.ObjectField(oRect, group.actor, typeof(GameObject), true);
				}
				//////

				///CONTEXT
				if ( (e.type == EventType.ContextClick && groupRect.Contains(e.mousePosition)) || plusClicked ){
                    // @modify slate sequencer
                    // @TQ
                    // avoid context click in Play modle
                    if (cutscene.currentTime > 0)
                    {
                        return;
                    }
                    //end
					var menu = new GenericMenu();
					foreach (var _info in EditorTools.GetTypeMetaDerivedFrom(typeof(CutsceneTrack))){
						var info = _info;
						if (info.attachableTypes == null || !info.attachableTypes.Contains(group.GetType())){
							continue;
						}

						var canAdd = !info.isUnique || (group.tracks.Find(track => track.GetType() == info.type) == null);
						var finalPath = string.IsNullOrEmpty(info.category)? info.name : info.category + "/" + info.name;
						/// @modify slate sequencer
               			/// @TQ
                        if (CheckGroupsSelected())
                        {
                            menu.AddItem(new GUIContent("Add Track/" + finalPath), false, (infoData) => {
                                EditorTools.TypeMetaInfo infoDummy = (EditorTools.TypeMetaInfo)infoData;
                                for (int idx = 0; idx < pickedGroupList.Count; idx++)
                                {
                                    var groupDummy = pickedGroupList[idx];
                                    if ("Properties Track" == infoDummy.name)
                                    {
                                        if (!groupDummy.HasPropertyTrack())
                                            groupDummy.AddTrack(info.type);
                                    }
                                    else
                                    {
                                        groupDummy.AddTrack(info.type);
                                    }                                                            
                                }
                            }, info);
                        }
                        else
                        {
                            if (canAdd)
                            {
                                menu.AddItem(new GUIContent("Add Track/" + finalPath), false, () => 
                                {
                                    group.AddTrack(info.type);
                                });
                            }
                            else
                            {
                                menu.AddDisabledItem(new GUIContent("Add Track/" + finalPath));
                            }
                        }
                        ///end										
					}
                    /// @modify slate sequencer
                    /// @TQ
                    /// 
                    menu.AddItem(new GUIContent("Auto H or S Actor"), !group.isActive, () =>
                    {
                        var oldcut = current.cutscene;
                        var actorDisableTrack = group.AddTrack<ActorActionTrack>("AutoDisable");
                        var activeClip = actorDisableTrack.AddAction<SetActorActiveState>(0);
                        activeClip.activeState = ActiveState.Disable;
                        actorDisableTrack.name = "AutoDisable";
                        /// 控制subcutsence中Actor的隐藏和显示
                        var allSubcuts = current.cutscene.directorGroup.tracks.FindAll( dummy => dummy.name.Contains("SubCut_Track"));
                        for (int i =0; i< allSubcuts.Count; i++)
                        {
                            var subcut = allSubcuts[i];
                            var subClips = subcut.actions.FindAll( t => t is SubCutscene);
                            foreach (var cp in subClips)
                            {  
                                var theCutsenceClip = cp as SubCutscene;
                                var theCutsence = theCutsenceClip.cutscene;
                                var characterGroup = theCutsence.groups.Find(
                                    t=>ReferenceEquals(t.actor, group.actor )
                                    );
                                if (characterGroup != null)
                                {
                                    var subActorDisableTrack = characterGroup.AddTrack<ActorActionTrack>("AutoHorS");
                                    var subActiveClipEnable = subActorDisableTrack.AddAction<SetActorActiveState>(0);
                                    ///var subActiveClipDisable = subActorDisableTrack.AddAction<SetActorActiveState>(theCutsence.length);
                                    subActiveClipEnable.activeState = ActiveState.Enable;
                                    subActiveClipEnable.length = theCutsence.length;
                                   // subActiveClipDisable.activeState = ActiveState.Disable;
                                    subActorDisableTrack.name = "AutoHorS";
                                    break;
                                }
                            }
                        }
                        Selection.activeGameObject = oldcut.gameObject;

                        ///
                    });

					menu.AddItem(new GUIContent("Disable Group"), !group.isActive, ()=>{
                        if (CheckGroupsSelected())
                        {
                            for (int idx = 0; idx < pickedGroupList.Count; idx++)
                            {
                                var groupDummy = pickedGroupList[idx];
                                groupDummy.isActive = !groupDummy.isActive;
                            }
                        }
                        else
                        {
                            group.isActive = !group.isActive;
                        }                      
                    });
					menu.AddItem(new GUIContent("Lock Group"), group.isLocked, ()=>{
                        if (CheckGroupsSelected())
                        {
                            for (int idx = 0; idx < pickedGroupList.Count; idx++)
                            {
                                var groupDummy = pickedGroupList[idx];
                                groupDummy.isLocked = !groupDummy.isLocked;
                            }
                        }
                        else
                        {
                            group.isLocked = !group.isLocked;
                        }         
                    });
                     ///end
					if ( !(group is DirectorGroup) ){
						menu.AddItem(new GUIContent("Select Actor (Double Click)"), false, ()=>{ Selection.activeObject = group.actor; });
						menu.AddItem(new GUIContent("Replace Actor"), false, ()=>{ group.actor = null; });
						menu.AddItem(new GUIContent("Duplicate"), false, ()=>
							{
								cutscene.DuplicateGroup(group);
								InitClipWrappers();
							});
						menu.AddSeparator("/");
					/// @modify slate sequencer
        			/// @TQ
						menu.AddItem(new GUIContent("Delete Group"), false, ()=>
							{
                                if (CheckGroupsSelected())
                                {
                                    if (EditorUtility.DisplayDialog("Delete Groups", "Are you sure?", "YES", "NO!"))
                                    {
                                        for (int idx = 0; idx < pickedGroupList.Count; idx++)
                                        {
                                            var groupDummy = pickedGroupList[idx];
                                            cutscene.DeleteGroup(groupDummy);
                                            InitClipWrappers();
                                        }
                                        pickedGroupList.Clear();
                                    }
                                }
                                else
                                {
                                    if (EditorUtility.DisplayDialog("Delete Group", "Are you sure?", "YES", "NO!"))
                                    {
                                        cutscene.DeleteGroup(group);
                                        InitClipWrappers();
                                    }

                                }
                                
							});
						///end
					}
					menu.ShowAsContext();
					e.Use();
				}


				///REORDERING
				if (e.type == EventType.MouseDown && e.button == 0 && groupRect.Contains(e.mousePosition)){
					CutsceneUtility.selectedObject = !ReferenceEquals(CutsceneUtility.selectedObject, group)? group : null;

                    if (ReferenceEquals(CutsceneUtility.selectedObject, group) && e.modifiers != EventModifiers.Control && e.modifiers != EventModifiers.Shift)
                    {
                        pickedGroupList.Remove(group);
                    }
                    /// @modify slate sequencer
                    /// @TQ
                    /// 
                    pickedTrackList.Clear();
					if ( !(group is DirectorGroup) )
                    {
                        if (e.modifiers == EventModifiers.Control)
                        {
                            
                            if (pickedGroupList.Contains(group))
                            {
                                pickedGroupList.Remove(group);
                            }
                            else
                            {
                                pickedGroupList.Add(group);
                            }
                            if (pickedGroupList.Count == 1)
                            {
                                CutsceneUtility.selectedObject = pickedGroupList[0];
                            }
                           
                        }
                        else if (e.modifiers == EventModifiers.Shift)
                        {
                            if (pickedGroupList.Count > 0)
                            {
                                var pickedFirstDummy = pickedGroupList[0];
                                var pickedLastDummy = pickedGroupList[pickedGroupList.Count - 1];
                                int pickedFirstIdxInGroups = cutscene.groups.IndexOf(pickedFirstDummy);
                                int pickedLastIdxInGroups = cutscene.groups.IndexOf(pickedLastDummy);
                                int currentSelectIdxInGroups = cutscene.groups.IndexOf(group);
    
                                int minIdx = Mathf.Min(pickedFirstIdxInGroups, pickedLastIdxInGroups);
                                int maxIdx = Mathf.Max(pickedFirstIdxInGroups, pickedLastIdxInGroups);

                                if (currentSelectIdxInGroups >= minIdx )
                                {
                                    maxIdx = currentSelectIdxInGroups; 
                                }
                                if (currentSelectIdxInGroups < minIdx)
                                {
                                    minIdx = currentSelectIdxInGroups;
                                }
                                                         
                                pickedGroupList.Clear();
                                for (int idx = 0; idx < cutscene.groups.Count; idx++)
                                {
                                    if ( idx >= minIdx && idx<= maxIdx)
                                    {
                                        var groupDummy = cutscene.groups[idx];
                                        if ((!pickedGroupList.Contains(groupDummy) && !groupDummy.HideCheceked) || (groupDummy.HideCheceked && Prefs.showChoosenTrack))
                                            pickedGroupList.Add(groupDummy);
                                    }

                                }

                                //bool bStart = false;
                                //for (int idx = 0; idx < cutscene.groups.Count; idx++)
                                //{
                                //    var groupDummy = cutscene.groups[idx];
                                //    if (pickedGroupList.Contains(groupDummy))
                                //    {
                                //        bStart = true;
                                //    }
                                //    if (bStart)
                                //    {
                                //        if ((!pickedGroupList.Contains(groupDummy) && !groupDummy.HideCheceked) || (groupDummy.HideCheceked && Prefs.showChoosenTrack ))
                                //            pickedGroupList.Add(groupDummy);
                                //    }                            
                                //    if (group == groupDummy)
                                //    {
                                //        bStart = false;
                                //        int idxDummy = pickedGroupList.IndexOf(group)+1;

                                //        if (pickedGroupList.Count > idxDummy)
                                //        {
                                //            pickedGroupList.RemoveRange(idxDummy, pickedGroupList.Count - idxDummy);
                                //        }
                                //        if (idxDummy == 1)
                                //        {
                                //            pickedGroupList.Clear();
                                //        }

                                //    }
                                //}



                            }
                            
                        }
                        else
                        {
                            pickedGroupList.Clear();
                            if (pickedGroupList.Count == 0)
                                pickedGroupList.Add(group);
                            pickedGroup = group;
                        }
						/// end
                    }
					if (e.clickCount == 2){
						Selection.activeGameObject = group.actor;
					}
					e.Use();
				}
	

				if (pickedGroup != null && pickedGroup != group && !(group is DirectorGroup) ){
					if (groupRect.Contains(e.mousePosition)){
						var markRect = new Rect(groupRect.x, (cutscene.groups.IndexOf(pickedGroup) < g)? groupRect.yMax - 2 : groupRect.y, groupRect.width, 2);
						GUI.color = Color.grey;
						GUI.DrawTexture(markRect, Styles.whiteTexture);
						GUI.color = Color.white;
					}

					if (e.rawType == EventType.MouseUp && e.button == 0 && groupRect.Contains(e.mousePosition)){
						cutscene.groups.Remove(pickedGroup);
						cutscene.groups.Insert(g, pickedGroup);
						pickedGroup = null;
						e.Use();
					}
				}

				///SHOW TRACKS (?)
				if (!group.isCollapsed){
					ShowListTracks(e, group, ref nextYPos);
	
					//draw vertical graphic on left side of nested track rects
					GUI.color = groupSelected? listSelectionColor : groupColor;
					var verticalRect = Rect.MinMaxRect(groupRect.x, groupRect.yMax, groupRect.x+3, nextYPos - 2);
					GUI.DrawTexture(verticalRect, Styles.whiteTexture);
					GUI.color = Color.white;
				}
			}
		}

		void ShowListTracks(Event e, CutsceneGroup group, ref float nextYPos){

			//TRACKS
			for (int t = 0; t < group.tracks.Count; t++){
				var track     = group.tracks[t];
                track.TrackHeightOffset = viewCurveBoxHeightMax;
				/// @modify slate sequencer
                /// @TQ
                if (!Prefs.showChoosenTrack && (track.isHideChecked))
                {
                    continue;
                }
                if (Prefs.HideAllTrack)
                {
                    continue;
                }
                /// end
                var yPos      = nextYPos;

				var trackRect = new Rect(10, yPos, leftRect.width - TRACK_RIGHT_MARGIN - 10, track.finalHeight);
				nextYPos += track.finalHeight + TRACK_MARGINS;

				//GRAPHICS
				GUI.color = new Color(1,1,1,0.2f);
				GUI.Box(trackRect, "", (GUIStyle)"flow node 0");
				GUI.color = track.isActive || !isProSkin? Color.white : Color.grey;
				GUI.Box(trackRect, "");
                var trackSelected = (!CheckTracksSelected()&&(ReferenceEquals(track, CutsceneUtility.selectedObject) || track == pickedTrack));
                var trackMutiSelected = (pickedTrackList.Contains(track)&&CheckTracksSelected());
                trackSelected &= !trackMutiSelected;
                if (trackSelected || trackMutiSelected)
                {
                    GUI.color = trackSelected ? listSelectionColor : trackMutiSelected ? mutiSelectColor : new Color(1, 1, 1, 0.2f);
                    GUI.DrawTexture(trackRect, whiteTexture);
				}
  
                //custom color indicator
                if (track.isActive && track.color != Color.white && track.color.a > 0.2f){
					GUI.color = track.color;
					var colorRect = new Rect(trackRect.xMax + 1, trackRect.yMin, 2, track.finalHeight);
					GUI.DrawTexture(colorRect, whiteTexture);
				}
				GUI.color = Color.white;
				//

				/////
				GUI.BeginGroup(trackRect);
				track.OnTrackInfoGUI(trackRect);
				GUI.EndGroup();
                /////

                AddCursorRect(trackRect, pickedTrack == null? MouseCursor.Link : MouseCursor.MoveArrow);
			
				//CONTEXT
				if (e.type == EventType.ContextClick && trackRect.Contains(e.mousePosition)){
					var menu = new GenericMenu();
                    /// @modify slate sequencer
                    /// @TQ
                    /// 
                    if (CheckTracksSelected())
                    {
                        menu.AddItem(new GUIContent("Unselect Tracks"), !track.isActive, () =>
                        {
                            pickedTrackList.Clear();
                        });

                        menu.AddItem(new GUIContent("Merge Clips"), !track.isActive, () =>
                        {
                            if (CheckTrackCanMerge())
                            {
                                string fileName = System.DateTime.Now.ToString("MMddHHmmssfff");
                                var trackDm = pickedTrackList[0];
                                var typeDm = trackDm.GetType();
                                var groupDm = trackDm.parent as CutsceneGroup;
                                var newAddTrack = groupDm.AddTrack(typeDm, typeDm.Name+"Track Merge" + fileName);
                                for (int i = 0; i < pickedTrackList.Count; i++)
                                {
                                    for (int a = 0; a < pickedTrackList[i].actions.Count; a++)
                                    {
                                        var act = pickedTrackList[i].actions[a];
                                        CutsceneUtility.CopyClip(act);
                                        var copy = CutsceneUtility.PasteClip(newAddTrack, act.startTime);
                                    }
                                }
                                for (int idx = 0; idx < pickedTrackList.Count; idx++)
                                {
                                    var trackDummy = pickedTrackList[idx];
                                    group.DeleteTrack(trackDummy);
                                    InitClipWrappers();
                                }
                                pickedTrackList.Clear();
                            }
                            
                        });
                    }

                    if (track is CameraTrack)
                    {
                        var ct = track as CameraTrack;
                        menu.AddItem(new GUIContent("Active Track"), false, () =>
                        {
                            var cTracks = group.tracks.FindAll( dummy => dummy is CameraTrack);
                            foreach (var cts in cTracks)
                            {
                                if (!ReferenceEquals(cts, track))
                                {
                                    cts.isActive = false;
                                }
                                else
                                {
                                    cts.isActive = true;
                                }
                            }
                        });
                        if (track.isActive)
                        {
                            menu.AddItem(new GUIContent("Save Active Track"), false, () =>
                            {
                                if (EditorUtility.DisplayDialog("保存Smooth数据!", "是否保存平滑后数据，并删除镜头原始录制数据?", "是", "否!"))
                                {
                                    var cTracks = group.tracks.FindAll(dummy => dummy is CameraTrack);
                                    foreach (var cts in cTracks)
                                    {
                                        if (!ReferenceEquals(cts, track))
                                        {
                                            group.DeleteTrack(cts);
                                        }
                                    }
                                }
                            });
                            
                        }

                    }

                    menu.AddItem(new GUIContent("Disable Track"), !track.isActive, ()=> {
                        if (CheckTracksSelected())
                        {
                            for (int idx = 0; idx < pickedTrackList.Count; idx++)
                            {
                                var trackDummy = pickedTrackList[idx];
                                trackDummy.isActive = !trackDummy.isActive;
                            }
                        }
                        else
                        {
                            track.isActive = !track.isActive;
                        }
                    });
					menu.AddItem(new GUIContent("Lock Track"), track.isLocked, ()=> {
                        if (CheckTracksSelected())
                        {
                            for (int idx = 0; idx < pickedTrackList.Count; idx++)
                            {
                                var trackDummy = pickedTrackList[idx];
                                trackDummy.isLocked = !trackDummy.isLocked;
                            }
                        }
                        else
                        {
                            track.isLocked = !track.isLocked;
                        }                  
                    });
                    if (track is PropertiesTrack)
                    {
                        var propertyTrack = track as PropertiesTrack;
                        menu.AddItem(new GUIContent("Unfold Track"), track.showCurves, () =>
                        {
                            if (CheckTracksSelected())
                            {
                                for (int idx = 0; idx < pickedTrackList.Count; idx++)
                                {
                                    var trackDummy = pickedTrackList[idx];
                                    trackDummy.showCurves = !trackDummy.showCurves;
                                }
                            }
                            else
                            {
                                track.showCurves = !track.showCurves;
                            }
                        });
                    }
                    ///end
                    if (track.GetType().RTGetAttribute<UniqueElementAttribute>(true) == null){
						menu.AddItem(new GUIContent("Duplicate"), false, ()=>
							{
								/// @modify slate sequencer
        						/// @TQ
                                if (CheckTracksSelected())
                                {
                                    for (int idx = 0; idx < pickedTrackList.Count; idx++)
                                    {
                                        var trackDummy = pickedTrackList[idx];
                                        if (trackDummy.GetType().RTGetAttribute<UniqueElementAttribute>(true) == null)
                                        {
                                            group.DuplicateTrack(trackDummy);
                                            InitClipWrappers();
                                        }
                                    }
                                }
                                else
                                {
                                    group.DuplicateTrack(track);
                                    InitClipWrappers();
                                }
								///end                             
							});
					} else {
						menu.AddDisabledItem(new GUIContent("Duplicate") );
					}
					menu.AddSeparator("/");
					menu.AddItem(new GUIContent("Delete Track"), false, ()=>
						{
							/// @modify slate sequencer
							/// @TQ
                            if (CheckTracksSelected())
                            {
                                if (EditorUtility.DisplayDialog("Delete Tracks", "Are you sure?", "YES", "NO!"))
                                {
                                    for (int idx = 0; idx < pickedTrackList.Count; idx++)
                                    {
                                        var trackDummy = pickedTrackList[idx];
                                        group.DeleteTrack(trackDummy);
                                        InitClipWrappers();
                                    }
                                    pickedTrackList.Clear();
                                }
                            }
                            else
                            {
                                if (EditorUtility.DisplayDialog("Delete Track", "Are you sure?", "YES", "NO!"))
                                {
                                    group.DeleteTrack(track);
                                    InitClipWrappers();
                                }
                            }
                         	///end
						});
					menu.ShowAsContext();
					e.Use();
				}

				//REORDERING
				if (e.type == EventType.MouseDown && e.button == 0 && trackRect.Contains(e.mousePosition)){
					CutsceneUtility.selectedObject = !ReferenceEquals(CutsceneUtility.selectedObject, track)? track : null;
                    /// @modify slate sequencer
                    /// @TQ
                    /// 
                    if (ReferenceEquals(CutsceneUtility.selectedObject, track) && e.modifiers != EventModifiers.Control && e.modifiers != EventModifiers.Shift)
                    {
                        pickedTrackList.Remove(track);
                    }
                    pickedGroupList.Clear();
                    if (e.modifiers == EventModifiers.Control)
                    {
                        if (pickedTrackList.Contains(track))
                        {
                            pickedTrackList.Remove(track);
                        }
                        else
                        {
                            pickedTrackList.Add(track);
                        }
                        if (pickedTrackList.Count == 1)
                        {
                            CutsceneUtility.selectedObject = pickedTrackList[0];
                        }

                    }
                    else if (e.modifiers == EventModifiers.Shift)
                    {
                        if (pickedTrackList.Count > 0)
                        {
                            var pickedFirstDummy = pickedTrackList[0];
                            var pickedLastDummy = pickedTrackList[pickedTrackList.Count - 1];
                            int pickedFirstIdxInGroups = group.tracks.IndexOf(pickedFirstDummy);
                            int pickedLastIdxInGroups = group.tracks.IndexOf(pickedLastDummy);
                            int currentSelectIdxInGroups = group.tracks.IndexOf(track);

                            int minIdx = Mathf.Min(pickedFirstIdxInGroups, pickedLastIdxInGroups);
                            int maxIdx = Mathf.Max(pickedFirstIdxInGroups, pickedLastIdxInGroups);

                            if (currentSelectIdxInGroups >= minIdx)
                            {
                                maxIdx = currentSelectIdxInGroups;
                            }
                            if (currentSelectIdxInGroups < minIdx)
                            {
                                minIdx = currentSelectIdxInGroups;
                            }

                            pickedTrackList.Clear();
                            for (int idx = 0; idx < group.tracks.Count; idx++)
                            {
                                if (idx >= minIdx && idx <= maxIdx)
                                {
                                    var trackDummy = group.tracks[idx];
                                    if ((!pickedTrackList.Contains(trackDummy) && !trackDummy.isHideChecked) || (trackDummy.isHideChecked && Prefs.showChoosenTrack))
                                        pickedTrackList.Add(trackDummy);
                                }

                            }
                        }

                    }
                    else
                    {
                        pickedTrackList.Clear();
                        if (pickedTrackList.Count == 0)
                            pickedTrackList.Add(track);
                        pickedTrack = track;
                    }
					///end
                    e.Use();
				}

				if (pickedTrack != null && pickedTrack != track && ReferenceEquals(pickedTrack.parent, group) ){
					if (trackRect.Contains(e.mousePosition)){
						var markRect = new Rect(trackRect.x, (group.tracks.IndexOf(pickedTrack) < t)? trackRect.yMax - 2 : trackRect.y, trackRect.width, 2);
						GUI.color = Color.grey;
						GUI.DrawTexture(markRect, Styles.whiteTexture);
						GUI.color = Color.white;
					}

					if (e.rawType == EventType.MouseUp && e.button == 0 && trackRect.Contains(e.mousePosition)){
						group.tracks.Remove(pickedTrack);
						group.tracks.Insert(t, pickedTrack);
						pickedTrack = null;
						e.Use();
					}
				}
			}
		}










		//middle - the actual timeline tracks
		void ShowTimeLines(Rect centerRect){
            if (cutscene == null)
                return;
			//temporary delegate used to call GUI after EndWindows (thus show on top)
			System.Action postWindowsGUI = null;

			var e = Event.current;

			//bg graphic
			var bgRect = Rect.MinMaxRect(centerRect.xMin, TOP_MARGIN + TOOLBAR_HEIGHT + scrollPos.y, centerRect.xMax, screenHeight - TOOLBAR_HEIGHT + scrollPos.y);
			GUI.color = new Color(0,0,0,0.2f);
			GUI.DrawTextureWithTexCoords(bgRect, Styles.stripes, new Rect(0,0, bgRect.width/-7, bgRect.height/-7));
			GUI.color = Color.white;
			GUI.Box(bgRect, "", (GUIStyle)"TextField");


			//Begin Group
			GUI.BeginGroup(centerRect);

			//starting height
			var nextYPos = FIRST_GROUP_TOP_MARGIN;

			//master sections
			var sectionsRect = Rect.MinMaxRect(  Mathf.Max(TimeToPos(viewTimeMin), TimeToPos(0)), 3, TimeToPos(viewTimeMax), 18  );
			if (cutscene.directorGroup != null){ //it never should
				ShowGroupSections(cutscene.directorGroup, sectionsRect);
			}

			//Begin Windows
			BeginWindows();

			//GROUPS
			for (int g = 0; g < cutscene.groups.Count; g++){
				var group = cutscene.groups[g];
				/// @modify slate sequencer
                /// @TQ
                if (!Prefs.showChoosenTrack && group.HideCheceked)
                {
                    continue;
                }
                if (Prefs.HideAllTrack)
                {
                    continue;
                }
				/// end
                if ( FilteredOutBySearch(group, searchString) ){
					group.isCollapsed = true;
					continue;
				}

				var groupRect = Rect.MinMaxRect( Mathf.Max(TimeToPos(viewTimeMin), TimeToPos(0)), nextYPos, TimeToPos(viewTimeMax), nextYPos + GROUP_HEIGHT );
				nextYPos += GROUP_HEIGHT;

				//if collapsed, just show a heat minimap of clips.
				if (group.isCollapsed){

					GUI.color = new Color(0,0,0,0.15f);
					var collapseRect = Rect.MinMaxRect(groupRect.xMin + 2, groupRect.yMin + 2, groupRect.xMax, groupRect.yMax -4);
					GUI.DrawTexture(collapseRect, Styles.whiteTexture);
					GUI.color = Color.white;

					GUI.color = new Color(0.5f,0.5f,0.5f,0.5f);
					foreach(var track in group.tracks){
						foreach(var clip in track.actions){
							var start = TimeToPos(clip.startTime);
							var end = TimeToPos(clip.endTime);
							GUI.DrawTexture(Rect.MinMaxRect(start + 0.5f, collapseRect.y + 2, end - 0.5f, collapseRect.yMax - 2), Styles.whiteTexture);
						}
					}
					GUI.color = Color.white;
					continue;
				}


				//TRACKS
				for (int t = 0; t < group.tracks.Count; t++){
					var track         = group.tracks[t];
					/// @modify slate sequencer
                    /// @TQ
                    if (!Prefs.showChoosenTrack && track.isHideChecked)
                    {
                        continue;
                    }

                    /// end

                    var yPos          = nextYPos;
					var trackPosRect  = Rect.MinMaxRect( Mathf.Max(TimeToPos(viewTimeMin), TimeToPos(track.startTime)), yPos, TimeToPos(viewTimeMax), yPos + track.finalHeight);
					var trackTimeRect = Rect.MinMaxRect( Mathf.Max(viewTimeMin, track.startTime), 0, viewTimeMax, 0);
					nextYPos += track.finalHeight + TRACK_MARGINS;

					//GRAPHICS
					GUI.backgroundColor = isProSkin? Color.black : new Color(0,0,0,0.1f);
					GUI.Box(trackPosRect, "");
					Handles.color = new Color(0.2f, 0.2f, 0.2f);
					Handles.DrawLine(new Vector2(trackPosRect.x, trackPosRect.y+1), new Vector2(trackPosRect.xMax, trackPosRect.y+1));
					Handles.DrawLine(new Vector2(trackPosRect.x, trackPosRect.yMax), new Vector2(trackPosRect.xMax, trackPosRect.yMax));
					if (track.showCurves){
						Handles.DrawLine(new Vector2(trackPosRect.x, trackPosRect.y + track.defaultHeight), new Vector2(trackPosRect.xMax, trackPosRect.y + track.defaultHeight));
					}
					Handles.color = Color.white;
					if (viewTimeMin < 0){ //just visual clarity
						GUI.Box(Rect.MinMaxRect(TimeToPos(viewTimeMin), trackPosRect.yMin, TimeToPos(0), trackPosRect.yMax), "");
					}
					if ((track!= null) && ((track.parent != null)&&(track.startTime > track.parent.startTime) || (track.parent!=null)&&(track.endTime < track.parent.endTime))){
						Handles.color = Color.white;
						GUI.color = new Color(0,0,0,0.2f);
						if (track.startTime > track.parent.startTime){
							var tStart = TimeToPos(track.startTime);
							var r = Rect.MinMaxRect(TimeToPos(0), yPos, tStart, yPos + track.finalHeight);
							GUI.DrawTexture(r, whiteTexture);
							GUI.DrawTextureWithTexCoords(r, Styles.stripes, new Rect(0,0, r.width/7, r.height/7));
							var a = new Vector2(tStart, trackPosRect.yMin);
							var b = new Vector2(a.x, trackPosRect.yMax);
							Handles.DrawLine(a, b);
						}
						if (track.endTime < track.parent.endTime){
							var tEnd = TimeToPos(track.endTime);
							var r = Rect.MinMaxRect(tEnd, yPos, TimeToPos(length), yPos + track.finalHeight);
							GUI.DrawTexture(r, whiteTexture);
							GUI.DrawTextureWithTexCoords(r, Styles.stripes, new Rect(0,0, r.width/7, r.height/7));
							var a = new Vector2(tEnd, trackPosRect.yMin);
							var b = new Vector2(a.x, trackPosRect.yMax);
							Handles.DrawLine(a, b);	
						}
						GUI.color = Color.white;
						Handles.color = Color.white;
					}
					GUI.backgroundColor = Color.white;

					if ( ReferenceEquals(CutsceneUtility.selectedObject, track) ){
						GUI.color = Color.grey;
						GUI.Box(trackPosRect, "", Styles.hollowFrameHorizontalStyle);
						GUI.color = Color.white;
					}
					//////

					if (track.isLocked){
						if (e.isMouse && trackPosRect.Contains(e.mousePosition)){
							e.Use();
						}
					}
					
					//...
					var cursorTime = SnapTime( PosToTime(mousePosition.x) );
                    // @modify slate sequencer
                    // @rongxia
                    if (track.DragFileAddClipFunc == null)
                    {
                        track.DragFileAddClipFunc = DragFileToAddClip;
						/// @modify slate sequencer
                        /// @TQ
                        track.AddPropertyToPickGroupsFunc = AddPropertyToPickGroups;
                        /// end
                    }
                    // @end


                    track.OnTrackTimelineGUI(trackPosRect, trackTimeRect, cursorTime, TimeToPos, DoMutiSelectionRect());
					//...

					//ACTION CLIPS
					for (int a= 0; a < track.actions.Count; a++){
						var action = track.actions[a];
						var ID = UID(g,t,a);
						ActionClipWrapper clipWrapper = null;

						if (!clipWrappers.TryGetValue(ID, out clipWrapper)){
							InitClipWrappers();
							clipWrapper = clipWrappers[ID];
						}

						if (clipWrapper.action != action){
							InitClipWrappers();
							clipWrapper = clipWrappers[ID];
						}

						//find and store next/previous clips to wrapper
						var nextClip = a < track.actions.Count -1? track.actions[a + 1] : null;
						var previousClip = a != 0? track.actions[a - 1] : null;
						clipWrapper.nextClip = nextClip;
						clipWrapper.previousClip = previousClip;
						

						//get the action box rect
						var clipRect = clipWrapper.rect;

						//modify it
						clipRect.y = yPos;
						clipRect.width = Mathf.Max(action.length / viewTime * centerRect.width, 6);
						clipRect.height = track.defaultHeight;


					
						//get the action time and pos
						var xTime = action.startTime;
						var xPos = clipRect.x;

						if (anyClipDragging && ReferenceEquals(CutsceneUtility.selectedObject, action) ){

							var lastTime = xTime; //for multiSelection drag
							xTime = PosToTime(xPos + leftRect.width);
							xTime = SnapTime(xTime);
							xTime = Mathf.Clamp(xTime, 0, maxTime - 0.1f);

							//handle multisection. Limit xmin, xmax by their bound rect
							if (multiSelection != null && multiSelection.Count > 1){
								var delta = xTime - lastTime;
								var boundMin = Mathf.Min( multiSelection.Select(b => b.action.startTime).ToArray() );
								// var boundMax = Mathf.Max( multiSelection.Select(b => b.action.endTime).ToArray() );
								if (boundMin + delta < 0){
									xTime -= delta;
									delta = 0;
								}

								foreach(var cw in multiSelection)
                                {
									if (cw.action != action){
										cw.action.startTime += delta;                               
                                    }
/// @modify slate sequencer
/// add by TQ
                                    MoveCameraClipWithSubCutsence(delta, cw.action,true, cw);
/// end
                                }
                                /// @modify slate sequencer
								/// add by TQ
                                DopeSheetEditor.MoveTheDopeSheetLineOutside(delta);
								/// end
							}

                            //clamp and cross blend between other nearby clips
							/// @modify slate sequencer
							/// add by TQ
							if ( multiSelection == null || multiSelection.Count <= 1 ){
							/// end
								var preCursorClip = track.actions.LastOrDefault(act => act != action && act.startTime < cursorTime );
								var postCursorClip = track.actions.FirstOrDefault(act => act != action && act.endTime > cursorTime );

								if (e.shift){ //when shifting track clips always clamp to previous clip and no need to clamp to next
									preCursorClip = previousClip;
									postCursorClip = null;
								}

								var preTime = preCursorClip != null? preCursorClip.endTime : 0 ;
								var postTime = postCursorClip != null? postCursorClip.startTime : maxTime + action.length;
								if (Prefs.magnetSnapping && !e.control){ //magnet snap
									if (Mathf.Abs( (xTime + action.length) - postTime) <= magnetSnapInterval){
										xTime = postTime - action.length;
									}
									if (Mathf.Abs(xTime - preTime) <= magnetSnapInterval){
										xTime = preTime;
									}

                                    if(Mathf.Abs((xTime + action.length) - cutscene.currentTime) <= magnetSnapInterval)
                                    {
                                        xTime = cutscene.currentTime - action.length;
                                    }
                                    if (Mathf.Abs(xTime - cutscene.currentTime) <= magnetSnapInterval)
                                    {
                                        xTime = cutscene.currentTime;
                                    }

                                    if (Mathf.Abs((xTime + action.length) - cutscene.startPlayTime) <= magnetSnapInterval)
                                    {
                                        xTime = cutscene.startPlayTime - action.length;
                                    }
                                    if (Mathf.Abs(xTime - cutscene.startPlayTime) <= magnetSnapInterval)
                                    {
                                        xTime = cutscene.startPlayTime;
                                    }

                                    if (Mathf.Abs((xTime + action.length) - cutscene.length) <= magnetSnapInterval)
                                    {
                                        xTime = cutscene.length - action.length;
                                    }
                                    if (Mathf.Abs(xTime - cutscene.length) <= magnetSnapInterval)
                                    {
                                        xTime = cutscene.length;
                                    }
                                }

                                // @modify slate sequencer
                                // @TQ
                                if (action is ICrossBlendable || (Prefs.autoCutCam && action is CameraShot))
                                {
                                    if ((preCursorClip is ICrossBlendable || (Prefs.autoCutCam && action is CameraShot)) && preCursorClip != null && preCursorClip.GetType() == action.GetType())
                                    {
                                        preTime -= Mathf.Min(action.length / 2, preCursorClip.length / 2);
                                    }

                                    if ((postCursorClip is ICrossBlendable || (Prefs.autoCutCam && action is CameraShot)) && postCursorClip != null && postCursorClip.GetType() == action.GetType())
                                    {
                                        postTime += Mathf.Min(action.length / 2, postCursorClip.length / 2);
                                    }
                                }
                                //end

                                //does it fit?
                                if (action.length > postTime - preTime){
									xTime = lastTime;
								}

								if (xTime != lastTime){
									xTime = Mathf.Clamp(xTime, preTime, postTime - action.length);
									//Shift all the next clips along with this one if shift is down
									if (e.shift){
										foreach(var cw in clipWrappers.Values.Where(c => c.action.parent == action.parent && c.action != action && c.action.startTime > lastTime)){
											cw.action.startTime += xTime - lastTime;
										}
									}
									/// @modify slate sequencer
									/// add by TQ
                                    DopeSheetEditor.MoveTheDopeSheetLineOutside(xTime - lastTime);
                                    MoveCameraClipWithSubCutsence(xTime - lastTime, action, true, clipWrapper);
                                    ///end
                                }
							}


							//Apply xTime
							action.startTime = xTime;
                        }

						//apply xPos
						clipRect.x = TimeToPos(xTime);

                        // @modify slate sequencer
                        // @TQ
                        //set crossblendable blend properties
                        if (!anyClipDragging){
                            if (!Prefs.autoCut)
                            {
                                var overlap = previousClip != null ? Mathf.Max(previousClip.endTime - action.startTime, 0) : 0;
                                if (overlap > 0)
                                {
                                    action.blendIn = overlap;
                                    previousClip.blendOut = overlap;
                                }
                            }
                            else
                            {
                                var overlap = previousClip != null ? Mathf.Max(previousClip.endTime - action.startTime, 0) : 0;
                                if (overlap > 0)
                                {
                                    previousClip.endTime -= overlap;
                                }
                            }
                            if (Prefs.autoCutCam && action is CameraShot)
                            {
                                var overlap = previousClip != null ? Mathf.Max(previousClip.endTime - action.startTime, 0) : 0;
                                if (overlap > 0)
                                {
                                    previousClip.endTime -= overlap;
                                }
                            }

                        }
                        //end 

						//dont draw if outside of view range and not selected
						var isSelected = ReferenceEquals(CutsceneUtility.selectedObject, action) || (multiSelection != null && multiSelection.Select(b => b.action).Contains(action) );
						var isVisible = Rect.MinMaxRect(0, scrollPos.y, centerRect.width, centerRect.height).Overlaps(clipRect);
						if ( !isSelected && !isVisible ){
							clipWrapper.rect = default(Rect); //we basicaly nullify the rect
							continue;
						}

						//draw selected rect
						if (isSelected){
							var selRect = Rect.MinMaxRect(clipRect.xMin-2, clipRect.yMin-2, clipRect.xMax+2, clipRect.yMax+2);
							GUI.color = highlighColor;
							GUI.DrawTexture(selRect, Slate.Styles.whiteTexture);
							GUI.color = Color.white;
						}

						//determine color and draw clip
						var color = track.color;
						color = action.isValid? color : new Color(1, 0.3f, 0.3f);
						color = track.isActive? color : Color.grey;
/// @modify slate sequencer
/// add by TQ
                        color = CheckIsSubSysCamClip(action)? Color.gray : color;
/// end
                        GUI.color = color;
						GUI.Box(clipRect, "", Styles.clipBoxStyle);
						GUI.color = Color.white;

						clipWrapper.rect = GUI.Window(ID, clipRect, ActionClipWindow, string.Empty, GUIStyle.none);
						if (!isProSkin){ GUI.color = new Color(1,1,1,0.5f);	GUI.Box(clipRect, ""); GUI.color = Color.white;	}

						//forward external Clip GUI
						var nextPosX = TimeToPos( nextClip != null? nextClip.startTime : viewTimeMax);
						var prevPosX = TimeToPos( previousClip != null? previousClip.endTime : viewTimeMin);
						var extRectLeft = Rect.MinMaxRect(prevPosX, clipRect.yMin, clipRect.xMin, clipRect.yMax);
						var extRectRight = Rect.MinMaxRect(clipRect.xMax, clipRect.yMin, nextPosX, clipRect.yMax);
						action.ShowClipGUIExternal(extRectLeft, extRectRight);

						//draw info text outside if clip is too small
						if (clipRect.width <= 20){
							GUI.Label(extRectRight, string.Format("<size=9>{0}</size>", action.info) );
						}
					}

					if (!track.isActive || track.isLocked){

						postWindowsGUI += ()=>
						{
							//overlay dark for disabled tracks
							if (!track.isActive){
								GUI.color = new Color(0,0,0,0.2f);
								GUI.DrawTexture(trackPosRect, whiteTexture);
								GUI.DrawTextureWithTexCoords(trackPosRect, Styles.stripes, new Rect(0,0, (trackPosRect.width/5), (trackPosRect.height/5) ));
								GUI.color = Color.white;
							}

							//overlay stripes for locked tracks
							if (track.isLocked){
								GUI.color = new Color(0,0,0,0.15f);
								GUI.DrawTextureWithTexCoords(trackPosRect, Styles.stripes, new Rect(0,0, trackPosRect.width/20, trackPosRect.height/20));
								GUI.color = Color.white;
							}

							if (isProSkin){
								string overlayLabel = null;
								if (!track.isActive && track.isLocked){
									overlayLabel = "DISABLED & LOCKED";
								} else {
									if (!track.isActive){
										overlayLabel = "DISABLED";
									}
									if (track.isLocked){
										overlayLabel = "LOCKED";
									}
								}
								var size = Styles.centerLabel.CalcSize( new GUIContent(overlayLabel) );
								var bgLabelRect = new Rect(0, 0, size.x, size.y);
								bgLabelRect.center = trackPosRect.center;
								GUI.Label(trackPosRect, string.Format("<b>{0}</b>", overlayLabel), Styles.centerLabel);
								GUI.color = Color.white;
							}
						};
					}

				}


				//highligh selected group
				if ( ReferenceEquals(CutsceneUtility.selectedObject, group) ){
					var r = Rect.MinMaxRect(groupRect.xMin, groupRect.yMin, groupRect.xMax, nextYPos );
					GUI.color = Color.grey;
					GUI.Box(r, "", Styles.hollowFrameHorizontalStyle);
					GUI.color = Color.white;
				}


			}

			EndWindows();

			//call postwindow delegate
			if (postWindowsGUI != null){
				postWindowsGUI();
				postWindowsGUI = null;
			}

			//this is done in the same GUI.Group
			DoMultiSelection();

			GUI.EndGroup();

			//border shadows
			GUI.color = new Color(1,1,1,0.2f);
			GUI.Box(bgRect, "", Styles.shadowBorderStyle);
			GUI.color = Color.white;

			///darken the time after cutscene length
			if (viewTimeMax > length){
                var endPos = Mathf.Max(TimeToPos(length) + leftRect.width, centerRect.xMin);
                var darkRect = Rect.MinMaxRect(endPos, centerRect.yMin, centerRect.xMax, centerRect.yMax);
                GUI.color = new Color(0, 0, 0, 0.3f);
                GUI.Box(darkRect, "", (GUIStyle)"TextField");
                GUI.color = Color.white;
            }

            if (startPlayTime > 0)
            {
                var startPos = Mathf.Max(TimeToPos(0) + leftRect.width, centerRect.xMin);
                var endPos = Mathf.Max(TimeToPos(startPlayTime) + leftRect.width, centerRect.xMin);
                var darkRect = Rect.MinMaxRect(startPos, centerRect.yMin, endPos, centerRect.yMax);
                GUI.color = new Color(0, 0, 0, 0.3f);
                GUI.Box(darkRect, "", (GUIStyle)"TextField");
                GUI.color = Color.white;
            }

			///darken the time before zero
			if (viewTimeMin < 0){
				var startPos = Mathf.Min( TimeToPos(0) + leftRect.width, centerRect.xMax );
				var darkRect = Rect.MinMaxRect(centerRect.xMin, centerRect.yMin, startPos, centerRect.yMax);
				GUI.color = new Color(0,0,0,0.3f);
				GUI.Box(darkRect, "", (GUIStyle)"TextField");
				GUI.color = Color.white;
			}

			if (GUIUtility.hotControl == 0 || e.rawType == EventType.MouseUp){
				anyClipDragging = false;
			}
		}



		//Group sections...
		void ShowGroupSections(CutsceneGroup group, Rect rect){
			var e = Event.current;
			GenericMenu sectionsMenu = null;
			if (e.type == EventType.ContextClick && rect.Contains(e.mousePosition)){
				var t = PosToTime(mousePosition.x);
				sectionsMenu = new GenericMenu();
				sectionsMenu.AddItem( new GUIContent("Add Section Here"), false, ()=>{ group.sections.Add(new Section("Section", t)); } );
			}

			var sections = new List<Section>(group.sections.OrderBy(s => s.time));
			if (sections.Count == 0){
				sections.Insert(0, new Section("No Sections", 0));
				sections.Add(new Section("Outro", maxTime));
			} else {
				sections.Insert(0, new Section("Intro", 0));
				sections.Add(new Section("Outro", maxTime));
			}

			for (var i = 0; i < sections.Count-1; i++){
				var section1 = sections[i];
				var section2 = sections[i + 1];
				var pos1 = TimeToPos(section1.time);
				var pos2 = TimeToPos(section2.time);
				var y = rect.y;
				
				var sectionRect = Rect.MinMaxRect(pos1, y, pos2 - 2, y + GROUP_HEIGHT - 5);
				var markRect    = new Rect(sectionRect.x + 2, sectionRect.y + 2, 2, sectionRect.height-4);
				var clickRect   = new Rect(0, y, 15, sectionRect.height);
				clickRect.center = markRect.center;

				GUI.color = section1.color;
				if (section1.colorizeBackground){
					GUI.DrawTexture(Rect.MinMaxRect(sectionRect.xMin, sectionRect.yMax+1, sectionRect.xMax, screenHeight + scrollPos.y), whiteTexture);
				}
				GUI.DrawTexture(sectionRect, whiteTexture);
				GUI.color = new Color(1,1,1,0.2f);
				GUI.DrawTexture(markRect, whiteTexture);
				GUI.color = Color.white;
				GUI.Label(sectionRect, string.Format(" <i>{0}</i>", section1.name) );

				if (sectionRect.Contains(e.mousePosition)){
					if (e.type == EventType.MouseDown && e.button == 0 ){
						if (e.clickCount == 2){
							viewTimeMin = section1.time;
							viewTimeMax = section2.time;
							e.Use();
						}
					}
					if (i != 0 && e.type == EventType.ContextClick && sectionsMenu != null){
						sectionsMenu.AddItem(new GUIContent("Edit"), false, ()=>
						{
							DoPopup(()=>
								{
									section1.name = EditorGUILayout.TextField("Name", section1.name);
									var previousSectionTime = sections.Last(s => s.time < section1.time && s != section1).time;
									var nextSectionTime = sections.First(s => s.time > section1.time && s != section1).time;
									section1.time = EditorGUILayout.Slider("Time", section1.time, previousSectionTime + 0.1f, nextSectionTime - 0.1f);
									section1.color = EditorGUILayout.ColorField("Color", section1.color);
									section1.colorizeBackground = EditorGUILayout.Toggle("Colorize Background", section1.colorizeBackground);
								});
						});
						sectionsMenu.AddItem(new GUIContent("Focus (Double Click)"), false, ()=>{ viewTimeMin = section1.time; viewTimeMax = section2.time; } );
						sectionsMenu.AddSeparator("/");
						sectionsMenu.AddItem(new GUIContent("Delete Section"), false, ()=>{ group.sections.Remove(section1); } );
					}
				}

				if (i != 0 && clickRect.Contains(e.mousePosition)){
					this.AddCursorRect(clickRect, MouseCursor.SlideArrow);
					if (e.type == EventType.MouseDown && e.button == 0){
						draggedSection = section1;
						e.Use();
					}
				}
			}

			if (draggedSection != null){
				var lastTime = draggedSection.time;
				var newTime = PosToTime(mousePosition.x);
				var previousSectionTime = sections.Last(s => s.time < lastTime).time;
				var nextSectionTime = sections.First(s => s.time > lastTime).time;
				newTime = SnapTime(newTime);
				newTime = Mathf.Clamp(newTime, previousSectionTime + 0.1f, nextSectionTime - 0.1f); //dont think a section should be as small as 1sec anyways.
				newTime = Mathf.Clamp(newTime, 0, maxTime);
				draggedSection.time = newTime;

				//shift clips and sections after drag section.
				if (e.shift){
					foreach(var cw in clipWrappers.Values.Where(c => c.action.startTime >= lastTime)){
						if (cw.action.isLocked){
							continue;
						}
						var max = cw.previousClip != null? cw.previousClip.endTime : 0;
						if (cw.previousClip != null && cw.action is ICrossBlendable){
							max -= Mathf.Min( cw.previousClip.length / 2, cw.action.length / 2);
						}
						cw.action.startTime += newTime - lastTime;
						cw.action.startTime = Mathf.Max(cw.action.startTime, max);
					}

					///This is very unoptimized but PropertyTrack will be deprecated in the future.
					foreach(var propTrack in cutscene.directables.OfType<PropertiesTrack>()){
						if (propTrack.isLocked){
							continue;
						}
						var data = propTrack.animationData;
						if (data.isValid){
							var curves = data.GetCurvesAll();
							foreach(var curve in curves){
								for (var i = 0; i < curve.length; i++){
									var key = curve[i];
									if (key.time >= lastTime){
										key.time += newTime - lastTime;
										curve.MoveKey(i, key);
									}
								}

								curve.UpdateTangentsFromMode();
							}
						}

						CutsceneUtility.RefreshAllAnimationEditorsOf(data);
					}
					///


					foreach(var section in group.sections.Where(s => s != draggedSection && s.time > lastTime)){
						section.time += newTime - lastTime;
					}
				}

				//shift all clips with time > to this section if shift is down
				if (e.control && !e.shift){
					foreach(var section in group.sections.Where(s => s != draggedSection && s.time > lastTime)){
						section.time += newTime - lastTime;
					}
				}

				if (e.rawType == EventType.MouseUp){
					draggedSection = null;
					group.sections = group.sections.OrderBy(s => s.time).ToList();
				}
			}

			if (sectionsMenu != null){
				sectionsMenu.ShowAsContext();
				e.Use();
			}			
		}

		/// @modify slate sequencer
		/// add by TQ
        void SetPropertyTrackTimeSelect(float? mouseStartPos = null, float? mouseEndPos = null, Rect? wrapRect = null) 
        {
            for (int g = 0; g < cutscene.groups.Count; g++)
            {
                //GROUP
                var group = cutscene.groups[g];
                //TRACKS
                for (int t = 0; t < group.tracks.Count; t++)
                {
                    PropertiesTrack track = group.tracks[t] as PropertiesTrack;
                    CameraTrack trackCam1 = group.tracks[t] as CameraTrack;
                    if (trackCam1 != null)
                    {
                        trackCam1.DopeSheetStartPos = mouseStartPos;
                        trackCam1.DopeSheedEndPos = mouseEndPos;
                        trackCam1.MagnetSnapInterval = magnetSnapInterval;
                    }

                    if (track == null)
                    {
                        continue;
                    }
                    track.DopeSheetStartPos = mouseStartPos;
                    track.DopeSheedEndPos = mouseEndPos;
                    
                    track.MagnetSnapInterval = magnetSnapInterval;
                    if (wrapRect == null)
                    {
                        continue;
                    }
                    if (track.DopePropertyRect == null)
                    {
                        continue;
                    }
                    if (!AIntersectAndEncapsulatesB((Rect)wrapRect, (Rect)track.DopePropertyRect))
                    {
                        track.DopeSheetStartPos = null;
                        track.DopeSheedEndPos = null;
                        track.DopePropertyRect = null;
                        continue;
                    }
                    track.DopeSheetStartPos = mouseStartPos;
                    track.DopeSheedEndPos = mouseEndPos;
                }
            }
        }
        /// end
        /// 
        Rect? DoMutiSelectionRect()
        {
            var e = Event.current;

            var r = new Rect();
            var bigEnough = false;
            /// @modify slate sequencer
            /// add by TQ
            if (multiSelectStartPos != null)
            {
                var start = (Vector2)multiSelectStartPos;
                if ((start - e.mousePosition).magnitude > 10)
                {
                    bigEnough = true;
                    r.xMin = Mathf.Max(Mathf.Min(start.x, e.mousePosition.x), 0);
                    r.xMax = Mathf.Min(Mathf.Max(start.x, e.mousePosition.x), screenWidth);
                    r.yMin = Mathf.Min(start.y, e.mousePosition.y);
                    r.yMax = Mathf.Max(start.y, e.mousePosition.y);
                }
            }      
            /// end
            if (e.rawType == EventType.MouseUp)
            {
                if (bigEnough)
                {
                    return r;
                }              
            }
            return null;
        }


		//This is done in a GUILayoutArea, thus must use e.mousePosition instead of this.mousePosition
		void DoMultiSelection(){
			
			var e = Event.current;

			var r = new Rect();
			var bigEnough = false;
/// @modify slate sequencer
/// add by TQ
            if (multiSelectStartPos != null)
            {
                var start = (Vector2)multiSelectStartPos;
                if ((start - e.mousePosition).magnitude > 10)
                {
                    bigEnough = true;
                    r.xMin = Mathf.Max(Mathf.Min(start.x, e.mousePosition.x), 0);
                    r.xMax = Mathf.Min(Mathf.Max(start.x, e.mousePosition.x), screenWidth);
                    r.yMin = Mathf.Min(start.y, e.mousePosition.y);
                    r.yMax = Mathf.Max(start.y, e.mousePosition.y);
                    GUI.color = isProSkin ? Color.white : new Color(1, 1, 1, 0.3f);
                    GUI.Box(r, "");
                    foreach (var wrapper in clipWrappers.Values.Where(b => AEncapsulatesB(r, b.rect) && !b.action.isLocked))
                    {
                        GUI.color = new Color(0.5f, 0.5f, 1, 0.5f);
                        GUI.Box(wrapper.rect, "", Slate.Styles.clipBoxStyle);
                        GUI.color = Color.white;
                    }

                    ///设置Property Track的
                    SetPropertyTrackTimeSelect(start.x, e.mousePosition.x, r);

                }
            }
            else
            {
                ///设置Property Track的
                SetPropertyTrackTimeSelect(null, null, null);
            }
/// end
            if (e.rawType == EventType.MouseUp){
				if (bigEnough){
					multiSelection = clipWrappers.Values.Where(b => AIntersectAndEncapsulatesB(r, b.rect) && !b.action.isLocked ).ToList();
                    if (multiSelection.Count == 1)
                    {
                        CutsceneUtility.selectedObject = multiSelection[0].action;
                        multiSelection = null;
                    }
				}
				multiSelectStartPos = null;
			}

			if (multiSelection != null && multiSelection.Count > 0)
            {
				var boundRect = GetBoundRect(multiSelection.Select(b => b.rect).ToArray(), 4f);
				GUI.color = isProSkin? Color.white : new Color(1,1,1,0.3f);
				GUI.Box(boundRect, "");

				var leftDragRect = new Rect(boundRect.xMin - 6, boundRect.yMin, 4, boundRect.height);
				var rightDragRect = new Rect(boundRect.xMax + 2, boundRect.yMin, 4, boundRect.height);
				AddCursorRect(leftDragRect, MouseCursor.ResizeHorizontal);
				AddCursorRect(rightDragRect, MouseCursor.ResizeHorizontal);				
				GUI.color = isProSkin? new Color(0.7f, 0.7f, 0.7f) : Color.grey;
				GUI.DrawTexture(leftDragRect, Styles.whiteTexture);
				GUI.DrawTexture(rightDragRect, Styles.whiteTexture);
				GUI.color = Color.white;

				if (e.type == EventType.MouseDown && (leftDragRect.Contains(e.mousePosition) || rightDragRect.Contains(e.mousePosition)) ){
					multiSelectionScaleDirection = leftDragRect.Contains(e.mousePosition)? -1 : 1;
					var minTime = Mathf.Min( multiSelection.Select(b => b.action.startTime).ToArray() );
					var maxTime = Mathf.Max( multiSelection.Select(b => b.action.endTime).ToArray() );
					preMultiSelectionRetimeMinMax = Rect.MinMaxRect( minTime, 0, maxTime, 0 );
					foreach(var wrapper in multiSelection){
						wrapper.BeginRetime();
					}
					e.Use();
				}

				if (e.type == EventType.MouseDrag && multiSelectionScaleDirection != 0){
					foreach(var clipWrapper in multiSelection){
						var clip = clipWrapper.action;
						var preClipStartTime = clipWrapper.preScaleStartTime;
						var preClipEndTime = clipWrapper.preScaleEndTime;
						var preTimeMin = preMultiSelectionRetimeMinMax.xMin;
						var preTimeMax = preMultiSelectionRetimeMinMax.xMax;
						var pointerTime = SnapTime( PosToTime(mousePosition.x ) );

						var lerpMin = multiSelectionScaleDirection == -1? Mathf.Clamp(pointerTime, 0, preTimeMax) : preTimeMin;
						var lerpMax = multiSelectionScaleDirection == 1? Mathf.Max(pointerTime, preTimeMin) : preTimeMax;

						var normIn = Mathf.InverseLerp(preTimeMin, preTimeMax, preClipStartTime);
						clip.startTime = Mathf.Lerp(lerpMin, lerpMax, normIn);

						var normOut = Mathf.InverseLerp(preTimeMin, preTimeMax, preClipEndTime);
						clip.endTime = Mathf.Lerp(lerpMin, lerpMax, normOut);

						if (e.shift){
							clipWrapper.UpdateRetime();
						}
					}
					e.Use();
				}

				if (e.rawType == EventType.MouseUp){
					multiSelectionScaleDirection = 0;
					foreach(var clipWrapper in multiSelection){
						clipWrapper.EndRetime();
					}
				}
			}

			if (e.type == EventType.MouseDown && e.button == 0 && GUIUtility.hotControl == 0){
				multiSelection = null;
				multiSelectStartPos = e.mousePosition;
			}

			GUI.color = Color.white;
		}
		// @modify slate sequencer
		//@add by TQ
        bool AIntersectAndEncapsulatesB(Rect a, Rect b)
        {
            if (a == default(Rect) || b == default(Rect))
            {
                return false;
            }

            bool isIntersect = a.xMax >= b.xMin && b.xMax >= a.xMin && a.yMax >= b.yMin && b.yMax >= a.yMin;

            return AEncapsulatesB(a, b) || (isIntersect); 

        }
		//end	
    //this could be an extension but it's only used here so...
    bool AEncapsulatesB(Rect a, Rect b){
			if (a == default(Rect) || b == default(Rect)){
				return false;
			}
			return a.xMin <= b.xMin && a.xMax >= b.xMax && a.yMin <= b.yMin && a.yMax >= b.yMax;
		}

		///Gets the bound rect out of many rects
		Rect GetBoundRect(Rect[] rects, float padding = 0f){
			var minX = float.PositiveInfinity;
			var minY = float.PositiveInfinity;
			var maxX = float.NegativeInfinity;
			var maxY = float.NegativeInfinity;
			
			for (var i = 0; i < rects.Length; i++){
				minX = Mathf.Min(minX, rects[i].xMin);
				minY = Mathf.Min(minY, rects[i].yMin);
				maxX = Mathf.Max(maxX, rects[i].xMax);
				maxY = Mathf.Max(maxY, rects[i].yMax);
			}

			minX -= padding;
			minY -= padding;
			maxX += padding;
			maxY += padding;
			return Rect.MinMaxRect(minX, minY, maxX, maxY);
		}


		//ActionClip window callback. Its ID is based on the UID function that is based on the index path to the action.
		//The ID of the window is also the same as the ID to use for for clipWrappers dictionary as key to get the clipWrapper for the action that represents this window
		void ActionClipWindow(int id){
			ActionClipWrapper wrapper = null;
			if (clipWrappers.TryGetValue(id, out wrapper)){
				wrapper.OnClipGUI();
			}
		}





        //A wrapper of an ActionClip placed in cutscene
		class ActionClipWrapper{
			
            const float CLIP_DOPESHEET_HEIGHT = 13f;
            const float SCALE_RECT_WIDTH = 4;

            public ActionClip action;
            public bool isScalingStart;
            public bool isScalingEnd;
            public bool isControlBlendIn;
            public bool isControlBlendOut;
            public float preScaleStartTime;
            public float preScaleEndTime;

            // @modify slate sequencer
            // add by TQ
            public float preClipOffset;
            public List<float> preActionCurrentTime = null;
            //end

            public ActionClip previousClip;
            public ActionClip nextClip;

            private Event e;
            private float overlapIn;
            private float overlapOut;
            private float blendInPosX;
            private float blendOutPosX;
            private bool hasActiveParameters;
            private bool hasParameters;
            private float pointerTime;
            private float snapedPointerTime;
            private bool isScalable;
            private Dictionary<AnimationCurve, Keyframe[]> retimingKeys;

            private CutsceneEditor editor {
				get {return CutsceneEditor.current;}
            }

            private List<ActionClipWrapper> multiSelection {
				get {return editor.multiSelection;}
				set {
                    editor.multiSelection = value;
                }
            }

            private Rect _rect;
            public Rect rect {
				get {return action.isCollapsed? default(Rect) : _rect;}
				set {_rect = value;}
            }

			public ActionClipWrapper(ActionClip action){
                this.action = action;
            }

			public void ResetInteraction(){
                isControlBlendIn = false;
                isControlBlendOut = false;
                isScalingStart = false;
                isScalingEnd = false;
                editor.clipScalingGuideTime = null;
            }

			public void OnClipGUI(){

                e = Event.current;

                overlapIn = previousClip != null ? Mathf.Max(previousClip.endTime - action.startTime, 0) : 0;
                overlapOut = nextClip != null ? Mathf.Max(action.endTime - nextClip.startTime, 0) : 0;
                blendInPosX = (action.blendIn / action.length) * rect.width;
                blendOutPosX = ((action.length - action.blendOut) / action.length) * rect.width;
                hasParameters = action.hasParameters;
                hasActiveParameters = action.hasActiveParameters;

                pointerTime = editor.PosToTime(editor.mousePosition.x);
                snapedPointerTime = editor.SnapTime(pointerTime);

                var lengthProp = action.GetType().GetProperty("length", BindingFlags.Instance | BindingFlags.Public);
                isScalable = lengthProp != null && lengthProp.DeclaringType != typeof(ActionClip) && lengthProp.CanWrite && action.length > 0;

                //...
                var localRect = new Rect(0, 0, rect.width, rect.height);
                if (action.isLocked) {
                    if (e.isMouse && localRect.Contains(e.mousePosition)) {
                        e.Use();
                    }
                }
/// @modify slate sequencer
/// add by TQ
                if (CutsceneEditor.current.CheckIsSubSysCamClip(action))
                {
                    if (e.isMouse && localRect.Contains(e.mousePosition))
                    {
                        e.Use();
                    }
                }
/// end
                action.ShowClipGUI(localRect);
                if (hasActiveParameters && action.length > 0) {
                    ShowClipDopesheet(localRect);
                }
                //...


                //BLEND GRAPHICS
                if (action.blendIn > 0) {
                    Handles.color = new Color(0, 0, 0, 0.5f);
                    Handles.DrawAAPolyLine(2, new Vector3[] { new Vector2(0, rect.height), new Vector2(blendInPosX, 0) });
                    Handles.color = new Color(0, 0, 0, 0.3f);
                    Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(0, 0), new Vector3(0, rect.height), new Vector3(blendInPosX, 0) });
                }

                if (action.blendOut > 0 && overlapOut == 0) {
                    Handles.color = new Color(0, 0, 0, 0.5f);
                    Handles.DrawAAPolyLine(2, new Vector3[] { new Vector2(blendOutPosX, 0), new Vector2(rect.width, rect.height) });
                    Handles.color = new Color(0, 0, 0, 0.3f);
                    Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(rect.width, 0), new Vector2(blendOutPosX, 0), new Vector2(rect.width, rect.height) });
                }

                if (overlapIn > 0) {
                    Handles.color = Color.black;
                    Handles.DrawAAPolyLine(2, new Vector3[] { new Vector2(blendInPosX, 0), new Vector2(blendInPosX, rect.height) });
                }

                Handles.color = Color.white;


                //SCALING IN/OUT, DRAG RECTS
                var allowScaleIn = isScalable && rect.width > SCALE_RECT_WIDTH * 2;
                var dragRect = new Rect((allowScaleIn ? SCALE_RECT_WIDTH : 0), 0, (isScalable ? rect.width - (allowScaleIn ? SCALE_RECT_WIDTH * 2 : SCALE_RECT_WIDTH) : rect.width), rect.height - (hasActiveParameters ? CLIP_DOPESHEET_HEIGHT : 0));
                editor.AddCursorRect(dragRect, MouseCursor.Link);

                var controlRectIn = new Rect(0, 0, SCALE_RECT_WIDTH, rect.height - (hasActiveParameters ? CLIP_DOPESHEET_HEIGHT : 0));
                var controlRectOut = new Rect(rect.width - SCALE_RECT_WIDTH, 0, SCALE_RECT_WIDTH, rect.height - (hasActiveParameters ? CLIP_DOPESHEET_HEIGHT : 0));
                if (isScalable) {
                    GUI.color = new Color(0, 1, 1, 0.3f);
                    if (overlapOut <= 0)
                    {
                        editor.AddCursorRect(controlRectOut, MouseCursor.ResizeHorizontal);
                        if (e.type == EventType.MouseDown && e.button == 0 && !e.control)
                        {
                            if (controlRectOut.Contains(e.mousePosition))
                            {
                                isScalingEnd = true;
/// @modify slate sequencer
/// add by TQ
                                CutsceneUtility.selectedObject = action;
/// end
                                preScaleStartTime = action.startTime;
                                preScaleEndTime = action.endTime;
                                BeginRetime();
                                e.Use();
                            }
                        }
                    }

                    if (overlapIn <= 0 && allowScaleIn)
                    {
                        editor.AddCursorRect(controlRectIn, MouseCursor.ResizeHorizontal);
                        if (e.type == EventType.MouseDown && e.button == 0 && !e.control)
                        {
                            if (controlRectIn.Contains(e.mousePosition))
                            {
                                isScalingStart = true;
/// @modify slate sequencer
/// add by TQ
                                CutsceneUtility.selectedObject = action;
/// end
                                preScaleStartTime = action.startTime;
                                // @modify slate sequencer
                                //drag start to cut the animation clip * add by TQ
                                if (action is ISubClipContainable)
                                {
                                    ISubClipContainable subClip = action as ISubClipContainable;
                                    preClipOffset = subClip.subClipOffset;
                                    ISubClipHasCurrentTime subTime = action as ISubClipHasCurrentTime;
                                    if (subTime != null)
                                    {
                                        preActionCurrentTime = subTime.CurrentTimes;
                                    }
                                }
                                //end *

                                preScaleEndTime = action.endTime;
                                BeginRetime();
                                e.Use();
                            }
                        }
                    }
                    GUI.color = Color.white;
                }

                //BLENDING IN/OUT
                if (e.type == EventType.MouseDown && e.button == 0 && e.control) {
                    var blendInProp = action.GetType().GetProperty("blendIn", BindingFlags.Instance | BindingFlags.Public);
                    var isBlendableIn = blendInProp != null && blendInProp.DeclaringType != typeof(ActionClip) && blendInProp.CanWrite;
                    var blendOutProp = action.GetType().GetProperty("blendOut", BindingFlags.Instance | BindingFlags.Public);
                    var isBlendableOut = blendOutProp != null && blendOutProp.DeclaringType != typeof(ActionClip) && blendOutProp.CanWrite;
                    if (isBlendableIn && controlRectIn.Contains(e.mousePosition)) {
                        isControlBlendIn = true;
                        e.Use();
                    }
                    if (isBlendableOut && controlRectOut.Contains(e.mousePosition)) {
                        isControlBlendOut = true;
                        e.Use();
                    }
                }


                if (isControlBlendIn) {
                    action.blendIn = Mathf.Clamp(pointerTime - action.startTime, 0, action.length - action.blendOut);
                }

                if (isControlBlendOut) {
                    action.blendOut = Mathf.Clamp(action.endTime - pointerTime, 0, action.length - action.blendIn);
                }

                if (isScalingStart) {
                    var prev = previousClip != null ? previousClip.endTime : 0;
                    if (Prefs.magnetSnapping && !e.control) { //magnet snap
                        if (Mathf.Abs(snapedPointerTime - prev) <= editor.magnetSnapInterval) {
                            snapedPointerTime = prev;
                        }
                    }
                    /// @modify slate sequencer
                    /// add by TQ
                    if (Prefs.magnetSnapping && !e.control)
                    { //magnet snap
                        if (Mathf.Abs(snapedPointerTime - current.cutscene.currentTime) <= editor.magnetSnapInterval)
                        {
                            snapedPointerTime = current.cutscene.currentTime;
                        }
                    }
                    /// end

                    if (action is ICrossBlendable && previousClip is ICrossBlendable) {
                        prev -= Mathf.Min(action.length / 2, previousClip.length / 2);
                    }
                    action.startTime = snapedPointerTime;
                    action.startTime = Mathf.Clamp(action.startTime, prev, preScaleEndTime);
/// @modify slate sequencer
/// add by TQ
                   // CutsceneEditor.current.MoveCameraClipWithSubCutsence(snapedPointerTime, action, false);
/// end
                    action.endTime = preScaleEndTime;
                    // @modify slate sequencer
                    // drag start to cut the animation clip   Add by TQ
                    if (action is ISubClipContainable)
                    {
                        ISubClipContainable subClip = action as ISubClipContainable;
                        subClip.subClipOffset = Mathf.Min(preClipOffset + (preScaleStartTime - action.startTime), 0);
                        if (subClip != null && action.onDragClipDelegate != null)
                        {
                            action.onDragClipDelegate(preActionCurrentTime, (preScaleStartTime - action.startTime));
                        }
                    }
                    // end
                 
                    editor.clipScalingGuideTime = action.startTime;
					if (e.shift){
						UpdateRetime();
					}
				}

				if (isScalingEnd){
					var next = nextClip != null? nextClip.startTime : editor.maxTime;
					if (Prefs.magnetSnapping && !e.control){ //magnet snap
						if (Mathf.Abs(snapedPointerTime - next) <= editor.magnetSnapInterval){
							snapedPointerTime = next;
						}
                        /// @modify slate sequencer
                        /// by TQ
                        if (Mathf.Abs(snapedPointerTime - current.cutscene.currentTime) <= editor.magnetSnapInterval)
                        {
                            snapedPointerTime = current.cutscene.currentTime;
                        }
                        /// end
                    }

					if (action is ICrossBlendable && nextClip is ICrossBlendable){
						next += Mathf.Min(action.length/2, nextClip.length/2);
					}
					action.endTime = snapedPointerTime;
					action.endTime = Mathf.Clamp( action.endTime, 0, next );
					editor.clipScalingGuideTime = action.endTime;
					if (e.shift){
						UpdateRetime();
					}
				}

				if (e.type == EventType.MouseDrag && e.button == 0 && dragRect.Contains(e.mousePosition)&& !editor.movingScrubCarret)
                {
					editor.anyClipDragging = true;
				}

				if (e.type == EventType.MouseDown){

					if (e.control){
						if (multiSelection == null){
							multiSelection = new List<ActionClipWrapper>(){this};
						}
						if (multiSelection.Contains(this)){
							multiSelection.Remove(this);
						} else {
							multiSelection.Add(this);
						}
					} else {
						CutsceneUtility.selectedObject = action;
						if (multiSelection != null && !multiSelection.Select(cw => cw.action).Contains(action)){
							multiSelection = null;
						}
					}

					if (e.clickCount == 2){
						//do this with reflection to get the declaring actor in case action has 'new' declaration. This is only done in Shot right now.
						Selection.activeObject = action.GetType().GetProperty("actor").GetValue(action, null) as Object;
                        var subCutDummy = action as SubCutscene;
                        if (subCutDummy != null && subCutDummy.cutscene != null)
                        {
                            subCutDummy.cutscene.PrevCutscene = current.cutscene;
                        }
					}
				}


				if (e.rawType == EventType.ContextClick){
					DoClipContextMenu();
				}

				// @modify slate sequencer
                // Keyboard Shortcuts
                // begin * add Shortcuts to cut clip by TQ 2017.9.30
                if (Event.current.type == EventType.KeyUp)
                {
                   /// Event e = Event.current;
                    DGEditorShortcut[] temp = ShortcutManager.GetKeyboardShortcuts("StoryEngine", null);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (e.keyCode == temp[i].key && e.modifiers == temp[i].modifiers && ShortcutManager.shortcutActions[temp[i].action] == ShortcutText.ClipCut)
                        {
                            CutsceneUtility.CutClip(action);
                            e.Use();
                         
                        }
                        if (e.keyCode == temp[i].key && e.modifiers == temp[i].modifiers && ShortcutManager.shortcutActions[temp[i].action] == ShortcutText.ClipSelectBehind)
                        {
                            editor.SelectAllBehindClip();
                            e.Use();
                           
                        }
                    }
                }
                //end

                if (e.rawType == EventType.MouseUp){
                    /// @modify slate sequencer
                    /// add by TQ
                    //subcutsence cam sys
                    if (e.button == 0)
                    {
                        CutsceneEditor.current.SubCutsenceSysCamFunc(editor.anyClipDragging);
                        ResetInteraction();
                    }
                    /// end
                    EndRetime();
				}

				if (e.button == 0){
					GUI.DragWindow(dragRect);
				}

				//Draw info text if big enough
				if (rect.width > 20){
					var r = new Rect(0, 0, rect.width, rect.height);
					if (overlapIn > 0){	r.xMin = blendInPosX; }
					if (overlapOut > 0){ r.xMax = blendOutPosX;	}
					var label = string.Format("<size=10>{0}</size>", action.info);
					GUI.color = Color.black;
					GUI.Label(r, label);
					GUI.color = Color.white;
				}
			}


			//initialize original keys dictionary
			public void BeginRetime(){
				preScaleStartTime = action.startTime;
				preScaleEndTime = action.endTime;
				if (hasActiveParameters){
					retimingKeys = new Dictionary<AnimationCurve, Keyframe[]>();
					foreach(var curve in action.animationData.GetCurvesAll()){
						retimingKeys[curve] = curve.keys;
					}
				}
			}

			//denetialize retiming keys
			public void EndRetime(){
				retimingKeys = null;
			}

			//do retiming keys
			public void UpdateRetime(){
				
				if (retimingKeys == null){
					return;
				}

				//retime keys. get all curves even if param disabled for retiming
				foreach (var curve in action.animationData.GetCurvesAll()){
					for (var i = 0; i < curve.keys.Length; i++){
						var preKey = retimingKeys[curve][i];
						
						//in case key outside of length range, simply offset it
						if (curve[i].time > action.length){
							var offsetDiff = (action.endTime - preScaleEndTime) + (preScaleStartTime - action.startTime);
							preKey.time += offsetDiff;
							curve.MoveKey(i, preKey );
							continue;
						}

						var preLength = preScaleEndTime - preScaleStartTime;
						var newTime = Mathf.Lerp(0, action.length, preKey.time/preLength);
						preKey.time = newTime;

						curve.MoveKey(i, preKey );
					}

					curve.UpdateTangentsFromMode();
				}

				//notify changes
				CutsceneUtility.RefreshAllAnimationEditorsOf(action.animationData);
			}

			///Split the clip in two, at specified local time
			public ActionClip Split(float time){
				
				if (hasParameters){
					foreach(var param in action.animationData.animatedParameters){
						if (param.HasAnyKey()){
							param.TryKeyIdentity(time - action.startTime);
						}
					}
				}

				CutsceneUtility.CopyClip(action);
				var copy = CutsceneUtility.PasteClip( (CutsceneTrack)action.parent, time);
				copy.startTime = time;
				copy.endTime = action.endTime;
				action.endTime = time;
				// @modify slate sequencer
				// add by TQ
                if (copy is IInitLengthable)
                {
                   (copy as IInitLengthable).InitLength = false;
                }
                if (action is IInitLengthable)
                {
                   (action as IInitLengthable).InitLength = false;
                }
				//end
                copy.blendIn = 0;
				action.blendOut = 0;
				CutsceneUtility.selectedObject = null;
				CutsceneUtility.SetCopyType(null);

				if (hasParameters){
					foreach(var param in copy.animationData.animatedParameters){
						foreach(var curve in param.curves){
							var finalKeys = new List<Keyframe>();
							foreach (var key in curve.keys){
								var modKey = key;
								modKey.time -= action.length;
								if (modKey.time >= 0){
									finalKeys.Add(modKey);
								}
							}
							curve.keys = finalKeys.ToArray();
						}
					}
				}

				if (copy is ISubClipContainable){
					(copy as ISubClipContainable).subClipOffset -= action.length;
				}

				return copy;
			}

			//Show the clip dopesheet
			void ShowClipDopesheet(Rect rect){
				var dopeRect = new Rect(0, rect.height - CLIP_DOPESHEET_HEIGHT, rect.width, CLIP_DOPESHEET_HEIGHT);
				GUI.color = isProSkin?  new Color(0,0.2f,0.2f,0.5f) : new Color(0,0.8f,0.8f,0.5f);
				GUI.Box(dopeRect, "", Slate.Styles.clipBoxFooterStyle);
				GUI.color = Color.white;
				DopeSheetEditor.DrawDopeSheet(action.animationData, action, dopeRect, 0, action.length, false);
			}

			//CONTEXT
			void DoClipContextMenu(){

				var menu = new GenericMenu();

				if (multiSelection != null && multiSelection.Contains(this)){
					menu.AddItem(new GUIContent("Delete Clips"), false, ()=>
					{
						editor.SafeDoAction( ()=>
							{
								foreach(var act in multiSelection.Select(b => b.action).ToArray()){
									(act.parent as CutsceneTrack).DeleteAction(act);
								}
								editor.InitClipWrappers();
								multiSelection = null;
							});
					});
/// @modify slate sequencer
/// add by TQ
                    menu.AddItem(new GUIContent("Batch Cut"), false, () => {
                        foreach (var act in multiSelection.Select(b => b.action).ToArray())
                        {
                            var trackDummy = act.parent as CutsceneTrack;
                            CutsceneUtility.SetCopyType(null);
                            var linkCamClip = act as ActionClips.StoryEngineClip.StoryCameraClip;
                            if (linkCamClip != null && linkCamClip.linkClip != null && linkCamClip.fromCutsence != null)
                            {
                                continue;
                            }
                            CutsceneUtility.CutClips(trackDummy,act);
                            (act.parent as CutsceneTrack).DeleteAction(act);
                            DopeSheetEditor.BatchCutsKeysOutside();
                            editor.InitClipWrappers();
                            multiSelection = null;
                        }
                    });

                    menu.AddItem(new GUIContent("Batch Delete"), false, () =>
                    {
                        editor.SafeDoAction(() =>
                        {
                            foreach (var act in multiSelection.Select(b => b.action).ToArray())
                            {
                                (act.parent as CutsceneTrack).WrapDeleteAction(act);
                            }
                            editor.InitClipWrappers();
                            multiSelection = null;
                        });

                        DopeSheetEditor.DeleteTheDopeSheetLineOutside();
                    });
                    /// end
                    menu.ShowAsContext();
					e.Use();
					return;
				}
/// @modify slate sequencer
/// add by TQ
/// 
                if (action is SubCutscene)
                {
                    var actionTrackName = action.parent.name;
                    string[] nameArr = actionTrackName.Split('_');
                    var pCutsence = action.root as Cutscene;
                    UnityEngine.SceneManagement.Scene scene = pCutsence.gameObject.scene;
                    List<Slate.Cutscene> trackCutsenceList = new List<Slate.Cutscene>();
                    bool isSubcutRecord = pCutsence.PrevCutscene != null && pCutsence.name.Contains("Take") && pCutsence.name.Contains("_Cut_");
                    if (nameArr.Length >= 2)
                    {
                        var containStr = nameArr[1];
                        if (isSubcutRecord)
                        {
                            containStr = containStr + "_SubCut_";
                        }
                        else
                        {
                            containStr = containStr + "_Cut_";
                        }
                        
                        GameObject[] gos = scene.GetRootGameObjects();
                        foreach (GameObject go in gos)
                        {
                            Slate.Cutscene result = null;
                            if (go.name.Contains("_group") && go.name.Contains("_Cut_")&& go.name.Contains("Take"))
                            {
                               var results = go.GetComponentsInChildren<Slate.Cutscene>();
                                for (int i =0;i< results.Length;i++)
                                {
                                    var resultDummy = results[i];
                                    if (!resultDummy.name.Contains(containStr))
                                        continue;
                                    trackCutsenceList.Add(resultDummy);
                                }
                            }
                            if (go.name.Contains("_group") && !go.name.Contains("_Cut_") && go.name.Contains("Take"))
                            {
                                var results = go.GetComponentsInChildren<Slate.Cutscene>();
                                for (int i = 0; i < results.Length; i++)
                                {
                                    var resultDummy = results[i];
                                    if (!resultDummy.name.Contains(containStr))
                                        continue;
                                    trackCutsenceList.Add(resultDummy);
                                } 
                            }

                            if (go.activeSelf)
                            {
                                result = go.GetComponent<Slate.Cutscene>();
                            }
                            if (result == null)
                                continue;
                            if (!result.name.Contains(containStr))
                                continue;                          
                            trackCutsenceList.Add(result);
                        }
                        var actionCutsence = action as SubCutscene;
                        GenericMenu.MenuFunction2 SelectSequencer = (object idx) =>
                        {
                            var invalidCamClip = pCutsence.cameraTrackCustom.actions.FindAll(g => g.isActive == false || g.startTime < actionCutsence.startTime + actionCutsence.length);
                            foreach (var incc in invalidCamClip)
                            {
                                pCutsence.cameraTrackCustom.DeleteAction(incc);
                            }

                            actionCutsence.cutscene = trackCutsenceList[(int)idx];
                            actionCutsence.length = actionCutsence.cutscene.length;
                        };         
                        for (int i = 0; i< trackCutsenceList.Count; i++)
                        {                                                  
                           menu.AddItem(new GUIContent(string.Format("Record Choose/[{0}]", trackCutsenceList[i].name)), trackCutsenceList[i].name == actionCutsence.cutscene.name, SelectSequencer, i);                           
                        }
                        if (trackCutsenceList.Count != 0)
                        {
                            menu.AddItem(new GUIContent("Record View"), false, () =>
                                {
                                    string orignalName = actionCutsence.cutscene.name;
                                    string editNamePrefix = orignalName.Substring(0, orignalName.IndexOf("_Cut_") + "_Cut_".Length);
                                    string targetName = editNamePrefix + "Edited";
                                    if (GameObject.Find(targetName) != null)
                                    {
                                        GameObject GO = GameObject.Find(targetName);
                                        Selection.activeObject = GO;
                                        var dummyCut = GO.GetComponent<Cutscene>();
                                        if (dummyCut != null)
                                        {
                                            dummyCut.PrevCutscene = pCutsence;
                                            dummyCut.EditFromCutClip = actionCutsence;
                                            dummyCut.IsSubEdite = true;
                                        }
                                        
                                        return;
                                    }

                                    Cutscene  editedSence =  Cutscene.Create();
                                    editedSence.name = targetName;

                                    for (int i = 0; i < trackCutsenceList.Count; i++)
                                    {
                                        var alreadyTrack = editedSence.GetTrackByName(trackCutsenceList[i].name);
                                        Slate.DirectorActionTrack subCutsenceTrack = null;
                                        if (alreadyTrack == null)
                                        {
                                            subCutsenceTrack = editedSence.directorGroup.AddTrack<Slate.DirectorActionTrack>(trackCutsenceList[i].name);
                                        }
                                        else
                                        {
                                            subCutsenceTrack = alreadyTrack as Slate.DirectorActionTrack;
                                            if (subCutsenceTrack != null)
                                            {
                                                var sbCut = subCutsenceTrack.actions.Find(t => (t.info.Contains("Take" + editedSence.CutRecord + "_Cut_"))) as Slate.ActionClips.SubCutscene;
                                                subCutsenceTrack.WrapDeleteAction(sbCut);
                                                
                                            }
                                        }

                                        subCutsenceTrack.isActive = trackCutsenceList[i].name == orignalName;
                                        Slate.ActionClips.SubCutscene subCutscene = subCutsenceTrack.AddAction<SubCutscene>(0);
                                       
                                        subCutscene.cutscene = trackCutsenceList[i];
                                        subCutscene.length = subCutscene.cutscene.length;
                                        
                                    }
                                    editedSence.PrevCutscene = pCutsence;
                                    editedSence.EditFromCutClip = actionCutsence;
                                    editedSence.IsSubEdite = true;
                                    UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(editedSence.gameObject, pCutsence.gameObject.scene);
                                }
                            );
                        }
                    }

                    if (pCutsence.PrevCutscene != null && pCutsence.IsSubEdite)
                    {
                        menu.AddItem(new GUIContent("Set Active"), false, () =>
                            {
                                List<CutsceneTrack> templist = pCutsence.directorGroup.tracks.FindAll(t => t.name == "Composition Track");
                                for (int i = 0; i< templist.Count; i++)
                                {
                                    CutsceneTrack t = templist[i];
                                    if (object.ReferenceEquals(action.parent, t))
                                    {
                                        t.isActive = true;
                                    }
                                    else
                                    {
                                        t.isActive = false;
                                    }
                                }
                            }
                        );
                    }

                }


				menu.AddItem(new GUIContent("Copy Clip"), false, ()=> {
                    CutsceneUtility.WrapCopyClip(action);} 
                );
				menu.AddItem(new GUIContent("Cut Clip"), false, ()=> 
                {
                    CutsceneUtility.WrapCutClip(action);
                } );
/// end

/// @modify slate sequencer
/// add by TQ
                menu.AddItem(new GUIContent("Batch Cut"), false, () => {
                    CutsceneUtility.CutClip(action);
                    DopeSheetEditor.BatchCutsKeysOutside();
                } );
/// end
				if (isScalable){

					menu.AddItem(new GUIContent("StretchFit Clip"), false, ()=>
					{
						action.startTime = previousClip != null? previousClip.endTime : action.parent.startTime;
						action.endTime = nextClip != null? nextClip.startTime : action.parent.endTime;
					});

					if (action.length > 0){
						menu.AddItem(new GUIContent("Split Here"), false, ()=>
						{
							var clickTime = snapedPointerTime;
							Split(clickTime);
						});

						menu.AddItem(new GUIContent("Trim Start ( [ )"), false, ()=>
						{
							var temp = action.endTime;
							action.startTime = pointerTime;
							action.endTime += temp - action.endTime;								
						});

						menu.AddItem(new GUIContent("Trim End ( ] )"), false, ()=>
						{
							action.endTime = pointerTime;
						});
					}
				}

				menu.AddSeparator("/");

				if (hasActiveParameters){
					menu.AddItem(new GUIContent("Remove Animation"), false, ()=>
					{
						if (EditorUtility.DisplayDialog("Remove Animation", "All Animation Curve keys of all animated parameters for this clip will be removed.\nAre you sure?", "Yes", "No")){
							editor.SafeDoAction( ()=>{ action.ResetAnimatedParameters(); } );
						}
					});
				}

				menu.AddItem(new GUIContent("Delete Clip"), false, ()=>
				{
/// @modify slate sequencer
/// add by TQ
					editor.SafeDoAction( ()=>{ (action.parent as CutsceneTrack).WrapDeleteAction(action); editor.InitClipWrappers(); } );
/// end
				});

/// @modify slate sequencer
/// add by TQ
/// Batch Delete Function
                menu.AddItem(new GUIContent("Batch Delete"), false, () =>
                {
                    editor.SafeDoAction(() => { (action.parent as CutsceneTrack).WrapDeleteAction(action); editor.InitClipWrappers(); });
                    DopeSheetEditor.DeleteTheDopeSheetLineOutside();
                });
/// end

                menu.ShowAsContext();
				e.Use();
			}
		}
/// @modify slate sequencer
/// @TQ
/// youtube
        public void SubCutsenceSysCamFunc(bool isDrag)
        {
            int offsetEnd = 0;
            int offsetStart = 0;
            var action = CutsceneUtility.selectedObject as ActionClips.SubCutscene;
            if (action == null)
                return;
            if (isDrag)
            {
                return;
            }
            Cutscene subCutsence = action.cutscene;
            if (subCutsence == null)
            {
                return;
            }
            //action.clipOffset = -subCutsence.startPlayTime;
            //action.length = subCutsence.length - subCutsence.startPlayTime;
            var subCameraTrackCustom = subCutsence.cameraTrack;
            if (subCameraTrackCustom == null)
            {
                return;
            }

            List<ActionClip> subCameraClips = subCameraTrackCustom.actions;
            List<ActionClip> linkClips = new List<ActionClip>();
            var subCameraCount = subCameraClips.Count;
            Cutscene actionRoot = action.root as Cutscene;
            if (actionRoot != null && actionRoot.cameraTrackCustom != null)
            {
                List<ActionClip> allClips = actionRoot.cameraTrackCustom.actions;
                foreach (var clip in allClips)
                {
                    ActionClips.StoryEngineClip.StoryCameraClip cur = clip as ActionClips.StoryEngineClip.StoryCameraClip;
                    if (cur != null && cur.fromCutsence != null && ReferenceEquals(cur.fromCutsence, action.cutscene)
                        && cur.linkClip != null && ReferenceEquals(cur.linkClip, action))
                    {
                        linkClips.Add(clip);
                    }
                }
                foreach (var act in linkClips)
                {
                    actionRoot.cameraTrackCustom.DeleteAction(act);
                    CutsceneEditor.current.OutInitClipWrappers();
                }
                for (int i = 0; i < subCameraClips.Count; i++)
                {
                    var subDummy = subCameraClips[i];
                    var s_Time = subCameraClips[i].startTime + 0;
                    var e_Time = subCameraClips[i].endTime + 0;
                    if (e_Time > action.length - action.ClipOffset && action.length - action.ClipOffset >= s_Time)
                    {
                        offsetEnd = subCameraClips.Count - 1 - i;
                    }
                    var offsetTime = Mathf.Abs(action.ClipOffset) + 0;
                    if (e_Time >= offsetTime && offsetTime > s_Time)
                    {
                        offsetStart = i;
                    }
                }
                if (subCameraCount > 0)
                {
                    var lastclip = subCameraClips[subCameraCount - offsetEnd - 1];
                    var firstClip = subCameraClips[offsetStart] as ActionClips.StoryEngineClip.StoryCameraClip;

                    for (int i = offsetStart; i <= (subCameraCount - offsetEnd - 1); i++)
                    {
                        var copyJson = JsonUtility.ToJson(subCameraClips[i]);
                        var copyType = subCameraClips[i].GetType();
                        if (copyType != null)
                        {
                            var newAction = actionRoot.cameraTrackCustom.AddActionCustomer(copyType, subCameraClips[i].startTime);
                            JsonUtility.FromJsonOverwrite(copyJson, newAction);
                            newAction.startTime = action.startTime + subCameraClips[i].startTime;
                            ActionClips.StoryEngineClip.StoryCameraClip camAction = newAction as ActionClips.StoryEngineClip.StoryCameraClip;

                            if (camAction != null)
                            {
                                camAction.startTime = camAction.startTime + action.ClipOffset;
                                if (lastclip != null && ReferenceEquals(lastclip, subCameraClips[i]))
                                {
                                    camAction.endTime = action.endTime;
                                }
                                if (firstClip != null && ReferenceEquals(firstClip, subCameraClips[i]) /*&& !ReferenceEquals(lastclip, firstClip)*/)
                                {
                                    camAction.startTime = action.startTime;
                                    if (ReferenceEquals(lastclip, firstClip))
                                    {
                                        camAction.clipOffset = (subCameraClips[i].startTime + action.ClipOffset);
                                        camAction.length = action.length;
                                    }
                                    else
                                    {
                                        camAction.clipOffset = (subCameraClips[i].startTime + action.ClipOffset);
                                        camAction.length = camAction.length + camAction.clipOffset;
                                    }
                                }
                                camAction.fromCutsence = action.cutscene;
                                camAction.linkClip = action;
                                action.linkID = action.GetInstanceID().ToString();
                                camAction.linkID = action.GetInstanceID().ToString();
                            }
                        }
                    }
                }
            }
        }
/// end

/// @modify slate sequencer
/// add by TQ
        private void MoveCameraClipWithSubCutsence(float deltaTime , ActionClip _Clip, bool useDt =  true, ActionClipWrapper _ClipWrapper = null)
        {
            if (useDt && _ClipWrapper != null && _ClipWrapper.isScalingStart)
            {
                return;
            }
            var subCutsenceClip = _Clip as SubCutscene;
            if (subCutsenceClip == null)
                return;
            if (subCutsenceClip.cutscene == null)
                return;
            if (subCutsenceClip != null && subCutsenceClip.cutscene.cameraTrackCustom != null)
            {
                List<ActionClip> curCamActions = current.cutscene.cameraTrackCustom.actions;

                if (subCutsenceClip.cutscene.cameraTrackCustom == null)
                    return;

                List<ActionClip> subCamActions = subCutsenceClip.cutscene.cameraTrackCustom.actions;
                if (subCamActions.Count == 0)
                    return;
                for (int i = 0; i < curCamActions.Count; i++)
                {
                    bool inMut = false;
                    if (multiSelection != null)
                    {
                        foreach (var cw in multiSelection)
                        {
                            if (cw.action == curCamActions[i])
                            {
                                inMut = true;
                                break;
                            }
                        }
                    }
                    if (inMut)
                    {
                        continue;
                    }
                    ActionClips.StoryEngineClip.StoryCameraClip cur = curCamActions[i] as ActionClips.StoryEngineClip.StoryCameraClip;
                    if (cur!= null && cur.fromCutsence!= null && ReferenceEquals(cur.fromCutsence, subCutsenceClip.cutscene) 
                        && cur.linkClip != null && ReferenceEquals(cur.linkClip, _Clip))
                    {
                        if (useDt)
                        {
                            cur.startTime = cur.startTime + deltaTime;
                        }
                        else
                        {
                            cur.startTime = cur.startTime ;
                        }           
                    }                    
                }
            }        
        }
/// end

/// @modify slate sequencer
/// add by TQ
        public bool CheckIsSubSysCamClip(ActionClip _clip)
        {
            var dummyClip = _clip as ActionClips.StoryEngineClip.StoryCameraClip;
            if (dummyClip != null && dummyClip.fromCutsence !=  null && dummyClip.linkClip != null)
            {
                return true;
            }
            return false;
        }
/// end

        private void AddPropertyToPickGroups(MemberInfo member, Transform child)
        {
            if (pickedGroupList.Count <= 0)
                return;
            for (int i = 0; i < pickedGroupList.Count; i++)
            {
                var g = pickedGroupList[i] as CutsceneGroup;
                if (g != null)
                {
                    PropertiesTrack t = g.GetPropertyTrack();
                    
                    if (t != null)
                    {
                        var go = t.animatedParametersTarget as GameObject;
                        t.animationData.TryAddParameter(member, t, go.transform, go.transform);

                    }
                }

            }
        }
        /// end
        // @modify slate sequencer
        // @rongxia
        private void DragFileToAddClip(CutsceneTrack cut, Rect clipsPosRect)
        {
            if ((Event.current.type == EventType.DragPerform) && clipsPosRect.Contains((Event.current.mousePosition)))
            {
                for (int i = 0; i < DragAndDrop.paths.Length; i++)
                {
                    string fileName = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), DragAndDrop.paths[i]);
                    fileName = fileName.Replace("\\", "/");
                    
                    foreach (ICutsceneTrackDropFile dropFile in dropFileList)
                    {
                        float t = 0f;
                        if (cut.actions != null && cut.actions.Count != 0)
                        {
                            t = cut.actions[cut.actions.Count - 1].endTime;
                        }

                        if (dropFile.DragFileToAddClip(cut, fileName, t))
                        {
                            continue;
                        }
                    }
                }
                
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    var o = DragAndDrop.objectReferences[i];

                    bool isAudioTrack = cut is AudioTrack;
                    if (isAudioTrack)
                    {
                        AudioClip dragAudio = o as AudioClip;
                        if (dragAudio != null)
                        {
                            ActionClips.PlayAudio clip = null;
                            if (cut.actions == null || cut.actions.Count == 0)
                                clip = cut.AddAction<ActionClips.PlayAudio>(0f);
                            else
                                clip = cut.AddAction<ActionClips.PlayAudio>(cut.actions[cut.actions.Count - 1].endTime);

                            clip.audioClip = dragAudio;
                            clip.length = clip.audioClip.length;
                        }

                    }
                }
                Event.current.Use();
                return;
            }
        }
        // @end
    }
}

#endif