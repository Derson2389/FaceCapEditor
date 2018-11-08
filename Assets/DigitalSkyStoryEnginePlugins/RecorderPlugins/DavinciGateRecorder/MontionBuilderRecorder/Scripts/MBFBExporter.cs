using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class MBFBExporter
{
    [DllImport("MotionBuilderFbxBridge")]
    public static extern void MBFB_CreateExporter();
    [DllImport("MotionBuilderFbxBridge")]
    public static extern bool MBFB_WriteToFile(string path);
    [DllImport("MotionBuilderFbxBridge")]
    public static extern bool MBFB_CloseExporter(string path);

    [DllImport("MotionBuilderFbxBridge")]
    public static extern IntPtr MBFB_CreateNodeLcl(IntPtr parent, string name, float[] translation, float[] rotation, float[] scale);
    [DllImport("MotionBuilderFbxBridge")]
    public static extern IntPtr MBFB_CreateNodeGbl(IntPtr parent, string name, float[] translation, float[] rotation, float[] scale);
    [DllImport("MotionBuilderFbxBridge")]
    public static extern void MBFB_SetNodeTransformLcl(IntPtr node, float[] translation, float[] rotation, float[] scale);
    [DllImport("MotionBuilderFbxBridge")]
    public static extern void MBFB_SetNodeTransformGbl(IntPtr node, float[] translation, float[] rotation, float[] scale);

    [DllImport("MotionBuilderFbxBridge")]
    public static extern bool MBFB_CreateSkeleton(int boneCount, IntPtr[] boneList);

    [DllImport("MotionBuilderFbxBridge")]
    public static extern bool MBFB_AddAnimate(IntPtr node, string propertyName, string name, int framecount, float[] time, float[] value, float[] arriveTangent, float[] leaveTangent);

    [DllImport("MotionBuilderFbxBridge")]
    public static extern void MBFB_SplitMatrix(float[] mat, float[] outLoc, float[] outRot);

    public static float[] ConvertLocationFloat(Vector3 pos)
    {
        float[] value = new float[3];
        value[0] = pos.x;
        value[1] = pos.y;
        value[2] = pos.z;
        return value;
    }

    public static float[] ConvertSizeFloat(Vector3 size)
    {
        float[] value = new float[3];
        value[0] = size.x;
        value[1] = size.y;
        value[2] = size.z;
        return value;
    }

    public static float[] ConvertRotationFloat(Quaternion rot)
    {
        float[] value = new float[4];
        value[0] = rot.x;
        value[1] = rot.y;
        value[2] = rot.z;
        value[3] = rot.w;
        return value;
    }

    public static float[] ConvertEulerRotationFloat(Quaternion rot)
    {
        float[] value = new float[3];
        Vector3 euler = rot.eulerAngles;

        value[0] = euler.x;
        value[1] = euler.y;
        value[2] = euler.z;

        return value;
    }

    public static float[] ConvertMatrixToFloat(Matrix4x4 mat)
    {
        float[] value = new float[16];
        for(int i=0; i<16; i++)
        {
            value[i] = mat[i];
        }

        return value;
    }

    public static Vector3 DeconvertLocationFloat(float[] values)
    {
        return new Vector3(values[0], values[1], values[2]);
    }

    public static Vector3 DeconvertSizeFloat(float[] values)
    {
        return new Vector3(values[0], values[1], values[2]);
    }

    public static Quaternion DeconvertRotationFloat(float[] values)
    {
        return new Quaternion(values[0], values[1], values[2], values[3]);
    }
}
