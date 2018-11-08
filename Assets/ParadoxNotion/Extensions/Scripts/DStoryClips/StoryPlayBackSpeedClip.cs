using UnityEngine;
using System.Collections;

namespace Slate.ActionClips.StoryEngineClip
{
    [Category("DStoryEngine")]
    public class StoryPlayBackSpeedClip : DirectorActionClip
    {
        [SerializeField]
        [HideInInspector]
        private float _length = 0f;   

        [SerializeField]
        [HideInInspector]
        private string _dataName = "";
        public string dataName
        {
            get { return _dataName; }
            set { _dataName = value; }
        }

        [SerializeField]
        public float playBackSpeed = 1.0f;

        public override string info
        {
            get {
                return dataName == "" ? "playBackSpeed" : dataName;
            }
        }

        public override float length
        {
            get {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        public void OnDataLoad()
        {
            //Debug.Log("DialogClip.OnDataLoad-> enter");

        }

#if UNITY_EDITOR
        public void OnDataRemove()
        {
            
        }
#endif

        protected override void OnEnter()
        {
            //Debug.Log("DialogClip.OnEnter-> enter");
            root.playbackSpeed = playBackSpeed;
        }

        protected override void OnUpdate(float time, float previousTime)
        {
            
        }

        protected override void OnExit()
        {
           
        }
    }
}

