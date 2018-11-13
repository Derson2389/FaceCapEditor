using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DigitalSky.Tracker
{
    public class BoneAnimation
    {
        public static string[] animProperties = new string[]
        {
            "m_LocalPosition.x",
            "m_LocalPosition.y"
            //"m_LocalPosition.z",
            //"m_LocalRotation.x",
            //"m_LocalRotation.y",
            //"m_LocalRotation.z",
            //"m_LocalRotation.w"
        };

        // 保存一个bone节点的位移和旋转值的动画曲线集合
        private Dictionary<string, AnimationCurve> _boneCurves = null;
        private TrackBindingTarget _bindTarget = null;
        //private Transform _boneTran = null;

        public BoneAnimation(TrackBindingTarget bindTarget)
        {
            //_boneTran = tran;
            _bindTarget = bindTarget;

            _boneCurves = new Dictionary<string, AnimationCurve>();
            for (int i = 0; i < animProperties.Length;i++ )
            {
                _boneCurves.Add(animProperties[i], new AnimationCurve());                
            }
            
        }

        public void AddKey(float time)
        {
            Vector3 pos = _bindTarget.GetLocalPosition();
            _boneCurves[animProperties[0]].AddKey(new Keyframe(time, pos.x, float.PositiveInfinity, float.PositiveInfinity));
            _boneCurves[animProperties[1]].AddKey(new Keyframe(time, pos.y, float.PositiveInfinity, float.PositiveInfinity));

            ///_boneCurves[animProperties[2]].AddKey(new Keyframe(time, pos.z, float.PositiveInfinity, float.PositiveInfinity));

            //Quaternion rot = Quaternion.Euler(_bindTarget.GetLocalRotation());
            //_boneCurves[animProperties[3]].AddKey(new Keyframe(time, rot.x, float.PositiveInfinity, float.PositiveInfinity));
            //_boneCurves[animProperties[4]].AddKey(new Keyframe(time, rot.y, float.PositiveInfinity, float.PositiveInfinity));
            //_boneCurves[animProperties[5]].AddKey(new Keyframe(time, rot.z, float.PositiveInfinity, float.PositiveInfinity));
            //_boneCurves[animProperties[6]].AddKey(new Keyframe(time, rot.w, float.PositiveInfinity, float.PositiveInfinity));
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
            curveBinding.type = typeof(UnityEngine.Transform);
            curveBinding.propertyName = bindName;

            return curveBinding;
        }

        /// <summary>
        /// 创建Recorder录制的AnimatinClip对象
        /// </summary>
        /// <returns></returns>
        public void BindingClip(AnimationClip clip)
        {
            foreach(var item in _boneCurves)
            {
                // 将_animationCurves的每一条曲线绑定到AnimationClip上
                //EditorCurveBinding curveBinding = GetCurveBinding(TrackRecorder.GetPath(_boneTran.gameObject), item.Key);
                //AnimationCurve curve = item.Value;
                //AnimationUtility.SetEditorCurve(clip, curveBinding, curve);

                AnimationCurve curve = item.Value;
                clip.SetCurve(TrackRecorder.GetPath(_bindTarget.target), typeof(UnityEngine.Transform), item.Key, curve);
            }
        }

#endif
    }

    public class BoneTrackRecorder : TrackRecorder
    {
        // 保存blendshape值的动画曲线集合
        private List<BoneAnimation> _animationCurves = null;
        private Dictionary<string, TrackBindingTarget> _animateNames = null;
        public BlendShapeCtrlClip _ctrlShape = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
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
            _animationCurves = new List<BoneAnimation>();
            for (int i = 0; i < _retargeter.trackBindings.Count; i++)
            {
                if (_retargeter.trackBindings[i].target == null)
                    continue;

                /*TrackBindingTarget trackTarget = _retargeter.trackBindings[i].target;
                if (trackTarget.boneTransform == null)
                    continue;*/

                _animationCurves.Add(new BoneAnimation(_retargeter.trackBindings[i].target));
            }

            _ctrlShape = new BlendShapeCtrlClip();
            _animateNames = new Dictionary<string, TrackBindingTarget>();
            if (_retargeter is dxyz.PrevizTrackRetargeter)
            {
                var previzTrack = _retargeter as dxyz.PrevizTrackRetargeter;
                _ctrlShape.CtrlConfigDataFile = previzTrack.controllerConfiguration;
                _ctrlShape.EditKeyable(0);
                for (int i = 0; i < _retargeter.trackBindings.Count; i++)
                {
                    if (_retargeter.trackBindings[i].target == null)
                        continue;                 
                    _animateNames.Add(_retargeter.trackBindings[i].bindName, _retargeter.trackBindings[i].target);
                }
            }
           
            _init = true;
        }

        public override void Record(float time)
        {
            if (!_init)
                return;

            // 根据trackBinding数据在动画曲线上添加关键帧
            for (int i = 0; i < _animationCurves.Count; i++)
            {
                _animationCurves[i].AddKey(time);
            }
            if (_ctrlShape != null&& _ctrlShape.editKey != null)
            {
                foreach (string name in _animateNames.Keys)
                {
                    Vector3 pos = _animateNames[name].GetLocalPosition();
                    Vector2 vec = new Vector2(pos.x, pos.z);
                    _ctrlShape.editKey.Addkey(name, vec, time);
                }               
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

            // 将boneAnimation的每一条曲线绑定到AnimationClip上
            for (int i = 0; i < _animationCurves.Count; i++)
            {
                _animationCurves[i].BindingClip(recordClip);
            }

            return recordClip;
        }

#endif
    }
}
