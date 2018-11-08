using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slate{

	[Attachable(typeof(ActionTrack))]
	///Clips are added in CutsceneTracks to make stuff happen
	abstract public class ActionClip : MonoBehaviour, IDirectable, IKeyable {

		[SerializeField] [HideInInspector]
		private float _startTime;
		[SerializeField] [HideInInspector]
		private AnimationDataCollection _animationData;
		
		public IDirector root{ get {return parent != null? parent.root : null;} }
		public IDirectable parent{ get; private set; }
		public GameObject actor{ get {return parent != null? parent.actor : null;} }
		IEnumerable<IDirectable> IDirectable.children{ get {return null;} }

        /// @modify slate sequencer
		/// add by TQ
        /// add drag to add drag Clip function  
        public delegate void onDragClipFunc(List<float> currentTimes, float deltaTime);
        public onDragClipFunc onDragClipDelegate = null;
        //


        ///All animated parameters are stored within this collection object
        public AnimationDataCollection animationData{
			get {return _animationData;}
			private set {_animationData = value;}
		}

        ///...
        // @modify slate sequencer
        // @hushuang
        // change this to virtual, support override the startTime of the ActionClip
        virtual public float startTime{
			get {return _startTime;}
			set
			{
				if (_startTime != value){
					_startTime = Mathf.Max(value, 0);
					blendIn = Mathf.Clamp(blendIn, 0, length - blendOut);
					blendOut = Mathf.Clamp(blendOut, 0, length - blendIn);
				}
			}
		}

        ///...
        /// @modify slate sequencer
        // @hushuang
        // change this to virtual, support override the endTime of the ActionClip
        virtual public float endTime{
			get {return startTime + length;}
			set
			{
				if (startTime + length != value){
					length = Mathf.Max(value - startTime, 0);
					blendOut = Mathf.Clamp(blendOut, 0, length - blendIn);
					blendIn = Mathf.Clamp(blendIn, 0, length - blendOut);
				}
			}
		}

		///...
		public bool isActive{
			get {return parent != null? parent.isActive && isValid : false;}
		}

		///...
		public bool isCollapsed{
			get {return parent != null? parent.isCollapsed : false;}
		}

		//...
		public bool isLocked{
			get {return parent != null? parent.isLocked : false;}
		}

		///The length of the clip.
		///Override for scalable clips.
		virtual public float length{
			get {return 0;}
			set {}
		}

		///The blend in value of the clip. A value of zero means instant.
		///Override for blendable in clips.
		virtual public float blendIn{
			get {return 0;}
			set {}
		}

		///The blend out value of the clip. A value of zero means instant.
		///Override for blendable out clips.
		virtual public float blendOut{
			get {return 0;}
			set {}
		}

		///A short summary.
		///Overide this to show something specific in the action clip in the editor.
		virtual public string info{
			get
			{
				var nameAtt = this.GetType().RTGetAttribute<NameAttribute>(true);
				if (nameAtt != null){
					return nameAtt.name;
				}
				return this.GetType().Name.SplitCamelCase();
			}
		}

		///Is everything ok for the clip to work?
		virtual public bool isValid{
			get { return actor != null; }
		}

		virtual public TransformSpace defaultTransformSpace{
			get {return TransformSpace.WorldSpace;}
		}

		//An array of properties/fields that will be possible to animate.
		//By default all properties/fields in the actionclip class with an [AnimatableParameter] attribute will be used.
		private MemberInfo[] _cachedParamsInfo;
		private MemberInfo[] animatedParametersInfo{
			get { return _cachedParamsInfo != null? _cachedParamsInfo : _cachedParamsInfo = this.GetType().RTGetPropsAndFields().Where( p => p.RTGetAttribute<AnimatableParameterAttribute>(true) != null).ToArray(); }
		}

		//If the params target is not this, registration of parameters should be handled manually
		private bool handleParametersRegistrationManually{
			get { return !ReferenceEquals(animatedParametersTarget, this); }
		}

		///The target instance of the animated properties/fields.
		///By default the instance of THIS action clip is used.
		///Do NOT override if you don't know why! :)
		virtual public object animatedParametersTarget{
			get { return this; }
		}

		///Whether or not clip weight will be used in parameters automatically.
		virtual public bool useWeightInParameters{
			get {return false;}
		}

		///Does the clip has any parameters?
		public bool hasParameters{
			get {return animationData != null && animationData.isValid;}
		}

		///Does the clip has any active parameters?
		public bool hasActiveParameters{
			get
			{
				if (!hasParameters || !isValid){ return false; }
				for (var i = 0; i < animationData.animatedParameters.Count; i++){
					if (animationData.animatedParameters[i].enabled){
						return true;
					}
				}
				return false;
			}
		}

		bool IDirectable.Initialize(){ return OnInitialize(); }
		void IDirectable.Enter(){ SetAnimParamsSnapshot(); OnEnter(); }
		void IDirectable.Update(float time, float previousTime){ UpdateAnimParams(time, previousTime); OnUpdate(time, previousTime); }
		void IDirectable.Exit(){ OnExit(); }
		void IDirectable.ReverseEnter(){ OnReverseEnter(); }
		void IDirectable.Reverse(){ RestoreAnimParamsSnapshot(); OnReverse(); }
		
		void IDirectable.RootEnabled(){ OnRootEnabled(); }
		void IDirectable.RootDisabled(){ OnRootDisabled(); }
		void IDirectable.RootUpdated(float time, float previousTime){ OnRootUpdated(time, previousTime); }
		

#if UNITY_EDITOR			
		void IDirectable.DrawGizmos(bool selected){
			if (selected && actor != null && isValid){
				OnDrawGizmosSelected();
			}
		}
		
		private Dictionary<MemberInfo, Attribute[]> paramsAttributes = new Dictionary<MemberInfo, Attribute[]>();
		void IDirectable.SceneGUI(bool selected){
			
			if (!selected || actor == null || !isValid){
				return;
			}

			if (hasParameters){

				for (var i = 0; i < animationData.animatedParameters.Count; i++){
					var animParam = animationData.animatedParameters[i];
					if (!animParam.isValid || animParam.animatedType != typeof(Vector3)){
						continue;
					}
					var m = animParam.GetMemberInfo();
					Attribute[] attributes = null;
					if (!paramsAttributes.TryGetValue(m, out attributes)){
						attributes = (Attribute[])m.GetCustomAttributes(false);
						paramsAttributes[m] = attributes;
					}

					ITransformableHelperParameter link = null;
					var animAtt = attributes.FirstOrDefault(a => a is AnimatableParameterAttribute) as AnimatableParameterAttribute;
					if (animAtt != null){ //only in case parameter has been added manualy. Probably never.
						if (!string.IsNullOrEmpty(animAtt.link)){
							try {link = (GetType().GetField(animAtt.link).GetValue(this) as ITransformableHelperParameter);}
							catch (Exception exc) {Debug.LogError(exc.Message);}
						}
					}

					if (link == null || link.useAnimation){

						var space = link != null? link.space : defaultTransformSpace;

						var posHandleAtt = attributes.FirstOrDefault(a => a is PositionHandleAttribute) as PositionHandleAttribute;
						if (posHandleAtt != null){
							DoParameterPositionHandle(animParam, space);
						}

						var trajAtt = attributes.FirstOrDefault(a => a is ShowTrajectoryAttribute) as ShowTrajectoryAttribute;
						if (trajAtt != null && animParam.enabled){
							CurveEditor3D.Draw3DCurve(animParam, this, GetSpaceTransform(space), length/2, length);
						}
					}
				}
			}

			OnSceneGUI();
		}

#pragma warning disable 612, 618 //for handles. will fix
		protected void DoParameterPositionHandle(AnimatedParameter animParam, TransformSpace space){
			UnityEditor.EditorGUI.BeginChangeCheck();
			var originalPos = (Vector3)animParam.GetCurrentValue();
			var pos = TransformPoint( originalPos, space );
			var newPos = UnityEditor.Handles.PositionHandle(pos, Quaternion.identity);
			newPos = InverseTransformPoint(newPos, space);
			UnityEditor.Handles.SphereCap(-10, pos, Quaternion.identity, 0.1f);
			if (UnityEditor.EditorGUI.EndChangeCheck()){
				UnityEditor.Undo.RecordObject(this, "Position Change");
				if (RootTimeWithinRange()){
					if (!Event.current.shift){
						animParam.SetCurrentValue( newPos );
					} else {
						animParam.OffsetValue(newPos - originalPos);
					}
				} else {
					animParam.SetCurrentValue( newPos );
					animParam.OffsetValue(newPos - originalPos);
				}

				UnityEditor.EditorUtility.SetDirty(this);
			}			
		}

		protected void DoVectorPositionHandle(TransformSpace space, ref Vector3 position){
			UnityEditor.EditorGUI.BeginChangeCheck();
			var pos = TransformPoint(position, space);
			var newPos = UnityEditor.Handles.PositionHandle(pos, Quaternion.identity);
			UnityEditor.Handles.SphereCap(-10, pos, Quaternion.identity, 0.1f);
			if (UnityEditor.EditorGUI.EndChangeCheck()){
				UnityEditor.Undo.RecordObject(this, "Parameter Change");
				position = InverseTransformPoint(newPos, space);
				UnityEditor.EditorUtility.SetDirty(this);
			}			
		}
#pragma warning restore 612, 618 

#endif


		virtual protected bool OnInitialize(){return true;}
		virtual protected void OnEnter(){}
		virtual protected void OnUpdate(float time, float previousTime){OnUpdate(time);}
		virtual protected void OnUpdate(float time){}
		virtual protected void OnExit(){}
		virtual protected void OnReverse(){}
		virtual protected void OnReverseEnter(){}
		virtual protected void OnDrawGizmosSelected(){}
		virtual protected void OnSceneGUI(){}
		virtual protected void OnCreate(){}
		virtual protected void OnAfterValidate(){}

		virtual protected void OnRootEnabled(){}
		virtual protected void OnRootDisabled(){}
		virtual protected void OnRootUpdated(float time, float previousTime){}

		///After creation
		public void PostCreate(IDirectable parent){
			this.parent = parent;
			CreateAnimationDataCollection();
			OnCreate();
		}

		//Validate the clip
		public void Validate(){OnAfterValidate();}
		public void Validate(IDirector root, IDirectable parent){
			this.parent = parent;
			ValidateAnimParams();
			hideFlags = HideFlags.HideInHierarchy;
			OnAfterValidate();
		}

		///Is the root time within clip time range? A helper method.
		public bool RootTimeWithinRange(){
			return root.currentTime >= startTime && root.currentTime <= endTime && root.currentTime > 0;
		}

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

		///Returns the previous clip in parent track
		public ActionClip GetPreviousClip(){
			return parent.children.LastOrDefault(c => c.startTime < this.startTime) as ActionClip;
		}

		///Returns the next clip in parent track
		public ActionClip GetNextClip(){
			return parent.children.FirstOrDefault(c => c.startTime > this.startTime) as ActionClip;
		}

		///The current clip weight based on blend properties and based on root current time.
		public float GetClipWeight(){ return GetClipWeight(root.currentTime - startTime); }
		///The weight of the clip at specified local time based on its blend properties.
		public float GetClipWeight(float time){ return GetClipWeight(time, this.blendIn, this.blendOut); }
		///The weight of the clip at specified local time based on provided override blend in/out properties
		public float GetClipWeight(float time, float blendInOut){ return GetClipWeight(time, blendInOut, blendInOut); }
		public float GetClipWeight(float time, float blendIn, float blendOut){

			if (time <= 0){
				return blendIn <= 0? 1 : 0;
			}

			if (time >= length){
				return blendOut <= 0? 1 : 0;
			}

			if (time < blendIn){
				return time/blendIn;
			}

			if (time > length - blendOut){
				return (length - time)/blendOut;
			}

			return 1;			
		}

		///Get the AnimatedParameter of name. The name is usually the same as the field/property name that [AnimatableParameter] is used on.
		public AnimatedParameter GetParameter(string paramName){
			return animationData != null? animationData.GetParameterOfName(paramName) : null;
		}

		///Enable/Disable an AnimatedParameter of name
		public void SetParameterEnabled(string paramName, bool enabled){
			var animParam = GetParameter(paramName);
			if (animParam != null){
				animParam.SetEnabled(enabled, root.currentTime - startTime);
			}
		}

		///Re-Init/Reset all existing animated parameters
		public void ResetAnimatedParameters(){
			if (animationData != null){
				animationData.Reset();
			}
		}

		//Creates the animation data collection out of the fields/properties marked with [AnimatableParameter] attribute
		void CreateAnimationDataCollection(){

			if (handleParametersRegistrationManually){
				return;
			}

			if (animatedParametersInfo != null && animatedParametersInfo.Length != 0 ){
				animationData = new AnimationDataCollection(animatedParametersInfo, this, null, null);
			}
		}

		//Validate the animation parameters vs the animation data collection to be synced, adding or removing as required.
		void ValidateAnimParams(){
#if SLATE_USE_EXPRESSIONS
			FlushExpressionEnvironment();
#endif
			if (animationData != null){
				animationData.Validate(this);
			}

			//we don't need validation in runtime
			if (Application.isPlaying){
				return;
			}

			if (handleParametersRegistrationManually){
				return;
			}

			if (animatedParametersInfo == null || animatedParametersInfo.Length == 0){
				animationData = null;
				return;
			}

			//try append new
			for (var i = 0; i < animatedParametersInfo.Length; i++){
				var member = animatedParametersInfo[i];
				if (member != null){
					animationData.TryAddParameter(member, this, null, null);
				}
			}

			//cleanup
			foreach(var animParam in animationData.animatedParameters.ToArray()){
				if (!animParam.isValid){
					animationData.animatedParameters.Remove(animParam);
					continue;
				}

				if (!animatedParametersInfo.Select(m => m.Name).Contains(animParam.GetMemberInfo().Name )){
					animationData.animatedParameters.Remove(animParam);
					continue;
				}
			}
		}

		//Set an animation snapshot for all parameters
		void SetAnimParamsSnapshot(){
			if (hasParameters){
				animationData.SetVirtualTransformParent( GetSpaceTransform(TransformSpace.CutsceneSpace) );
				animationData.SetSnapshot();
			}
		}

		//Update the animation parameters, setting their evaluated values
		 protected void UpdateAnimParams(float time, float previousTime){
			if (hasParameters){
				animationData.Evaluate(time, previousTime, useWeightInParameters? GetClipWeight(time) : 1 );
			}			
		}

		//Restore the animation snapshot on all parameters
		void RestoreAnimParamsSnapshot(){
			if (hasParameters){
				animationData.RestoreSnapshot();
			}
		}

#if UNITY_EDITOR
		//Try record keys
		bool IKeyable.TryAutoKey(float time){
			if (hasParameters && ReferenceEquals(CutsceneUtility.selectedObject, this) ){
				return animationData.TryAutoKey(time);
			}
			return false;
		}
#endif


#if SLATE_USE_EXPRESSIONS
		///The ExpressionEnvironment used
		private StagPoint.Eval.Environment expressionEnvironment;
		StagPoint.Eval.Environment IDirectable.GetExpressionEnvironment(){
			if (expressionEnvironment != null){
				return expressionEnvironment;
			}

			expressionEnvironment = parent.GetExpressionEnvironment().Push();
			Slate.Expressions.ExpressionActionClipWrapper.Wrap(this, expressionEnvironment);
			return expressionEnvironment;
		}

		//release environment
		void FlushExpressionEnvironment(){
			expressionEnvironment = null;
		}
#endif

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		///Show clip GUI contents
		public void ShowClipGUI(Rect rect){
			OnClipGUI(rect);
		}

		///This is called outside of the clip for UI on the the left/right sides of the clip.
		public void ShowClipGUIExternal(Rect left, Rect right){
			OnClipGUIExternal(left, right);
		}

		///Override for extra clip GUI contents.
		virtual protected void OnClipGUI(Rect rect){}
		///Override for extra clip GUI contents outside of clip.
		virtual protected void OnClipGUIExternal(Rect left, Rect right){}

        /// <summary>
        /// @modify slate sequencer
        /// @hushuang
        /// enable custom edit key at time 
        /// </summary>
        /// <param name="time"></param>
        virtual public void EditKeyable(float time) {}
		#endif
	}
}