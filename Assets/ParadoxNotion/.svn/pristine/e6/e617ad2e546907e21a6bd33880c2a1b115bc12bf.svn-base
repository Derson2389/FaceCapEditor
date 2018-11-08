#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Slate{

	[CustomEditor(typeof(ActionClips.AnimateProperties))]
	public class AnimatePropertiesInspector : ActionClipInspector<ActionClips.AnimateProperties> {

		public override void OnInspectorGUI(){

			base.OnInspectorGUI();

			GUILayout.Space(10);

			if (GUILayout.Button("Add Property")){
				EditorTools.ShowAnimatedPropertySelectionMenu(action.actor.gameObject, AnimatedParameter.supportedTypes, (prop, comp)=>
				{
					action.animationData.TryAddParameter(prop, action, comp.transform, action.actor.transform);
				});
			}
		}
	}
}

#endif