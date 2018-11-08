#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Slate{

	[CustomEditor(typeof(CameraTrack))]
	public class CameraTrackInspector : CutsceneTrackInspector {

		private CameraTrack track{
			get {return (CameraTrack)target;}
		}

        public static float filterFrequency = 120.0f;
        public static float filterMinCutoff = 1.0f;
        public static float filterBeta = 0.0f;
        public static float filterDcutoff = 1.0f;

        ///public List<float> Thresholds = new List<float>();

        //public void InitThresholdValue()
        //{
        //    if (!track.name.Contains("Smooth CamTrack_") && Thresholds.Count == 0)
        //    {
        //        var actions = track.actions;
        //        foreach (var ac in actions)
        //        {                 
        //            AnimatedParameter AP = ac.animationData.GetParameterOfName("Rotation");
        //            List<Vector3> rotations = new List<Vector3>();

        //            int frameCount = AP.GetCurves()[0].length;
        //            var frame_x_s = AP.GetCurves()[0].keys;
        //            var frame_y_s = AP.GetCurves()[1].keys;
        //            var frame_z_s = AP.GetCurves()[2].keys;

        //            for (int j = 0; j < frameCount; j++)
        //            {
        //                Vector3 vec = new Vector3(frame_x_s[j].value, frame_y_s[j].value, frame_z_s[j].value);
        //                rotations.Add(vec);
        //            }

        //            if (rotations.Count > 0)
        //            {
        //                Thresholds.Add(Slate.CurveSmooth.GetSmoothRotationThreshold(rotations));
        //            }
        //        }
        //    }
        //}
        public override void OnInspectorGUI(){
			base.OnInspectorGUI();
            ///InitThresholdValue();

            GUILayout.BeginVertical("box");
			GUILayout.Label("Active Time Offset");
			var _in = track.startTime;
			var _out = track.endTime;
			EditorGUILayout.MinMaxSlider(ref _in, ref _out, track.parent.startTime, track.parent.endTime);
			track.startTime = _in;
			track.endTime = _out;
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			var length = track.endTime - track.startTime;
			track._blendIn = EditorGUILayout.Slider("Gameplay Blend In", track._blendIn, 0, length/2);
			track._blendOut = EditorGUILayout.Slider("Gameplay Blend Out", track._blendOut, 0, length/2);
			track.interpolation = (EaseType)EditorGUILayout.EnumPopup("Interpolation", track.interpolation);
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			track.cineBoxFadeTime = EditorGUILayout.Slider("CineBox Fade Time", track.cineBoxFadeTime, 0, 1f);
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			track.appliedSmoothing = EditorGUILayout.Slider("Post Smoothing", track.appliedSmoothing, 0, DirectorCamera.MAX_DAMP);
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			track.exitCameraOverride = (Camera)EditorGUILayout.ObjectField("Exit Camera Override", track.exitCameraOverride, typeof(Camera), true);
			if (track.exitCameraOverride == Camera.main && Camera.main != null){
				EditorGUILayout.HelpBox("The Main Camera is already the default exit camera. No need to be assigned as an override.", MessageType.Warning);
			}
			GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            track.exitCameraOverride = (Camera)EditorGUILayout.ObjectField("Exit Camera Override", track.exitCameraOverride, typeof(Camera), true);
            if (track.exitCameraOverride == Camera.main && Camera.main != null)
            {
                EditorGUILayout.HelpBox("The Main Camera is already the default exit camera. No need to be assigned as an override.", MessageType.Warning);
            }
            GUILayout.EndVertical();

            if (!track.name.Contains("Smooth CamTrack_"))
            {
                GUILayout.BeginVertical("box");
                //for(int i = 0; i< Thresholds.Count; i++)
                //{
                //    Thresholds[i] = EditorGUILayout.FloatField("Cam "+i+" Threshold", Thresholds[i]);
                //}
                track.customerfilterType = (int)(CurveSmooth.SmoothTypePost)EditorGUILayout.EnumPopup("SmoothType", (CurveSmooth.SmoothTypePost)track.customerfilterType);
                if (track.customerfilterType == (int)CurveSmooth.SmoothTypePost.oneEuro)
                {
                    track.customerfilterMinCutoff = EditorGUILayout.FloatField("Smooth MinCutoff", track.customerfilterMinCutoff);
                    track.customerfilterBeta = EditorGUILayout.FloatField("Smooth Beta", track.customerfilterBeta);
                    track.customerfilterDcutoff = EditorGUILayout.FloatField("Smooth MinCutoff", track.customerfilterDcutoff);
                }
                else if(track.customerfilterType == (int)CurveSmooth.SmoothTypePost.average)
                {
                    track.customerfilterFrames = EditorGUILayout.IntField("filter Frames", track.customerfilterFrames);
                }
                
                if (GUILayout.Button("生成平滑数据轨"))
                {
                    SmoothCamera.StartSmooth(track.customerfilterFrames);
                    var group = track.parent as DirectorGroup;
                    var CloneTrackName = string.Format("Smooth CamTrack_{0}_{1}_{2}", track.customerfilterMinCutoff, track.customerfilterBeta, track.customerfilterDcutoff);
                    if (track.customerfilterType == (int)CurveSmooth.SmoothTypePost.average)
                    {
                        CloneTrackName = string.Format("Smooth CamTrack_F{0}", track.customerfilterFrames);
                    }
                    if (track.customerfilterType == (int)CurveSmooth.SmoothTypePost.kalMan)
                    {
                        CloneTrackName = string.Format("Smooth CamTrack_{0}", "kalMan");
                    }

                    var exitTrack = group.tracks.Find(t => t.name == CloneTrackName);
                    if (exitTrack != null)
                    {
                        return;
                    }

                    var CloneTrack = group.CloneTrack(track);
                    CloneTrack.name = CloneTrackName;
                    var actionList = CloneTrack.actions;

                    ///控制track开启状态
                    var cameraTracks = group.tracks.FindAll(t => t is CameraTrack);
                    foreach (var ct in cameraTracks)
                    {
                        if (!ReferenceEquals(ct, CloneTrack))
                        {
                            ct.isActive = false;
                        }
                        else
                        {
                            ct.isActive = true;
                        }
                    }
                    ///

                    for (int i = 0; i < actionList.Count; i++)
                    {
                        var clip = actionList[i] as ActionClips.StoryEngineClip.StoryCameraClip;
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
                            if (track.customerfilterType == (int)CurveSmooth.SmoothTypePost.average)
                            {
                                List<Quaternion> qa = Slate.CurveSmooth.SmoothRotationTQ(rotations, track.customerfilterType);

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
                            else if(track.customerfilterType == (int)CurveSmooth.SmoothTypePost.oneEuro
                                || track.customerfilterType == (int)CurveSmooth.SmoothTypePost.kalMan)
                            {
                                 Slate.CurveSmooth.SetSmoothParam(track.customerfilterFrequency,track.customerfilterBeta, track.customerfilterMinCutoff, track.customerfilterDcutoff);                                
                                 List<Vector3> vecList = Slate.CurveSmooth.SmoothRotationVectorTQ(rotations, track.customerfilterType);

                                for (int a = 0; a < frameCount; a++)
                                {
                                    Vector3 rot = vecList[a];

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
                GUILayout.EndVertical();
            }
        }
	}
}

#endif