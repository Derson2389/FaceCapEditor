using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RecordNodeInfo
{
    public static string[] AnimProperties = new string[]
    {
        "m_LocalPosition.x",
        "m_LocalPosition.y",
        "m_LocalPosition.z",
        "m_LocalRotation.x",
        "m_LocalRotation.y",
        "m_LocalRotation.z",
        "m_LocalRotation.w"
    };

    public Transform Target;
    public string TreePath;
    public List<RecordNodeInfo> Children = new List<RecordNodeInfo>();

    public class RecordKeyframe
    {
        public float FrameTime;
        public Vector3 FramePosition;
        public Quaternion FrameRotation;

        public RecordKeyframe(float time, Transform trans)
        {
            FrameTime = time;
            FramePosition = trans.localPosition;
            FrameRotation = trans.localRotation;
        }
    }

    public List<RecordKeyframe> FrameList = new List<RecordKeyframe>();
    public AnimationCurve[] MappingCurve = new AnimationCurve[7];

    public static string GetTreePath(Transform target, Transform root)
    {
        if (root == null || target == null)
        {
            return string.Empty;
        }

        string path = string.Empty;

        if (target != root)
        {
            path = target.name;
        }

        Transform obj = target.parent;
        while (obj && obj != root)
        {
            path = path.Insert(0, obj.name + "/");
            obj = obj.parent;
        }

        return path;
    }

    public RecordNodeInfo(Transform root, Transform target)
    {
        TreePath = GetTreePath(target, root);
        Target = target;
        for (int i = 0; i < target.childCount; i++)
        {
            Children.Add(new RecordNodeInfo(root, target.GetChild(i)));
        }
    }

    public void StartRecord()
    {
        for (int i = 0; i < 7; i++)
        {
            MappingCurve[i] = new AnimationCurve();
        }


        FrameList.Clear();
        foreach (RecordNodeInfo child in Children)
        {
            child.StartRecord();
        }
    }

    public void Recored(float time)
    {
        FrameList.Add(new RecordKeyframe(time, Target));

        Vector3 pos = Target.localPosition;
        MappingCurve[0].AddKey(new Keyframe(time, pos.x, float.PositiveInfinity, float.PositiveInfinity));
        MappingCurve[1].AddKey(new Keyframe(time, pos.y, float.PositiveInfinity, float.PositiveInfinity));
        MappingCurve[2].AddKey(new Keyframe(time, pos.z, float.PositiveInfinity, float.PositiveInfinity));

        Quaternion rot = Target.localRotation;
        MappingCurve[3].AddKey(new Keyframe(time, rot.x, float.PositiveInfinity, float.PositiveInfinity));
        MappingCurve[4].AddKey(new Keyframe(time, rot.y, float.PositiveInfinity, float.PositiveInfinity));
        MappingCurve[5].AddKey(new Keyframe(time, rot.z, float.PositiveInfinity, float.PositiveInfinity));
        MappingCurve[6].AddKey(new Keyframe(time, rot.w, float.PositiveInfinity, float.PositiveInfinity));

        foreach (RecordNodeInfo child in Children)
        {
            child.Recored(time);
        }
    }

    public void FillClip(AnimationClip clip)
    {
        //if (string.IsNullOrEmpty(TreePath) == false)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("保存AnimationClip", string.Format("保存骨骼[{0}]", TreePath), (float)(new System.Random().Next(100)) / 100.0f);
#endif

            for (int i = 0; i < 7; i++)
            {
                if (MappingCurve[i].keys.Length > 0)
                {
                    clip.SetCurve(TreePath, typeof(UnityEngine.Transform), AnimProperties[i], MappingCurve[i]);
                }
            }
        }

        foreach (RecordNodeInfo child in Children)
        {
            child.FillClip(clip);
        }
    }

#if UNITY_EDITOR
    public static AnimationClip CreateClip(GameObject target, RecordNodeInfo root)
    {
        string fileName = DateTime.Now.ToString("MMddHHmmssfff");
        string filePath = "Assets/" + GateRecorderManagerBrage.RecordSavePath + "/" + fileName + ".anim";

        AnimationClip clip = new AnimationClip();
        clip.legacy = true;

        root.FillClip(clip);
         
        string fileDir = Application.dataPath + "/" + GateRecorderManagerBrage.RecordSavePath + target.name + "/";
        if (!Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }

        AnimationClip animationClip2 = AssetDatabase.LoadMainAssetAtPath(filePath) as AnimationClip;
        if (animationClip2)
        {
            EditorUtility.CopySerialized(clip, animationClip2);
            AssetDatabase.SaveAssets();
            UnityEngine.Object.DestroyImmediate(clip);
        }
        else
        {
            AssetDatabase.CreateAsset(clip, filePath);
        }

        return clip;
    }
#endif
}
