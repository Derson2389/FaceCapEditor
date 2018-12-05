#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Slate{

	[CustomEditor(typeof(BlendShapeCtrlClip))]
	public class BlendShapeCtrlClipInspector : ActionClipInspector<BlendShapeCtrlClip> {

		public override void OnInspectorGUI(){

			base.OnInspectorGUI();

			GUILayout.Space(10);

			if (GUILayout.Button("Load Face Ctrl Config"))
            {
                var bscc = target as BlendShapeCtrlClip;
                if (bscc != null)
                {
                    AnimationDataCollection anim = bscc.animationData;
                    float lenth = bscc.length;
                    int cnt = (int)lenth / 10;

                    var group = bscc.parent.parent as ActorGroup;
                    var track = group.AddTrack<ActorActionTrack>("AutoAdd");
                    float startTime = bscc.startTime;
                    for (int i =1; i <= cnt; i++)
                    {
                        
                        var newBlendshape = track.AddAction<BlendShapeCtrlClip>(startTime);
                        newBlendshape.CtrlConfigDataFile = bscc.CtrlConfigDataFile;
                        if (i == cnt)
                        {
                            newBlendshape.length = bscc.endTime - startTime;

                        }
                        else
                        {
                            newBlendshape.length = 10;
                        }
                        foreach (var an in anim.animatedParameters)
                        {
                            var key1 = an.GetCurves()[0].keys.Where(k=>k.time > startTime && k.time <=startTime + newBlendshape.length).ToList();
                            var newAn = newBlendshape.animationData.GetParameterOfName(an.parameterName);
                            Vector2 keyValueLength = (Vector2)an.GetEvalValue(startTime + newBlendshape.length);
                            for (int j =0; j < key1.Count(); j++)
                            {
                                float time = key1[j].time - startTime;
                                newAn.GetCurves()[0].AddKey(time, key1[j].value);

                            }
                            newAn.GetCurves()[0].AddKey(startTime + newBlendshape.length, keyValueLength.x);
                            //newAn.GetCurves()[0].keys = key1.ToArray();

                            var key2 = an.GetCurves()[1].keys.Where(k => k.time > startTime && k.time <= startTime + newBlendshape.length).ToList();
                     
                            
                            for (int  l = 0; l < key2.Count(); l++)
                            {
                                float time = key2[l].time - startTime;
                                newAn.GetCurves()[1].AddKey(time, key2[l].value);
                            }
                            newAn.GetCurves()[1].AddKey(startTime + newBlendshape.length, keyValueLength.y);
                            ///newAn.GetCurves()[1].keys = key2.ToArray();


                        }

                        startTime +=10;
                    }
                }
			}
		}
	}
}

#endif