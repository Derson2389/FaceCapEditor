#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Slate{
    
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label){ return -2; }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            var attribute = (HelpBoxAttribute) base.attribute;
            EditorGUILayout.PropertyField(property);
            EditorGUILayout.HelpBox(attribute.text, MessageType.None);
            GUILayout.Space(10);
        }
    }
}

#endif