using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MBR;

public class MBNodeInfo
{
    protected MBNodeInfo m_Parent;
    protected List<MBNodeInfo> m_Childern = new List<MBNodeInfo>();
    protected string m_NodeName;
    protected Matrix4x4 m_GlobalMatrix = Matrix4x4.identity;
    protected Vector3 m_GlobalPosition = Vector3.zero;
    protected Quaternion m_GlobalRotation = Quaternion.identity;
    
    protected List<long> m_RecordTime = new List<long>();
//     protected List<Vector3> m_RecordPosition = new List<Vector3>();
//     protected List<Quaternion> m_RecordRotation = new List<Quaternion>();
    protected List<Matrix4x4> m_RecordMatrix = new List<Matrix4x4>();

    protected IntPtr m_ExptortPtr = IntPtr.Zero;
    protected List<float[]> m_FBXPosition = new List<float[]>();
    protected List<float[]> m_FBXRotation = new List<float[]>();

    public MBNodeInfo Parent
    {
        get
        {
            return m_Parent;
        }
    }

    public int ChildCount
    {
        get
        {
            return m_Childern.Count;
        }
    }

    public string NodeName
    {
        set
        {
            m_NodeName = value;
        }

        get
        {
            return m_NodeName;
        }
    }


    public MBNodeInfo(MBNodeInfo parent)
    {
        m_Parent = parent;
        if (parent != null)
        {
            parent.m_Childern.Add(this);
        }
    }

