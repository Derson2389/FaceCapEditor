using UnityEngine;
using System.Collections;
/// @modify slate sequencer
/// add by TQ
using System;
using System.Collections.Generic;
using System.Linq;
/// end

namespace Slate.ActionClips{
/// @modify slate sequencer
/// add by TQ
	[Category("Composition")]
	[Description("SubCutscenes are used for organization. Notice that the CameraTrack of the SubCutscene is ignored if this Cutscene already has an active CameraTrack.")]
	public class SubCutscene : DirectorActionClip, ISubClipContainable
    {

		[Required]
		public Cutscene cutscene;
		private bool wasCamTrackActive;
        [SerializeField]
        private string cutsceneName;

        [SerializeField]
        [HideInInspector]
        private float _length = 1f;
        [SerializeField]
        public float clipOffset = 0f;

        [SerializeField]
        [HideInInspector]
        private string _linkID = string.Empty;

        #region forEditor
        [SerializeField]
        [HideInInspector]
        public int offsetEnd = 0;
        [SerializeField]
        [HideInInspector]
        public int offsetStart = 0;
        #endregion


        public string linkID
        {
            set { _linkID = value; }
            get { return _linkID;  }
        }

        public float ClipOffset
        {
            set
            {
                clipOffset = value;
            }
            get
            {
                return clipOffset;
            }
        }


        public override string info{
			get
			{
				if (ReferenceEquals(cutscene, root)){ return "        SubCutscene can't be same as this cutscene"; }
				return cutscene != null? string.Format("        SubCutscene\n        '{0}'", cutscene.name) : "No Cutscene Selected";
			}
		}

		public override bool isValid{
			get
            {
                if (cutscene != null)
                {
                    cutsceneName = cutscene.gameObject.name;
                }
                else
                {
                    if (!string.IsNullOrEmpty(cutsceneName))
                    {
                        GameObject go = GameObject.Find(cutsceneName);
                        if (go != null)
                        {
                            var comp = go.GetComponent<Cutscene>();
                            if (comp != null)
                            {
                                cutscene = comp;
                            }
                        }
                    }
                }
                return cutscene != null && !ReferenceEquals(cutscene, root);
            }
		}

        public override float startTime
        {
            get
            {
                return base.startTime;
            }

            set
            {
                base.startTime = value;
            }
        }

        public override float endTime
        {
            get { return startTime + length; }
            set
            {
                if (startTime + length != value)
                {
                    customerLength(Mathf.Max(value - startTime, 0));
                    blendOut = Mathf.Clamp(blendOut, 0, length - blendIn);
                    blendIn = Mathf.Clamp(blendIn, 0, length - blendOut);
                }
            }
        }

        public void customerLength(float value)
        {
            _length = value;
            //cutLinkCameraClip();
        }
        public override float length
        {
            get { return _length; }
            set
            {
                _length = value;
            }
        }

