using System;
using System.Collections;
using System.Collections.Generic;

namespace DigitalSky.Network
{
    public class NetEvent<T, S> : INetEvent where T : class
    {
        public object userData;

        // NetEvent 数据类型
        public Type type
        {
            get { return typeof(T); }
        }

        // NetEvent 发送者
        private Object _sender;
        public Object sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        // NetEvent 接收的字节数据
        private byte[] _data = null;
        public byte[] data
        {
            get { return _data; }
            set { _data = value; }
        }

        private Action<T, S> _action;

        public NetEvent(Action<T, S> action)
        {
            _action = action;
        }

        // 接受完成回调函数
        public virtual void OnReceived() 
        {
            if (_action != null && _data != null)
            {
                T receiveData = _data.NetDeserialize<T>();
                _action((T)receiveData, (S)sender);
            }           
        }
    }
}
