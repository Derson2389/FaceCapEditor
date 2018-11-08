using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DirectorRoom_C2S_RefreshCamera : NetworkRecvMsg
{
    public override void Process(NetworkConnectClient connection)
    {
        DirectorRoom_S2C_RefreshCamera sendMsg = new DirectorRoom_S2C_RefreshCamera();
        connection.AddSendMessage(sendMsg);
    }
}
