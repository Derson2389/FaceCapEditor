using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MBR
{
    public abstract class NetworkRecvMsg
    {
        private byte[] m_MsgBuffer = null;

        public void InitBuffer(byte[] msgBuffer)
        {
            m_MsgBuffer = msgBuffer;
        }

        public bool Parse(NetworkConnectClient connection)
        {
            if (m_MsgBuffer == null)
            {
                return true;
            }

            bool result = true;
            try
            {
                MemoryStream memoryStream = new MemoryStream(m_MsgBuffer, false);
                BinaryReader br = new BinaryReader(memoryStream);
                result = Parse(br, connection);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public virtual bool Parse(BinaryReader br, NetworkConnectClient connection)
        {
            return true;
        }

        public virtual void Process(NetworkConnectClient connection)
        {

        }
    }
}

