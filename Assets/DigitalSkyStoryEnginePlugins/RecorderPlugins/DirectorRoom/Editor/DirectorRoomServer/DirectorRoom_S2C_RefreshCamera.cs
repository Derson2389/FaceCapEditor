using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DirectorRoom_S2C_RefreshCamera : NetworkSendMsg
{
    public DirectorRoom_S2C_RefreshCamera()
    {
        m_MsgId = (int)DirectorRoomServer.S2CMsgId.RefreshCamera;
    }

    protected override void Process(BinaryWriter bw)
    {
        int cameraCount = 0;
        cameraCount = DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Count;

        bw.Write(cameraCount);
        for (int i = 0; i < DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Count; i++)
        {
            bw.Write(DirectorRoomRecorderManager.Instance.CameraGateDirectorList[i].name);
        }
    }
}
