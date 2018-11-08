#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Slate{

	[CustomPropertyDrawer(typeof(AnimatableParameterAttribute))]
	public class AnimatableParameterDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label){ return -2; }
		public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label){
			var keyable = prop.serializedObject.targetObject as IKeyable;
			if (keyable != null){
				var animParam = keyable.animationData != null? keyable.animationData.GetParameterOfName(fieldInfo.Name) : null;
				if (animParam != null){
					AnimatableParameterEditor.ShowParameter(animParam, keyable, prop);
				}
			}
		}
	}
}

#endif