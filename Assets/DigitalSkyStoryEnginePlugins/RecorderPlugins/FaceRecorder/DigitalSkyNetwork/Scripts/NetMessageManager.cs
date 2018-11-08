using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Network
{
    public class NetMessageManager
    {
        // 保存注册的消息列表
        private HashSet<int> _messageSet = null;

        public NetMessageManager()
        {
            _messageSet = new HashSet<int>();

            RegisterMessage(NetMessageID.ConnectMsgId);
            RegisterMessage(NetMessageID.JoinMsgId);
            RegisterMessage(NetMessageID.RefuseMsgId);
        }

        public void RegisterMessage(NetMessageID netMessageId)
        {
            // 注册这个消息相应的数据类型
            if (!_messageSet.Contains((int)netMessageId))
                _messageSet.Add((int)netMessageId);
        }

        public void UnRegisterMessage(NetMessageID netMessageId)
        {
            // 取消注册
            if (_messageSet.Contains((int)netMessageId))
                _messageSet.Remove((int)netMessageId);
        }

        public bool IsRegister(int id)
        {
            if (_messageSet.Contains(id))
                return true;

            return false;
        }

        public void OnDestroy()
        {
            _messageSet.Clear();
        }
    }
}
