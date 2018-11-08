using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Tracker
{
    public class TrackBinding : ITrackBinding
    {
        protected string _bindName = "";
        /// <summary>
        /// 绑定名字， 用于查找对应BlendShape或骨骼名字
        /// </summary>
        public string bindName
        {
            get { return _bindName; }
            set { _bindName = value; }
        }

        protected float _bindWeight = 1f;
        /// <summary>
        /// 绑定权重， 与Tracking结果相乘得到最终权重，默认为1，即Tracking本身的权重
        /// </summary>
        public float bindWeight
        {
            get { return _bindWeight; }
            set { _bindWeight = value; }
        }

        protected int _filterSize = 1;
        /// <summary>
        /// 过滤大小， 取最近几次的平均值
        /// </summary>
        public int filterSize
        {
            get { return _filterSize; }
            set { _filterSize = value; }
        }

        protected Vector2 _limits = new Vector2(0, 1);
        /// <summary>
        /// Tracking结果区间范围
        /// </summary>
        public Vector2 limits
        {
            get { return _limits; }
            set { _limits = value; }
        }

        protected TrackBindingTarget _target = null;
        /// <summary>
        /// 绑定对象数组
        /// </summary>
        public TrackBindingTarget target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// 是否反转
        /// </summary>
        public bool inverted;

        /// <summary>
        /// 平滑系数
        /// </summary>
        public float filterConstant = 0.3f;

        protected float _normalizedValue;
        protected float[] _normalizedValueHistory;
        protected float _filteredValue;

        public TrackBinding()
        {
            _bindName = "";
            _target = null;
        }

        public virtual void Init()
        {
            _normalizedValueHistory = new float[filterSize];
        }

        public virtual void OnUpdateTrackData(TrackData data)
        {
            if (data == null || data.bindName != bindName)
                return;

            _normalizedValue = (data.blendshapeValue - limits.x) / (limits.y - limits.x);
            _normalizedValue = Mathf.Clamp01(_normalizedValue);
            if (inverted)
                _normalizedValue = 1f - _normalizedValue;

            // filter value
            _filteredValue = Filter(_normalizedValue);

            // set tracking value to TrackBindingTarget
            if (target != null)
            {
                target.SetBlendShape(_filteredValue * bindWeight * 100f);
                target.Apply();
            }
        } 


        protected float Filter(float value)
        {
            if (filterSize == 1)
                return value;

            // push back normalized history
            for (int i = 1; i < filterSize; i++)
                _normalizedValueHistory[i - 1] = _normalizedValueHistory[i];

            // add normalized value to history
            _normalizedValueHistory[filterSize - 1] = value;

            // get maximum variation
            float maxVariation = 0f;
            for (int i = 0; i < filterSize - 1; i++)
                maxVariation = Mathf.Max(maxVariation, Mathf.Abs(value - _normalizedValueHistory[i]));

            // get weights
            float[] weights = new float[filterSize];
            for (int i = 0; i < filterSize; i++)
            {
                weights[i] = Mathf.Exp(-i * filterConstant * maxVariation);
            }

            // get sum of weights
            float weightSum = 0f;
            for (int i = 0; i < filterSize; i++)
                weightSum += weights[i];

            // filter value
            float filteredValue = 0f;
            for (int i = 0; i < filterSize; i++)
            {
                filteredValue += weights[i] * _normalizedValueHistory[i] / weightSum;
            }

            return filteredValue;
        }
    }
}
