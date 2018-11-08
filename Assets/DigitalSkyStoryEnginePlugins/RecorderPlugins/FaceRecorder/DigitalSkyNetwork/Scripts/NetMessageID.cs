using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Network
{
    public enum NetMessageID
    {
        InvalidMsgId = 0,
        HeartBeatMsgId = 1,
        ConnectMsgId,
        JoinMsgId,
        RefuseMsgId,

        ARFaceBlendShapeMsgId = 20,
        ARCameraMsgId = 21,
        ARCameraTextureMsgId = 22,

        BytesMsgId = 50,
        ScreenCaptureYMsgId = 51,
        ScreenCaptureUVMsgId = 52,

        StringMsgId = 80
    }
}
