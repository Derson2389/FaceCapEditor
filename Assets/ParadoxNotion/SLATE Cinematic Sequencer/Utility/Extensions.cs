using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Slate{

	public static class TransformExtensions {

		public static Vector3 GetLocalEulerAngles(this Transform transform){

			if (Application.isPlaying){
				return transform.localEulerAngles;
			}
			
			#if UNITY_EDITOR && UNITY_5_4_OR_NEWER

			var t = typeof(Transform);
			var m = t.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
			var p = t.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
			return (Vector3)m.Invoke(transform, new object[]{ p.GetValue(transform, null) });
			
			#else

			return transform.localEulerAngles;

			#endif
		}

		public static void SetLocalEulerAngles(this Transform transform, Vector3 value){
			
			if (Application.isPlaying){
				transform.localEulerAngles = value;
				return;
			}

			#if UNITY_EDITOR && UNITY_5_4_OR_NEWER
			
			var t = typeof(Transform);
			var m = t.GetMethod("SetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
			var p = t.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
			m.Invoke(transform, new object[]{ value, p.GetValue(transform, null) });

			#else

			transform.localEulerAngles = value;

			#endif
		}

		///Gets existing T component or adds new one if not exists
		public static T GetAddComponent<T>(this GameObject go) where T:Component{
			return GetAddComponent(go, typeof(T)) as T;
		}

		///Gets existing T component or adds new one if not exists
		public static T GetAddComponent<T>(this Component comp) where T:Component{
			return GetAddComponent(comp.gameObject, typeof(T)) as T;
		}

		///Gets existing component or adds new one if not exists
		public static Component GetAddComponent(this GameObject go, System.Type type){
			var result = go.GetComponent(type);
			if (result == null){
				result = go.AddComponent(type);
			}
			return result;			
		}

		///Find transform with name within all children of root
		public static Transform FindInChildren(this Transform root, string name, bool includeHidden){
			if (root == null || string.IsNullOrEmpty(name)){
				return root;
			}

			return root.GetComponentsInChildren<Transform>(includeHidden).FirstOrDefault(t => t.name.ToUpper() == name.ToUpper());
		}


		///Get all shape names of a skinned mesh
		public static List<string> GetBlendShapeNames(this SkinnedMeshRenderer skinnedMesh){
			var result = new List<string>();
			if (skinnedMesh == null) return result;
			for (int i = 0; i < skinnedMesh.sharedMesh.blendShapeCount; i++){
				result.Add( skinnedMesh.sharedMesh.GetBlendShapeName(i) );
			}
			return result;
		}

		///Get the index of a shape name
		public static int GetBlendShapeIndex(this SkinnedMeshRenderer skinnedMesh, string shapeName){
			if (skinnedMesh == null) return -1;
			for (int i = 0; i < skinnedMesh.sharedMesh.blendShapeCount; i++){
				if (skinnedMesh.sharedMesh.GetBlendShapeName(i) == shapeName){
					return i;
				}
			}
			return -1;
		}


		///Convert camelCase to words as the name implies.
		public static string SplitCamelCase(this string s){
			if (string.IsNullOrEmpty(s)) return s;
			s = char.ToUpper(s[0]) + s.Substring(1);
			return System.Text.RegularExpressions.Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1").Trim();
		}


		///rect a fully encapsulated b?
		public static bool Encapsulates(this Rect a, Rect b){
			return a.x < b.x && a.xMax > b.xMax && a.y < b.y && a.yMax > b.yMax;
		}


		//get a screen space rect from camera, out of bounds provided
		public static Rect ToViewRect(this Bounds b, Camera cam){

		    var distance = cam.WorldToViewportPoint(b.center).z;
		    //The object is behind us
		    if (distance < 0){
		    	return new Rect();
		    }

		    //All 8 vertices of the bounds
			Vector2[] pts = new Vector2[8];
		    pts[0] = cam.WorldToViewportPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
		    pts[1] = cam.WorldToViewportPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
		    pts[2] = cam.WorldToViewportPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
		    pts[3] = cam.WorldToViewportPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
		    pts[4] = cam.WorldToViewportPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
		    pts[5] = cam.WorldToViewportPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
		    pts[6] = cam.WorldToViewportPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
		    pts[7] = cam.WorldToViewportPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
		 
		    // Calculate the min and max positions
		    Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		    Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
		    for (var i = 0; i < pts.Length; i++){
			    // Get them in GUI space
		    	pts[i].y = 1-pts[i].y;
		        min = Vector2.Min(min, pts[i]);
		        max = Vector2.Max(max, pts[i]);
		    }
		 	
			return Rect.MinMaxRect(min.x,min.y,max.x,max.y);
		}

	}
}