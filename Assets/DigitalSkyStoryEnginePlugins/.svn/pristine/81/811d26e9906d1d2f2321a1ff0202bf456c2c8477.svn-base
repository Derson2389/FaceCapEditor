using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Tracker
{
    public class StandardBone
    {
        // bone的骨骼标识
        public BipBoneHelper.BipBone boneType;

        // 保存bone的父结点
        public StandardBone parent;

        // 保存bone的子结点
        public List<StandardBone> children = new List<StandardBone>();

        // 保存bone的骨骼结点
        public Transform boneTransform = null;

        public GameObject rootObj = null;

        public StandardBone(BipBoneHelper.BipBone boneType, GameObject rootObj, StandardBone parent, Transform bone)
        {
            this.boneType = boneType;
            this.rootObj = rootObj;
            this.parent = parent;
            this.boneTransform = bone;

            if (parent != null)
                parent.children.Add(this);
        }
    }
}
