#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace Slate{

	///A collections of tools for the editor only
	public static class EditorTools {

		public static void Header(string text){
			GUI.backgroundColor = new Color(0, 0, 0, 0.3f);
			GUILayout.BeginVertical(Slate.Styles.headerBoxStyle);
			GUILayout.Label(string.Format("<b>{0}</b>", text));
			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
		}

		public static void BoldSeparator(){
			var tex = Slate.Styles.whiteTexture;
			var lastRect= GUILayoutUtility.GetLastRect();
			GUILayout.Space(14);
			GUI.color = new Color(0, 0, 0, 0.25f);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 1), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 9, Screen.width, 1), tex);
			GUI.color = Color.white;
		}

		///Gets the GameView panel's resolution size
		public static Vector2 GetGameViewSize(){
		    return Handles.GetMainGameViewSize();
		}
		// @modify slate sequencer
        // @TQ
        public static void ShowAnimatedPropertySelectionMenuForEffect(string ComponetName , GameObject go, System.Type[] paramTypes, System.Action<MemberInfo, Component> callback)
        {

            var menu = new GenericMenu();
            foreach (var _comp in go.GetComponentsInChildren<Component>(true))
            {
                var comp = _comp;

                if (comp == null)
                {
                    continue;
                }

                //var path = AnimationUtility.CalculateTransformPath(comp.transform, go.transform);
                if (comp.gameObject != go)
                {
                    //path = "Children/" + path;
                }
                else
                {
                   // path = "Self";
                }

                if (comp is Transform)
                {
                    //menu.AddItem(new GUIContent(path + "/Transform/Position"), false, () => { callback(typeof(Transform).GetProperty("localPosition"), comp); });
                    //menu.AddItem(new GUIContent(path + "/Transform/Rotation"), false, () => { callback(typeof(Transform).GetProperty("localEulerAngles"), comp); });
                    //menu.AddItem(new GUIContent(path + "/Transform/Scale"), false, () => { callback(typeof(Transform).GetProperty("localScale"), comp); });
                    continue;
                }

                var type = comp.GetType();
                foreach (var _prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var prop = _prop;

                    if (!prop.CanRead || !prop.CanWrite)
                    {
                        continue;
                    }

                    if (!paramTypes.Contains(prop.PropertyType))
                    {
                        continue;
                    }

                    var finalPath =  prop.Name.SplitCamelCase();
                    if (ComponetName == type.Name.SplitCamelCase())
                    {
                        menu.AddItem(new GUIContent(finalPath), false, () => { callback(prop, comp); });
                    }
                    
                }

                foreach (var _field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    var field = _field;
                    if (paramTypes.Contains(field.FieldType))
                    {
                        var finalPath = field.Name.SplitCamelCase();
                        if (ComponetName == type.Name.SplitCamelCase())
                        {
                            menu.AddItem(new GUIContent(finalPath), false, () => { callback(field, comp); });
                        }
                    }
                }
            }

            menu.ShowAsContext();
            Event.current.Use();
        }
        //end

        public static Stack<object> GetPath(string path)
        {
            Stack<object> obj;
            if(pathrecord.TryGetValue(path, out obj))
            {
                return obj;
            }
            return null;
        }

        private static Dictionary<string, Stack<object>> pathrecord = new Dictionary<string, Stack<object>>();
        ///Pops a menu for animatable properties selection
        public static void ShowAnimatedPropertySelectionMenu(GameObject go, System.Type[] paramTypes, System.Action<MemberInfo, Component, string, Stack<object>> callback)
        {
            pathrecord.Clear();
            Stack<object> traces = new Stack<object>();
			var menu = new GenericMenu();
			foreach (var _comp in go.GetComponentsInChildren<Component>(true)){
				var comp = _comp;

				if (comp == null){
					continue;
				}

                traces.Push(comp);
                var path = AnimationUtility.CalculateTransformPath(comp.transform, go.transform);
				if (comp.gameObject != go){
					path = "Children/" + path;
				} else {
					path = "Self";
				}

				if (comp is Transform){
					menu.AddItem(new GUIContent(path + "/Transform/Position"), false, ()=>{ callback( typeof(Transform).GetProperty("localPosition"), comp, path, traces );} );
					menu.AddItem(new GUIContent(path + "/Transform/Rotation"), false, ()=>{ callback( typeof(Transform).GetProperty("localEulerAngles"), comp, path, traces);} );
					menu.AddItem(new GUIContent(path + "/Transform/Scale"), false, ()=>{ callback( typeof(Transform).GetProperty("localScale"), comp, path, traces);} );
                    traces.Pop();
					continue;
				}

				var type = comp.GetType();
				foreach (var _prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)){
					var prop = _prop;

					if (!prop.CanRead || !prop.CanWrite){
						continue;
					}

					if (!paramTypes.Contains(prop.PropertyType)){
						continue;
					}

                    var tempTraces = new Stack<object>(traces);
                    tempTraces = new Stack<object>(tempTraces);
                    var finalPath = string.Format("{0}/{1}/{2}", path, type.Name.SplitCamelCase(), prop.Name.SplitCamelCase());

                    pathrecord.Add(finalPath, tempTraces);
                    menu.AddItem(new GUIContent(finalPath), false, ()=>{ callback(prop, comp, finalPath, tempTraces); } );
				}

				foreach (var _field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)){
					var field = _field;
                    // @modify slate sequencer
                    // @CYS
                    var fieldt = field.FieldType;
                    var isDef = fieldt.IsDefined(typeof(AnimatedClass), false);
					if (paramTypes.Contains(field.FieldType)){

                        var tempTraces = new Stack<object>(traces);
                        tempTraces = new Stack<object>(traces);
                        var finalPath = string.Format("{0}/{1}/{2}", path, type.Name.SplitCamelCase(), field.Name.SplitCamelCase());
                        pathrecord.Add(finalPath, tempTraces);
                        menu.AddItem(new GUIContent(finalPath), false, ()=>{ callback(field, comp, finalPath, tempTraces); });
					}else if ( isDef)
                    {
                        traces.Push(field);
                        var Expath = string.Format("{0}/{1}/{2}({3})", path, type.Name.SplitCamelCase(),fieldt.Name.SplitCamelCase(), field.Name);
                        ShowScriptAnimatedPropertySelectionMenu(fieldt, menu, paramTypes, callback, Expath, comp, traces);
                        traces.Pop();
                    }
                    ///end
				}

                traces.Pop();
            }

			menu.ShowAsContext();
			Event.current.Use();
		}
        // @modify slate sequencer
        // @CYS
        public static void ShowScriptAnimatedPropertySelectionMenu(Type so, GenericMenu menu, System.Type[] paramTypes, System.Action<MemberInfo, Component, string, Stack<object>> callback, string path, Component comp, Stack<object> traces)
        {
            traces = new Stack<object>(traces);
            traces = new Stack<object>(traces);
            var type = so;
            foreach (var _prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var prop = _prop;

                if (!prop.CanRead || !prop.CanWrite)
                {
                    continue;
                }

                var ptype = prop.PropertyType;

                bool isDef = ptype.GetCustomAttributes(typeof(AnimatedClass), false).Length > 0;
                var Expath = string.Format("{0}/{1}({2})", path, ptype.Name,prop.Name.SplitCamelCase());
                if( isDef)
                {
                    traces.Push(prop);
                    ShowScriptAnimatedPropertySelectionMenu(ptype, menu, paramTypes, callback, Expath, comp, traces);
                    traces.Pop();
                }

                if (!paramTypes.Contains(prop.PropertyType))
                {
                    continue;
                }

                var tempTraces = new Stack<object>(traces);
                tempTraces = new Stack<object>(tempTraces);
                var finalPath = string.Format("{0}/{1}", path, prop.Name.SplitCamelCase());
                pathrecord.Add(finalPath, tempTraces);
                menu.AddItem(new GUIContent(finalPath), false, () => { callback(prop, comp, finalPath, tempTraces); });
            }

            foreach (var _field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var field = _field;
                var fieldt = field.FieldType;
                var isDef = fieldt.IsDefined(typeof(AnimatedClass), false);
                if (paramTypes.Contains(field.FieldType))
                {
                    var finalPath = string.Format("{0}/{1}", path, field.Name.SplitCamelCase());
                    var tempTraces = new Stack<object>(traces);
                    tempTraces = new Stack<object>(tempTraces);
                    pathrecord.Add(finalPath, tempTraces);
                    menu.AddItem(new GUIContent(finalPath), false, () => { callback(field, comp, finalPath, tempTraces); });
                }
                else if (isDef)
                {
                    traces.Push(field);
                    var Expath = string.Format("{0}/{1}({2})", path, fieldt.Name.SplitCamelCase(), field.Name);
                    ShowScriptAnimatedPropertySelectionMenu(fieldt, menu, paramTypes, callback, Expath, comp, traces);
                    traces.Pop();

                }
            }

            //menu.ShowAsContext();
            //Event.current.Use();
        }
		///end

        ///Generic Popup for selection of any element within a list
		public static T Popup<T>(string prefix, T selected, List<T> options, params GUILayoutOption[] GUIOptions){

			var index = 0;
			if (options.Contains(selected)){
				index = options.IndexOf(selected) + 1;
			}

			var stringedOptions = new List<string>();
			if (options.Count == 0){
				stringedOptions.Add("NONE AVAILABLE");
			} else {
				stringedOptions.Add("NONE");
				stringedOptions.AddRange( options.Select(o => o != null? o.ToString() : "NONE" ) );
			}

			GUI.enabled = stringedOptions.Count > 1;
			if (!string.IsNullOrEmpty(prefix)) index = EditorGUILayout.Popup(prefix, index, stringedOptions.ToArray(), GUIOptions);
			else index = EditorGUILayout.Popup(index, stringedOptions.ToArray(), GUIOptions);
			GUI.enabled = true;

			return index == 0? default(T) : options[index - 1];
		}

		///Generic Popup for selection of any element within a list without adding NONE
		public static T CleanPopup<T>(string prefix, T selected, List<T> options, params GUILayoutOption[] GUIOptions){

			var index = -1;
			if (options.Contains(selected)){
				index = options.IndexOf(selected);
			}

			var stringedOptions = options.Select(o => o != null? o.ToString() : "NONE" );

			GUI.enabled = options.Count > 0;
			if (!string.IsNullOrEmpty(prefix)) index = EditorGUILayout.Popup(prefix, index, stringedOptions.ToArray(), GUIOptions);
			else index = EditorGUILayout.Popup(index, stringedOptions.ToArray(), GUIOptions);
			GUI.enabled = true;

			return index == -1? default(T) : options[index];
		}


		public struct TypeMetaInfo{
			public Type type;
			public string name;
			public string category;
			public Type[] attachableTypes;
			public bool isUnique;
		}

		//Get all non abstract derived types of base type in the current loaded assemplies
		public static List<TypeMetaInfo> GetTypeMetaDerivedFrom(Type baseType){
			var infos = new List<TypeMetaInfo>();
			foreach(var type in ReflectionTools.GetDerivedTypesOf(baseType)){
				
				if (type.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).FirstOrDefault() != null){
					continue;
				}

				var info = new TypeMetaInfo();
				info.type = type;

				var nameAtt = type.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
				info.name = nameAtt != null? nameAtt.name : type.Name.SplitCamelCase();

				var catAtt = type.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
				if (catAtt != null){ info.category = catAtt.category; }

				var attachAtt = type.GetCustomAttributes(typeof(AttachableAttribute), true).FirstOrDefault() as AttachableAttribute;
				if (attachAtt != null){ info.attachableTypes = attachAtt.types; }

				info.isUnique = type.GetCustomAttributes(typeof(UniqueElementAttribute), true).FirstOrDefault() as UniqueElementAttribute != null;

				infos.Add(info);
			}
			
			infos = infos.OrderBy(i => i.name).OrderBy(i => i.category).ToList();
			return infos;
		}


		///Fold States per object
		private static Dictionary<object, AnimBool> foldOutStates = new Dictionary<object, AnimBool>();
		///Get an object's fold state
		public static float GetObjectFoldOutFaded(object o){
			AnimBool fold = null;
			return foldOutStates.TryGetValue(o, out fold)? fold.faded : (foldOutStates[o] = new AnimBool(false)).faded;
		}
		public static bool GetObjectFoldOut(object o){
			AnimBool fold = null;
			return foldOutStates.TryGetValue(o, out fold)? fold.target : (foldOutStates[o] = new AnimBool(false)).target;
		}
		///Set an object's fold state
		public static void SetObjectFoldOut(object o, bool value){
			AnimBool fold = null;
			if (foldOutStates.TryGetValue(o, out fold)){
				fold.target = value;
				return;
			}
			fold = new AnimBool(value);
			fold.target = value;
			foldOutStates[o] = fold;
		}


		///Get a texture from an audio clip
		private static Dictionary<AudioClip, Texture2D> audioTextures = new Dictionary<AudioClip, Texture2D>();
		public static Texture2D GetAudioClipTexture(AudioClip clip, int width, int height){
			
			if (clip == null){
				return null;
			}

			//do this?
			width = 8192;

			Texture2D texture = null;
			if (audioTextures.TryGetValue(clip, out texture)){
				if (texture != null){
					return texture;
				}
			}

			if (clip.loadType != AudioClipLoadType.DecompressOnLoad){
				audioTextures[clip] = Slate.Styles.whiteTexture;
				Debug.LogWarning(string.Format("Can't get preview audio texture from audio clip '{0}' because it's load type is set to compressed", clip.name), clip);
				return null;
			}

			texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
	        float[] samples = new float[clip.samples * clip.channels];
	        int step = Mathf.CeilToInt((clip.samples * clip.channels) / width);
	        clip.GetData(samples, 0);
	        Color[] xy = new Color[width * height];
	        for (int x = 0; x < width * height; x++){
	            xy[x] = new Color(0, 0, 0, 0);
	        }
	 
	        texture.SetPixels(xy);
	 
	        int i = 0;
	        while (i < width){
	            int barHeight = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(samples[i * step]) * height, 0, height));
	            int add = samples[i * step] > 0 ? 1 : -1;
	            for (int j = 0; j < barHeight; j++){
	                texture.SetPixel(i, Mathf.FloorToInt(height / 2) - (Mathf.FloorToInt(barHeight / 2) * add) + (j * add), Color.white);
	            }
	            ++i;
	 
	        }
	 
	        texture.Apply();
			audioTextures[clip] = texture;
			return texture;
		}


		///Draws a looped audio clip texture within Rect and within provided maxLength (with optional offset).
		public static void DrawLoopedAudioTexture(Rect rect, AudioClip audioClip, float maxLength, float offset){
			
			if (audioClip == null){
				return;
			}

			var audioRect = rect;
			audioRect.width = (audioClip.length/maxLength) * rect.width;
			var t = GetAudioClipTexture(audioClip, (int)audioRect.width, (int)audioRect.height);
			if (t != null){
				Handles.color = new Color(0,0,0,0.2f);
				GUI.color = new Color(0.4f, 0.435f, 0.576f);
				audioRect.yMin += 2;
				audioRect.yMax -= 2;
				for (var f = offset; f < maxLength; f += audioClip.length){
					audioRect.x = (f/maxLength) * rect.width;
					rect.x = audioRect.x;
					GUI.DrawTexture(audioRect, t);
					Handles.DrawLine(new Vector2( rect.x, 0 ), new Vector2( rect.x, rect.height ));
				}
				Handles.color = Color.white;
				GUI.color = Color.white;
			}			
		}

		public static void DrawLoopedLines(Rect rect, float length, float maxLength, float offset){
			if (length != 0 && maxLength != 0){
				length = Mathf.Abs(length);
				maxLength = Mathf.Abs(maxLength);
				UnityEditor.Handles.color = new Color(0,0,0,0.2f);
				for (var f = offset; f < maxLength; f += length){
					var posX = (f/maxLength) * rect.width;
					UnityEditor.Handles.DrawLine(new Vector2( posX, 0 ), new Vector2( posX, rect.height ));
				}
				UnityEditor.Handles.color = Color.white;
			}		
		}
	}
}

#endif