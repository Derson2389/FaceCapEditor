using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DigitalSky.Network
{
    public class NetMessage
    {
        private NetMessageID _netMessageId = NetMessageID.InvalidMsgId;
        public NetMessageID netMessageId
        {
            get { return _netMessageId; }
        }

        private int _size = 0;
        public int size
        {
            get { return _size; }
        }

        private byte[] _dataBuffer = null;

        public bool isValid
        {
            get
            {
                if (netMessageId == NetMessageID.InvalidMsgId)
                    return false;

                return true;
            }
        }

        public NetMessage(NetMessageID netMessageId)
        {
            _netMessageId = netMessageId;
            _dataBuffer = null;
        }

        public NetMessage()
        {
            _netMessageId = NetMessageID.InvalidMsgId;
            _dataBuffer = null;
        }

        public virtual bool Parse(NetMessageManager netMessageManager, byte[] msgBuffer, int offset, int msgSize)
        {
            // 从网络流拷贝数据到消息对象
            byte[] netMessageBuffer = new byte[msgSize];
            Buffer.BlockCopy(msgBuffer, offset, netMessageBuffer, 0, msgSize);

            MemoryStream bufferStream = new MemoryStream(netMessageBuffer);
            BinaryReader br = new BinaryReader(bufferStream);

            // 读取消息体
            //int len = br.ReadInt32();
            int messageId = br.ReadInt32();

            if (!netMessageManager.IsRegister(messageId))
            {
                _netMessageId = NetMessageID.InvalidMsgId;
                return false;
            }

            _netMessageId = (NetMessageID)messageId;
            _size = msgSize;

            if (msgSize > 4)
                _dataBuffer = br.ReadBytes(msgSize - 4);
            else
                _dataBuffer = null;

            br.Close();
            bufferStream.Close();

            return true;
        }

        public virtual byte[] Packet()
        {
            MemoryStream bufferStream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(bufferStream);

            _size = 4;
            if (_dataBuffer != null)
                _size = _dataBuffer.Length + 4;

            bw.Write(_size);
            bw.Write((int)_netMessageId);

            if (_dataBuffer != null)
                bw.Write(_dataBuffer);

            bw.Flush();

            byte[] netMessageBuffer = bufferStream.ToArray();
            bw.Close();
            bufferStream.Close();

            return netMessageBuffer;
        }

        public void SetData(byte[] dataBuffer)
        {
            if (dataBuffer != null)
            {
                _dataBuffer = dataBuffer;
            }
        }

        public byte[] GetData()
        {
            return _dataBuffer;
        }
    }
}
