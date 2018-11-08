using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Tracker
{
    public interface ITrackBinding
    {
        /// <summary>
        /// 绑定名字， 用于查找对应BlendShape或骨骼名字
        /// </summary>
        string bindName { get; }

        /// <summary>
        /// 绑定权重， 与Tracking结果相乘得到最终权重，默认为1，即Tracking本身的权重
        /// </summary>
        float bindWeight { get; }

        /// <summary>
        /// 过滤大小， 取最近几次的平均值
        /// </summary>
        int filterSize { get; }

        /// <summary>
        /// Tracking结果区间范围
        /// </summary>
        Vector2 limits { get; }

        /// <summary>
        /// 绑定对象数组
        /// </summary>
        TrackBindingTarget target { get; }

        void Init();

        void OnUpdateTrackData(TrackData data);
    }
}

