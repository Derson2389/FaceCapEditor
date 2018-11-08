#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
/// @modify slate sequencer
/// add by TQ
using System.Collections.Generic;
/// end

namespace Slate{

	[CustomEditor(typeof(Cutscene))]
	public class CutsceneInspector : Editor {

		private bool optionsFold = true;
		private bool actorsFold = false;

		SerializedProperty updateModeProp;
		SerializedProperty wrapModeProp;
		SerializedProperty stopModeProp;
		SerializedProperty explLayersProp;
		SerializedProperty activeLayersProp;
		SerializedProperty playbackSpeedProp;
        SerializedProperty showCutsceneTimeProp;
        SerializedProperty eventSubCutsceneProp;

        private static Editor selectedObjectEditor;
		private static Cutscene cutscene;
		private static bool willResample;
		private static bool willDirty;

		void OnEnable(){
			cutscene = (Cutscene)target;
			selectedObjectEditor = null;
			willResample = false;
			willDirty = false;

			updateModeProp    = serializedObject.FindProperty("_updateMode");
			wrapModeProp      = serializedObject.FindProperty("_defaultWrapMode");
			stopModeProp      = serializedObject.FindProperty("_defaultStopMode");
			explLayersProp    = serializedObject.FindProperty("_explicitActiveLayers");
			activeLayersProp  = serializedObject.FindProperty("_activeLayers");
			playbackSpeedProp = serializedObject.FindProperty("_playbackSpeed");
            showCutsceneTimeProp = serializedObject.FindProperty("ShowCutsceneTime");
            eventSubCutsceneProp = serializedObject.FindProperty("EventSubCutscene");
        }

		void OnDisable(){
			cutscene = null;
			willResample = false;
			willDirty = false;
			if (selectedObjectEditor != null){
				DestroyImmediate(selectedObjectEditor, true);
			}
		}

        public override void OnInspectorGUI(){

			cutscene = (Cutscene)target;

			var e = Event.current;
			GUI.skin.GetStyle("label").richText = true;

			if (e.rawType == EventType.MouseDown && e.button == 0 ){ //generic undo
				//Undo.RegisterFullObjectHierarchyUndo(cutscene.groupsRoot.gameObject, "Cutscene Inspector");
				Undo.RecordObject(cutscene, "Cutscene Inspector");
				willDirty = true;
			}

			if (e.rawType == EventType.MouseUp && e.button == 0 || e.rawType == EventType.KeyUp){
				willDirty = true;
				if (CutsceneUtility.selectedObject != null && CutsceneUtility.selectedObject.startTime <= cutscene.currentTime){
					willResample = true;
				}
			}

			GUILayout.Space(5);
			if (GUILayout.Button("EDIT IN SLATE")){
				CutsceneEditor.ShowWindow(cutscene);
			}
			GUILayout.Space(5);

			DoCutsceneInspector();
            DoSelectionInspector();

            // @modify slate sequencer
            // @rongxia
            if (GUILayout.Button("REMOVE IDLE SHOTCAMERA"))
            {
                
                List<ShotCamera> allCamera =  cutscene.gameObject.GetComponentsInChildren<ShotCamera>().ToList<ShotCamera>();
                foreach(var track in cutscene.directorGroup.tracks)
                {
                    if(track as CameraTrack)
                    {
                        CameraTrack cameraTrack = (CameraTrack)track;
                        foreach(var action in cameraTrack.actions)
                        {
                            if(action as CameraShot)
                            {
                                allCamera.Remove(((CameraShot)action).targetShot);
                            }
                        }
                    }
                }
                foreach(var shotCamera in allCamera)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(shotCamera.gameObject);
                }
            }
            // @end

