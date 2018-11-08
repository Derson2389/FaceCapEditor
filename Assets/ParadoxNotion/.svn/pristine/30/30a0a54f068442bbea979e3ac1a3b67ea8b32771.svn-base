using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Slate{

    //CameraShot clip wraps a ShotCamera
    [Attachable(typeof(CameraTrack))]
    [Description("Camera Shots can be animated directly within this clip. You don't need to create an Actor Group to animate the shot (even though you can if desired).")]
    public class CameraShot : DirectorActionClip {

        public enum BlendInEffectType {
            None,
            FadeIn,
            CrossDissolve,
            EaseIn
        }

        public enum BlendOutEffectType {
            None,
            FadeOut
        }

        public enum ShotAnimationMode {
            UseInternal,
            UseExternalAnimationClip
        }

        [SerializeField] [HideInInspector]
        private float _length = 5;
        [SerializeField] [HideInInspector]
        private float _blendIn;
        [SerializeField] [HideInInspector]
        private float _blendOut;

        [SerializeField] [HideInInspector]
        private ShotCamera _targetShot;

        //shown in custom inspector
        [HideInInspector]
        public BlendInEffectType blendInEffect;
        [HideInInspector]
        public BlendOutEffectType blendOutEffect;
        [HideInInspector] [Range(0, 1)]
        public float steadyCamEffect;
        //

        //blend effects
        private Color lastFadeColor;
        //


        public override string info {
            get
            {
#if UNITY_EDITOR
                return targetShot != null ? (Prefs.showShotThumbnails && length > 0 ? null : targetShot.name) : "No Shot Selected";
#else
				return targetShot != null? targetShot.name : "No Shot Selected";
#endif
            }
        }

        public override bool isValid {
            get { return targetShot != null; }
        }

        public override float length {
            get { return _length; }
            set { _length = value; }
        }

        public override float blendIn {
            get { return blendInEffect != BlendInEffectType.None ? _blendIn : -1; }
            set { _blendIn = value; }
        }

        public override float blendOut {
            get { return blendOutEffect != BlendOutEffectType.None ? _blendOut : -1; }
            set { _blendOut = value; }
        }

        new public GameObject actor { //not REALY needed but makes it that double click the clip selects the camera. Special case.
            get { return targetShot ? targetShot.gameObject : base.actor; }
        }

        public CameraShot previousShot { get;  set; }

        public ShotCamera targetShot {
            get { return _targetShot; }
            set
            {
                if (_targetShot != value) {
                    _targetShot = value;
                    base.ResetAnimatedParameters();
                }
            }
        }

        private CameraTrack track {
            get { return (CameraTrack)parent; }
        }


        [AnimatableParameter]
        public Vector3 position {
            get { return targetShot ? targetShot.localPosition : Vector3.zero; }
            set { if (targetShot != null) targetShot.localPosition = value; }
        }

        [AnimatableParameter]
        public Vector3 rotation {
            get { return targetShot ? targetShot.localEulerAngles : Vector3.zero; }
            set { if (targetShot != null) targetShot.localEulerAngles = value; }
        }

        [AnimatableParameter(0.01f, 170f)]
        public float fieldOfView {
            get { return targetShot ? targetShot.fieldOfView : 60f; }
            set { if (targetShot != null) targetShot.fieldOfView = Mathf.Clamp(value, 0.01f, 170); }
        }

        [AnimatableParameter]
        public float focalPoint {
            get { return targetShot ? targetShot.focalPoint : 10f; }
            set { if (targetShot != null) targetShot.focalPoint = Mathf.Max(value, 0); }
        }

        [AnimatableParameter]
        public float focalRange {
            get { return targetShot ? targetShot.focalRange : 15f; }
            set { if (targetShot != null) targetShot.focalRange = Mathf.Max(value, 0); }
        }


        [AnimatableParameter]

        public float nearClipping {
            get { return targetShot ? targetShot.nearClipping : 0f; }
            set { if (targetShot != null) targetShot.nearClipping = Mathf.Max(value, 0f); }
        }


        [AnimatableParameter]

        public float farClipping
        {
            get { return targetShot ? targetShot.farClipping : 1000f; }
            set { if (targetShot != null) targetShot.farClipping = Mathf.Min(value, 1000f); }
        }

        protected override void OnAfterValidate(){

			var positionOverride = targetShot != null && targetShot.dynamicControlledPosition? true : false;
			var rotationOverride = targetShot != null && targetShot.dynamicControlledRotation? true : false;
			var fieldOfViewOverride = false;

			SetParameterEnabled("position", !positionOverride);
			SetParameterEnabled("rotation", !rotationOverride);
			SetParameterEnabled("fieldOfView", !fieldOfViewOverride);
		}

		///dynamic controller should update across the whole cutscene length regardless of whether or not this clip is in range
		protected override void OnRootEnabled(){ if(targetShot != null && !root.isReSampleFrame) targetShot.UpdateDynamicControllerHard(this); }
		protected override void OnRootDisabled(){ if(targetShot != null && !root.isReSampleFrame) targetShot.UpdateDynamicControllerHard(this); }
		protected override void OnRootUpdated(float time, float previousTime){ if(targetShot != null && !root.isReSampleFrame) targetShot.UpdateDynamicControllerSoft(this); }
		///

		protected override void OnEnter(){

            previousShot = track.currentShot;
			track.currentShot = this;

			lastFadeColor = DirectorGUI.fadeColor;
			DirectorGUI.UpdateFade(Color.clear);
		}

        protected override void OnExit()
        {
            base.OnExit();
        }

        protected override void OnUpdate(float time, float previousTime){

            if (steadyCamEffect > 0 && time != previousTime){
				DirectorCamera.ApplyNoise(steadyCamEffect, GetClipWeight(time, 1f));
			}


			if (blendInEffect == BlendInEffectType.FadeIn){
				if (time <= blendIn){
					var color = Color.black;
					color.a = Easing.Ease(EaseType.QuadraticInOut, 1, 0, GetClipWeight(time));
					DirectorGUI.UpdateFade(color);
				} else if (time < length - blendOut) {
					DirectorGUI.UpdateFade(Color.clear);
				}
			}

			if (blendOutEffect == BlendOutEffectType.FadeOut){
				if (time >= length - blendOut){
					var color = Color.black;
					color.a = Easing.Ease(EaseType.QuadraticInOut, 1, 0, GetClipWeight(time));
					DirectorGUI.UpdateFade(color);
				} else if (time > blendIn) {
					DirectorGUI.UpdateFade(Color.clear);
				}
			}

			if (blendInEffect == BlendInEffectType.CrossDissolve && previousShot != null && previousShot.targetShot != null){
				if (time <= blendIn){
					var res = new Vector2(Screen.width, Screen.height);
					#if UNITY_EDITOR
					res = EditorTools.GetGameViewSize();
					#endif

					var dissolver = previousShot.targetShot.GetRenderTexture( (int)res.x, (int)res.y );
					var ease = Easing.Ease(EaseType.QuadraticInOut, 0, 1, GetClipWeight(time));
					DirectorGUI.UpdateDissolve(dissolver, ease);
				} else {
					DirectorGUI.UpdateDissolve(null, 0);
				}
			}
		}


		protected override void OnReverse(){
			DirectorGUI.UpdateFade(lastFadeColor);
			DirectorGUI.UpdateDissolve(null, 0);
			track.currentShot = previousShot;
		}



		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		[System.NonSerialized]
		private int thumbRefresher;
		[System.NonSerialized]
		private Texture thumbnail;
		[System.NonSerialized]
		private float lastSampleTime;
		[System.NonSerialized]
		private Vector3 lastPos;
		[System.NonSerialized]
		private Quaternion lastRot;
		
		[System.NonSerialized]
		private Quaternion scWasRotation;
		[System.NonSerialized]
		private Vector3 scWasPivot;
		[System.NonSerialized]
		private float scWasSize;

		[System.NonSerialized]
		private bool _lookThrough;

		public bool lookThrough{
			get { return _lookThrough; }
			set
			{
				if (_lookThrough != value){
					_lookThrough = value;
					if (value == true){
						var sc = UnityEditor.SceneView.lastActiveSceneView;
						if (sc != null && targetShot != null){
							targetShot.cam.orthographic = sc.in2DMode;
							
							scWasRotation = sc.rotation;
							scWasPivot = sc.pivot;
							scWasSize = sc.size;

							sc.rotation = targetShot.rotation;
							sc.pivot = targetShot.position + (targetShot.transform.forward * 5);
							sc.size = 5;
							lastPos = sc.camera.transform.position;
							lastRot = sc.camera.transform.rotation;
						}
					}
					if (value == false){
						var sc = UnityEditor.SceneView.lastActiveSceneView;
						if (sc != null){
							sc.rotation = scWasRotation;
							sc.pivot = scWasPivot;
							sc.size = scWasSize;
						}
					}
				}
			}
		}


		protected override void OnSceneGUI(){
			
			if (targetShot == null){
				return;
			}

			UnityEditor.Handles.BeginGUI();
			GUI.backgroundColor = lookThrough? Color.red : Color.white;
			if (targetShot != null && GUI.Button(new Rect(2,2,200,20), lookThrough? "Exit Look Through Camera" : "Look Through Camera")){
				lookThrough = !lookThrough;
			}
			GUI.backgroundColor = Color.white;
			UnityEditor.Handles.EndGUI();

			var sc = UnityEditor.SceneView.lastActiveSceneView;
			if (lookThrough && sc != null){
				if (root.currentTime == lastSampleTime){

					if (sc.camera.transform.position != lastPos || sc.camera.transform.rotation != lastRot){
						UnityEditor.Undo.RecordObject(targetShot.transform, "Shot Change");
						targetShot.position = sc.camera.transform.position;
						targetShot.rotation = sc.camera.transform.rotation;
						UnityEditor.EditorUtility.SetDirty(targetShot.gameObject);
					}

					lastPos = sc.camera.transform.position;
					lastRot = sc.camera.transform.rotation;

				} else {

					if (sc.camera.transform.position != targetShot.position || sc.camera.transform.rotation != targetShot.rotation){
						sc.rotation = targetShot.rotation;
						sc.pivot = targetShot.position + (targetShot.transform.forward * 5);
						sc.size = 5;
					}
				}

				lastSampleTime = root.currentTime;
			}	

			//show other handles only if not in look through mode
			if (!lookThrough){
				var posParam = GetParameter("position");
				if (posParam.enabled){
					CurveEditor3D.Draw3DCurve(posParam, this, targetShot.transform.parent, length/2, length );
				}

				targetShot.OnSceneGUI();

/*
				//controls for focal point and focal range.
				UnityEditor.EditorGUI.BeginChangeCheck();
				var pos = targetShot.transform.position;
				var focalPos = targetShot.transform.position + (targetShot.transform.forward * focalPoint);
				var range = focalRange;
				focalPos = UnityEditor.Handles.FreeMoveHandle(focalPos, Quaternion.identity, 0.2f, Vector3.zero, UnityEditor.Handles.CircleCap);
				UnityEditor.Handles.DrawLine(targetShot.transform.position, focalPos);
				UnityEditor.Handles.color = Color.green;
				var rangeMin = targetShot.transform.position + (targetShot.transform.forward * (focalPoint - range));
				var rangeMax = targetShot.transform.position + (targetShot.transform.forward * (focalPoint + range));
				UnityEditor.Handles.DrawLine(rangeMin, rangeMax);
				range = UnityEditor.Handles.ScaleValueHandle(range, rangeMin, Quaternion.identity, 0.5f, UnityEditor.Handles.DotCap, 0);
				range = UnityEditor.Handles.ScaleValueHandle(range, rangeMax, Quaternion.identity, 0.5f, UnityEditor.Handles.DotCap, 0);
				UnityEditor.Handles.color = Color.white;
				pos = UnityEditor.Handles.PositionHandle(pos, Quaternion.identity);
				if (UnityEditor.EditorGUI.EndChangeCheck()){
					UnityEditor.Undo.RecordObject(targetShot.transform, "Camera Change");
					UnityEditor.Undo.RecordObject(targetShot, "Camera Change");
					targetShot.transform.LookAt(focalPos);
					focalPoint = Vector3.Distance(targetShot.transform.position, focalPos);
					focalRange = range;
					targetShot.transform.position = pos;
				}
*/
			}
		}
		

		//override to show shot previews
		protected override void OnClipGUI(Rect rect){

			if (targetShot == null || rect.width < 40){
				return;
			}

			if (Prefs.showShotThumbnails){

				if (thumbRefresher == 0 || thumbRefresher % Prefs.thumbnailsRefreshInterval == 0){
					var res = EditorTools.GetGameViewSize();
					var width = (int)res.x;
					var height = (int)res.y;
					thumbnail = targetShot.GetRenderTexture(width, height);
				}

				thumbRefresher ++;
				
				if (thumbnail != null){
					GUI.backgroundColor = Color.clear;
					var style = new GUIStyle("Box");
					style.alignment = TextAnchor.MiddleCenter;
					var thumbRect1 = new Rect(0, 0, 100, rect.height);

					if (blendIn > 0){
						var previousClip = GetPreviousClip();
						if (previousClip != null && previousClip.endTime > this.startTime){
							thumbRect1.x += ( blendIn /length) * rect.width;
						}
					}

					GUI.Box(thumbRect1, thumbnail, style);
					if (rect.width > 400){
						var thumbRect2 = new Rect(rect.width - 100, 0, 100, rect.height);
						GUI.Box(thumbRect2, thumbnail, style);
					}
					GUI.backgroundColor = Color.white;
				}

				if (targetShot.name != ShotCamera.DEFAULT_NAME){
					var bevelRect = rect;
					bevelRect.center += new Vector2(1,1);
					GUI.color = Color.white;
					GUI.Label(bevelRect, targetShot.name, Styles.centerLabel);
					GUI.color = new Color(0,0,0,0.7f);
					GUI.Label(rect, targetShot.name, Styles.centerLabel);
					GUI.color = Color.white;
				}

			}
		}

		#endif
	}
}