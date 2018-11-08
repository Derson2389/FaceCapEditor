#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
/// @modify slate sequencer
/// add by TQ
using Slate.ActionClips;
/// end

namespace Slate{

	///Utilities specific to Cutscenes
	public static class CutsceneUtility {

		[System.NonSerialized]
		private static string copyJson;
		[System.NonSerialized]
		private static System.Type copyType;
		[System.NonSerialized]
		private static IDirectable _selectedObject;
		[System.NonSerialized]
		public static Dictionary<AnimatedParameter, ChangedParameterCallbacks> changedParameterCallbacks = new Dictionary<AnimatedParameter, ChangedParameterCallbacks>();

		public static event System.Action<IDirectable> onSelectionChange;
		public static event System.Action<IAnimatableData> onRefreshAllAnimationEditors;

		/// @modify slate sequencer
		/// add by TQ
        [System.NonSerialized]
        private static Dictionary<CutsceneTrack, List<string>> copyJsonDic = new Dictionary<CutsceneTrack, List<string>>();
        private static Dictionary<CutsceneTrack, List<System.Type>> copyTypeDic = new Dictionary<CutsceneTrack, List<System.Type>>();
		/// end
        public struct ChangedParameterCallbacks{
			public System.Action Restore;
			public System.Action Commit;
			public ChangedParameterCallbacks(System.Action restore, System.Action commit){
				Restore = restore;
				Commit = commit;
			}
		}

		public static IDirectable selectedObject{
			get {return _selectedObject;}
			set
			{
				//select the root cutscene which in turns display the inspector of the object within it.
				if (value != null){	UnityEditor.Selection.activeObject = value.root.context; }
				_selectedObject = value;
				if (onSelectionChange != null){
					onSelectionChange(value);
				}
			}
		}

		public static void RefreshAllAnimationEditorsOf(IAnimatableData animatable){
			if (onRefreshAllAnimationEditors != null){
				onRefreshAllAnimationEditors(animatable);
			}
		}

		public static System.Type GetCopyType(){
			return copyType;
		}
		/// @modify slate sequencer
		/// add by TQ
        public static bool hasMutiClipsCut()
        {
            if (copyJsonDic.Count > 0)
                return true;
            return false;
        }
		/// end
		public static void SetCopyType(System.Type type){
			copyType = type;
		}

