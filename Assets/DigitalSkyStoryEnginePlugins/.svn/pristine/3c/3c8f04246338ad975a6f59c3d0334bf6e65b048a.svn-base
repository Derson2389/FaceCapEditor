using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BipBoneHelper
{
    public enum BipBone
    {
        Hips,

        LeftHips_Dummy,
        LeftUpLeg,
        LeftUpLegRoll,
        LeftLeg,
        LeftLegRoll,
        LeftFoot,
        LeftToeBase,

        RightHips_Dummy,
        RightUpLeg,
        RightUpLegRoll,
        RightLeg,
        RightLegRoll,
        RightFoot,
        RightToeBase,

        Spine_Dummy,
        Spine,
        Spine1,
        Spine2,
        LeftShoulder_Dummy,
        LeftShoulder,
        LeftArm_Dummy,
        LeftArm,
        LeftArmRoll,
        LeftForeArm,
        LeftForeArmRoll,
        LeftHand,
        LeftHandIndex1,
        LeftHandIndex2,
        LeftHandIndex3,
        LeftHandMiddle1,
        LeftHandMiddle2,
        LeftHandMiddle3,
        LeftHandPinky1,
        LeftHandPinky2,
        LeftHandPinky3,
        LeftHandRing1,
        LeftHandRing2,
        LeftHandRing3,
        LeftHandThumb1,
        LeftHandThumb2,
        LeftHandThumb3,

        Neck,
        Neck1,
        Head,

        RightShoulder_Dummy,
        RightShoulder,
        RightArm_Dummy,
        RightArm,
        RightArmRoll,
        RightForeArm,
        RightForeArmRoll,
        RightHand,
        RightHandIndex1,
        RightHandIndex2,
        RightHandIndex3,
        RightHandMiddle1,
        RightHandMiddle2,
        RightHandMiddle3,
        RightHandPinky1,
        RightHandPinky2,
        RightHandPinky3,
        RightHandRing1,
        RightHandRing2,
        RightHandRing3,
        RightHandThumb1,
        RightHandThumb2,
        RightHandThumb3,

        BoneCount,
    };

    public static string[] MaxBipBoneName =
    {
        "Pelvis",

        "LeftHips_Dummy",
        "L Thigh",
        "LeftUpLegRoll",
        "L Calf",
        "LeftLegRoll",
        "L Foot",
        "L Toe0",

        "RightHips_Dummy",
        "R Thigh",
        "RightUpLegRoll",
        "R Calf",
        "RightLegRoll",
        "R Foot",
        "R Toe0",

        "Spine",
        "Spine1",
        "Spine2",
        "Spine3",
        "LeftShoulder_Dummy",
        "L Clavicle",
        "LeftArm_Dummy",
        "L UpperArm",
        "LeftArmRoll",
        "L Forearm",
        "LeftForeArmRoll",
        "L Hand",
        "L Finger0",
        "L Finger01",
        "L Finger02",
        "L Finger2",
        "L Finger21",
        "L Finger22",
        "L Finger4",
        "L Finger41",
        "L Finger42",
        "L Finger3",
        "L Finger31",
        "L Finger32",
        "L Finger1",
        "L Finger11",
        "L Finger12",

        "Neck",
        "Neck1",
        "Head",

        "RightShoulder_Dummy",
        "R Clavicle",
        "RightArm_Dummy",
        "R UpperArm",
        "RightArmRoll",
        "R Forearm",
        "RightForeArmRoll",
        "R Hand",
        "R Finger0",
        "R Finger01",
        "R Finger02",
        "R Finger2",
        "R Finger21",
        "R Finger22",
        "R Finger4",
        "R Finger41",
        "R Finger42",
        "R Finger3",
        "R Finger31",
        "R Finger32",
        "R Finger1",
        "R Finger11",
        "R Finger12",
    };

    public static string[] MayaBipBoneName =
    {
        "Hips",

        "LeftHips_Dummy",
        "LeftUpLeg",
        "LeftUpLegRoll",
        "LeftLeg",
        "LeftLegRoll",
        "LeftFoot",
        "LeftToeBase",

        "RightHips_Dummy",
        "RightUpLeg",
        "RightUpLegRoll",
        "RightLeg",
        "RightLegRoll",
        "RightFoot",
        "RightToeBase",

        "Spine",
        "Spine1",
        "Spine2",
        "Spine3",
        "LeftShoulder_Dummy",
        "LeftShoulder",
        "LeftArm_Dummy",
        "LeftArm",
        "LeftArmRoll",
        "LeftForeArm",
        "LeftForeArmRoll",
        "LeftHand",
        "LeftHandIndex1",
        "LeftHandIndex2",
        "LeftHandIndex3",
        "LeftHandMiddle1",
        "LeftHandMiddle2",
        "LeftHandMiddle3",
        "LeftHandPinky1",
        "LeftHandPinky2",
        "LeftHandPinky3",
        "LeftHandRing1",
        "LeftHandRing2",
        "LeftHandRing3",
        "LeftHandThumb1",
        "LeftHandThumb2",
        "LeftHandThumb3",

        "Neck",
        "Neck1",
        "Head",

        "RightShoulder_Dummy",
        "RightShoulder",
        "RightArm_Dummy",
        "RightArm",
        "RightArmRoll",
        "RightForeArm",
        "RightForeArmRoll",
        "RightHand",
        "RightHandIndex1",
        "RightHandIndex2",
        "RightHandIndex3",
        "RightHandMiddle1",
        "RightHandMiddle2",
        "RightHandMiddle3",
        "RightHandPinky1",
        "RightHandPinky2",
        "RightHandPinky3",
        "RightHandRing1",
        "RightHandRing2",
        "RightHandRing3",
        "RightHandThumb1",
        "RightHandThumb2",
        "RightHandThumb3",
    };

    private static bool CompareName(string targetName, string boneName)
    {
        if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(boneName) || targetName.Length < boneName.Length)
        {
            return false;
        }

        if (targetName.LastIndexOf(boneName) == targetName.Length - boneName.Length)
        {
            return true;
        }

        return false;
    }

    public static BipBone FindBipBone(string boneName)
    {
        //         for (int i = 1; i < MaxBipBoneName.Length; i++)
        //         {
        //             if (CompareName(boneName, MaxBipBoneName[i]))
        //             {
        //                 return (BipBone)i;
        //             }
        //         }

        for (int i = 1; i < MayaBipBoneName.Length; i++)
        {
            if (CompareName(boneName, MayaBipBoneName[i]))
            {
                return (BipBone)i;
            }
        }

        //         for (int i = 1; i < (int)BipBone.BoneCount; i++)
        //         {
        //             if(CompareName(boneName, ((BipBone)i).ToString()))
        //             {
        //                 return (BipBone)i;
        //             }
        //         }

        return BipBone.BoneCount;
    }

    public static Transform FindBipBoneTransform(Transform target, BipBone bone)
    {
        if (target == null)
        {
            return null;
        }

        //         if (CompareName(target.name, MaxBipBoneName[(int)bone]))
        //         {
        //             return target;
        //         }

        if (CompareName(target.name, MayaBipBoneName[(int)bone]))
        {
            return target;
        }

        //         if (CompareName(target.name, bone.ToString()))
        //         {
        //             return target;
        //         }

        Transform trans = null;
        for (int i = 0; i < target.childCount; i++)
        {
            trans = FindBipBoneTransform(target.GetChild(i), bone);
            if (trans != null)
            {
                return trans;
            }
        }

        return null;
    }

    public static string GetBonePath(Transform bone, Transform root)
    {
        if (root == null || bone == null)
        {
            return string.Empty;
        }

        string path = string.Empty;

        if (bone != root)
        {
            path = bone.name;
        }

        Transform obj = bone.parent;
        while (obj && obj != root)
        {
            path = path.Insert(0, obj.name + "/");
            obj = obj.parent;
        }

        return path;
    }
}
