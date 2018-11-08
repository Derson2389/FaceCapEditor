#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Slate
{

    [CustomEditor(typeof(ShotCamera))]
    public class ShotCameraInspector : Editor
    {

        private SerializedProperty focalPointProp;
        private SerializedProperty focalRangeProp;
        private SerializedProperty controllerProp;
//         private SerializedProperty nearProp;
//         private SerializedProperty farProp;

        private ShotCamera shot
        {
            get { return (ShotCamera)target; }
        }

        void OnEnable()
        {
            focalPointProp = serializedObject.FindProperty("_focalPoint");
            focalRangeProp = serializedObject.FindProperty("_focalRange");
            controllerProp = serializedObject.FindProperty("_dynamicController");
//             nearProp = serializedObject.FindProperty("_nearClipping");
//             farProp = serializedObject.FindProperty("_farClipping");
        }

        void OnSceneGUI()
        {
            shot.OnSceneGUI();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("The Camera Component attached above is mostly used for Editor Previews and thus the reason why it's not editable.\n\nYou can instead change the 'Render Camera' settings if so required found under the 'Director Camera Root' GameObject, and which is the only Camera Cutscenes are rendered from within.\n\nFor more options and for animating this Shot, please select a Shot Clip that makes use of this Shot Camera in a Slate Editor Window Camera Track.", MessageType.Info);

            // @modify slate sequencer
            // @rongxia
            float L = 36.003f;
            float fFoucsLength = (L / 2.0f) / Mathf.Tan(shot.fieldOfView / 180.0f * Mathf.PI / 2.0f);
            float newFoucsLength = EditorGUILayout.Slider(new GUIContent("focus length", "the camera's focus length"), fFoucsLength, 2.5f, 3500f);
            if(newFoucsLength != fFoucsLength)
            {
                fFoucsLength = newFoucsLength;
                shot.fieldOfView = Mathf.Clamp(2.0f * Mathf.Atan((L / 2.0f) / fFoucsLength) / Mathf.PI * 180.0f, 1, 179);
            }
            // @end

            shot.fieldOfView = EditorGUILayout.Slider("Field Of View", shot.fieldOfView, 5, 170);
            serializedObject.Update();
            EditorGUILayout.PropertyField(focalPointProp);
            EditorGUILayout.PropertyField(focalRangeProp);
            shot.nearClipping = EditorGUILayout.Slider("Near Clipping", shot.nearClipping, 0.01f, 1000f);
            shot.farClipping = EditorGUILayout.Slider("Far Clipping", shot.farClipping, 0.01f, 1000f);
            EditorGUILayout.PropertyField(controllerProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif