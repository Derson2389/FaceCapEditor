using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Slate{

	///Tracks are contained within CutsceneGroups and contain ActionsClips within
	abstract public class CutsceneTrack : MonoBehaviour, IDirectable{

		[SerializeField]
		private string _name;
		[SerializeField]
		private Color _color = Color.white;
		[SerializeField] [HideInInspector]
		private bool _active = true;
		[SerializeField] [HideInInspector]
		private bool _isLocked = false;
		[SerializeField] [HideInInspector]
		private List<ActionClip> _actionClips = new List<ActionClip>();

        // @modify slate sequencer
        // @TQ
        // DragFileToAddClip function
        public delegate void DragFileToAddClip(CutsceneTrack obj, Rect clipsPosRect);
        public DragFileToAddClip DragFileAddClipFunc = null;

        public delegate void AddPropertyToPickGroups(MemberInfo member, Transform child);
        public AddPropertyToPickGroups AddPropertyToPickGroupsFunc = null;

        public float? DopeSheetStartPos = null;
        public float? DopeSheedEndPos = null;
        public float? MagnetSnapInterval = null;

        [SerializeField]
        [HideInInspector]
        private int? _recordIdx = null;

        // end slate sequencer

        //the actor to be used in the track taken from it's parent group
        public GameObject actor{
			get {return parent != null? parent.actor : null;}
		}

		//the name...
		new public string name{
			get {return string.IsNullOrEmpty(_name)? GetType().Name.SplitCamelCase() : _name;}
			set
			{
				if (_name != value){
					_name = value;
					base.name = value;
				}
			}
		}

		///Coloring of clips within this track
		public Color color{
			get {return _color.a > 0.1f? _color : Color.white;}
		}

		//all action clips of this track
		public List<ActionClip> actions{
			get {return _actionClips;}
			set {_actionClips = value;}
		}

		virtual public string info{
			get {return string.Empty;}
		}

		IEnumerable<IDirectable> IDirectable.children{
			get {return actions.Cast<IDirectable>();}
		}

		public int layerOrder{get; set;}

		public IDirector root{ get {return parent != null? parent.root : null;} }
		public IDirectable parent{ get; private set; }

		public bool isCollapsed{
			get {return parent != null? parent.isCollapsed : false;}
		}

		public bool isActive{
			get {return parent != null? parent.isActive && _active : false;}
			set {
                _active = value;
            }
		}

		public bool isLocked{
			get {return parent != null? parent.isLocked || _isLocked : false;}
			set {_isLocked = value;}
		}
        // @modify slate sequencer
        // @TQ
        [SerializeField]
        [HideInInspector]
        private bool _isShowInTimeline = true;
        public bool isShowInTimeline
        {
            get { return _isShowInTimeline;  }
            set
            {
                _isShowInTimeline = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool _isHideChecked = false;

        public bool isHideChecked
        {
            get { return _isHideChecked; }
            set
            {
                _isHideChecked = value;
            }
        }

        private float _trackHeightOffset = 0f;
        public float TrackHeightOffset
        {
            get { return _trackHeightOffset;  }
            set { _trackHeightOffset = value; }
        }


        public int? RecordIdx
        {
            get { return _recordIdx;  }
            set { _recordIdx = value; }
        }
		///end
		virtual public float startTime{
			get {return parent != null? parent.startTime : 0;}
			set {}
		}

		virtual public float endTime{
			get {return parent != null? parent.endTime : 0;}
			set {}
		}

		virtual public float blendIn{
			get {return 0f;}
			set {}
		}

		virtual public float blendOut{
			get {return 0f;}
			set {}
		}

		bool IDirectable.Initialize(){
			//layers are type based
			layerOrder = parent.children.Where( t => t.GetType() == this.GetType() ).Reverse().ToList().IndexOf(this);
			return OnInitialize();
		}

		//when the cutscene starts
		void IDirectable.Enter(){OnEnter();}
		//when the cutscene is updated
		void IDirectable.Update(float time, float previousTime){OnUpdate(time, previousTime);}
		//when the cutscene stops
		void IDirectable.Exit(){OnExit();}
		//when the cutscene enters backwards
		void IDirectable.ReverseEnter(){OnReverseEnter();}
		//when the cutscene is reversed/rewinded
		void IDirectable.Reverse(){OnReverse();}

		//when root is enabled/started
		void IDirectable.RootEnabled(){OnRootEnabled();}
		//when root is disabled/finished
		void IDirectable.RootDisabled(){OnRootDisabled();}
		//when root is updated
		void IDirectable.RootUpdated(float time, float previousTime){OnRootUpdated(time, previousTime);}

#if UNITY_EDITOR
		//Gizmos selected
		void IDirectable.DrawGizmos(bool selected){ if (selected) OnDrawGizmosSelected();}
		//Scene GUI stuff
		void IDirectable.SceneGUI(bool selected){ OnSceneGUI();}
#endif

		///After creation
		public void PostCreate(IDirectable parent){
			this.parent = parent;
			OnCreate();
		}

		///Validate the track and it's clips
		public void Validate(IDirector root, IDirectable parent){
			this.parent = parent;
			actions = GetComponents<ActionClip>().OrderBy(a => a.startTime).ToList();
			OnAfterValidate();
		}

		virtual protected void OnCreate(){}
		virtual protected void OnAfterValidate(){}
		virtual protected bool OnInitialize(){return true;}
		virtual protected void OnEnter(){}
		virtual protected void OnUpdate(float time, float previousTime){}
		virtual protected void OnExit(){}
		virtual protected void OnReverseEnter(){}
		virtual protected void OnReverse(){}
		virtual protected void OnDrawGizmosSelected(){}
		virtual protected void OnSceneGUI(){}

		virtual protected void OnRootEnabled(){}
		virtual protected void OnRootDisabled(){}
		virtual protected void OnRootUpdated(float time, float previousTime){}

        ///The weight of the track at time with optional override blend in out parameters.
        ///
        public float GetTrackWeight() { return this.GetWeight(root.currentTime - this.startTime, this.blendIn, this.blendOut); }
        public float GetTrackWeight(float time) { return this.GetWeight(time, this.blendIn, this.blendOut); }
        public float GetTrackWeight(float time, float blendInOut) { return this.GetWeight(time, blendInOut, blendInOut); }
        public float GetTrackWeight(float time, float blendIn, float blendOut) { return this.GetWeight(time, blendIn, blendOut); }



        ///Transforms a point in specified space
        public Vector3 TransformPoint(Vector3 point, TransformSpace space){
			return parent != null? parent.TransformPoint(point, space) : point;
		}

		///Inverse Transforms a point in specified space
		public Vector3 InverseTransformPoint(Vector3 point, TransformSpace space){
			return parent != null? parent.InverseTransformPoint(point, space) : point;
		}

		///Returns the final actor position in specified Space (InverseTransform Space)
		public Vector3 ActorPositionInSpace(TransformSpace space){
			return parent != null? parent.ActorPositionInSpace(space) : Vector3.zero;
		}

		///Returns the transform object used for specified Space transformations. Null if World Space.
		public Transform GetSpaceTransform(TransformSpace space){
			return parent != null? parent.GetSpaceTransform(space) : null;
		}

#if SLATE_USE_EXPRESSIONS
		///The ExpressionEnvironment used
		public StagPoint.Eval.Environment GetExpressionEnvironment(){
			return parent.GetExpressionEnvironment();
		}
#endif		

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		private const float PARAMS_TOP_MARGIN  = 5f;
		private const float PARAMS_LINE_HEIGHT = 18f;
		private const float PARAMS_LINE_MARGIN = 2f;
		private const float BOX_WIDTH          = 30f;

		private static CutsceneTrack _currentCurveViewingTrack;

		[SerializeField] [HideInInspector]
		private float _customHeight = 300f;

		private bool isResizingHeight = false;
		private float proposedHeight = 0f;
		private int inspectedParameterIndex = -1;
		/// @modify slate sequencer
	    /// @TQ
        public List<int> inspectedParameterIndexList = new List<int>();
        public bool refreshCurves = true;
        public bool CheckParameterMutiSelect()
        {
            return inspectedParameterIndexList.Count > 1;
        }
            
        /// end
        private object _icon;

        public int InspectedParameterIndex
        {
            set { inspectedParameterIndex = value; }
            get { return inspectedParameterIndex;  }
        }
		///The expansion height
		private float customHeight{
			get {return _customHeight;}
			set {_customHeight = Mathf.Clamp(value, proposedHeight, 500);}
		}

		///The final shown height
		public float finalHeight{
			get
			{
				if (showCurves){
                    return inspectedParameterIndex == -1? Mathf.Max(proposedHeight, defaultHeight + 50 ) : Mathf.Max(proposedHeight, customHeight)+ TrackHeightOffset;
				}
				return defaultHeight;
			}
		}

		//are curves shown?
		virtual public bool showCurves{
			get {return _currentCurveViewingTrack == this;}
			set {_currentCurveViewingTrack = value? this : null;}
		}

		///The default track height when not expanded
		virtual public float defaultHeight{
			get {return 30f;}
		}

		///Icon shown on left if any
		virtual public Texture icon{
			get
			{
				if (_icon == null){
					var att = this.GetType().RTGetAttribute<IconAttribute>(true);
					if (att != null){
						_icon = Resources.Load(att.iconName) as Texture;
						if (_icon == null){
							_icon = UnityEditor.EditorGUIUtility.FindTexture(att.iconName) as Texture;
						}
					}
					
					if (_icon == null){
						_icon = new object();
					}
				}

				return _icon as Texture;
			}
		}

		///Add an ActionClip to this Track
		public T AddAction<T>(float time) where T:ActionClip { return (T)AddAction(typeof(T), time); }
		public ActionClip AddAction(System.Type type, float time){

			var catAtt = type.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
			if (catAtt != null && actions.Count == 0){
				name = catAtt.category + " Track";
			}

			var newAction = UnityEditor.Undo.AddComponent(gameObject, type) as ActionClip;
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, "New Action");
			newAction.startTime = time;
			actions.Add(newAction);
			newAction.PostCreate(this);

			var nextAction = actions.FirstOrDefault(a => a.startTime > newAction.startTime);
			if (nextAction != null){
				newAction.endTime = Mathf.Min(newAction.endTime, nextAction.startTime);
			}

			root.Validate();
			CutsceneUtility.selectedObject = newAction;

			return newAction;
		}

/// @modify slate sequencer
/// add by TQ
        public ActionClip AddActionCustomer(System.Type type, float time)
        {

            var catAtt = type.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
            if (catAtt != null && actions.Count == 0)
            {
                name = catAtt.category + " Track";
            }

            var newAction = UnityEditor.Undo.AddComponent(gameObject, type) as ActionClip;
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "New Action");
            newAction.startTime = time;
            actions.Add(newAction);
            newAction.PostCreate(this);

            var nextAction = actions.FirstOrDefault(a => a.startTime > newAction.startTime);
            if (nextAction != null)
            {
                newAction.endTime = Mathf.Min(newAction.endTime, nextAction.startTime);
            }

            root.Validate();
            //CutsceneUtility.selectedObject = newAction;

            return newAction;
        }
/// end

        ///Remove an ActionClip from this Track
        public void DeleteAction(ActionClip action){
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Remove Action");
			actions.Remove(action);
			if ( ReferenceEquals(CutsceneUtility.selectedObject, action) ){
				CutsceneUtility.selectedObject = null;
			}
			UnityEditor.Undo.DestroyObjectImmediate(action);
			root.Validate();
		}
/// @modify slate sequencer
/// add by TQ
        public void WrapDeleteAction(ActionClip action)
        {
            if (action == null)
                return;     

            if (action is ActionClips.SubCutscene)
            {
                var pCutTrack = action.parent as CutsceneTrack;
                var pCut = pCutTrack.root as Cutscene;
                List<ActionClip> curCamActions = pCut.cameraTrackCustom != null?pCut.cameraTrackCustom.actions: new List<ActionClip>();
                List<ActionClip> linkClips = new List<ActionClip>();
                var subCut = (action as ActionClips.SubCutscene).cutscene;
                for (int i = 0; i < curCamActions.Count; i++)
                {
                    ActionClips.StoryEngineClip.StoryCameraClip cur = curCamActions[i] as ActionClips.StoryEngineClip.StoryCameraClip;
                    if (cur != null && cur.fromCutsence != null && ReferenceEquals(cur.fromCutsence, subCut)
                         && cur.linkClip != null && ReferenceEquals(cur.linkClip, action))
                    {
                        linkClips.Add(cur);
                    }
                }
                foreach (var act in linkClips)
                {
                    pCut.cameraTrackCustom.DeleteAction(act);
                }
                //if (subCut != null)
                //{
                //    GameObject.DestroyImmediate(subCut.gameObject);
                //}
            }
            DeleteAction(action);
        }
/// end
        ///The Editor GUI in the track info on the left
        virtual public void OnTrackInfoGUI(Rect trackRect){

			var e = Event.current;

			DoDefaultInfoGUI(e, trackRect);

			if (showCurves){
				var wasEnable = GUI.enabled;  
				GUI.enabled = true;
				DoParamsInfoGUI(e, trackRect, CutsceneUtility.selectedObject as IKeyable, CutsceneUtility.selectedObject is ActionClips.AnimateProperties);
				GUI.enabled = wasEnable;
			}

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
		}


		//default track info gui 
		protected void DoDefaultInfoGUI(Event e, Rect trackRect){

			var iconBGRect = new Rect(0, 0, BOX_WIDTH, defaultHeight);
			var textInfoRect = Rect.MinMaxRect(iconBGRect.xMax + 2, 0, trackRect.width - BOX_WIDTH - 2, defaultHeight);
			var curveButtonRect = new Rect(trackRect.width - BOX_WIDTH, 0, BOX_WIDTH, defaultHeight);
			// @modify slate sequencer
			// @TQ
            var checkBoxRect = new Rect(curveButtonRect.x - 30, 0, BOX_WIDTH, defaultHeight);

            GUI.backgroundColor = isHideChecked ? (UnityEditor.EditorGUIUtility.isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            GUI.Box(checkBoxRect, "", Styles.clipBoxStyle);
            GUI.color = isHideChecked ? new Color(1, 0.4f, 0.4f) : Color.white;
            if (GUI.Button(checkBoxRect, isHideChecked ? Styles.HideMIcon :Styles.DisplayMIcon, (GUIStyle)"box"))
            {
                //if (Prefs.showChoosenTrack)
                    isHideChecked = !isHideChecked;
            }
			///end
            GUI.backgroundColor = UnityEditor.EditorGUIUtility.isProSkin? new Color(0,0,0,0.7f) : new Color(0,0,0,0.2f);
			GUI.Box(iconBGRect, string.Empty);
			GUI.backgroundColor = Color.white;

			if (icon != null){
				var iconRect = new Rect(0,0,16,16);
				iconRect.center = iconBGRect.center;
				GUI.color = ReferenceEquals(CutsceneUtility.selectedObject, this)? Color.white : new Color(1,1,1,0.8f);
				GUI.DrawTexture(iconRect, icon);
				GUI.color = Color.white;
			}


			var nameString = string.Format("<size=11>{0}</size>", name);
			var infoString = string.Format("<size=9><color=#707070>{0}</color></size>", info);
			GUI.color = isActive? Color.white : Color.grey;
			GUI.Label(textInfoRect, string.Format("{0}\n{1}", nameString, infoString));			
			GUI.color = Color.white;

            var wasEnable = GUI.enabled;  
			GUI.enabled = true;
			var curveIconRect = new Rect(0,0,16,16);
			curveIconRect.center = curveButtonRect.center - new Vector2(0,1);
			var curveIconColor = UnityEditor.EditorGUIUtility.isProSkin? Color.white : Color.black;
			curveIconColor.a = showCurves? 1 : 0.3f;

           
            if (GUI.Button(curveButtonRect, string.Empty, GUIStyle.none)){
				showCurves = !showCurves;
			}


			curveButtonRect.xMin += 4;
			curveButtonRect.xMax -= 4;
			curveButtonRect.yMin += 4;
			curveButtonRect.yMax -= 4;
			GUI.color = new Color(0.2f,0.2f,0.2f,0.1f);
			GUI.Box(curveButtonRect, "", Styles.clipBoxStyle);

			GUI.color = curveIconColor;
			GUI.DrawTexture(curveIconRect, showCurves? Styles.curveIcon1 : Styles.curveIcon);

			GUI.color = UnityEditor.EditorGUIUtility.isProSkin? Color.grey : Color.grey;
			if (!isActive){
				var hiddenRect = new Rect(0,0,16,16);
				hiddenRect.center = curveButtonRect.center - new Vector2(curveButtonRect.width, 0);
				if (GUI.Button(hiddenRect, Styles.hiddenIcon, GUIStyle.none)){ /*isActive = !isActive;*/ }
			}

			if (isLocked){
				var lockRect = new Rect(0,0,16,16);
				lockRect.center = curveButtonRect.center - new Vector2(curveButtonRect.width, 0);
				if (!isActive){
					lockRect.center -= new Vector2(16, 0);
				}
				if (GUI.Button(lockRect, Styles.lockIcon, GUIStyle.none)){ /*isLocked = !isLocked;*/ }
			}


			GUI.color = Color.white;
			GUI.enabled = wasEnable;
		}

		//show selected clip animated parameters list info
		protected void DoParamsInfoGUI(Event e, Rect trackRect, IKeyable keyable, bool showAddPropertyButton){
			
			//bg graphic
			var expansionRect = Rect.MinMaxRect(5, defaultHeight, trackRect.width - 3, finalHeight - 3);
			GUI.color = UnityEditor.EditorGUIUtility.isProSkin? new Color(0.22f, 0.22f, 0.22f) : new Color(0.7f, 0.7f, 0.7f);
			GUI.DrawTexture(expansionRect, Styles.whiteTexture);
			GUI.color = new Color(0,0,0,0.05f);
			GUI.Box(expansionRect, string.Empty, Styles.shadowBorderStyle);
			GUI.color = Color.white;


			//allow resize height
			if (inspectedParameterIndex >= 0){
				var resizeRect = Rect.MinMaxRect(0, finalHeight - 4, trackRect.width, finalHeight);
				UnityEditor.EditorGUIUtility.AddCursorRect(resizeRect, UnityEditor.MouseCursor.ResizeVertical);
				GUI.color = Color.grey;
				GUI.DrawTexture(resizeRect, Styles.whiteTexture);
				GUI.color = Color.white;
				if (e.type == EventType.MouseDown && e.button == 0 && resizeRect.Contains(e.mousePosition)){ isResizingHeight = true; e.Use(); }
				if (e.type == EventType.MouseDrag && isResizingHeight){ customHeight += e.delta.y; }
				if (e.rawType == EventType.MouseUp){ isResizingHeight = false; }
			}

			proposedHeight = 0f;

			if ( ((keyable == null) || !ReferenceEquals(keyable.parent, this)) && !ReferenceEquals(keyable, this) ){
				GUI.Label(expansionRect, "No Clip Selected", Styles.centerLabel);
				inspectedParameterIndex = -1;
				return;				
			}

			if (!showAddPropertyButton){
				if (keyable is ActionClip && !(keyable as ActionClip).isValid){
					GUI.Label(expansionRect, "Clip Is Invalid", Styles.centerLabel);
					return;
				}

				if (keyable.animationData == null || !keyable.animationData.isValid){
					if (keyable is ActionClip){
						GUI.Label(expansionRect, "Clip Has No Animatable Parameters", Styles.centerLabel);
						return;
					}
				}
			}

			proposedHeight = defaultHeight + PARAMS_TOP_MARGIN;
			if (keyable.animationData != null && keyable.animationData.animatedParameters != null){
				if (inspectedParameterIndex >= keyable.animationData.animatedParameters.Count){
					inspectedParameterIndex = -1;
				}

				var paramsCount = keyable.animationData.animatedParameters.Count;
				for (var i = 0; i < paramsCount; i++){
					var animParam = keyable.animationData.animatedParameters[i];
                    // @modify slate sequencer
                    // @TQ
                    if (this is PropertiesTrack)
                    {
                        //var pTrack = this as PropertiesTrack;
                        if (!Prefs.showChoosenTrack && (animParam.hideChecked))
                            continue;
                    }
                    //end
                    var paramRect = new Rect(expansionRect.xMin + 4, proposedHeight, expansionRect.width - 8, PARAMS_LINE_HEIGHT );
					proposedHeight += PARAMS_LINE_HEIGHT + PARAMS_LINE_MARGIN;
                    var paramSelected = inspectedParameterIndex == i;
                    var paramMutiSelected = (inspectedParameterIndexList.Contains(i) && CheckParameterMutiSelect());
                    paramSelected &= !paramMutiSelected;
                    GUI.color = paramSelected ? new Color(0.5f, 0.5f, 1f, 0.4f) : paramMutiSelected ? new Color(229f/255f, 228f/255f,172f/255f ): new Color(0, 0.5f, 0.5f, 0.5f);
					GUI.Box(paramRect, string.Empty, Styles.headerBoxStyle);
					GUI.color = Color.white;
					
					var paramName = string.Format(" <size=10><color=#252525>{0}</color></size>", animParam.ToString() );
					paramName = inspectedParameterIndex == i? string.Format("<b>{0}</b>", paramName) : paramName;
					GUI.Label(paramRect, paramName, Styles.leftLabel);

					var gearRect = new Rect(paramRect.xMax - 16 - 4, paramRect.y, 16, 16);
					// @modify slate sequencer
					// @TQ
                    var hideRect = new Rect(gearRect.x - 16 , paramRect.y, 16, 16);
                    gearRect.center = new Vector2(gearRect.center.x, paramRect.y + (paramRect.height/2) - 1);

                    var colorCache = GUI.backgroundColor;
                    GUI.backgroundColor = animParam.hideChecked ? (UnityEditor.EditorGUIUtility.isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    GUI.Box(hideRect, "", Styles.clipBoxStyle);
                    GUI.color = animParam.hideChecked ? new Color(1, 0.4f, 0.4f) : Color.white;
                    if (GUI.Button(hideRect, animParam.hideChecked ? Styles.HideMIcon :Styles.DisplayMIcon, (GUIStyle)"box"))
                    {
                       // if (Prefs.showChoosenTrack)
                            animParam.hideChecked = !animParam.hideChecked;
                    }
                    GUI.color = Color.white;
                    GUI.backgroundColor = colorCache;
					//end
                    GUI.enabled = animParam.enabled;
					if (GUI.Button(gearRect, Styles.gearIcon, GUIStyle.none)){
						AnimatableParameterEditor.DoParamGearContextMenu(animParam, keyable);
					}
					if (GUI.Button(paramRect, string.Empty, GUIStyle.none)){
                        /// @modify slate sequencer
                        /// @TQ
                        if (e.control)
                        {
                            if (!inspectedParameterIndexList.Contains(i))
                            {
                                if (inspectedParameterIndex != i)
                                    inspectedParameterIndexList.Add(i);
                            }
                            else
                            {
                                inspectedParameterIndexList.Remove(i);
                            }
                        }
                        else if (e.shift)
                        {
                            if (inspectedParameterIndexList.Count > 0)
                            {
                                var pickedFirstDummy = inspectedParameterIndexList[0];
                                var pickedLastDummy = inspectedParameterIndexList[inspectedParameterIndexList.Count - 1];
                                

                                int minIdx = Mathf.Min(pickedFirstDummy, pickedLastDummy);
                                int maxIdx = Mathf.Max(pickedFirstDummy, pickedLastDummy);

                                if (i >= minIdx)
                                {
                                    maxIdx = i;
                                }
                                if (i < minIdx)
                                {
                                    minIdx = i;
                                }

                                inspectedParameterIndexList.Clear();
                                for (int idx = 0; idx < paramsCount; idx++)
                                {
                                    if (idx >= minIdx && idx <= maxIdx)
                                    {
                                        var groupDummy = idx;
                                        if (!inspectedParameterIndexList.Contains(groupDummy))
                                            inspectedParameterIndexList.Add(groupDummy);
                                    }

                                }


                            }

                        }
                        else
                        {

                            inspectedParameterIndexList.Clear();
                            if (inspectedParameterIndexList.Count == 0)
                                inspectedParameterIndexList.Add(i);
                            inspectedParameterIndex = inspectedParameterIndex == i ? -1 : i;
                            //inspectedParameterIndex = inspectedParameterIndex == i ? -1 : i;
                            //if (inspectedParameterIndexList.Contains(i))
                            //{
                            //    inspectedParameterIndexList.Remove(i);
                            //}
                            //if (inspectedParameterIndex == -1)
                            //{
                            //    inspectedParameterIndexList.Clear();
                            //}
                        }
                        this.refreshCurves = false;
                        CurveEditor.FrameAllCurvesOf(animParam);
                    }
                    ///end
					GUI.enabled = true;
                }

				proposedHeight += PARAMS_TOP_MARGIN;

				if (inspectedParameterIndex >= 0){
					var controlRect = Rect.MinMaxRect(expansionRect.x + 6, proposedHeight + 5, expansionRect.xMax - 6, proposedHeight + 50);
					var animParam = keyable.animationData.animatedParameters[inspectedParameterIndex];
					GUILayout.BeginArea(controlRect);
                    var chooseTrack = CutsceneUtility.selectedObject as CutsceneTrack;
                    bool isCheckThisTrack = false;
                    if (chooseTrack != null && chooseTrack == this)
                        isCheckThisTrack = true;
                    AnimatableParameterEditor.ShowMiniParameterKeyControls(animParam, keyable, isCheckThisTrack);
					GUILayout.EndArea();
					proposedHeight = controlRect.yMax + 10;
				}
			}

			if ( showAddPropertyButton && inspectedParameterIndex == -1){
				var buttonRect = Rect.MinMaxRect(expansionRect.x + 6, proposedHeight + 5, expansionRect.xMax - 38, proposedHeight + 25);
                var go = keyable.animatedParametersTarget as GameObject;
				GUI.enabled = go != null && root.currentTime <= 0;
                if (GUI.Button(buttonRect, "Add Property"))
                {
                    /// @modify slate sequencer
                    /// @CYS 
                    EditorTools.ShowAnimatedPropertySelectionMenu(go, AnimatedParameter.supportedTypes, (prop, comp, path, traces) =>
                    {
                        keyable.animationData.TryAddParameter(prop, keyable, comp.transform, go.transform, traces);
                        ///End
                        /// @modify slate sequencer
                        /// @TQ
                        /// add by TQ 
                        keyable.animationData.SortAnimatedParametersList();
                        ///end
					    /// @modify slate sequencer
                        /// @TQ
                        if (AddPropertyToPickGroupsFunc != null)
                            AddPropertyToPickGroupsFunc(prop, comp.transform);
                        /// end

                    });
                }
               	// @modify slate sequencer
				// @TQ			
                var hideRect = Rect.MinMaxRect(buttonRect.x + buttonRect.width + 1, proposedHeight + 5, expansionRect.xMax - 2, proposedHeight + 25);
                var BgColorCache = GUI.backgroundColor;
                GUI.backgroundColor = isHideChecked ? (UnityEditor.EditorGUIUtility.isProSkin ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
                GUI.Box(hideRect, "", Styles.clipBoxStyle);
                GUI.color = isHideChecked ? new Color(1, 0.4f, 0.4f) : Color.white;
                if (GUI.Button(hideRect, isHideChecked ? Styles.HideMIcon: Styles.DisplayMIcon, (GUIStyle)"box"))
                {
                    //if (Prefs.showChoosenTrack)
                        isHideChecked = !isHideChecked;
                }
               
                GUI.backgroundColor = BgColorCache;
				///end
                GUI.enabled = true;
                proposedHeight = buttonRect.yMax + 10;
            }

			//consume event
			if (e.type == EventType.MouseDown && expansionRect.Contains(e.mousePosition)){
				e.Use();
			}	
		}



		///The Editor GUI within the timeline rectangle
		virtual public void OnTrackTimelineGUI(Rect posRect, Rect timeRect, float cursorTime, System.Func<float, float> TimeToPos, Rect? selectRect = null){
			var e = Event.current;

			var clipsPosRect = Rect.MinMaxRect(posRect.xMin, posRect.yMin, posRect.xMax, posRect.yMin + defaultHeight);
			DoTrackContextMenu(e, clipsPosRect, cursorTime);

			if (showCurves){
				var curvesPosRect = Rect.MinMaxRect(posRect.xMin, clipsPosRect.yMax, posRect.xMax, posRect.yMax);
				DoClipCurves(e, curvesPosRect, timeRect, TimeToPos, CutsceneUtility.selectedObject as IKeyable, null, selectRect);
			}
		}


        /// @modify slate sequencer
        /// @hushuang
        /// enable override DoTrackContextMenu for children
        virtual protected void DoTrackContextMenu(Event e, Rect clipsPosRect, float cursorTime){
            // @modify slate sequencer
            // @TQ
            // DragFileToAddClip function 			
#if UNITY_EDITOR
            if (DragFileAddClipFunc != null)
            {
                DragFileAddClipFunc(this, clipsPosRect);
            }
#endif
            ///end modify slate sequencer
			if (e.type == EventType.ContextClick && clipsPosRect.Contains(e.mousePosition)){

				var attachableTypeInfos = new List<EditorTools.TypeMetaInfo>();
				
				var existing = actions.FirstOrDefault();
				var existingCatAtt = existing != null? existing.GetType().GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute : null;
				foreach (var info in EditorTools.GetTypeMetaDerivedFrom(typeof(ActionClip))){

					if (!info.attachableTypes.Contains(this.GetType())){
						continue;
					}

					if (existingCatAtt != null){
						if (existingCatAtt.category == info.category){
							attachableTypeInfos.Add(info);
						}
					} else {
						attachableTypeInfos.Add(info);
					}
				}

				if (attachableTypeInfos.Count > 0){
					var menu = new UnityEditor.GenericMenu();
					foreach (var _info in attachableTypeInfos){
						var info = _info;
						var category = string.IsNullOrEmpty(info.category)? string.Empty : (info.category + "/");
						var tName = info.name;
						menu.AddItem(new GUIContent(category + tName), false, ()=> { AddAction(info.type, cursorTime); } );
					}

					var copyType = CutsceneUtility.GetCopyType();
					if (copyType != null && attachableTypeInfos.Select(i => i.type).Contains(copyType) ){
						menu.AddSeparator("/");
/// @modify slate sequencer
/// add by TQ
						menu.AddItem(new GUIContent( string.Format("Paste Clip ({0})", copyType.Name) ), false, ()=>{
                            if (copyType.Name == "SubCutscene") 
                            {
                                CutsceneUtility.PasteSubcutsenceClip(this, cursorTime);
                            }
                            else
                            {
                                CutsceneUtility.PasteClip(this, cursorTime);
                            }
                            DopeSheetEditor.PasteKeysSelectedOutSide(cursorTime);
                        });
					}

                    var copyMultiClips = CutsceneUtility.hasMutiClipsCut();
                    if (copyMultiClips && copyType == null)
                    {
                        menu.AddSeparator("/");
                        menu.AddItem(new GUIContent(string.Format("Paste Clips")), false, () => {
                            CutsceneUtility.PasteClips(cursorTime);
                            DopeSheetEditor.PasteKeysSelectedOutSide(cursorTime);
                        });

                        menu.AddSeparator("/");
                        menu.AddItem(new GUIContent(string.Format("Paste Clips(OneTrack)")), false, () =>
                        {
                            CutsceneUtility.PasteClips(cursorTime, this);
                            DopeSheetEditor.PasteKeysSelectedOutSide(cursorTime);
                        });
                    }
/// end
                    menu.ShowAsContext();
					e.Use();
				}
			}
		}

		protected void DoClipCurves(Event e, Rect posRect, Rect timeRect, System.Func<float, float> TimeToPos, IKeyable keyable, float? offset = null, Rect? selectRect = null){

            if (selectRect != null)
            {
                offset = selectRect.Value.x - posRect.x;
            }
            else
            {
                offset = 0;
            }
           
            //track expanded bg
            GUI.color = new Color(0,0,0,0.08f);
			GUI.Box(posRect, string.Empty, Styles.timeBoxStyle);
			GUI.color = Color.white;

			if ( ((keyable == null) || !ReferenceEquals(keyable.parent, this)) && !ReferenceEquals(keyable, this) ){
				GUI.color = new Color(1,1,1,0.3f);
				GUI.Label(posRect, "Select a Clip of this Track to view it's Animated Parameters here", Styles.centerLabel);					
				GUI.color = Color.white;
				return;
			}

			var finalPosRect = posRect;
			var finalTimeRect = timeRect;

			//adjust rects
			if (keyable is ActionClip){
				finalPosRect.xMin = Mathf.Max(posRect.xMin, TimeToPos(keyable.startTime));
				finalPosRect.xMax = Mathf.Min(posRect.xMax, TimeToPos(keyable.endTime));
				finalTimeRect.xMin = Mathf.Max(timeRect.xMin, keyable.startTime) - keyable.startTime;
				finalTimeRect.xMax = Mathf.Min(timeRect.xMax, keyable.endTime) - keyable.startTime;
			}

			//add some top/bottom margins
			finalPosRect.yMin += 1;
			finalPosRect.yMax -= 3;
			finalPosRect.width = Mathf.Max(finalPosRect.width, 5);

			//dark bg
			GUI.color = new Color(0.1f,0.1f,0.1f,0.5f);
			GUI.DrawTexture(posRect, Styles.whiteTexture);
			GUI.color = Color.white;


			//out of view range
			if (keyable is ActionClip){
				if (keyable.startTime > timeRect.xMax || keyable.endTime < timeRect.xMin){
					return;
				}
			}


			//keyable bg
			GUI.color = UnityEditor.EditorGUIUtility.isProSkin? new Color(0.25f,0.25f,0.25f, 0.9f) : new Color(0.7f, 0.7f, 0.7f, 0.9f);
			GUI.Box(finalPosRect, string.Empty, Styles.clipBoxFooterStyle);
			GUI.color = Color.white;
			
			//if too small do nothing more
			if (finalPosRect.width <= 5){
				return;
			}

			if (keyable is ActionClip && !(keyable as ActionClip).isValid){
				GUI.Label(finalPosRect, "Clip Is Invalid", Styles.centerLabel);
				return;
			}

			if (keyable.animationData == null || !keyable.animationData.isValid){
				if (keyable is ActionClip){
					GUI.Label(finalPosRect, "Clip has no Animatable Parameters", Styles.centerLabel);
				} else {
					GUI.Label(finalPosRect, "Track has no Animated Properties. You can add some on the left side", Styles.centerLabel);
				}
				return;
			}

			if (inspectedParameterIndex >= keyable.animationData.animatedParameters.Count){
				inspectedParameterIndex = -1;
			}


			//vertical guides from params to dopesheet
			if (inspectedParameterIndex == -1){
				var yPos = PARAMS_TOP_MARGIN;
				for (var i = 0; i < keyable.animationData.animatedParameters.Count; i++){
					// var animParam = keyable.animationData.animatedParameters[i];
					var paramRect = new Rect(0, posRect.yMin + yPos, finalPosRect.xMin - 2, PARAMS_LINE_HEIGHT);
					yPos += PARAMS_LINE_HEIGHT + PARAMS_LINE_MARGIN;
					paramRect.yMin += 1f;
					paramRect.yMax -= 1f;
					GUI.color = new Color(0, 0.5f, 0.5f, 0.1f);
					GUI.DrawTexture(paramRect, Styles.whiteTexture);
					GUI.color = Color.white;
				}
			}


			//begin in group and neutralize rect
			GUI.BeginGroup(finalPosRect);
			finalPosRect = new Rect(0, 0, finalPosRect.width, finalPosRect.height);

			if (inspectedParameterIndex == -1){
				var yPos = PARAMS_TOP_MARGIN;
				for (var i = 0; i < keyable.animationData.animatedParameters.Count; i++){
					var animParam = keyable.animationData.animatedParameters[i];
					/// @modify slate sequencer
                    /// @TQ
                    if (!Prefs.showChoosenTrack && animParam.hideChecked)
                    {
                        continue;
                    }
                    /// end
                    var paramRect = new Rect(finalPosRect.xMin, finalPosRect.yMin + yPos, finalPosRect.width, PARAMS_LINE_HEIGHT);
					yPos += PARAMS_LINE_HEIGHT + PARAMS_LINE_MARGIN;
					paramRect.yMin += 1f;
					paramRect.yMax -= 1f;
					GUI.color = new Color(1,1,1,0.3f);
					GUI.Box(paramRect, string.Empty);
                    GUI.color = Color.white;
                   
                    if (animParam.enabled)
                    {
                        Rect selectRectDumy = new Rect();
                        Rect tempDummy = new Rect();
                        if (selectRect != null)
                        {
                            selectRectDumy = paramRect;
                            selectRectDumy.y = paramRect.y + posRect.y;
                            tempDummy = selectRect.Value;
                            tempDummy.x = offset.Value;
                        }
                          
                        if (selectRect!= null && CutsceneUtility.AIntersectAndEncapsulatesB(tempDummy, selectRectDumy) /*paramRect.y + posRect.y > selectRect.Value.y && paramRect.y + posRect.y < + selectRect.Value.y + selectRect.Value.height*/)
                        {
                            DopeSheetEditor.DrawDopeSheet(animParam, keyable, paramRect, finalTimeRect.x, finalTimeRect.width, true, offset, offset  + selectRect.Value.width, MagnetSnapInterval);
                        }
                        else
                        {
                            DopeSheetEditor.DrawDopeSheet(animParam, keyable, paramRect, finalTimeRect.x, finalTimeRect.width, true);
                        }              
                    } else {
						GUI.color = new Color(0,0,0,0.2f);
						GUI.DrawTextureWithTexCoords(paramRect, Styles.stripes, new Rect(0,0, paramRect.width/7, paramRect.height/7));
						GUI.color = Color.white;						
					}
				}
			}

			if (inspectedParameterIndex >= 0){
                var animParam = keyable.animationData.animatedParameters[inspectedParameterIndex];
                var dopeRect = finalPosRect;
                dopeRect.y += 4f;
                dopeRect.height = 16f;
				GUI.color = new Color(1,1,1,0.3f);
                GUI.Box(dopeRect, string.Empty);
                GUI.color = Color.white;
                DopeSheetEditor.DrawDopeSheet(animParam, keyable, dopeRect, finalTimeRect.x, finalTimeRect.width, true, null, null, MagnetSnapInterval);
                var curveRect = finalPosRect;
                curveRect.yMin = dopeRect.yMax + 4;
				/// @modify slate sequencer
		    	/// @TQ
                if (inspectedParameterIndexList.Count > 0)
                {
                    CurveEditor.DrawCurvesMutiply(animParam, keyable.animationData.animatedParameters, inspectedParameterIndexList , keyable, curveRect, finalTimeRect, this);
                }
                else
                {
                    CurveEditor.DrawCurves(animParam, keyable, curveRect, finalTimeRect, this);
                }
				/// end
            }

			//consume event
			if (e.type == EventType.MouseDown && finalPosRect.Contains(e.mousePosition)){
				//e.Use();
			}

			GUI.EndGroup();

/*
			//darken out of clip range time
			//will use if I make curve editing full-width
			if (Prefs.fullWidthCurveEditing){
				var darkBefore = Rect.MinMaxRect( posRect.xMin, posRect.yMin, Mathf.Max(posRect.xMin, TimeToPos(keyable.startTime)), posRect.yMax );
				var darkAfter = Rect.MinMaxRect( Mathf.Min(posRect.xMax, TimeToPos(keyable.endTime)), posRect.yMin, posRect.xMax, posRect.yMax );
				GUI.color = new Color(0.1f,0.1f,0.1f,0.6f);
				GUI.DrawTexture(darkBefore, Styles.whiteTexture);
				GUI.DrawTexture(darkAfter, Styles.whiteTexture);
				GUI.color = Color.white;
			}
*/
			
		}

#endif

        public void BakeAllClip(float updateFrameRate = 30.0f)
        {
            var lastTime = -1f;
            var updateInterval = (1f / updateFrameRate);
            OnEnter();
            for (var i = startTime; i <= endTime + updateInterval; i += updateInterval)
            {
                OnUpdate(i - startTime, i - startTime - updateInterval);

                foreach (var clip in (this as IDirectable).children)
                {

                    if (i >= clip.startTime && lastTime < clip.startTime)
                    {
                        clip.Enter();
                    }

                    if (i >= clip.startTime && i <= clip.endTime)
                    {
                        clip.Update(i - clip.startTime, i - clip.startTime - updateInterval);
                    }

                    if ((i > clip.endTime || i >= this.endTime) && lastTime <= clip.endTime)
                    {
                        clip.Exit();
                    }
                }

                lastTime = i;
            }
            OnExit();
        }

        public void BakeEnter()
        {
            OnEnter();
        }
    }
}