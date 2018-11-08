using System;
using System.Collections;
using System.Collections.Generic;

namespace DigitalSky.Network
{
    public interface INetEvent
    {
        // NetEvent 数据类型
        Type type { get; }

        // NetEvent 发送者
        Object sender { get; set; }

        // NetEvent 接收的字节数据
        byte[] data { get; set; }

        // 接受完成回调函数
        void OnReceived();
    }
}
