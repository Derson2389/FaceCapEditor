#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Slate
{
    // @modify slate sequencer
    // @hushuang
    // enable custom inspector for children class.
    [CustomEditor(typeof(CameraShot), true)]
    public class CameraShotInspector : ActionClipInspector<CameraShot>
    {

        SerializedProperty blendInEffectProp;
        SerializedProperty blendOutEffectProp;
        SerializedProperty steadyCamEffectProp;

        SerializedObject shotSerializedObject;
        SerializedProperty shotControllerProp;

        public static float filterFrequency = 120.0f;
        public static float filterMinCutoff = 1.0f;
        public static float filterBeta = 0.0f;
        public static float filterDcutoff = 1.0f;
        public static int filterType = 0;
        public static int filterFrames = 10;

        //public void InitCameraThreshold()
        //{
        //    AnimatedParameter AP = action.animationData.GetParameterOfName("Rotation");
        //    List<Vector3> rotations = new List<Vector3>();

        //    int frameCount = AP.GetCurves()[0].length;
        //    var frame_x_s = AP.GetCurves()[0].keys;
        //    var frame_y_s = AP.GetCurves()[1].keys;
        //    var frame_z_s = AP.GetCurves()[2].keys;

        //    for (int j = 0; j < frameCount; j++)
        //    {
        //        Vector3 vec = new Vector3(frame_x_s[j].value, frame_y_s[j].value, frame_z_s[j].value);
        //        rotations.Add(vec);
        //    }
        //    cameraThreshold = Slate.CurveSmooth.GetSmoothRotationThreshold(rotations);
        //}

        void OnEnable()
        {
            blendInEffectProp = serializedObject.FindProperty("blendInEffect");
            blendOutEffectProp = serializedObject.FindProperty("blendOutEffect");
            steadyCamEffectProp = serializedObject.FindProperty("steadyCamEffect");
            action.lookThrough = false;
            ///InitCameraThreshold();
        }

        void OnDisable()
        {
            action.lookThrough = false;
        }


        public override void OnInspectorGUI()
        {

            base.ShowCommonInspector();


            serializedObject.Update();
            EditorGUILayout.PropertyField(blendInEffectProp);
            EditorGUILayout.PropertyField(blendOutEffectProp);
            EditorGUILayout.PropertyField(steadyCamEffectProp);
            serializedObject.ApplyModifiedProperties();

            if (action.parent.children.OfType<CameraShot>().FirstOrDefault() == action)
            {
                if (action.blendInEffect == CameraShot.BlendInEffectType.EaseIn)
                {
                    EditorGUILayout.HelpBox("The 'Ease In' option has no effect in the first shot clip of the track.", MessageType.Warning);
                }
                if (action.blendInEffect == CameraShot.BlendInEffectType.CrossDissolve)
                {
                    EditorGUILayout.HelpBox("The 'Cross Dissolve' option has no usable effect in the first shot clip of the track.", MessageType.Warning);
                }
            }




            if (action.targetShot == null)
            {
                EditorGUILayout.HelpBox("Select or Create a Shot Camera to be used by this clip.", MessageType.Error);
            }

            if (GUILayout.Button(action.targetShot == null ? "Select Shot" : "Replace Shot"))
            {
                if (action.targetShot == null || EditorUtility.DisplayDialog("Replace Shot", "Selecting a new target shot will reset all animation data of this clip.", "OK", "Cancel"))
                {
                    ShotPicker.Show(Event.current.mousePosition, action.root, (shot) => { action.targetShot = shot; });
                }
            }

            var parentTrack = action.parent as CameraTrack; 
            if (parentTrack.name.Contains("Smooth CamTrack_"))
            {
                filterType = EditorGUILayout.IntField("Smooth Type", filterType);

                if (filterType == (int)CurveSmooth.SmoothTypePost.average)
                {
                    filterFrames = EditorGUILayout.IntField("Frames", filterFrames);
                }
                else if(filterType == (int)CurveSmooth.SmoothTypePost.oneEuro)
                {
                    filterMinCutoff = EditorGUILayout.FloatField("Cam MinCutOff", filterMinCutoff);
                    filterBeta = EditorGUILayout.FloatField("Cam Beta", filterMinCutoff);
                    filterDcutoff = EditorGUILayout.FloatField("Cam D cutoff", filterMinCutoff);
                }
                if (GUILayout.Button("平滑数据"))
                {
                    SmoothCamera.StartSmooth(filterFrames);
                    var clip = action as ActionClips.StoryEngineClip.StoryCameraClip;
                    AnimatedParameter AP = clip.animationData.GetParameterOfName("Rotation");
                    List<Vector3> rotations = new List<Vector3>();

                    int frameCount = AP.GetCurves()[0].length;
                    var frame_x_s = AP.GetCurves()[0].keys;
                    var frame_y_s = AP.GetCurves()[1].keys;
                    var frame_z_s = AP.GetCurves()[2].keys;

                    for (int j = 0; j < frameCount; j++)
                    {
                        Vector3 vec = new Vector3(frame_x_s[j].value, frame_y_s[j].value, frame_z_s[j].value);
                        rotations.Add(vec);
                    }
                    if (rotations.Count > 0)
                    {
                        if (filterType == (int)CurveSmooth.SmoothTypePost.oneEuro 
                            || filterType == (int)CurveSmooth.SmoothTypePost.kalMan)
                        {
                            Slate.CurveSmooth.SetSmoothParam(filterFrequency, filterBeta, filterMinCutoff, filterDcutoff);

                            List<Vector3> qaVec = Slate.CurveSmooth.SmoothRotationVectorTQ(rotations, filterType);
                            for (int a = 0; a < frameCount; a++)
                            {
                                Vector3 rot = qaVec[a];

                                AP.GetCurves()[0].RemoveKey(a);
                                AP.GetCurves()[1].RemoveKey(a);
                                AP.GetCurves()[2].RemoveKey(a);

                                AP.GetCurves()[0].AddKey(new Keyframe(frame_x_s[a].time, rot.x, float.PositiveInfinity, float.PositiveInfinity));
                                AP.GetCurves()[1].AddKey(new Keyframe(frame_y_s[a].time, rot.y, float.PositiveInfinity, float.PositiveInfinity));
                                AP.GetCurves()[2].AddKey(new Keyframe(frame_z_s[a].time, rot.z, float.PositiveInfinity, float.PositiveInfinity));
                            }
                        }
                        else if (filterType == (int)CurveSmooth.SmoothTypePost.average)
                        {
                            List<Quaternion> qa = Slate.CurveSmooth.SmoothRotationTQ(rotations, filterType);
                            for (int a = 0; a < frameCount; a++)
                            {
                                Vector3 rot = qa[a].eulerAngles;
                                AP.GetCurves()[0].RemoveKey(a);
                                AP.GetCurves()[1].RemoveKey(a);
                                AP.GetCurves()[2].RemoveKey(a);

                                AP.GetCurves()[0].AddKey(new Keyframe(frame_x_s[a].time, rot.x, float.PositiveInfinity, float.PositiveInfinity));
                                AP.GetCurves()[1].AddKey(new Keyframe(frame_y_s[a].time, rot.y, float.PositiveInfinity, float.PositiveInfinity));
                                AP.GetCurves()[2].AddKey(new Keyframe(frame_z_s[a].time, rot.z, float.PositiveInfinity, float.PositiveInfinity));
                            }
                        }
                    }
                }
            }

            if (action.targetShot == null && GUILayout.Button("Create Shot"))
            {
                action.targetShot = ShotCamera.Create(action.root.context.transform);
            }

            if (action.targetShot != null)
            {

                if (GUILayout.Button("Find in Scene"))
                {
                    Selection.activeGameObject = action.targetShot.gameObject;
                }

                var lastRect = GUILayoutUtility.GetLastRect();
                var rect = new Rect(lastRect.x, lastRect.yMax + 5, lastRect.width, 200);

                var res = EditorTools.GetGameViewSize();
                var texture = action.targetShot.GetRenderTexture((int)res.x, (int)res.y);
                var style = new GUIStyle("Box");
                style.alignment = TextAnchor.MiddleCenter;
                GUI.Box(rect, texture, style);

                if (action.targetShot.dynamicController.composer.trackingMode == DynamicCameraController.Composer.TrackingMode.FrameComposition)
                {
                    var scale = Mathf.Min(rect.width / res.x, rect.height / res.y);
                    var result = new Vector2((res.x * scale), (res.y * scale));
                    var boundedRect = new Rect(0, 0, result.x, result.y);
                    boundedRect.center = rect.center;
                    GUI.BeginGroup(boundedRect);
                    action.targetShot.dynamicController.DoGUI(action.targetShot, boundedRect);
                    GUI.EndGroup();
                }


                GUILayout.Space(205);

                var helpRect = new Rect(rect.x + 10, rect.yMax - 20, rect.width - 20, 16);
                GUI.color = EditorGUIUtility.isProSkin ? new Color(0, 0, 0, 0.6f) : new Color(1, 1, 1, 0.6f);
                GUI.DrawTexture(helpRect, Slate.Styles.whiteTexture);
                GUI.color = Color.white;
                GUI.Label(helpRect, "Left: Rotate, Middle: Pan, Right: Dolly, Alt+Right: Zoom");

                var e = Event.current;
                if (rect.Contains(e.mousePosition))
                {
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.Pan);
                    if (e.type == EventType.MouseDrag)
                    {

                        Undo.RecordObject(action.targetShot.transform, "Shot Change");
                        Undo.RecordObject(action.targetShot.cam, "Shot Change");
                        Undo.RecordObject(action.targetShot, "Shot Change");

                        var in2DMode = false;
                        var sc = UnityEditor.SceneView.lastActiveSceneView;
                        if (sc != null)
                        {
                            in2DMode = sc.in2DMode;
                        }

                        // @modify slate sequencer
                        // @TQ
                        //look
                        if (e.button == 0 && !in2DMode)
                        {
                            var deltaRot = new Vector3(e.delta.y, e.delta.x, 0) * 0.1f;
                            action.targetShot.localEulerAngles += deltaRot;
                            e.Use();
                        }
                        //pan
                        if (e.button == 2 || (e.button == 0 && in2DMode))
                        {
                            var deltaPos = new Vector3(-e.delta.x, e.delta.y, 0) * (e.shift ? 0.001f : 0.01f);
                            action.targetShot.transform.Translate(deltaPos);
                            e.Use();
                        }
                        //dolly in/out
                        if (e.button == 1 && !e.alt)
                        {
                            action.targetShot.transform.Translate(0, 0, e.delta.x * 0.01f);
                            e.Use();
                        }
                        //end 
                        //fov
                        if (e.button == 1 && e.alt)
                        {
                            action.fieldOfView -= e.delta.x;
                            e.Use();
                        }

                        EditorUtility.SetDirty(action.targetShot.transform);
                        EditorUtility.SetDirty(action.targetShot.cam);
                        EditorUtility.SetDirty(action.targetShot);
                    }
                }



                ////The shot dynamic controller settings
                if (shotSerializedObject == null || shotSerializedObject.targetObject != action.targetShot)
                {
                    if (action.targetShot != null)
                    {
                        shotSerializedObject = new SerializedObject(action.targetShot);
                        shotControllerProp = shotSerializedObject.FindProperty("_dynamicController");
                    }
                }

                EditorGUI.BeginChangeCheck();
                if (shotSerializedObject != null)
                {
                    shotSerializedObject.Update();
                    EditorGUILayout.PropertyField(shotControllerProp);
                    shotSerializedObject.ApplyModifiedProperties();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    action.Validate();
                }
                /////

                base.ShowAnimatableParameters();
            }

            // @modify slate sequencer
            // @rongxia

            // @end
        }
    }
}

#endif