		public static void CopyClip(ActionClip action){
			copyJson = JsonUtility.ToJson(action, false);
			copyType = action.GetType();
		}

/// @modify slate sequencer
/// add by TQ
/// 
        public static void WrapCopyClip(ActionClip action)
        {
            var subcutType = action as SubCutscene;
            if (subcutType != null)
            {
                var pCutTrack = action.parent as CutsceneTrack;
                var pCut = pCutTrack.root as Cutscene;
                List<ActionClip> curCamActions = pCut.cameraTrack.actions;
                List<ActionClip> linkClips = new List<ActionClip>();
                var subCut = (action as ActionClips.SubCutscene).cutscene;
                for (int i = 0; i < curCamActions.Count; i++)
                {
                    ActionClips.StoryEngineClip.StoryCameraClip cur = curCamActions[i] as ActionClips.StoryEngineClip.StoryCameraClip;
                    if (cur != null && cur.fromCutsence != null && ReferenceEquals(cur.fromCutsence, subCut)
                        && cur.linkClip != null && ReferenceEquals(cur.linkClip, action))
                    {
                        linkClips.Add(cur);
                    }
                }
                foreach (var act in linkClips)
                {
                    CutsceneUtility.SetCopyType(null);
                    CutsceneUtility.CutClips(pCut.cameraTrack, act, true);
                    //pCut.cameraTrack.DeleteAction(act);
                }
            }
            CopyClip(action);

        }
/// end


/// @modify slate sequencer
/// add by TQ
///
        public static void CutClips(CutsceneTrack track, ActionClip action, bool linkCut = false)
        {
            if (track == null)
                return;
            
            if (action is ActionClips.SubCutscene && !linkCut)
            {
                var subcutType = action as SubCutscene;
                if (subcutType != null)
                {
                    var pCutTrack = action.parent as CutsceneTrack;
                    var pCut = pCutTrack.root as Cutscene;
                    List<ActionClip> curCamActions = pCut.cameraTrack.actions;
                    List<ActionClip> linkClips = new List<ActionClip>();
                    var subCut = (action as ActionClips.SubCutscene).cutscene;
                    for (int i = 0; i < curCamActions.Count; i++)
                    {
                        ActionClips.StoryEngineClip.StoryCameraClip cur = curCamActions[i] as ActionClips.StoryEngineClip.StoryCameraClip;
                        if (cur != null && cur.fromCutsence != null && ReferenceEquals(cur.fromCutsence, subCut)
                            && cur.linkClip != null && ReferenceEquals(cur.linkClip, action))
                        {
                            linkClips.Add(cur);
                        }
                    }
                    foreach (var act in linkClips)
                    {
                        CutsceneUtility.SetCopyType(null);
                        CutsceneUtility.CutClips(pCut.cameraTrack, act, true);
                        pCut.cameraTrack.DeleteAction(act);
                    }      
                }
            }
/// end
            List<string> copyStrDummy = null;
            List<System.Type> typeDummy = null;
            if (copyJsonDic.TryGetValue(track, out copyStrDummy) && copyTypeDic.TryGetValue(track, out typeDummy))
            {
                var copyJson = JsonUtility.ToJson(action, false);
                if (!copyStrDummy.Contains(copyJson))
                {
                    copyStrDummy.Add(copyJson);
                    typeDummy.Add(action.GetType());
                }            
            }
            else
            {
                copyStrDummy = new List<string>();
                typeDummy = new List<System.Type>();
                var copyJson = JsonUtility.ToJson(action, false);              
                if (!copyStrDummy.Contains(copyJson))
                {
                    copyStrDummy.Add(copyJson);
                    typeDummy.Add(action.GetType());
                }
                copyJsonDic.Add(track, copyStrDummy);
                copyTypeDic.Add(track, typeDummy); 
            }                
        }
        public static void PasteClips(float time, CutsceneTrack desTrack = null)
        {
            foreach (var dummy in copyJsonDic)
            {
                var track = dummy.Key;
                var copyValue = dummy.Value;
                List<System.Type> typeValue = null ;
                copyTypeDic.TryGetValue(track, out typeValue);
                if (desTrack != null && track.GetType() != desTrack.GetType())
                {
                    continue;
                }
                else if(desTrack!= null && track.GetType() == desTrack.GetType())
                {
                    track = desTrack;
                }

                float offsetX = 0f;
                for (int i = 0; i < copyValue.Count; i++)
                {
                    var copyTypeDummy = typeValue[i];
/// @modify slate sequencer
/// add by TQ
                    if (copyTypeDummy.Name == "StoryCameraClip")
                    {
                        ActionClips.StoryEngineClip.StoryCameraClip newActionDummy = UnityEditor.Undo.AddComponent(track.gameObject, copyTypeDummy) as ActionClips.StoryEngineClip.StoryCameraClip;//new ActionClips.StoryEngineClip.StoryCameraClip();                  
                        JsonUtility.FromJsonOverwrite(copyValue[i], newActionDummy);
                        if (newActionDummy.fromCutsence != null && newActionDummy.linkID != null)
                        {
                            GameObject.DestroyImmediate(newActionDummy);
                            continue;
                        }
                        GameObject.DestroyImmediate(newActionDummy);
                    }
                                  
                    var newAction = track.AddAction(copyTypeDummy, time);
                    JsonUtility.FromJsonOverwrite(copyValue[i], newAction);
                    if (newAction is ActionClips.SubCutscene)
                    {
                        var newSubCutAction = newAction as ActionClips.SubCutscene;
                        List<string> camTypeValue = null;
                        List<System.Type>  Camtype = null;
                        var rootCut = track.root as Cutscene;
                        copyJsonDic.TryGetValue(rootCut.cameraTrack, out camTypeValue);
                        copyTypeDic.TryGetValue(rootCut.cameraTrack, out Camtype);
                        for (int idx = 0; idx < camTypeValue.Count; idx++)
                        {
                            var copyCamTypeDummy = Camtype[idx];
                            var newActionDummy = UnityEditor.Undo.AddComponent(rootCut.cameraTrack.gameObject, copyCamTypeDummy) as ActionClips.StoryEngineClip.StoryCameraClip;//new ActionClips.StoryEngineClip.StoryCameraClip();
                            JsonUtility.FromJsonOverwrite(camTypeValue[idx], newActionDummy);
                            if (newActionDummy.fromCutsence != null && newActionDummy.linkID == newSubCutAction.linkID)
                            {
                                var newCamAction = rootCut.cameraTrack.AddAction(copyCamTypeDummy, time);
                                JsonUtility.FromJsonOverwrite(camTypeValue[idx], newCamAction);
                                var subnewCamAction = newCamAction as ActionClips.StoryEngineClip.StoryCameraClip;
                                if (subnewCamAction!= null)
                                {
                                    subnewCamAction.linkID = newSubCutAction.linkID;
                                    subnewCamAction.linkClip = newSubCutAction;
                                }
                                if (idx == 0)///取得偏移值
                                {
                                    offsetX = newCamAction.startTime - time;
                                    newCamAction.startTime = time;
                                }
                                else
                                {
                                    newCamAction.startTime = newCamAction.startTime - offsetX;
                                }

                                var nextActionDummy = rootCut.cameraTrack.actions.FirstOrDefault(a => a.startTime > newCamAction.startTime);
                                if (nextActionDummy != null && newCamAction.endTime > nextActionDummy.startTime)
                                {
                                    newCamAction.endTime = nextActionDummy.startTime;
                                    break;
                                }                            
                            }
                            GameObject.DestroyImmediate(newActionDummy);
                        }
                    }
/// end
                    if (i == 0)///取得偏移值
                    {
                        offsetX = newAction.startTime - time;
                        newAction.startTime = time;
                    }
                    else
                    {
                        newAction.startTime = newAction.startTime - offsetX;
                    }                   
                                                          
                    var nextAction = track.actions.FirstOrDefault(a => a.startTime > newAction.startTime);
                    if (nextAction != null && newAction.endTime > nextAction.startTime)
                    {
                        newAction.endTime = nextAction.startTime;
                        break;
                    }
                }
            }

            copyJsonDic.Clear();
            copyTypeDic.Clear();

        }

/// @modify slate sequencer
/// add by TQ
        public static void PasteSubcutsenceClip(CutsceneTrack cutTrack , float time)
        {
           var newActionClip =   PasteClip(cutTrack, time);
            foreach (var dummy in copyJsonDic)
            {
                var track = dummy.Key;
                var copyValue = dummy.Value;
                List<System.Type> typeValue = null;
                copyTypeDic.TryGetValue(track, out typeValue);
                float offsetX = 0f;
                for (int i = 0; i < copyValue.Count; i++)
                {
                    var copyTypeDummy = typeValue[i];
                    var newAction = track.AddAction(copyTypeDummy, time);
                    JsonUtility.FromJsonOverwrite(copyValue[i], newAction);
                    if (i == 0)///取得偏移值
                    {
                        offsetX = newAction.startTime - time;
                        newAction.startTime = time;
                    }
                    else
                    {
                        newAction.startTime = newAction.startTime - offsetX;
                    }

                    var nextAction = track.actions.FirstOrDefault(a => a.startTime > newAction.startTime);
                    if (nextAction != null && newAction.endTime > nextAction.startTime)
                    {
                        newAction.endTime = nextAction.startTime;
                        break;
                    }
                    var dummyAction = newAction as ActionClips.StoryEngineClip.StoryCameraClip;
                    if (dummyAction != null)
                    {
                        dummyAction.linkClip = newActionClip;
                        var newSubActionClip = newActionClip as ActionClips.SubCutscene;
                        if (newSubActionClip!= null)
                        {
                            newSubActionClip.linkID = newActionClip.GetInstanceID().ToString();
                        }
                        dummyAction.linkID = newActionClip.GetInstanceID().ToString();
                    }
                }
            }
            copyJsonDic.Clear();
            copyTypeDic.Clear();
            copyJson = null;
            copyType = null;
        }
/// end
        public static void CutClip(ActionClip action){
			copyJson = JsonUtility.ToJson(action, false);
			copyType = action.GetType();
			(action.parent as CutsceneTrack).DeleteAction(action);
		}

/// @modify slate sequencer
/// add by TQ
        public static void WrapCutClip(ActionClip action)
        {
            var subcutType = action as SubCutscene;
            if (subcutType != null)
            {
                var pCutTrack = action.parent as CutsceneTrack;
                var pCut = pCutTrack.root as Cutscene;
                List<ActionClip> curCamActions = pCut.cameraTrack.actions;
                List<ActionClip> linkClips = new List<ActionClip>();
                var subCut = (action as ActionClips.SubCutscene).cutscene;
                for (int i = 0; i < curCamActions.Count; i++)
                {
                    ActionClips.StoryEngineClip.StoryCameraClip cur = curCamActions[i] as ActionClips.StoryEngineClip.StoryCameraClip;
                    if (cur != null && cur.fromCutsence != null && ReferenceEquals(cur.fromCutsence, subCut)
                        && cur.linkClip != null && ReferenceEquals(cur.linkClip, action))
                    {
                        linkClips.Add(cur);
                    }
                }
                foreach (var act in linkClips)
                {
                    CutsceneUtility.SetCopyType(null);
                    CutsceneUtility.CutClips(pCut.cameraTrack, act, true);
                    pCut.cameraTrack.DeleteAction(act);
                }
                //CutsceneUtility.CutClips(pCutTrack,action);
               // pCutTrack.DeleteAction(action);
               // return;        
            }          
            CutClip(action);
        }
/// end
		public static ActionClip PasteClip(CutsceneTrack track, float time){
			if (copyType != null){
				var newAction = track.AddAction(copyType, time);
				JsonUtility.FromJsonOverwrite(copyJson, newAction);
				newAction.startTime = time;

				var nextAction = track.actions.FirstOrDefault(a => a.startTime > newAction.startTime);
				if (nextAction != null && newAction.endTime > nextAction.startTime){
					newAction.endTime = nextAction.startTime;
				}

				return newAction;
			}
			return null;
		}

        public static bool AIntersectAndEncapsulatesB(Rect a, Rect b)
        {
            if (a == default(Rect) || b == default(Rect))
            {
                return false;
            }

            bool isIntersect = a.xMax >= b.xMin && b.xMax >= a.xMin && a.yMax >= b.yMin && b.yMax >= a.yMin;

            return AEncapsulatesB(a, b) || (isIntersect);

        }
        //end	
        //this could be an extension but it's only used here so...
        public static bool AEncapsulatesB(Rect a, Rect b)
        {
            if (a == default(Rect) || b == default(Rect))
            {
                return false;
            }
            return a.xMin <= b.xMin && a.xMax >= b.xMax && a.yMin <= b.yMin && a.yMax >= b.yMax;
        }

    }
}

#endif