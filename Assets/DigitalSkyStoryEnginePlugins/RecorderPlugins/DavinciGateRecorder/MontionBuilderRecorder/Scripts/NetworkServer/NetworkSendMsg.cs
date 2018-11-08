using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MBR
{
    public abstract class NetworkSendMsg
    {
        protected int m_MsgId = -1;
        private byte[] m_BufferData = null;

        protected NetworkSendMsg(int id)
        {
            m_MsgId = id;
        }

        public byte[] GetBufferData()
        {
            return m_BufferData;
        }

        public void Process()
        {
            int len = 0;
            MemoryStream bufferStream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(bufferStream);
            bw.Write(len);
            bw.Write(m_MsgId);
            Process(bw);
            len = ((int)bufferStream.Length - 4);
            bw.Seek(0, SeekOrigin.Begin);
            bw.Write(len);
            bw.Flush();

            m_BufferData = bufferStream.ToArray();
        }

        protected virtual void Process(BinaryWriter bw)
        {

        }
    }

}

