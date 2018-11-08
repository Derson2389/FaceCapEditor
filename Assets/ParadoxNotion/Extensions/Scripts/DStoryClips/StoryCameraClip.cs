using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Slate.ActionClips.StoryEngineClip
{
    public class StoryCameraClip : CameraShot, ISubClipContainable, ISubClipHasCurrentTime
    {
        [SerializeField]
        public float clipOffset;

        [SerializeField]
        public bool canDragClipEnable;

        [SerializeField]
        public string lookTargetName;

        [SerializeField]
        public string followTargetName;
/// @modify slate sequencer
/// add by TQ
        [SerializeField]
        private Cutscene _fromCutsence;
        [SerializeField]
        private ActionClip _linkClip;

        [SerializeField]
        private string _linkID = string.Empty;
        [SerializeField]
        public Cutscene fromCutsence
        {
            get { return _fromCutsence;  }
            set { _fromCutsence = value; }
        }
        [SerializeField]
        public string linkID
        {
            get { return _linkID;  }
            set { _linkID = value; }
        }

        public ActionClip linkClip
        {
            get { return _linkClip;  }
            set { _linkClip = value; }
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

        [SerializeField]
        public string showName = string.Empty;
        public override string info
        {
            get { return isValid && showName!=string.Empty ? string.Format("<i>'{0}'</i>", showName) : ""; }
        }
        ///end


        float ISubClipContainable.subClipOffset
        {
            get { return clipOffset; }
            set { clipOffset = value; }
        }

        ///end modify slate sequencer
        private List<float> currentTimes;
        public List<float> CurrentTimes
        {
            get
            {
                return currentTimes;
            }

            set
            {
                currentTimes = value;
            }

        }

        public bool CanDragClipEnable
        {
            get
            {
                return canDragClipEnable;
            }

            set
            {
                canDragClipEnable = value;
            }
        }

#if UNITY_EDITOR
        public bool isLinkedClip()
        {
            if ( this.fromCutsence != null && this.linkClip != null)
            {
                return true;
            }
            return false;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            //targetShot = ShotCamera.Create(this.root.context.transform);
        }
#endif

        void IDirectable.Update(float time, float previousTime) {
            if (this.isLinkedClip())
            {
                var CameraTrack = this.parent as CameraTrack;
                if (CameraTrack.isActive && CameraTrack.actions.Count > 0 && CameraTrack.NeedUpdate)
                {
                    CameraTrack.NeedUpdate = false;
                }
                return;
            }
            
            var temp = this.parent as CameraTrack;
            //if (!temp.checkSelfIsActiveCameraTrack())
            //    return;
            if (!temp.isActive)
            {
                return;
            }
            

            base.UpdateAnimParams(time, previousTime); OnUpdate(time, previousTime);
        }


        protected override void OnReverseEnter()
        {
            if (this.isLinkedClip())
            {
                var CameraTrack = this.parent as CameraTrack;
                if (CameraTrack.isActive && CameraTrack.actions.Count > 0)
                {
                    CameraTrack.NeedUpdate = false;
                }
                return;
            }
            base.OnReverseEnter();

            if (lookTargetName != "" && targetShot != null)
            {
                GameObject lookTarget = GameObject.Find(lookTargetName);
                if (lookTarget != null)
                    targetShot.dynamicController.composer.target = lookTarget.transform;
            }

            if (followTargetName != "" && targetShot != null)
            {
                GameObject followTarget = GameObject.Find(followTargetName);
                if (followTarget != null)
                    targetShot.dynamicController.transposer.target = followTarget.transform;
            }
            if (this.parent is CameraTrack)
            {
                var CameraTrack = this.parent as CameraTrack;
                if (CameraTrack.isActive && CameraTrack.actions.Count > 0 && !this.isLinkedClip())
                {
                    CameraTrack.NeedUpdate = true;
                    
                }
            }
        }
        protected override void OnReverse()
        {
            if (this.parent is CameraTrack)
            {
                var cameratrack = this.parent as CameraTrack;
                if (previousShot == null)
                {
                    previousShot = cameratrack.currentShot;
                    cameratrack.currentShot = this;
                }                
            }
            base.OnReverse();
        }

        protected override void OnExit()
        {
            //if (this.parent is CameraTrack)
            //{
            //    var cameratrack = this.parent as CameraTrack;
            //    cameratrack.NeedUpdate = false;
            //}
            base.OnExit();
        }
        protected override void OnEnter()
        {
            if (this.isLinkedClip())
            {
                var CameraTrack = this.parent as CameraTrack;
                if(CameraTrack.isActive && CameraTrack.actions.Count > 0 )
                {
                    CameraTrack.NeedUpdate = false;
                }
                return;
            }

            base.OnEnter();

            if (lookTargetName != "" && targetShot != null)
            {
                GameObject lookTarget = GameObject.Find(lookTargetName);
                if (lookTarget != null)
                    targetShot.dynamicController.composer.target = lookTarget.transform;
            }

            if (followTargetName != "" && targetShot != null)
            {
                GameObject followTarget = GameObject.Find(followTargetName);
                if (followTarget != null)
                    targetShot.dynamicController.transposer.target = followTarget.transform;
            }

            if (this.parent is CameraTrack)
            {
                var CameraTrack = this.parent as CameraTrack;
                if (CameraTrack.isActive && CameraTrack.actions.Count > 0 && !this.isLinkedClip())
                {
                    CameraTrack.NeedUpdate = true;

                }
            }

        }
    }
}
