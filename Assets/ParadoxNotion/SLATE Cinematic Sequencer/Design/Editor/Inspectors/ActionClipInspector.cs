#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Slate{

	public class ActionClipInspector<T> : ActionClipInspector where T:ActionClip{
		protected T action{
			get {return (T)target;}
		}
	}

	[CustomEditor(typeof(ActionClip), true)]
	public class ActionClipInspector : Editor {

		private ActionClip action{
			get {return (ActionClip)target;}
		}

		public override void OnInspectorGUI(){
			ShowCommonInspector();
			ShowAnimatableParameters();
			if (GUI.changed){
				action.Validate();
			}
		}

		protected void ShowCommonInspector(bool showBaseInspector = true){
			ShowErrors();
			ShowInOut();
			ShowBlending();
			if (showBaseInspector){
				base.OnInspectorGUI();
			}
		}


		//Shows all animatable parameters of the clip
		protected void ShowAnimatableParameters(){
			if (action.hasParameters){
				foreach(var animParam in action.animationData.animatedParameters){
					if (animParam.isProperty || animParam.declaringType != action.GetType() ){ //field based parameters are shown through AnimatableParameterDrawer
						GUILayout.Space(2);
						AnimatableParameterEditor.ShowParameter(animParam, action);
					}
				}
			}
		}

		void ShowErrors(){

			if (action.actor == null){
	            EditorGUILayout.HelpBox("The target Actor is null.", MessageType.Error);
	            GUILayout.Space(5);
	            return;
			}

			if (!action.isValid){

				var type = action.GetType();
			    while (type != null && type != typeof(ActionClip)) {
			        var cur = type.IsGenericType? type.GetGenericTypeDefinition() : type;
			        if (typeof(Slate.ActionClips.ActorActionClip<>) == cur) {
			        	var requiredType = type.GetGenericArguments()[0];
			        	if (action.actor.GetComponent(requiredType) == null ){
				            EditorGUILayout.HelpBox(string.Format("This clip requires the actor to have the '{0}' Component", requiredType.Name), MessageType.Error);
				            GUILayout.Space(5);
				        }
			            return;
			        }
			        type = type.BaseType;
			    }


	            EditorGUILayout.HelpBox("The clip is currently invalid. Please make sure the required parameters are set.", MessageType.Error);
	            GUILayout.Space(5);
	            return;
			}
		}

		void ShowInOut(){

			var previousClip = action.parent.children.Where(a => !ReferenceEquals(a, action) && a.startTime < action.startTime).LastOrDefault();
			var previousTime = previousClip != null? previousClip.endTime : action.parent.startTime;
			if (action is ICrossBlendable && previousClip is ICrossBlendable){
				previousTime -= Mathf.Min(action.length/2, (previousClip.endTime - previousClip.startTime) /2);
			}

			var nextClip = action.parent.children.Where(a => !ReferenceEquals(a, action) && a.endTime > action.endTime).FirstOrDefault();
			var nextTime = nextClip != null? nextClip.startTime : action.parent.endTime;
			if (action is ICrossBlendable && nextClip is ICrossBlendable){
				nextTime += Mathf.Min(action.length/2, (nextClip.endTime - nextClip.startTime) /2);
			}

			var lengthProp = action.GetType().GetProperty("length", BindingFlags.Instance | BindingFlags.Public );
			var isScalable = lengthProp != null && lengthProp.DeclaringType != typeof(ActionClip) && lengthProp.CanWrite;
			var doFrames = Prefs.timeStepMode == Prefs.TimeStepMode.Frames;

			GUILayout.BeginVertical( (GUIStyle)"box" );
			GUILayout.BeginHorizontal();

			var _in = action.startTime;
			var _length = action.length;
			var _out = action.endTime;

			if (isScalable){
				GUILayout.Label("IN", GUILayout.Width(30));
				if (doFrames){
					_in *= Prefs.frameRate;
					_in = (int)EditorGUILayout.DelayedIntField(Mathf.RoundToInt(_in), GUILayout.Width(80));
					_in = _in * (1f/Prefs.frameRate);
				} else {
					_in = EditorGUILayout.DelayedFloatField(_in, GUILayout.Width(80));
				}

				GUILayout.FlexibleSpace();
				GUILayout.Label("◄");
				if (doFrames){
					_length *= Prefs.frameRate;
					_length = (int)EditorGUILayout.DelayedIntField(Mathf.RoundToInt(_length), GUILayout.Width(80));
					_length = _length * (1f/Prefs.frameRate);
				} else {
					_length = EditorGUILayout.DelayedFloatField(_length, GUILayout.Width(80));
				}
				GUILayout.Label("►");
				GUILayout.FlexibleSpace();

				GUILayout.Label("OUT", GUILayout.Width(30));
				if (doFrames){
					_out *= Prefs.frameRate;
					_out = (int)EditorGUILayout.DelayedIntField(Mathf.RoundToInt(_out), GUILayout.Width(80));
					_out = _out * (1f/Prefs.frameRate);
				} else {
					_out = EditorGUILayout.DelayedFloatField(_out, GUILayout.Width(80));
				}
			}

			GUILayout.EndHorizontal();

			if (isScalable){
				if (_in >= action.parent.startTime && _out <= action.parent.endTime){
					if (_out > _in){
						EditorGUILayout.MinMaxSlider(ref _in, ref _out, previousTime, nextTime);
					} else {
						_in = EditorGUILayout.Slider(_in, previousTime, nextTime);
						_out = _in;
					}
				}
			} else {
				GUILayout.Label("IN", GUILayout.Width(30));
				_in = EditorGUILayout.Slider(_in, 0, action.parent.endTime);
				_out = _in;
			}


			if (GUI.changed){
				
				if (_length != action.length){
					_out = _in + _length;
				}

				_in = Mathf.Round(_in / Prefs.snapInterval) * Prefs.snapInterval;
				_out = Mathf.Round(_out / Prefs.snapInterval) * Prefs.snapInterval;

				_in = Mathf.Clamp(_in, previousTime, _out);
				_out = Mathf.Clamp(_out, _in, nextClip != null? nextTime : float.PositiveInfinity);

				action.startTime = _in;
				action.endTime = _out;
			}

            if (Mathf.Abs((action.endTime - action.startTime) - action.length)>0.001)
            {
                EditorGUILayout.HelpBox("Clip is outside of playable range", MessageType.Error);
            }


			if (_in > action.parent.endTime){
				EditorGUILayout.HelpBox("Clip is outside of playable range", MessageType.Warning);
			} else {
				if (_out > action.parent.endTime){
					EditorGUILayout.HelpBox("Clip end time is outside of playable range", MessageType.Warning);
				}
			}

			if (_out < action.parent.startTime){
				EditorGUILayout.HelpBox("Clip is outside of playable range", MessageType.Warning);
			} else {
				if (_in < action.parent.startTime){
					EditorGUILayout.HelpBox("Clip start time is outside of playable range", MessageType.Warning);
				}	
			}

			GUILayout.EndVertical();
		}

		void ShowBlending(){
			var blendInProp = action.GetType().GetProperty("blendIn", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			var blendOutProp = action.GetType().GetProperty("blendOut", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			if ((blendInProp != null || blendOutProp != null) && action.length > 0){
				GUILayout.BeginVertical( (GUIStyle)"box" );
				GUILayout.BeginHorizontal();
				//clips can return negative to hide option when they dont need blend based on user settings in inspector
				if (blendInProp != null && blendInProp.CanWrite && action.blendIn != -1){ 
					GUILayout.BeginVertical();
					GUILayout.Label("Blend In");
					var max = action.length - action.blendOut;
					action.blendIn = EditorGUILayout.Slider(action.blendIn, 0, max );
					action.blendIn = Mathf.Clamp(action.blendIn, 0, max);
					GUILayout.EndVertical();
				}
				//clips can return negative to hide option when they dont need blend based on user settings in inspector
				if (blendOutProp != null && blendOutProp.CanWrite && action.blendOut != -1){ 
					GUILayout.BeginVertical();
					GUILayout.Label("Blend Out");
					var max = action.length - action.blendIn;
					action.blendOut = EditorGUILayout.Slider(action.blendOut, 0, max );
					action.blendOut = Mathf.Clamp(action.blendOut, 0, max);
					GUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
		}

	}
}

#endif