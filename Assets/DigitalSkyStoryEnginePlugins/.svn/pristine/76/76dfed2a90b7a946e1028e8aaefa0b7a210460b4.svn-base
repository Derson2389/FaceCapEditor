using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DirectorRoom_C2S_ChangeCurrentCamera : NetworkRecvMsg
{
    private string m_CameraName;

    public override bool Parse(BinaryReader br, NetworkConnectClient connection)
    {
        m_CameraName = br.ReadString();
        return true;
    }

    public override void Process(NetworkConnectClient connection)
    {
        DirectorRoomClient client = (DirectorRoomClient)connection;
        client.TargetCamera = DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Find(c => c.name == m_CameraName);
        Debug.Log(string.Format("[{0}]设定目标摄像机为[{1}]", connection.ConnectionSocket.RemoteEndPoint.ToString(), m_CameraName));
    }
}
