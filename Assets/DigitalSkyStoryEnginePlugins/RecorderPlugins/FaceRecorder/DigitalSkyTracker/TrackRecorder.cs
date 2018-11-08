using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DigitalSky.Tracker
{
    public class TrackRecorder : MonoBehaviour
    {
        protected TrackRetargeter _retargeter;
        protected bool _init = false;
        protected bool _isRecording = false;
        protected float _frameTime = 0;

        // Use this for initialization
        void Start()
        {

        }

        public virtual void Init(TrackRetargeter retargeter)
        {
            // 如果retargeter重定向失败，则不能录制
            _retargeter = retargeter;
            if (_retargeter == null || _retargeter.isBinding == false)
            {
                _init = false;
                return;
            }

            _init = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_init || !_isRecording)
                return;

            _frameTime += Time.deltaTime;
            Record(_frameTime);
        }

        public virtual void Record(float time)
        {
            if (!_init)
                return;
        }

        public void StartRecord()
        {
            if (!_init)
                return;

            _isRecording = true;
            _frameTime = 0f;
        }

        public void StopRecord()
        {
            _isRecording = false;
        }

        /// <summary>
        /// 获取对象到根结点的路径
        /// </summary>
        /// <param name="gameObject">需要获取路径的对象</param>
        /// <returns></returns>
        public static string GetPath(GameObject gameObject)
        {
            string path = "";
            List<string> parentNames = new List<string>();

            Transform child = gameObject.transform;
            while (child.parent != null)
            {
                parentNames.Add(child.name);
                child = child.parent;
            }

            for (int i = parentNames.Count - 1; i >= 0; i--)
            {
                path = path + parentNames[i] + "/";
            }

            return path.Substring(0, path.Length - 1);
        }

#if UNITY_EDITOR
        public static AnimationClip CreateClip(bool loop, float frameRate)
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = frameRate;

            SerializedObject serializedClip = new SerializedObject(clip);
            SerializedProperty loopTimeProperty = serializedClip.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (loopTimeProperty != null)
                loopTimeProperty.boolValue = loop;
            serializedClip.ApplyModifiedProperties();

            return clip;
        }

        public virtual AnimationClip CreateAnimationClip()
        {
            AnimationClip recordClip = CreateClip(true, 60);
            recordClip.legacy = true;

            return recordClip;
        }
#endif
    }
}

