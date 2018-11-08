using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Tracker
{
    public class StandardBoneData : MonoBehaviour
    {
        // 需要构建骨架的目标对象
        public GameObject target;

        public bool init
        {
            get { return _rootBone != null; }
        }

        // 骨骼根结点
        private StandardBone _rootBone;
        public StandardBone rootBone
        {
            get { return _rootBone; }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(GameObject targetObj = null)
        {
            if (targetObj == null)
                target = gameObject;
            else
                target = targetObj;

            Transform hipsTran = BipBoneHelper.FindBipBoneTransform(target.transform, BipBoneHelper.BipBone.Hips);
            if (hipsTran == null && target.transform.childCount > 0)
            {
                hipsTran = target.transform.transform.GetChild(0);
            }

            if (hipsTran == null)
                return;

            _rootBone = new StandardBone(BipBoneHelper.BipBone.Hips, target, null, hipsTran);
            CreateBoneTree(target, _rootBone, _rootBone.boneTransform);
        }

        private void CreateBoneTree(GameObject rootObj, StandardBone parent, Transform parentTrans)
        {
            for (int i = 0; i < parentTrans.childCount; i++)
            {
                Transform trans = parentTrans.GetChild(i);
                BipBoneHelper.BipBone boneType = BipBoneHelper.FindBipBone(trans.name);
                if (boneType != BipBoneHelper.BipBone.BoneCount)
                {
                    StandardBone bone = new StandardBone(boneType, rootObj, parent, trans);
                    CreateBoneTree(rootObj, bone, bone.boneTransform);
                }
            }
        }
    }
}
