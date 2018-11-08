#if UNITY_EDITOR && UNITY_5_4_OR_NEWER

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Slate{

    // @modify slate sequencer
    // @hushuang
    // enable custom inspector for children class.
    [CustomEditor(typeof(ActionClips.PlayAnimatorClip), true)]
	public class PlayAnimatorClipInspector : ActionClipInspector<ActionClips.PlayAnimatorClip> {

		public override void OnInspectorGUI(){

			base.OnInspectorGUI();

			EditorGUILayout.HelpBox("'Steer Local Rotation' is only used if RootMotion is enabled in the Animator Track. You can animate this parameter to rotate the actor while using it's RootMotion. This is very useful for animations like Walk or Run. Most of the times, simply animating 'Y' will suffice.", MessageType.Info);

			if (action.animationClip != null && GUILayout.Button("Set at Clip Length")){
				action.length = action.animationClip.length;	
			}
		}
	}
}

#endif