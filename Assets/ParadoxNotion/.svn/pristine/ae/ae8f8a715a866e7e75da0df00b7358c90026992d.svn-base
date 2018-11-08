#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Slate{

	[CustomPropertyDrawer(typeof(PositionParameter))]
	public class PositionParameterDrawer : PropertyDrawer {

		private float height;
		public override float GetPropertyHeight(SerializedProperty property, GUIContent content){
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent content){
			var lines = 0;
			var rect = new Rect();
			var lineHeight = EditorGUIUtility.singleLineHeight;

			GUI.color = new Color(1,1,1,0.3f);
			GUI.Box(position, "", (GUIStyle)"flow node 0");
			GUI.color = Color.white;

			EditorGUI.LabelField(position, content.text);
			lines++;

			var transformProp = prop.FindPropertyRelative("_transform");
			var posProp       = prop.FindPropertyRelative("_vector");
			var spaceProp     = prop.FindPropertyRelative("_space");


			EditorGUI.indentLevel ++;

			rect = new Rect(position.x, position.y + lineHeight * lines, position.width-16, lineHeight);
			EditorGUI.PropertyField(rect, transformProp);
			lines++;

			if (transformProp.objectReferenceValue == null){

				rect = new Rect(position.x, position.y + lineHeight * lines, position.width-16, lineHeight);
				EditorGUI.PropertyField(rect, posProp);
				lines++;

				rect = new Rect(position.x, position.y + lineHeight * lines, position.width-16, lineHeight);
				EditorGUI.PropertyField(rect, spaceProp);
				lines++;

			} else {

				GUI.enabled = false;
				rect = new Rect(position.x, position.y + lineHeight * lines, position.width-16, lineHeight);
				EditorGUI.EnumPopup(rect, "Space", TransformSpace.WorldSpace);
				lines++;
				GUI.enabled = true;
			}

			EditorGUI.indentLevel --;

			height = lines * lineHeight;
			height += 2;
		}
	}
}

#endif