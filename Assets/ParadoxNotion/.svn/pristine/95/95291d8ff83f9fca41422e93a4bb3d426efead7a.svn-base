using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Slate{

	///Creates a snapshot of an object to be restored later on.
	public class ObjectSnapshot{

		private Dictionary<Object, string> serialized;

		///Create new ObjectSnapShot storing target object
		public ObjectSnapshot(Object target, bool fullObjectHierarchy = false){
			Store(target, fullObjectHierarchy);
		}

		public void Store(Object target, bool fullObjectHierarchy = false){
			
			if (target == null){
				return;
			}

			serialized = new Dictionary<Object, string>();

			if (target is MonoBehaviour || target is ScriptableObject){
				serialized[target] = JsonUtility.ToJson(target);
			// } else {
			// 	#if UNITY_EDITOR
			// 	if (!Application.isPlaying){
			// 		serialized[target] = UnityEditor.EditorJsonUtility.ToJson(target);
			// 	}
			// 	#endif
			}
			
			if (target is GameObject){
				var go = (GameObject)target;
				var components = fullObjectHierarchy? go.GetComponentsInChildren<Component>(true) : go.GetComponents<Component>();
				for (var i = 0; i < components.Length; i++){
					var component = components[i];
					if (component != null){ //for MissingScript just in case user has those
						if (component is MonoBehaviour){
							serialized[component] = JsonUtility.ToJson(component);
							continue;
						}
						// #if UNITY_EDITOR
						// if (!Application.isPlaying){
						// 	serialized[component] = UnityEditor.EditorJsonUtility.ToJson(component);
						// }
						// #endif
					}
				}
			}
		}

		public void Restore(){
			foreach(var pair in serialized){
				if (pair.Key != null){
					if (pair.Key is MonoBehaviour || pair.Key is ScriptableObject){
						JsonUtility.FromJsonOverwrite(pair.Value, pair.Key);
						continue;
					}
					// #if UNITY_EDITOR
					// if (!Application.isPlaying){
					// 	UnityEditor.EditorJsonUtility.FromJsonOverwrite(pair.Value, pair.Key);
					// }
					// #endif
				}
			}
		}
	}
}