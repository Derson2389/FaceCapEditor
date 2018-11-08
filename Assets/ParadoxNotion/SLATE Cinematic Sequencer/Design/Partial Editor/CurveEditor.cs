#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Slate{

	///A curve editor and renderer using Unity's native one by reflection
	public static class CurveEditor {

		///Raised when CurveEditor modifies curves, with argument being the IAnimatable the curves belong to.
		public static event Action<IAnimatableData> onCurvesUpdated;
	    /// @modify slate sequencer
	    /// @TQ
        public static List<AnimatedParameter> _choosedList = new List<AnimatedParameter>();
        public static AnimationCurve[] curveArr = null;
        /// @modify slate sequencer
        /// add by TQ
        //public static bool refreshCurves = true;

        public delegate void onDragCurves(IKeyable _keyAble);
        public static onDragCurves onDragCurvesDelegates = null;
        /// end

        /// end
        private static Dictionary<IAnimatableData, CurveRenderer> cache = new Dictionary<IAnimatableData, CurveRenderer>();
		public static void DrawCurves(IAnimatableData animatable, IKeyable keyable, Rect posRect, Rect timeRect, CutsceneTrack ct = null){
			CurveRenderer instance = null;
		/// @modify slate sequencer
	    /// @TQ
            if (!cache.TryGetValue(animatable, out instance))
            {
                cache[animatable] = instance = new CurveRenderer(animatable, keyable, posRect);
            }
            else
            {
                if (ct!=null && !ct.refreshCurves)
                {
                    instance.ReInitCurves(animatable.GetCurves());
                    cache[animatable] = instance;
                    FrameAllCurvesOf(animatable);
                    ct.refreshCurves = true;
/// @modify slate sequencer
/// add by TQ
                    //onDragCurvesDelegates =  ct.DragCurvesSysncOthers;
/// end
                }
            }
		/// end
			instance.Draw(posRect, timeRect);
		}

		/// @modify slate sequencer
	    /// @TQ
        public static void DrawCurvesMutiply(IAnimatableData animatable,List<AnimatedParameter> animatableList,List<int> animatableIds , IKeyable keyable, Rect posRect, Rect timeRect, CutsceneTrack ct = null)
        {
            CurveRenderer instance = null;
            _choosedList.Clear();
            for (int i = 0; i< animatableIds.Count; i++)
            {
                _choosedList.Add(animatableList[animatableIds[i]]);
            }
            if (!cache.TryGetValue(animatable, out instance))
            {
                cache[animatable] = instance = new CurveRenderer(animatable, _choosedList, keyable, posRect);
            }
            else
            {
                //refresh curve
                if (ct != null && !ct.refreshCurves)
                {
                    curveArr = new AnimationCurve[0];
                    ArrayUtility.AddRange<AnimationCurve>(ref curveArr, animatable.GetCurves());
                    for (int i = 0; i < _choosedList.Count; i++)
                    {
                        ArrayUtility.AddRange<AnimationCurve>(ref curveArr, _choosedList[i].GetCurves());
                    }
                    instance.ReInitCurves(curveArr);
                    cache[animatable] = instance;
                    FrameAllCurvesOf(animatable);
                    ct.refreshCurves = true;
                    curveArr = null;
/// @modify slate sequencer
/// add by TQ
                 //   onDragCurvesDelegates = ct.DragCurvesSysncOthers;
/// end
                }
            }     
            instance.Draw(posRect, timeRect);
        }
		///end
        static CurveEditor(){
			AnimatedParameter.onParameterChanged         += RefreshCurvesOf;
			DopeSheetEditor.onCurvesUpdated              += RefreshCurvesOf;
			CurveEditor3D.onCurvesUpdated                += RefreshCurvesOf;
			CutsceneUtility.onRefreshAllAnimationEditors += RefreshCurvesOf;
		}

		///Refresh curves of target animatable
		static void RefreshCurvesOf(IAnimatableData animatable){
			CurveRenderer curveRenderer = null;
			if (cache.TryGetValue(animatable, out curveRenderer)){
				curveRenderer.RefreshCurves();
				return;
			}

			if (animatable is AnimationDataCollection){
				var data = (AnimationDataCollection)animatable;
				if (data.animatedParameters != null){
					foreach(var animParam in data.animatedParameters ){
						if (cache.TryGetValue(animParam, out curveRenderer)){
							curveRenderer.RefreshCurves();
						}
					}
				}
			}
		}


		public static void FrameAllCurvesOf(IAnimatableData animatable){
			CurveRenderer instance = null;
			if (!cache.TryGetValue(animatable, out instance)){
				return;
			}
			instance.RecalculateBounds();
			instance.FrameClip(true, true);
		}


		///The actual class responsible
		class CurveRenderer{

			public IAnimatableData animatable;
		/// @modify slate sequencer
	    /// @TQ
            public List<AnimatedParameter> animatableList;
		/// end
            public IKeyable keyable;

			private AnimationCurve[] curves;
			private Rect posRect;
			private Rect timeRect;

			private static Assembly editorAssembly;
			private static Type cEditorType;
			private static Type cRendererType;
			private static Type cWrapperType;
			private static ConstructorInfo cEditorCTR;

			private object cEditor;

			public CurveRenderer(IAnimatableData animatable, IKeyable keyable, Rect posRect){
				this.animatable = animatable;
				this.keyable = keyable;
				this.curves = animatable.GetCurves();
				this.posRect = posRect;
				Undo.undoRedoPerformed += ()=>{ RefreshCurves(); };
				Init();
			}

			/// @modify slate sequencer
	    	/// @TQ
            public void ReInitCurves(AnimationCurve[] curveArr)
            {
                this.curves = curveArr;
            }

            public CurveRenderer(IAnimatableData animatable, List<AnimatedParameter> animatableList, IKeyable keyable, Rect posRect)
            {
                this.animatable = animatable;
                this.animatableList = animatableList;
                this.keyable = keyable;
                AnimationCurve [] curveArr = new AnimationCurve[0];
                for (int i = 0; i < animatableList.Count; i++)
                {
                    ArrayUtility.AddRange<AnimationCurve>(ref curveArr, animatableList[i].GetCurves());

                }
                this.curves = curveArr;
                this.posRect = posRect;
                Undo.undoRedoPerformed += () => { RefreshCurves(); };
                Init();
            }
			///end
            public void Init(){
				//init meta info
				editorAssembly = typeof(Editor).Assembly;
				cEditorType    = editorAssembly.GetType("UnityEditor.CurveEditor");
				cRendererType  = editorAssembly.GetType("UnityEditor.NormalCurveRenderer");
				cWrapperType   = editorAssembly.GetType("UnityEditor.CurveWrapper");
				cEditorCTR     = cEditorType.GetConstructor(new Type[]{typeof(Rect), cWrapperType.MakeArrayType(), typeof(bool)});

				//create curve editor with wrappers
				var wrapperArray = GetCurveWrapperArray(curves);
				cEditor = cEditorCTR.Invoke(new object[]{ posRect, wrapperArray, true } );

				CreateDelegates();

				//set settings
				#if UNITY_5_4_OR_NEWER
				var settings = GetCurveEditorSettings();
				cEditorType.GetProperty("settings").SetValue(cEditor, settings, null);
				#else
				hRangeLocked = true;
				vRangeLocked = false;
				hRangeMin = 0;
				hSlider = false;
				vSlider = true;
				#endif				

				invSnap = 1f/Prefs.snapInterval;
				lastSnapPref = Prefs.snapInterval;
				ignoreScrollWheelUntilClicked = true;				


				RecalculateBounds();
				FrameClip(true, true);
			}

			private Action onGUI;
			private float lastSnapPref;

			private Action<Rect> rectSetter;
			private Func<Rect> rectGetter;
			
			private Action<Rect> shownAreaSetter;
			private Func<Rect> shownAreaGetter;

			private Action<float> hRangeMaxSetter;
			private Func<float> hRangeMaxGetter;

		/// @modify slate sequencer
	    /// @TQ
            private Color GetBalancedColor(Color c)
            {
                return new Color(0.15f + 0.75f * c.r, 0.2f + 0.6f * c.g, 0.1f + 0.9f * c.b);
            }
		///end
            Array GetCurveWrapperArray(AnimationCurve[] curves){
				if (curves == null){
					return Array.CreateInstance(cWrapperType, 0);
				}

				var wrapperArray = Array.CreateInstance(cWrapperType, curves.Length);
				for (var i = 0; i < curves.Length; i++){
					var curve = curves[i];
					var cWrapper = Activator.CreateInstance(cWrapperType);
					
					var clr = Color.white;
					/// @modify slate sequencer
				    /// @TQ
                    if (i <= 4)
                    {
                        if (i % 3 == 2) clr = Color.red;
                        if (i % 3 == 1) clr = Color.green;
                        if (i % 3 == 0) clr = Color.blue;
                        if (i == 3) clr = Color.white;
                    }
                    else
                    {
                        float num2 = 6.28318548f * (float)((this.animatable.GetType().GetHashCode()) % 1000);
                        num2 -= Mathf.Floor(num2);
                        clr = GetBalancedColor(Color.HSVToRGB(num2, 1f, 1f));
                    } 
         			/// end
                    cWrapperType.GetField("color").SetValue(cWrapper, clr);
					cWrapperType.GetField("id").SetValue(cWrapper, i);
					var cRenderer = Activator.CreateInstance(cRendererType, new object[]{curve} );
					cWrapperType.GetProperty("renderer").SetValue(cWrapper, cRenderer, null);
					
					var setWrapMethod = cRendererType.GetMethod("SetWrap", new Type[]{ typeof(WrapMode), typeof(WrapMode) });
					setWrapMethod.Invoke(cRenderer, new object[]{ curve.preWrapMode, curve.postWrapMode });

					wrapperArray.SetValue(cWrapper, i);
				}

				return wrapperArray;
			}

			object GetCurveEditorSettings(){
				var settingsType = editorAssembly.GetType("UnityEditor.CurveEditorSettings");
				var settings = Activator.CreateInstance(settingsType);
				settingsType.GetField("allowDraggingCurvesAndRegions").SetValue(settings, false);
				settingsType.GetField("allowDeleteLastKeyInCurve").SetValue(settings, true);

				#if UNITY_5_5_OR_NEWER
				settingsType.GetField("undoRedoSelection").SetValue(settings, true);
				settingsType.GetField("rectangleToolFlags", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(settings, 1);
				#endif

				settingsType.GetProperty("hRangeLocked", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(settings, true, null);
				settingsType.GetProperty("vRangeLocked", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(settings, false, null);
				settingsType.GetProperty("hSlider").SetValue(settings, false, null);
				settingsType.GetProperty("vSlider").SetValue(settings, true, null);
				settingsType.GetProperty("hRangeMin").SetValue(settings, 0, null);

				return settings;
			}


			//create delegates for some properties and methods for performance
			void CreateDelegates(){

				onGUI = cEditorType.GetMethod("OnGUI").RTCreateDelegate<Action>(cEditor);

				rectSetter = cEditorType.GetProperty("rect").GetSetMethod().RTCreateDelegate<Action<Rect>>(cEditor);
				rectGetter = cEditorType.GetProperty("rect").GetGetMethod().RTCreateDelegate<Func<Rect>>(cEditor);

				shownAreaSetter = cEditorType.GetProperty("shownArea").GetSetMethod().RTCreateDelegate<Action<Rect>>(cEditor);
				shownAreaGetter = cEditorType.GetProperty("shownArea").GetGetMethod().RTCreateDelegate<Func<Rect>>(cEditor);

				hRangeMaxSetter = cEditorType.GetProperty("hRangeMax").GetSetMethod().RTCreateDelegate<Action<float>>(cEditor);
				hRangeMaxGetter = cEditorType.GetProperty("hRangeMax").GetGetMethod().RTCreateDelegate<Func<float>>(cEditor);


				//Append OnCurvesUpdated to curve editor event
				var field = cEditorType.GetField("curvesUpdated");
				var methodInfo = this.GetType().GetMethod("OnCurvesUpdated", BindingFlags.Instance | BindingFlags.NonPublic);
				Delegate handler = Delegate.CreateDelegate(field.FieldType, this, methodInfo);
				field.SetValue(cEditor, handler);
			}


			public Rect rect{
				get {return rectGetter();}
				set {rectSetter(value);}
			}

			public Rect shownArea{
				get {return shownAreaGetter();}
				set {shownAreaSetter(value);}
			}

			public bool ignoreScrollWheelUntilClicked{
				get {return (bool)cEditorType.GetProperty("ignoreScrollWheelUntilClicked").GetValue(cEditor, null);}
				set {cEditorType.GetProperty("ignoreScrollWheelUntilClicked").SetValue(cEditor, value, null);}
			}

			public bool hRangeLocked{
				get {return (bool)cEditorType.GetProperty("hRangeLocked").GetValue(cEditor, null);}
				set {cEditorType.GetProperty("hRangeLocked").SetValue(cEditor, value, null);}
			}

			public bool vRangeLocked{
				get {return (bool)cEditorType.GetProperty("vRangeLocked").GetValue(cEditor, null);}
				set {cEditorType.GetProperty("vRangeLocked").SetValue(cEditor, value, null);}
			}

			public bool hSlider{
				get {return (bool)cEditorType.GetProperty("hSlider").GetValue(cEditor, null);}
				set {cEditorType.GetProperty("hSlider").SetValue(cEditor, value, null);}
			}

			public bool vSlider{
				get {return (bool)cEditorType.GetProperty("vSlider").GetValue(cEditor, null);}
				set {cEditorType.GetProperty("vSlider").SetValue(cEditor, value, null);}
			}

			public float hRangeMin{
				get {return (float)cEditorType.GetProperty("hRangeMin").GetValue(cEditor, null);}
				set {cEditorType.GetProperty("hRangeMin").SetValue(cEditor, value, null);}
			}

			public float hRangeMax{
				get {return hRangeMaxGetter();}
				set {hRangeMaxSetter(value);}
			}
/// @modify slate sequencer
/// add by TQ
            public bool hasSelection
            {
                get { return (bool)cEditorType.GetProperty("hasSelection").GetValue(cEditor, null); }
                set { cEditorType.GetProperty("hasSelection").SetValue(cEditor, value, null); }
            }
/// end

            /*
                        public Rect shownAreaInsideMargins{
                            get {return (Rect)cEditorType.GetProperty("shownAreaInsideMargins").GetValue(cEditor, null);}
                            set {cEditorType.GetProperty("shownAreaInsideMargins").SetValue(cEditor, value, null);}
                        }

                        public bool enableMouseInput{
                            get {return (bool)cEditorType.GetProperty("enableMouseInput").GetValue(cEditor, null);}
                            set {cEditorType.GetProperty("enableMouseInput").SetValue(cEditor, value, null);}
                        }

                        public bool hAllowExceedBaseRangeMin{
                            get {return (bool)cEditorType.GetProperty("hAllowExceedBaseRangeMin").GetValue(cEditor, null);}
                            set {cEditorType.GetProperty("hAllowExceedBaseRangeMin").SetValue(cEditor, value, null);}								
                        }

                        public float vRangeMin{
                            get {return (float)cEditorType.GetProperty("vRangeMin").GetValue(cEditor, null);}
                            set {cEditorType.GetProperty("vRangeMin").SetValue(cEditor, value, null);}
                        }

                        public float vRangeMax{
                            get {return (float)cEditorType.GetProperty("vRangeMax").GetValue(cEditor, null);}
                            set {cEditorType.GetProperty("vRangeMax").SetValue(cEditor, value, null);}
                        }

                        public bool hasSelection{
                            get {return (bool)cEditorType.GetProperty("hasSelection").GetValue(cEditor, null);}
                            set {cEditorType.GetProperty("hasSelection").SetValue(cEditor, value, null);}
                        }

                        public void SetShownVRangeInsideMargins(float min, float max){
                            cEditorType.GetMethod("SetShownVRangeInsideMargins").Invoke(cEditor, new object[]{min, max});
                        }
            */

            public float invSnap{
				get {return (float)cEditorType.GetField("invSnap").GetValue(cEditor);}
				set {cEditorType.GetField("invSnap").SetValue(cEditor, value);}
			}

			public int axisLock{
				set {cEditorType.GetField("m_AxisLock", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(cEditor, value);}
			}

			public void FrameClip(bool h, bool v){
				#if UNITY_5_4_OR_NEWER
				cEditorType.GetMethod("FrameClip").Invoke(cEditor, new object[]{h, v});
				#else
				FrameSelected(true, true);
				#endif
			}

			public void FrameSelected(bool h, bool v){
				cEditorType.GetMethod("FrameSelected").Invoke(cEditor, new object[]{h, v});
			}

			public void SelectNone(){
				cEditorType.GetMethod("SelectNone").Invoke(cEditor, null);
			}

			public void RecalculateBounds(){
				cEditorType.GetProperty("animationCurves").SetValue(cEditor, GetCurveWrapperArray(curves), null);
			}

			public void RefreshCurves(){
				cEditorType.GetProperty("animationCurves").SetValue(cEditor, GetCurveWrapperArray(curves), null);
			}
/// @modify slate sequencer
/// add by TQ
            public bool CheckAnimataChanged(IAnimatableData oldData, IAnimatableData newData)
            {
                if (oldData == null)
                    return false;

                var OldAllCurves = oldData.GetCurves();
                var OldAllKeys = new List<Keyframe>();
                for (var i = 0; i < OldAllCurves.Length; i++)
                {
                    OldAllKeys.AddRange(OldAllCurves[i].keys);
                    
                }
                OldAllKeys = OldAllKeys.OrderBy(k => k.time).ToList();

                var NewAllCurves = oldData.GetCurves();
                var NewAllKeys = new List<Keyframe>();
                for (var i = 0; i < NewAllCurves.Length; i++)
                {
                    NewAllKeys.AddRange(NewAllCurves[i].keys);

                }
                NewAllKeys = NewAllKeys.OrderBy(k => k.time).ToList();



                return false;
            }

            public IAnimatableData oldDataDummy = null;
/// end
            public void Draw(Rect posRect, Rect timeRect){

				if (curves == null || curves.Length == 0){
					GUI.Label(posRect, "No Animation Curves to Display", Styles.centerLabel);
					return;
				}

				var e = Event.current;
				hRangeMax = timeRect.xMax;
				rect = posRect;
				shownArea = Rect.MinMaxRect(timeRect.xMin, shownArea.yMin, timeRect.xMax, shownArea.yMax);


				if (Prefs.lockHorizontalCurveEditing && e.rawType == EventType.MouseDrag){
					axisLock = 2;
/// @modify slate sequencer
/// add by TQ
                    if (true)
                    {
                        oldDataDummy = animatable ;
                    }
/// end
				}

				if (Prefs.snapInterval != lastSnapPref){
					lastSnapPref = Prefs.snapInterval;
					invSnap = 1/Prefs.snapInterval;
				}


				if (e.rawType == EventType.MouseUp){
					RecalculateBounds();
					if (GUIUtility.hotControl != 0){
						OnCurvesUpdated();
/// @modify slate sequencer
/// add by TQ
                     CheckAnimataChanged(oldDataDummy, animatable);
/// end
                    }
				}

				if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 2 && posRect.Contains(e.mousePosition) ){
					FrameClip(true, true);
					e.Use();
				}


				//INFO
				GUI.color = new Color(1,1,1,0.2f);
				var infoWidth = Mathf.Min(posRect.width, 125);
				var labelRect = new Rect(posRect.xMax - infoWidth - 10, posRect.y + 2, infoWidth, 18);
				GUI.Label(labelRect, "(F: Frame Selection)");
				labelRect.y += 18;
				GUI.Label(labelRect, "(Click x2: Frame All)");
				labelRect.y += 18;
				GUI.Label(labelRect, "(Alt+: Pan/Zoom)");
				GUI.color = Color.white;


				//OnGUI
				try { onGUI(); }
				catch (Exception exc){ SelectNone(); Debug.LogError(exc.Message); }
			}

			
			//raise event
			void OnCurvesUpdated(){
				if (onCurvesUpdated != null){
					onCurvesUpdated(animatable);
/// @modify slate sequencer
/// add by TQ
                    if (onDragCurvesDelegates != null)
                    {             
                        onDragCurvesDelegates(this.keyable);  
                    }
/// end
                }
			}

		}
	}
}

#endif