        private void cutLinkCameraClip(bool isStart = false)
        {
            var _cutsence = this.root as Cutscene;
            if (_cutsence != null)
            {
                var cameraTrack = _cutsence.cameraTrackCustom;
                List<ActionClip> CameraClips = cameraTrack.actions;

                List<ActionClip> linkClips = new List<ActionClip>();
                List<ActionClip> subCameraClips = cutscene.cameraTrackCustom.actions;
                float endOffset = length - cutscene.length;
                float startOffset = clipOffset;

                int clipCount = CameraClips.Count;
                for (int i = 0; i < clipCount; i++)
                {
                    var subClipsDummy = CameraClips[i] as StoryEngineClip.StoryCameraClip;
                    if (subClipsDummy != null && subClipsDummy.linkClip != null && subClipsDummy.fromCutsence != null && ReferenceEquals(subClipsDummy.fromCutsence, cutscene))
                    {
                        linkClips.Add(CameraClips[i]);
                    }
                }
                if (linkClips.Count <= 0)
                {
                    return;
                }
                linkClips.Where(d => d is ActionClip).OrderBy(d => d.startTime);
                int linkClipCount = linkClips.Count;
                var lastclip = linkClips[linkClipCount - offsetEnd - 1];         
                var firstClip = linkClips[offsetStart] as StoryEngineClip.StoryCameraClip;

                for (int i = 0; i< subCameraClips.Count; i++)
                {
                    var subDummy = subCameraClips[i];
                    var s_Time = subCameraClips[i].startTime + startTime;
                    var e_Time = subCameraClips[i].endTime + startTime;
                    if (e_Time > endTime && endTime >= s_Time)
                    {
                        offsetEnd = subCameraClips.Count - 1 - i;
                    }
                    var offsetTime = Math.Abs(clipOffset) + startTime;
                    if (e_Time >= offsetTime && offsetTime > s_Time)
                    {
                        offsetStart = i;
                    }
                }
                
                if (lastclip != null )
                {
                    ActionClip nextClip = null;
                    ActionClip preClip = null;
                    if (offsetEnd > 1)
                    {
                        nextClip = linkClips[linkClips.Count - offsetEnd];
                    }
                    if (offsetEnd < linkClips.Count - 1)
                    {
                        preClip = linkClips[linkClips.Count - offsetEnd -2];
                    }

                    lastclip.endTime = endTime;

                    if (nextClip != null)
                    {
                        lastclip.startTime = preClip != null ? preClip.startTime + preClip.length : lastclip.startTime;
                        lastclip.endTime = Mathf.Clamp(endTime, preClip == null ? endTime : preClip.startTime + preClip.length, nextClip == null ? endTime : nextClip.startTime);                            
                    }

                    for (int i = linkClipCount - offsetEnd; i < linkClipCount; i++)
                    {
                          linkClips[i].startTime = endTime;
                    }
                }

                if (isStart)
                {
                    float endTimeOffset = 0f;
                    endTimeOffset = subCameraClips[offsetStart].endTime;
                    ActionClip nextClip = null;
                    ActionClip preClip = null;
                    if (offsetStart + 1 < linkClips.Count)
                    {
                        nextClip = linkClips[offsetStart + 1];
                    }
                    if (offsetStart - 1 > 0)
                    {
                        preClip = linkClips[offsetStart - 1];
                    }

                    if (firstClip != null && firstClip.length > 0)
                    {
                        //firstClip.startTime = Mathf.Clamp(startTime, preClip == null ? startTime : preClip.endTime, nextClip == null ? startTime : nextClip.startTime);
                        //firstClip.clipOffset = clipOffset;
                        //var preferEndTime = firstClip.startTime + endTimeOffset + clipOffset;
                        //firstClip.endTime = Mathf.Min(preferEndTime, nextClip == null ? preferEndTime : nextClip.startTime);
                        //firstClip.length = firstClip.endTime - firstClip.startTime;
                    }

                }
            }

        }

        new public GameObject actor{ //this is not really needed but makes double clicking the clip, select the target cutscene
			get {return isValid? cutscene.gameObject : base.actor;}
		}

        float ISubClipContainable.subClipOffset
        {
            get {
                return clipOffset;
            }
            set
            {
                clipOffset = value;
            }
        }

        protected override void OnEnter(){

            if (cutscene.cameraTrackCustom != null && cutscene.cameraTrackCustom.actions.Count > 0)
            {
                var rootCut = (Cutscene)root;
                if (rootCut.cameraTrackCustom != null)
                    rootCut.cameraTrackCustom.NeedUpdate = false;
            }
            
        }

		protected override void OnReverseEnter(){
            
            base.OnReverseEnter();
            			
        }

		protected override void OnExit(){
            ///cutscene.SkipAll();
            var rootCut = (Cutscene)root;
            if (rootCut.cameraTrackCustom != null && cutscene.cameraTrackCustom != null && cutscene.cameraTrackCustom.actions.Count > 0)
            {
                rootCut.cameraTrackCustom.NeedUpdate = true;
            }
        }

        protected override void OnReverse(){
            //if (cutscene.cameraTrackCustom != null){
            //	cutscene.cameraTrackCustom.isActive = wasCamTrackActive;
            //             var rootCut = (Cutscene)root;
            //             if (rootCut.cameraTrackCustom != null)
            //             {                   
            //                 rootCut.cameraTrackCustom.isActive = !false;
            //                 rootCut.cameraTrackCustom.ModifyCameraTrack(rootCut.cameraTrackCustom);
            //             }
            //         }

            
            cutscene.Rewind();
            var rootCut = (Cutscene)root;
            if (rootCut.cameraTrackCustom != null && cutscene.cameraTrackCustom != null && cutscene.cameraTrackCustom.actions.Count > 0)
            {
                rootCut.cameraTrackCustom.NeedUpdate = true;
            }
        }

		protected override void OnUpdate(float time)
        {      
                cutscene.Sample(time - clipOffset);
                if (time >=  1f && cutscene.cameraTrackCustom != null && cutscene.cameraTrackCustom.cineBoxFadeTime != 0)
                {
                    cutscene.cameraTrackCustom.cineBoxFadeTime = 0;
                }                           	
		}
    

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		protected override void OnClipGUI(Rect rect){
			if (cutscene != null){
				GUI.color = new Color(1,1,1,0.9f);
				GUI.DrawTexture(new Rect(0, 0, rect.height, rect.height), Slate.Styles.cutsceneIcon);
				GUI.color = Color.white;
                EditorTools.DrawLoopedLines(rect, cutscene.length, this.length, clipOffset);
            }
		}

#endif
    }
/// end
}