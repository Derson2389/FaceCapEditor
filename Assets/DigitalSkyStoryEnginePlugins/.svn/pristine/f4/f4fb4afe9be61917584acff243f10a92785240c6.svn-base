using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class DirectorRoomServer : NetworkServer
{
    public enum S2CMsgId
    {
        RefreshCamera,
        SyncCameraImage,
        DockHeart,
    }

    public enum C2SMsgId
    {
        RefreshCamera,
        ChangeCurrentCamera,
        DockHeart,
    }

    public DirectorRoomServer()
    {
        m_ListenPort = 60001;

        m_RecvMessageDictionary.Add((int)C2SMsgId.RefreshCamera, typeof(DirectorRoom_C2S_RefreshCamera));
        m_RecvMessageDictionary.Add((int)C2SMsgId.ChangeCurrentCamera, typeof(DirectorRoom_C2S_ChangeCurrentCamera));
        m_RecvMessageDictionary.Add((int)C2SMsgId.DockHeart, typeof(DirectorRoom_C2S_DockHeart));
    }

    protected override NetworkConnectClient CreateClient(Socket socket)
    {
        return new DirectorRoomClient(socket);
    }

    protected override void OnServerUpdate()
    {
        long time = DateTime.Now.Ticks;
        foreach (NetworkConnectClient connection in m_Connections.Values) 
        {
            DirectorRoomClient client = (DirectorRoomClient)connection;

            if(client.LastDockTime + 50000000  < time)
            {
                connection.AddSendMessage(new DirectorRoom_S2C_DockHeart());
                client.LastDockTime = time;
            }

            if (client.TargetCamera != null && client.TargetCamera.CameraData != null
                && client.CameraData != client.TargetCamera.CameraData)
            {
                client.CameraData = client.TargetCamera.CameraData;
                NetworkSendMsg sendMsg = new DirectorRoom_S2C_SyncCameraImage(client.TargetCamera.Camera.targetTexture.width,
                    client.TargetCamera.Camera.targetTexture.height, client.CameraData, client.TargetCamera.CameraTime);
                connection.AddSendMessage(sendMsg);
            }
        }
    }
}
