using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;

namespace dxyz
{
    public class PrevizTrackBinding : TrackBinding
    {
        public enum Type
        {
            INVALID,
            BONE,
            HEAD_BONE,
            BLENDSHAPE
        }

        public Type type = Type.INVALID;
        public int ecsCoeffIndex = -1;

        public PrevizCtrlHandler _ctrlHander = null;


        public PrevizTrackBinding(): base()
        {
            
        }

        public override void OnUpdateTrackData(TrackData data)
        {
            PrevizTrackData trackData = (PrevizTrackData)data;

            // 如果目标节点为空，则略过
            if (target == null)
                return;

            if (trackData.type == Type.BLENDSHAPE)
            {                
                _normalizedValue = Mathf.Clamp01(trackData.blendshapeValue);

                // filter value
                _filteredValue = Filter(_normalizedValue);

                // set tracking value to TrackBindingTarget
                if (target != null)
                    target.SetBlendShape(_filteredValue * 100f);
            }
            else if (trackData.type == Type.BONE || trackData.type == Type.HEAD_BONE)
            {
                if (trackData.type == Type.BONE)
                {
                    // TODO: move this into grabber3.
                    //target.boneTransform.localPosition = trackData.boneLocalTrans;

                    //Quaternion rQuat = Quaternion.Euler(-trackData.boneLocalRot.x, -trackData.boneLocalRot.y, -trackData.boneLocalRot.z);
                    //target.boneTransform.localRotation = target.boneLocalNeutralRot * rQuat;

                    target.SetLocalPosition(trackData.boneLocalTrans);

                    Quaternion rQuat = Quaternion.Euler(-trackData.boneLocalRot.x, -trackData.boneLocalRot.y, -trackData.boneLocalRot.z);
                    Quaternion rot = target.boneLocalNeutralRot * rQuat;
                    target.SetLocalRotation(rot.eulerAngles);
                }
                else
                {
                    // TODO: move this into grabber3.
                    //Quaternion rQuat = Quaternion.Euler(trackData.boneLocalRot.x, trackData.boneLocalRot.y, trackData.boneLocalRot.z);
                    //target.boneTransform.localRotation = target.boneLocalNeutralRot * rQuat;

                    Quaternion rQuat = Quaternion.Euler(trackData.boneLocalRot.x, trackData.boneLocalRot.y, trackData.boneLocalRot.z);
                    Quaternion rot = target.boneLocalNeutralRot * rQuat;
                    target.SetLocalRotation(rot.eulerAngles);
                }
            }

            target.Apply(_ctrlHander);
        }
    }
}
