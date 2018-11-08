using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DirectorRoom_S2C_SyncCameraImage : NetworkSendMsg
{
    private long m_Time;
    private int m_Width;
    private int m_Height;
    private byte[] m_Data;

    public DirectorRoom_S2C_SyncCameraImage(int width, int height, byte[] data, long time)
    {
        m_MsgId = (int)DirectorRoomServer.S2CMsgId.SyncCameraImage;
        m_Time = time;
        m_Width = width;
        m_Height = height;
        m_Data = data;
    }

    protected override void Process(BinaryWriter bw)
    {
        bw.Write(m_Time);
        bw.Write(m_Width);
        bw.Write(m_Height);
        bw.Write(m_Data.Length);
        bw.Write(m_Data);
    }
}
