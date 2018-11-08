#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
 
namespace Slate{

	public enum TangentMode
	{
		Editable,
		Smooth,
		Linear,
		Constant,
	}

 	//Wraps Unity's AnimationUtility class a bit better like exposing -for reasons unknown- internal methods.
 	public static class CurveUtility {

 		#if UNITY_5_5_OR_NEWER
 		private static Action<AnimationCurve> UpdateTangentsFromModeDelegate;
 		#else
 		private static Action<AnimationCurve, int> UpdateTangentsFromModeAtIndexDelegate;
 		#endif

 		private static Func<Keyframe, AnimationUtility.TangentMode> GetKeyLeftTangentModeDelefate;
 		private static Func<Keyframe, AnimationUtility.TangentMode> GetKeyRightTangentModeDelefate;
 		private static Func<Keyframe, bool> GetKeyBrokenDelegate;


 		static CurveUtility(){
 			var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
 			MethodInfo method = null;

			#if UNITY_5_5_OR_NEWER
			method = typeof(AnimationUtility).GetMethod("UpdateTangentsFromMode", flags);
			UpdateTangentsFromModeDelegate = method.RTCreateDelegate<Action<AnimationCurve>>(null);
			#else
			method = typeof(AnimationUtility).GetMethod("Internal_UpdateTangents", flags);
			UpdateTangentsFromModeAtIndexDelegate = method.RTCreateDelegate<Action<AnimationCurve, int>>(null);			
			#endif

			var prefix = string.Empty;
			#if !UNITY_5_5_OR_NEWER
			prefix = "Internal_";
			#endif

			method = typeof(AnimationUtility).GetMethod( prefix + "GetKeyLeftTangentMode", flags, null, new Type[]{typeof(Keyframe)}, null );
			GetKeyLeftTangentModeDelefate = method.RTCreateDelegate<Func<Keyframe, AnimationUtility.TangentMode>>(null);
			method = typeof(AnimationUtility).GetMethod( prefix + "GetKeyRightTangentMode", flags, null, new Type[]{typeof(Keyframe)}, null );
			GetKeyRightTangentModeDelefate = method.RTCreateDelegate<Func<Keyframe, AnimationUtility.TangentMode>>(null);
			method = typeof(AnimationUtility).GetMethod( prefix + "GetKeyBroken", flags, null, new Type[]{typeof(Keyframe)}, null );
			GetKeyBrokenDelegate = method.RTCreateDelegate<Func<Keyframe, bool>>(null);
 		}
 
		///...
		public static void UpdateTangentsFromMode(this AnimationCurve curve){
			
			#if UNITY_5_5_OR_NEWER

			UpdateTangentsFromModeDelegate(curve);
			if (curve.length > 1){
				var firstKey = curve[0];
				if (GetKeyTangentMode(firstKey) == TangentMode.Smooth){
					firstKey.inTangent = 0;
					firstKey.outTangent = 0;
					curve.MoveKey(0, firstKey);
				}
				var lastKey = curve[curve.length-1];
				if (GetKeyTangentMode(lastKey) == TangentMode.Smooth){
					lastKey.inTangent = 0;
					lastKey.outTangent = 0;
					curve.MoveKey(curve.length-1, lastKey);
				}
			}

			#else

			for (var i = 0; i < curve.length; i++) {
				UpdateTangentsFromModeAtIndexDelegate(curve, i);
				if (i == 0 || i + 1 == curve.length){
					var key = curve[i];
					if (GetKeyTangentMode(key) == TangentMode.Smooth ){
						key.inTangent = 0;
						key.outTangent = 0;
						curve.MoveKey(i, key);
					}
				}
			}


			#endif
		}

 
		///...
		public static void SetKeyTangentMode(this AnimationCurve curve, int index, TangentMode tangentMode){
			SetKeyLeftTangentMode(curve, index, tangentMode);
			SetKeyRightTangentMode(curve, index, tangentMode);
			SetKeyBroken(curve, index, false);
		}
 
		public static void SetKeyLeftTangentMode(AnimationCurve curve, int index,  TangentMode tangentMode){
			AnimationUtility.SetKeyLeftTangentMode(curve, index, (AnimationUtility.TangentMode)tangentMode);
		}

		public static void SetKeyRightTangentMode(AnimationCurve curve, int index,  TangentMode tangentMode){
			AnimationUtility.SetKeyRightTangentMode(curve, index, (AnimationUtility.TangentMode)tangentMode);
		}


		///...
		public static TangentMode GetKeyTangentMode(Keyframe keyframe){
			var leftTangent = GetKeyLeftTangentMode(keyframe);
			var rightTangent = GetKeyRightTangentMode(keyframe);
			if (leftTangent == rightTangent){
				return leftTangent;
			}
			return TangentMode.Editable;
		}

		public static TangentMode GetKeyLeftTangentMode(Keyframe keyframe){
			return (TangentMode)GetKeyLeftTangentModeDelefate(keyframe);
		}

		public static TangentMode GetKeyRightTangentMode(Keyframe keyframe){
			return (TangentMode)GetKeyRightTangentModeDelefate(keyframe);
		}
 
 
		///...
		public static Keyframe SetKeyBroken(this AnimationCurve curve, int index, bool broken){
			AnimationUtility.SetKeyBroken(curve, index, broken);
			return curve[index];
		}

		public static bool GetKeyBroken(Keyframe keyframe){
			return GetKeyBrokenDelegate(keyframe);
		}
	}
}

#endif