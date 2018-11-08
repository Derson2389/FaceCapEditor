using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Tracker
{
    public class TrackData
    {
        /// <summary>
        /// 与ITrackBinding对应的bindName
        /// </summary>
        public string bindName = "";

        /// <summary>
        /// 实时Track的blendshape值
        /// </summary>
        public float blendshapeValue = 0;

        /// <summary>
        /// 实时Track的bone值, Local值
        /// </summary>
        public Vector3 boneLocalTrans = new Vector3();
        public Vector3 boneLocalRot = new Vector3();

        /// <summary>
        /// 实时Track的bone值, Global值
        /// </summary>
        public Vector3 boneGlobalTrans = new Vector3();
        public Vector3 boneGlobalRot = new Vector3();

        /// <summary>
        /// 表示当前是否禁用
        /// </summary>
        public bool used = true;
    }
}
