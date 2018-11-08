using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Tracker
{
    public class TrackBindingTarget
    {
        // for blendshapes tracking value
        private int _blendshapeIndex = -1;
        private float _orignalWeight = 0f;
        private float _blendshapeWeight = 0f;

        // for bones tracking value
        private Transform _boneTransform = null;
        private Vector3 _localPosition = Vector3.zero;
        private Vector3 _localRotation = Vector3.zero;

        // 保存的初始骨骼位置
        private Vector3 _boneLocalNeutralTrans = Vector3.zero;
        public Vector3 boneLocalNeutralTrans
        {
            get { return _boneLocalNeutralTrans; }
        }

        // 保存的初始骨骼旋转
        private Quaternion _boneLocalNeutralRot = Quaternion.identity;
        public Quaternion boneLocalNeutralRot
        {
            get { return _boneLocalNeutralRot; }
        }

        // BindingTarget的SkinnedMeshRenderer组件
        private SkinnedMeshRenderer _renderer = null;

        // 绑定的GameObject target
        private GameObject _target = null;
        public GameObject target
        {
            get { return _target; }
        }

        public TrackBindingTarget(GameObject target, int blendshape)
        {
            _target = target;
            _renderer = target.GetComponent<SkinnedMeshRenderer>();
            _blendshapeIndex = blendshape;

            if (_renderer && _blendshapeIndex >= 0)
                _orignalWeight = _renderer.GetBlendShapeWeight(_blendshapeIndex);

            _boneTransform = null;
            _boneLocalNeutralTrans = Vector3.zero;
            _boneLocalNeutralRot = Quaternion.identity;
        }

        public TrackBindingTarget(GameObject target, Transform bone)
        {
            _target = target;
            _renderer = null;
            _blendshapeIndex = -1;

            _boneTransform = bone;

            // Save the neutral translation and the neutral rotation of the bone.
            // This will be used while reading streaming for calculation and also
            // used for restoring default pose when server is disconnected.
            _boneLocalNeutralTrans = bone.localPosition;
            _boneLocalNeutralRot = bone.localRotation;           
        }

        /// <summary>
        /// 重置到初始值
        /// </summary>
        public void Reset()
        {
            if (_renderer && _blendshapeIndex >= 0)
                _renderer.SetBlendShapeWeight(_blendshapeIndex, _orignalWeight);

            if (_boneTransform != null)
            {
                _boneTransform.localPosition = _boneLocalNeutralTrans;
                _boneTransform.localRotation = _boneLocalNeutralRot;
            }
        }

        public void SetBlendShape(float value)
        {
            /*if (_renderer && blendshapeIndex >= 0)
                _renderer.SetBlendShapeWeight(blendshapeIndex, value);*/

            _blendshapeWeight = value;
        }

        public float GetBlendShape()
        {
            /*if (_renderer && blendshapeIndex >= 0)
                return _renderer.GetBlendShapeWeight(blendshapeIndex);

            return 0f;*/

            return _blendshapeWeight;
        }

        public string GetBlendShapeName()
        {
            if (_renderer && _blendshapeIndex >= 0)
                return _renderer.sharedMesh.GetBlendShapeName(_blendshapeIndex);

            return "";
        }

        public void SetLocalPosition(Vector3 pos)
        {
            _localPosition = pos;
        }

        public Vector3 GetLocalPosition()
        {
            return _localPosition;
        }

        public void SetLocalRotation(Vector3 rot)
        {
            _localRotation = rot;
        }

        public Vector3 GetLocalRotation()
        {
            return _localRotation;
        }

        public string GetBoneName()
        {
            if (_boneTransform != null)
                return _boneTransform.name;

            return "";
        }

        public void Apply()
        {
            if (_renderer && _blendshapeIndex >= 0)
                _renderer.SetBlendShapeWeight(_blendshapeIndex, _blendshapeWeight);

            if (_boneTransform != null)
            {
                _boneTransform.localRotation = Quaternion.Euler(_localRotation);
                _boneTransform.localPosition = _localPosition;
            }
        }

    }
}
