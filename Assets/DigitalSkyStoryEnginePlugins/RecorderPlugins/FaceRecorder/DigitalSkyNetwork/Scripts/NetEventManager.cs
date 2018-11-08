using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Network
{
    public class NetEventManager
    {
        // 注册的网络事件
        private Dictionary<int, INetEvent> _netEvents = new Dictionary<int, INetEvent>();

        public NetEventManager()
        {
            _netEvents = new Dictionary<int, INetEvent>();
        }

        // Use this for initialization
        public virtual void Init()
        {
            
        }

        //销毁
        public virtual void OnDestroy()
        {
            _netEvents.Clear();
        }

        public virtual void OnUpdateNetMessage(ClientNetMessage cnetMessage)
        {
            foreach(var item in _netEvents)
            {
                if(item.Key == (int)cnetMessage.netMessage.netMessageId)
                {
                    item.Value.sender = cnetMessage.clientId;
                    item.Value.data = cnetMessage.netMessage.GetData();

                    item.Value.OnReceived();
                }
            }
        }

        /// <summary>
        /// 注册网络消息事件
        /// </summary>
        /// <param name="netMessageId">网络消息Id</param>
        /// <param name="netEvent">事件对象</param>
        public void RegisterEvent(NetMessageID netMessageId, INetEvent netEvent)
        {
            if (!_netEvents.ContainsKey((int)netMessageId))
                _netEvents.Add((int)netMessageId, netEvent);
        }

        /// <summary>
        /// 注销网络消息事件
        /// </summary>
        /// <param name="netMessageId">网络消息Id</param>
        public void UnRegisterEvent(NetMessageID netMessageId)
        {
            if (_netEvents.ContainsKey((int)netMessageId))
                _netEvents.Remove((int)netMessageId);
        }

        /// <summary>
        /// 判断某个网络消息事件是否注册
        /// </summary>
        /// <param name="netMessageId"></param>
        /// <returns></returns>
        public bool IsRegisterEvent(NetMessageID netMessageId)
        {
            if (!_netEvents.ContainsKey((int)netMessageId))
                return false;

            return true;
        }
    }
}