    protected void DecodeNodeInfo(BinaryReader br, MBNodeInfo nodeInfo)
    {
        bool isNan = false;
        nodeInfo.m_NodeName = br.ReadServerString();
        for (int i = 0; i < 16; i++)
        {
            m_GlobalMatrix[i] = (float)br.ReadDouble();
            if(float.IsNaN(m_GlobalMatrix[i]))
            {
                isNan = true;
            }
        }

        if(isNan)
        {
            m_GlobalMatrix = Matrix4x4.identity;
        }

        m_GlobalPosition = m_GlobalMatrix.GetColumn(3);
        m_GlobalRotation = m_GlobalMatrix.rotation;

        int count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            DecodeNodeInfo(br, new MBNodeInfo(nodeInfo));
        }
    }

    protected void DecodeUpdateNode(BinaryReader br)
    {
        bool isNan = false;
        for (int i = 0; i < 16; i++)
        {
            m_GlobalMatrix[i] = (float)br.ReadDouble();
            if (float.IsNaN(m_GlobalMatrix[i]))
            {
                isNan = true;
            }
        }

        if (isNan)
        {
            m_GlobalMatrix = Matrix4x4.identity;
        }

        m_GlobalPosition = m_GlobalMatrix.GetColumn(3);
        m_GlobalRotation = m_GlobalMatrix.rotation;

        int count = m_Childern.Count;
        for (int i = 0; i < count; i++)
        {
            m_Childern[i].DecodeUpdateNode(br);
        }
    }

    protected void Record(long time)
    {
        m_RecordTime.Add(time);
        m_RecordMatrix.Add(m_GlobalMatrix);
//         m_RecordPosition.Add(m_GlobalPosition);
//         m_RecordRotation.Add(m_GlobalRotation);
        for (int i = 0; i < m_Childern.Count; i++)
        {
            m_Childern[i].Record(time);
        }
    }

    public void ClearRecordData()
    {
        m_RecordTime.Clear();
        m_RecordMatrix.Clear();
//         m_RecordPosition.Clear();
//         m_RecordRotation.Clear();
        for (int i = 0; i < m_Childern.Count; i++)
        {
            m_Childern[i].ClearRecordData();
        }
    }

    public bool CreateExporterNode(Vector3 locOffset)
    {
        float[] loc = new float[3];
        float[] rot = new float[3];

        MBFBExporter.MBFB_SplitMatrix(MBFBExporter.ConvertMatrixToFloat(m_RecordMatrix.Count > 0 ? m_RecordMatrix[0] : m_GlobalMatrix), loc, rot);
        if(m_RecordMatrix.Count > 0)
        {
            loc[0] += locOffset.x;
            loc[1] += locOffset.y;
            loc[2] += locOffset.z;
        }

        m_ExptortPtr = MBFBExporter.MBFB_CreateNodeGbl(m_Parent == null ? IntPtr.Zero : m_Parent.m_ExptortPtr, m_NodeName, loc, rot, MBFBExporter.ConvertSizeFloat(Vector3.one));

        //         m_ExptortPtr = MBFBExporter.MBFB_CreateNodeGbl(m_Parent == null ? IntPtr.Zero : m_Parent.m_ExptortPtr, m_NodeName
        //             , MBFBExporter.ConvertLocationFloat(m_RecordPosition.Count > 0 ? m_RecordPosition[0] + locOffset : m_GlobalPosition)
        //             , MBFBExporter.ConvertEulerRotationFloat(m_RecordRotation.Count > 0 ? m_RecordRotation[0] : m_GlobalRotation), MBFBExporter.ConvertSizeFloat(Vector3.one));
        if (m_ExptortPtr == IntPtr.Zero)
        {
            return false;
        }

        m_FBXPosition.Clear();
        m_FBXRotation.Clear();


        for (int i=0; i<m_Childern.Count; i++)
        {
            if(m_Childern[i].CreateExporterNode(locOffset) == false)
            {
                return false;
            }
        }

        return true;
    }

    private Quaternion GetRightHandedQ(Quaternion q)
    {
        float angle;
        Vector3 a;
        q.ToAngleAxis(out angle, out a);
        return Quaternion.AngleAxis(angle, Vector3.Scale(a, new Vector3(1f, -1f, -1f)));
    }

    protected void UpdateFBXFrame(int frame, Vector3 locOffset)
    {
        float[] loc = new float[3];
        float[] rot = new float[3];

        MBFBExporter.MBFB_SplitMatrix(MBFBExporter.ConvertMatrixToFloat(m_RecordMatrix[frame]), loc, rot);
        {
            loc[0] += locOffset.x;
            loc[1] += locOffset.y;
            loc[2] += locOffset.z;
        }

        float[] scale = MBFBExporter.ConvertSizeFloat(Vector3.one);
        MBFBExporter.MBFB_SetNodeTransformGbl(m_ExptortPtr, loc, rot, scale);
        
        m_FBXPosition.Add(loc);
        m_FBXRotation.Add(rot);

        for (int i = 0; i < m_Childern.Count; i++)
        {
            m_Childern[i].UpdateFBXFrame(frame, locOffset);
        }
    }

    protected void ExportTransformAnim()
    {
        int frameCount = m_RecordTime.Count;
        if(frameCount == 0)
        {
            return;
        }

        float[] time = new float[frameCount];
        float[] posX = new float[frameCount];
        float[] posY = new float[frameCount];
        float[] posZ = new float[frameCount];
        float[] rotX = new float[frameCount];
        float[] rotY = new float[frameCount];
        float[] rotZ = new float[frameCount];
        float[] arriveTangent = new float[frameCount];
        float[] leaveTangent = new float[frameCount];
            
        for (int i = 0; i < frameCount; i++)
        {
            time[i] = (float)m_RecordTime[i] * 0.001f;
            
            posX[i] = m_FBXPosition[i][0];
            posY[i] = m_FBXPosition[i][1];
            posZ[i] = m_FBXPosition[i][2];
            rotX[i] = m_FBXRotation[i][0];
            rotY[i] = m_FBXRotation[i][1];
            rotZ[i] = m_FBXRotation[i][2];
            arriveTangent[i] = 0.0f;
            leaveTangent[i] = 0.0f;
        }

        MBFBExporter.MBFB_AddAnimate(m_ExptortPtr, "Lcl Translation", "X", frameCount, time, posX, arriveTangent, leaveTangent);
        MBFBExporter.MBFB_AddAnimate(m_ExptortPtr, "Lcl Translation", "Y", frameCount, time, posY, arriveTangent, leaveTangent);
        MBFBExporter.MBFB_AddAnimate(m_ExptortPtr, "Lcl Translation", "Z", frameCount, time, posZ, arriveTangent, leaveTangent);

        MBFBExporter.MBFB_AddAnimate(m_ExptortPtr, "Lcl Rotation", "X", frameCount, time, rotX, arriveTangent, leaveTangent);
        MBFBExporter.MBFB_AddAnimate(m_ExptortPtr, "Lcl Rotation", "Y", frameCount, time, rotY, arriveTangent, leaveTangent);
        MBFBExporter.MBFB_AddAnimate(m_ExptortPtr, "Lcl Rotation", "Z", frameCount, time, rotZ, arriveTangent, leaveTangent);

        for (int i = 0; i < m_Childern.Count; i++)
        {
            m_Childern[i].ExportTransformAnim();
        }
    }

    public void GetAllBones(ref List<IntPtr> boneList)
    {
        boneList.Add(m_ExptortPtr);
        for (int i = 0; i < m_Childern.Count; i++)
        {
            m_Childern[i].GetAllBones(ref boneList);
        }
    }

    public void AddChild(MBNodeInfo child)
    {
        m_Childern.Add(child);
    }

    public MBNodeInfo GetChildFromIndex(int index)
    {
        return m_Childern[index];
    }

    public MBNodeInfo GetChild(string name)
    {
        MBNodeInfo info = null;
        for (int i = 0; i < m_Childern.Count; i++)
        {
            info = m_Childern[i];
            if (info.m_NodeName.Contains(name))
            {
                return info;
            }
        }

        return null;
    }

    public Vector3 GetReceivePosition()
    {
        if (float.IsNaN(m_GlobalPosition.x) || float.IsNaN(m_GlobalPosition.y) || float.IsNaN(m_GlobalPosition.z))
        {
            return Vector3.zero;
        }

        return new Vector3(-m_GlobalPosition.x, m_GlobalPosition.y, m_GlobalPosition.z) * 0.01f;
    }

    public Quaternion GetReceivedRotation()
    {
        if (float.IsNaN(m_GlobalRotation.x) || float.IsNaN(m_GlobalRotation.y) || float.IsNaN(m_GlobalRotation.z) || float.IsNaN(m_GlobalRotation.w))
        {
            return Quaternion.identity;
        }

        return new Quaternion(-m_GlobalRotation.x, m_GlobalRotation.y, m_GlobalRotation.z, -m_GlobalRotation.w);
    }
}
