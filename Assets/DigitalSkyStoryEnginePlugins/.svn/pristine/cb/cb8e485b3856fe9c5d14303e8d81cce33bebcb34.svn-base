using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DigitalSky.Tracker
{
    public class BlendAnimation
    {
        private AnimationCurve _curve = null;
        private TrackBindingTarget _bindTarget = null;

        public BlendAnimation(TrackBindingTarget bindTarget)
        {
            _bindTarget = bindTarget;
            _curve = new AnimationCurve();
            _curve.AddKey(0, 0);
        }

        public void AddKey(float time)
        {
            float blendWeight = _bindTarget.GetBlendShape();
            _curve.AddKey(time, blendWeight);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 获取AnimationClip的CurveBinding
        /// </summary>
        /// <param name="path">CurveBinding的结点路径</param>
        /// <param name="bindName">CurveBinding的绑定属性名字</param>
        /// <returns></returns>
        public EditorCurveBinding GetCurveBinding(string path, string bindName)
        {
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.path = path;
            curveBinding.type = typeof(UnityEngine.SkinnedMeshRenderer);
            curveBinding.propertyName = bindName;

            return curveBinding;
        }

        /// <summary>
        /// 绑定录制的Curve曲线到AnimatinClip对象
        /// </summary>
        /// <returns></returns>
        public void BindingClip(AnimationClip clip)
        {
            // 将_curve曲线绑定到AnimationClip上
            string propName = "blendShape." + _bindTarget.GetBlendShapeName();
            EditorCurveBinding curveBinding = GetCurveBinding(TrackRecorder.GetPath(_bindTarget.target), propName);
            AnimationUtility.SetEditorCurve(clip, curveBinding, _curve);
        }

#endif
    }

    public class BlendTrackRecorder : TrackRecorder
    {
        // 保存blendshape值的动画曲线集合
        private List<BlendAnimation> _animationCurves = null;

        // Use this for initialization
        void Start()
        {

        }

        public override void Init(TrackRetargeter retargeter)
        {
            // 如果retargeter重定向失败，则不能录制
            _retargeter = retargeter;
            if (_retargeter == null || _retargeter.isBinding == false)
            {
                _init = false;
                return;
            }

            // 通过trackBinding数据生成动画曲线信息
            _animationCurves = new List<BlendAnimation>();
            for (int i = 0; i < _retargeter.trackBindings.Count; i++)
            {
                if (_retargeter.trackBindings[i].target == null)
                    continue;

                TrackBindingTarget trackTarget = _retargeter.trackBindings[i].target;
                string blendName = trackTarget.GetBlendShapeName();
                if (blendName == "")
                    continue;

                _animationCurves.Add(new BlendAnimation(trackTarget));
            }

            _init = true;
        }

        public override void Record(float time)
        {
            if (!_init)
                return;

            // 根据trackBinding数据在blendshape动画曲线上添加关键帧
            for(int i = 0; i < _animationCurves.Count; i++)
            {
                _animationCurves[i].AddKey(time);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// 创建Recorder录制的AnimatinClip对象
        /// </summary>
        /// <returns></returns>
        public override AnimationClip CreateAnimationClip()
        {
            AnimationClip recordClip = CreateClip(true, 60);
            recordClip.legacy = true;

            if (_animationCurves == null)
                return recordClip;

            // 将blendAnimation动画曲线绑定到clip对象上
            for (int i = 0; i < _animationCurves.Count; i++)
            {
                _animationCurves[i].BindingClip(recordClip);
            }

            return recordClip;
        }

#endif
    }
}
