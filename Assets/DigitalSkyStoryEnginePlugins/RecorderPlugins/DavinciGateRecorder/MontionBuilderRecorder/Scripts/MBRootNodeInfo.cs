﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using MBR;

public class MBRootNodeInfo : MBNodeInfo
{
    private Socket m_Creator;
    private int m_Id;
    private string m_CreateIp;
    private long m_UpdateTime;
    private int m_NodeType; // 角色 0, Camera 1, 道具 2
    private string m_RootName;
    private bool m_IsRecording = false;
    private int m_RecordingCount = 0;

    private long m_StartRecordTime;

    public MBRootNodeInfo(int id, Socket socket)
        : base(null)
    {
        m_Id = id;
        m_Creator = socket;
        m_CreateIp = socket.RemoteEndPoint.ToString();
    }

    public int GetId()
    {
        return m_Id;
    }

    public int GetNodeType()
    {
        return m_NodeType;
    }

    public Socket GetSocket()
    {
        return m_Creator;
    }

    public bool IsRecording()
    {
        return m_IsRecording;
    }

    public string RootName
    {
        get
        {
            return m_RootName;
        }
    }

    public string CreateIp
    {
        get
        {
            return m_CreateIp;
        }
    }

    public void DecodeTree(BinaryReader br)
    {
        m_UpdateTime = br.ReadInt64();
        m_NodeType = br.ReadInt32();
        m_RootName = br.ReadServerString();

        Debug.Log("MontionRecorderServer Create New Node: " + m_RootName);

        DecodeNodeInfo(br, this);
    }

    public void DecodeUpdateTree(BinaryReader br)
    {
        m_UpdateTime = br.ReadInt64();
        DecodeUpdateNode(br);

        if(m_IsRecording)
        {
            RecordRoot();
        }
    }

    public void StartRecord()
    {
        m_RecordingCount++;
        if(m_RecordingCount == 1)
        {
            m_IsRecording = true;
            m_StartRecordTime = m_UpdateTime;
        }

        RecordRoot();
    }

    public void StopRecord()
    {
        m_RecordingCount--;
        if(m_RecordingCount == 0)
        {
            m_IsRecording = false;
            ClearRootRecordData();
        }
    }

    public void SaveToFBX(string path, Vector3 locOffset, MBNodeInfo rootExtend)
    {
        m_IsRecording = false;

        ExportToFBX(path, locOffset, rootExtend);
    }

    public void RecordRoot()
    {
        Record(m_UpdateTime - m_StartRecordTime);
    }

    public void ClearRootRecordData()
    {
        ClearRecordData();
    }


    public void ExportToFBX(string path, Vector3 locOffset, MBNodeInfo rootExtend)
    {
        if(m_RecordTime.Count == 0)
        {
            return;
        }

        MBFBExporter.MBFB_CreateExporter();

        MBNodeInfo tmpParent = rootExtend;
        if(rootExtend != null)
        {
            while(tmpParent.ChildCount > 0)
            {
                tmpParent = tmpParent.GetChildFromIndex(0);
            }
            tmpParent.AddChild(this);
        }

        m_Parent = tmpParent;

        if (rootExtend != null ? rootExtend.CreateExporterNode(locOffset) : CreateExporterNode(locOffset))
        {
            for(int i=0; i< m_RecordTime.Count; i++)
            {
                UpdateFBXFrame(i, locOffset);
            }

            ExportTransformAnim();

            if (m_NodeType == 0)
            {
                List<IntPtr> boneList = new List<IntPtr>();

                if (rootExtend != null)
                {
                    rootExtend.GetAllBones(ref boneList);
                }
                else
                {
                    GetAllBones(ref boneList);
                }

                MBFBExporter.MBFB_CreateSkeleton(boneList.Count, boneList.ToArray());
            }
        }

        m_Parent = null;

        MBFBExporter.MBFB_CloseExporter(path);
    }
}
