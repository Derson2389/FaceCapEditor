using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class DirectorRoomClient : NetworkConnectClient
{
    public DirectorRoomClient(Socket socket)
        : base(socket)
    {
        LastDockTime = System.DateTime.Now.Ticks;
    }

    public CameraDirectorInstance TargetCamera
    {
        get; set;
    }

    public byte[] CameraData
    {
        get; set;
    }

    public long LastDockTime
    {
        get; set;
    }
}
