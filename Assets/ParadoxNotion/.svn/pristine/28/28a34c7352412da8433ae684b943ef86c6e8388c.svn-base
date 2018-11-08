/*
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Slate{

	///NOT USED ANYMORE///
	public static class CurveDrawer {

		public static float TimeToPos(float time, float width, float maxTime){
			return (time / maxTime) * width;
		}

		public static float PosToTime(float pos, float width, float maxTime){
			return (pos / width) * maxTime;
		}

		static AnimationCurve selectedCurve;
		static Dictionary<AnimationCurve, int> draggingKeyIndex = new Dictionary<AnimationCurve, int>();
		public static void DrawCurves(AnimationCurve[] curves, Rect posRect, Rect timeRect, Color infoColor, bool editable = true){

			if (curves == null || curves.Length == 0){
				return;
			}

			var e = Event.current;

			for (int i = 0; i < curves.Length; i++){
				var curveColor = Color.white;
				if (i == 0) curveColor = Color.red;
				if (i == 1) curveColor = Color.green;
				if (i == 2) curveColor = Color.blue;
				DrawCurve(curves[i], posRect, timeRect, curveColor, infoColor, editable);
			}

			if (editable){
				if (e.type == EventType.MouseUp && e.button == 1){
					var menu = new GenericMenu();
					var time = PosToTime(e.mousePosition.x, posRect.width, timeRect.width);
					menu.AddItem(new GUIContent("Add Key"), false, ()=> {
						foreach (var curve in curves){
							var doAdd = true;
							for (int i = 0; i < curve.keys.Length; i++){
								if ( Mathf.Abs(time - curve.keys[i].time) <= 0.5f ){
									doAdd = false;
									break;
								}
							}

							if (doAdd){
								curve.AddKey(time, curve.Evaluate(time));
							}
						}
					});
					menu.ShowAsContext();
					e.Use();
				}

				if (e.type == EventType.MouseDown && e.button == 0){
					selectedCurve = null;
				}
			}

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
		}

		public static void DrawCurve(AnimationCurve curve, Rect posRect, Rect timeRect, Color curveColor, Color infoColor, bool editable = true){

			if (curve == null){
				return;
			}

			var e = Event.current;
			var width = posRect.width;
			var height = posRect.height;
			var minTime = timeRect.x;
			var maxTime = timeRect.width;
			var minValue = timeRect.y;
			var maxValue = timeRect.height;

			for (int i = 0; i < curve.keys.Length; i ++){

				var key = curve.keys[i];
				var keyPos = new Vector3(TimeToPos(key.time, width, maxTime), height - TimeToPos(key.value, height, maxValue), 0);

				var nextKey = i == curve.keys.Length -1? key : curve.keys[i+1];
				var nextKeyPos = new Vector3(TimeToPos(nextKey.time, width, maxTime), height - TimeToPos(nextKey.value, height, maxValue), 0);

				var num = Mathf.Abs(nextKey.time - key.time) * 0.333333f;

				var vectorA = new Vector2( key.time + num, key.value + num * key.outTangent );
				var outTangentPos = new Vector3( TimeToPos(vectorA.x, width, maxTime), height - TimeToPos(vectorA.y, height, maxValue), 0);

				var vectorB = new Vector2( nextKey.time - num, nextKey.value - num * nextKey.inTangent);
				var inTangentPos = new Vector3( TimeToPos(vectorB.x, width, maxTime), height - TimeToPos(vectorB.y, height, maxValue), 0);

				curveColor.a = curve == selectedCurve? curveColor.a : 0.5f;
				if (i != curve.keys.Length-1){
					Handles.DrawBezier(keyPos, nextKeyPos, outTangentPos, inTangentPos, curveColor, null, 2f);
				}

				if (i == 0){
					Handles.DrawBezier(keyPos, new Vector3(0, keyPos.y, 0), keyPos, keyPos, curveColor, null, 2f);
				}

				if (i == curve.keys.Length-1){
					Handles.DrawBezier(keyPos, new Vector3(width, keyPos.y, 0), keyPos, keyPos, curveColor, null, 2f);
				}


				if (editable){

					var keyRect = new Rect(keyPos.x-5, keyPos.y-5, Styles.dopeKey.width, Styles.dopeKey.height);
					GUI.DrawTexture(keyRect, Styles.dopeKey);

					if (keyRect.Contains(e.mousePosition) && e.type == EventType.MouseDown){
						draggingKeyIndex[curve] = i;
						selectedCurve = curve;
						e.Use();
					}

					if (e.type == EventType.MouseUp && e.button == 1 && keyRect.Contains(e.mousePosition)){
						var menu = new GenericMenu();
						menu.AddItem(new GUIContent("Remove Key"), false, delegate(object index){curve.RemoveKey((int)index);}, i);
						menu.ShowAsContext();
						draggingKeyIndex.Clear();
						e.Use();
					}

					if (curve == selectedCurve){
						Handles.color = new Color(1,1,1,0.5f);
						var a = new Vector2(outTangentPos.x - keyPos.x, outTangentPos.y - keyPos.y).normalized * 40;
						var outTangentHandlePos = new Vector3(keyPos.x + a.x, keyPos.y + a.y, 0f);
						Handles.DrawLine(new Vector3(keyPos.x, keyPos.y, 0f), outTangentHandlePos);
						var handleRect = new Rect(0,0,10,10);
						handleRect.center = outTangentHandlePos;
						if (e.button == 0 && e.type == EventType.MouseDown && handleRect.Contains(e.mousePosition)){

						}

						if (i < curve.length -1){
							var b = new Vector2(inTangentPos.x - nextKeyPos.x, inTangentPos.y - nextKeyPos.y).normalized * 40;
							var inTangentHandlePos = new Vector3(nextKeyPos.x + b.x, nextKeyPos.y + b.y, 0f);
							Handles.DrawLine(new Vector3(nextKeyPos.x, nextKeyPos.y, 0), inTangentHandlePos);
						}
					}
				}
			}

			if (editable){

				if (e.type == EventType.MouseUp){
					draggingKeyIndex.Clear();
				}

				if (draggingKeyIndex.ContainsKey(curve) && draggingKeyIndex[curve] >= 0 && e.button == 0){

					var newKey = new Keyframe();
					newKey.time = curve[draggingKeyIndex[curve]].time;
					newKey.value = curve[draggingKeyIndex[curve]].value;
					newKey.inTangent = curve[draggingKeyIndex[curve]].inTangent;
					newKey.outTangent = curve[draggingKeyIndex[curve]].outTangent;

					var dragInfoRect = new Rect( TimeToPos(newKey.time, width, maxTime) + 20, height - TimeToPos(newKey.value, height, maxValue), 100, 100);
					GUI.color = infoColor;
					GUI.Label(dragInfoRect, string.Format("<size=8>{0}\n{1}</size>", newKey.time.ToString("0.00"), newKey.value.ToString("0.00")));
					GUI.color = Color.white;

					if (e.type == EventType.MouseDrag){
						newKey.time = PosToTime(e.mousePosition.x, width, maxTime);
						newKey.time = Mathf.Clamp(newKey.time, minTime, maxTime);

						newKey.value = maxValue - PosToTime(e.mousePosition.y, height, maxValue);
						newKey.value = Mathf.Clamp(newKey.value, minValue, maxValue);
						draggingKeyIndex[curve] = curve.MoveKey(draggingKeyIndex[curve], newKey);
					}
				}

				if (e.button == 0 && e.clickCount == 2){
					var eval = curve.Evaluate( PosToTime(e.mousePosition.x, width, maxTime) );
					var posY = TimeToPos(eval, height, maxValue);
					if (Mathf.Abs( (height - e.mousePosition.y) - posY) < 5){
						var time = PosToTime(e.mousePosition.x, width, maxTime);
						var index = curve.AddKey(new Keyframe(time, curve.Evaluate(time)));
						if (index > 0){
							curve.SmoothTangents(index, 0f);
						}
						e.Use();
					}
				}

				if (e.button == 0 && e.type == EventType.MouseDown){
					selectedCurve = null;
				}
			}

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
		}
	}
}

#endif
*/