            // @modify slate sequencer
            // @TQ
            if (GUILayout.Button("SYSC ACTOR DATA"))
            {
                for (int g = 0; g < cutscene.groups.Count; g++)
                {
                    var group = cutscene.groups[g];
                    if (!string.IsNullOrEmpty(group.name)&& group.actor == null)
                    {
                        GameObject goDummy = GameObject.Find(group.name);
                        if (goDummy != null)
                        {
                            group.actor = goDummy;
                            continue;
                        }

                        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                        GameObject[] gos = scene.GetRootGameObjects();
                        foreach (GameObject go in gos)
                        {
                            if (go.name == group.name)
                            {
                                group.actor = go;
                            }
                            else
                            {
                                var transformDummy = go.transform.Find(group.name);
                                if (transformDummy != null)
                                {
                                    var goActor = transformDummy.gameObject;
                                    if (goActor != null)
                                    {
                                        group.actor = goActor;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // @end

            if (willDirty){
				willDirty = false;
				EditorUtility.SetDirty(cutscene);
				if (CutsceneUtility.selectedObject as UnityEngine.Object != null){
					EditorUtility.SetDirty( (UnityEngine.Object)CutsceneUtility.selectedObject );
				}
			}

			if (willResample){ //resample after the changes on fresh gui pass
				willResample = false;
				//delaycall so that other gui controls are finalized before resample.
				EditorApplication.delayCall += ()=>{ if (cutscene != null) cutscene.ReSample(); };
			}

			Repaint();

		}

		void DoCutsceneInspector(){

			GUI.color = new Color(0,0,0,0.2f);
			GUILayout.BeginHorizontal(Slate.Styles.headerBoxStyle);
			GUI.color = Color.white;
			GUILayout.Label(string.Format("<b>{0} Cutscene Settings</b>", optionsFold? "▼" : "▶"));
			GUILayout.EndHorizontal();

			var lastRect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.Link);
			if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition)){
				optionsFold = !optionsFold;
				Event.current.Use();
			}

			GUILayout.Space(2);
			if (optionsFold){
				serializedObject.Update();
				EditorGUILayout.PropertyField(updateModeProp);
				EditorGUILayout.PropertyField(wrapModeProp);
				EditorGUILayout.PropertyField(stopModeProp);
				EditorGUILayout.PropertyField(playbackSpeedProp);
				EditorGUILayout.PropertyField(explLayersProp);
				if (explLayersProp.boolValue == true){
					EditorGUILayout.PropertyField(activeLayersProp);
				}
                EditorGUILayout.PropertyField(showCutsceneTimeProp);
                EditorGUILayout.PropertyField(eventSubCutsceneProp);
                serializedObject.ApplyModifiedProperties();

				DoActorsInspector();
			}
		}

		void DoActorsInspector(){
			actorsFold = EditorGUILayout.Foldout(actorsFold, "Affected Group Actors");
			GUI.enabled = cutscene.currentTime == 0;
			if (actorsFold){
				EditorGUI.indentLevel++;
				var exists = false;
				foreach(var group in cutscene.groups.OfType<ActorGroup>()){
					var name = string.IsNullOrEmpty(group.name)? "(No Name Specified)" : group.name;
					group.actor = EditorGUILayout.ObjectField(name, group.actor, typeof(GameObject), true) as GameObject;
					exists = true;
				}
				if (!exists){
					GUILayout.Label("No Actor Groups");
				}
				EditorGUI.indentLevel--;
			}
			GUI.enabled = true;
		}

        /// <summary>
        /// @modify slate sequencer
        /// @hushuang
        /// change DoSelectionInspector to public to enable called from other EditorWindow.
        /// </summary>
		public static void DoSelectionInspector(){
            // @modify slate sequencer
            // @hushuang
            // fix bug when select other game object that is not cutscene in hierarchy window or double click clip in timeline will cause error when do DoSelectionInspector drawing from WindowResourcePanel.
            if (cutscene == null)
                return;

            TryCreateInspectedEditor();
			if (selectedObjectEditor != null){
				if (CutsceneUtility.selectedObject != null && !CutsceneUtility.selectedObject.Equals(null)){
					EditorTools.BoldSeparator();
					GUILayout.Space(4);
					ShowPreliminaryInspector();
					selectedObjectEditor.OnInspectorGUI();
/// @modify slate sequencer
/// add by TQ
                    var isAction = CutsceneUtility.selectedObject is ActionClips.SubCutscene && (CutsceneUtility.selectedObject as ActionClips.SubCutscene).isValid;
                    if (isAction && GUILayout.Button("Set at sub Cutsence Length"))
                    {
                        var action = CutsceneUtility.selectedObject as ActionClips.SubCutscene;
                        Cutscene subCutsence = action.cutscene;
                        action.clipOffset = 0f;
                        action.offsetEnd = 0;
                        action.offsetStart = 0;
                        action.length = subCutsence.length;
                        Cutscene actionRoot = action.root as Cutscene;
                        
                        if (actionRoot!= null )
                        {
                            List<ActionClip> subCameraClips = subCutsence.cameraTrack == null? new List<ActionClip>() :subCutsence.cameraTrack.actions;
 
                            ///  检查是已经存在和此关联的camera数据 
                            List<ActionClip> allClips = actionRoot.cameraTrackCustom.actions;
                            List<ActionClip> linkClips = new List<ActionClip>();
                        
                            foreach (var clip in allClips)
                            {
                                ActionClips.StoryEngineClip.StoryCameraClip cur = clip as ActionClips.StoryEngineClip.StoryCameraClip;
                                if (cur != null && cur.fromCutsence != null && ReferenceEquals(cur.fromCutsence, action.cutscene)
                                    && cur.linkClip != null && ReferenceEquals(cur.linkClip, action))
                                {
                                    linkClips.Add(clip);           
                                }
                            }
                            foreach (var act in linkClips)
                            {
                                actionRoot.cameraTrackCustom.DeleteAction(act);
                                CutsceneEditor.current.OutInitClipWrappers();
                            }
                            
                            for (int i = 0; i< subCameraClips.Count; i++)
                            {
                                var copyJson = JsonUtility.ToJson(subCameraClips[i]);
                                var copyType = subCameraClips[i].GetType();
                                if (copyType != null)
                                {
                                    var newAction = actionRoot.cameraTrackCustom.AddAction(copyType, subCameraClips[i].startTime);
                                    JsonUtility.FromJsonOverwrite(copyJson, newAction);
                                    newAction.startTime = action.startTime + subCameraClips[i].startTime;
                                    if (newAction.endTime >(action.startTime + action.length) )
                                    {
                                        newAction.endTime = action.startTime + action.length;
                                    }

                                    ActionClips.StoryEngineClip.StoryCameraClip camAction = newAction as ActionClips.StoryEngineClip.StoryCameraClip;
                                    if (camAction != null)
                                    {
                                        camAction.fromCutsence = action.cutscene;
                                        camAction.linkClip = action;
                                        action.linkID = action.GetInstanceID().ToString();
                                        camAction.linkID = action.GetInstanceID().ToString();
                                    }
                                }
                            }
                        }
                    }

                    if (isAction && GUILayout.Button("Set Camera Clip Info"))
                    {
                        int offsetEnd = 0;
                        int offsetStart = 0;

                        var action = CutsceneUtility.selectedObject as ActionClips.SubCutscene;
                        Cutscene subCutsence = action.cutscene;
                        action.ClipOffset = -subCutsence.startPlayTime;
                        action.length = subCutsence.length - subCutsence.startPlayTime;
                        List<ActionClip> subCameraClips = subCutsence.cameraTrack == null ? new List<ActionClip>() : subCutsence.cameraTrack.actions;
                        List<ActionClip> linkClips = new List<ActionClip>();
                        var subCameraCount = subCameraClips.Count;
                        Cutscene actionRoot = action.root as Cutscene;
                        if (actionRoot != null)
                        {
                            List<ActionClip> allClips = actionRoot.cameraTrackCustom.actions;
                            foreach (var clip in allClips)
                            {
                                ActionClips.StoryEngineClip.StoryCameraClip cur = clip as ActionClips.StoryEngineClip.StoryCameraClip;
                                if (cur != null && cur.fromCutsence != null && ReferenceEquals(cur.fromCutsence, action.cutscene)
                                    && cur.linkClip != null && ReferenceEquals(cur.linkClip, action))
                                {
                                    linkClips.Add(clip);
                                }
                            }
                            foreach (var act in linkClips)
                            {
                                actionRoot.cameraTrackCustom.DeleteAction(act);
                                CutsceneEditor.current.OutInitClipWrappers();
                            }
                            for (int i = 0; i < subCameraClips.Count; i++)
                            {
                                var subDummy = subCameraClips[i];
                                var s_Time = subCameraClips[i].startTime + 0;
                                var e_Time = subCameraClips[i].endTime + 0;
                                if (e_Time > action.length - action.ClipOffset && action.length - action.ClipOffset >= s_Time)
                                {
                                    offsetEnd = subCameraClips.Count - 1 - i;
                                }
                                var offsetTime = Mathf.Abs(action.ClipOffset) + 0;
                                if (e_Time >= offsetTime && offsetTime > s_Time)
                                {
                                    offsetStart = i;
                                }
                            }
                            if (subCameraClips.Count != 0)
                            {
                                var lastclip = subCameraClips[subCameraCount - offsetEnd - 1];
                                var firstClip = subCameraClips[offsetStart] as ActionClips.StoryEngineClip.StoryCameraClip;

                                for (int i = offsetStart; i <= (subCameraCount - offsetEnd - 1); i++)
                                {
                                    var copyJson = JsonUtility.ToJson(subCameraClips[i]);
                                    var copyType = subCameraClips[i].GetType();
                                    if (copyType != null)
                                    {
                                        var newAction = actionRoot.cameraTrackCustom.AddAction(copyType, subCameraClips[i].startTime);
                                        JsonUtility.FromJsonOverwrite(copyJson, newAction);
                                        newAction.startTime = action.startTime + subCameraClips[i].startTime;
                                        ActionClips.StoryEngineClip.StoryCameraClip camAction = newAction as ActionClips.StoryEngineClip.StoryCameraClip;

                                        if (camAction != null)
                                        {
                                            camAction.startTime = camAction.startTime + action.ClipOffset;
                                            if (lastclip != null && ReferenceEquals(lastclip, subCameraClips[i]))
                                            {
                                                camAction.endTime = action.endTime;
                                            }
                                            if (firstClip != null && ReferenceEquals(firstClip, subCameraClips[i]) /*&& !ReferenceEquals(lastclip, firstClip)*/)
                                            {
                                                if (ReferenceEquals(lastclip, firstClip))
                                                {
                                                    camAction.startTime = action.startTime;
                                                    camAction.clipOffset = (subCameraClips[i].startTime + action.ClipOffset);
                                                    camAction.length = action.length;
                                                }
                                                else
                                                {
                                                    camAction.startTime = action.startTime;
                                                    camAction.clipOffset = (subCameraClips[i].startTime + action.ClipOffset);
                                                    camAction.length = camAction.length + camAction.clipOffset;
                                                }
                                            }
                                            camAction.fromCutsence = action.cutscene;
                                            camAction.linkClip = action;
                                            action.linkID = action.GetInstanceID().ToString();
                                            camAction.linkID = action.GetInstanceID().ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
			}
		}
///end
        
		static void ShowPreliminaryInspector(){
					
				var type = CutsceneUtility.selectedObject.GetType();
				var nameAtt = type.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute;
				var name = nameAtt != null? nameAtt.name : type.Name.SplitCamelCase();
				var withinRange = cutscene.currentTime > 0 && cutscene.currentTime >= CutsceneUtility.selectedObject.startTime && cutscene.currentTime <= CutsceneUtility.selectedObject.endTime;
				var keyable = CutsceneUtility.selectedObject is IKeyable && (CutsceneUtility.selectedObject as IKeyable).animationData != null && (CutsceneUtility.selectedObject as IKeyable).animationData.isValid;
				var isActive = CutsceneUtility.selectedObject.isActive;

				GUI.color = new Color(0,0,0,0.2f);
				GUILayout.BeginHorizontal(Slate.Styles.headerBoxStyle);
				GUI.color = Color.white;
				GUILayout.Label(string.Format("<b><size=18>{0}{1}</size></b>", withinRange && keyable && isActive? "<color=#eb5b50>●</color> " : "", name ) );
				GUILayout.EndHorizontal();

				if (Prefs.showDescriptions){
					var descAtt = type.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
					var description = descAtt != null? descAtt.description : null;
					if (!string.IsNullOrEmpty(description)){
						EditorGUILayout.HelpBox(description, MessageType.None);
					}
				}

				GUILayout.Space(2);
		}


		static void TryCreateInspectedEditor(){

			if (selectedObjectEditor == null && CutsceneUtility.selectedObject as Object != null){
				selectedObjectEditor = Editor.CreateEditor( (Object)CutsceneUtility.selectedObject );
				// Editor.CreateCachedEditor( (Object)CutsceneUtility.selectedObject, null, ref selectedObjectEditor);
			}

			if (selectedObjectEditor != null && selectedObjectEditor.target != CutsceneUtility.selectedObject as Object){
				DestroyImmediate(selectedObjectEditor, true);
				if (CutsceneUtility.selectedObject != null){
					selectedObjectEditor = Editor.CreateEditor( (Object)CutsceneUtility.selectedObject );
					// Editor.CreateCachedEditor( (Object)CutsceneUtility.selectedObject, null, ref selectedObjectEditor);
				}
			}
		}
	}
}

#endif