using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;

namespace MBR
{
    public class NetworkConnectClient
    {
        private Socket m_Socket = null;
        private bool m_WillClose = false;

        private static readonly int kRecvMessageSize = 1024 * 10240;
        private byte[] m_ReceiveBuffer = new byte[kRecvMessageSize];
        private byte[] m_CacheBuffer = new byte[kRecvMessageSize];
        private int m_CacheUse = 0;

        private List<NetworkRecvMsg> m_RecvMessage = new List<NetworkRecvMsg>();
        private object m_RecvMutex = new object();

        private List<NetworkSendMsg> m_SendMessage = new List<NetworkSendMsg>();
        private object m_SendMutex = new object();

        private float m_IdleTime = 0.0f;

        public NetworkConnectClient(Socket socket)
        {
            m_Socket = socket;
            m_Socket.ReceiveTimeout = 300;
            m_Socket.SendTimeout = 300;
        }

        public Socket ConnectionSocket
        {
            get
            {
                return m_Socket;
            }
        }

        public float IdleTime
        {
            get
            {
                return m_IdleTime;
            }

            set
            {
                m_IdleTime = value;
            }
        }

        public bool IsWillColse()
        {
            return m_WillClose;
        }
        private void WillClose()
        {
            m_WillClose = true;
        }

        public void RecvBuffer(NetworkServer server)
        {
            if (m_WillClose || ConnectionSocket.Available <= 0 || ConnectionSocket.Poll(0, SelectMode.SelectRead) == false)
            {
                return;
            }

            int recvSize = 0;
            try
            {
                if (m_CacheUse > 0)
                {
                    Buffer.BlockCopy(m_CacheBuffer, 0, m_ReceiveBuffer, 0, m_CacheUse);
                }
                recvSize = ConnectionSocket.Receive(m_ReceiveBuffer, m_CacheUse, kRecvMessageSize - m_CacheUse, SocketFlags.None);
            }
            catch (System.Exception e)
            {
                Debug.Log(string.Format("Network:RecvBuffer Error[{0}]", e.ToString()));
                WillClose();
                return;
            }

            int offset = 0;
            int resetOffset = 0;
            recvSize += m_CacheUse;
            while ((recvSize - offset) >= 8)
            {
                resetOffset = offset;

                int msgSize = BitConverter.ToInt32(m_ReceiveBuffer, offset);
                if (msgSize == 0 || msgSize > kRecvMessageSize)
                {
                    Debug.Log("msgSize == 0 || msgSize > kRecvMessageSize");
                    WillClose();
                    return;
                }
                else if (msgSize > recvSize - offset - 4)
                {
                    offset = resetOffset;
                    break;
                }
                else
                {
                    offset += 4;

                    int msgId = BitConverter.ToInt32(m_ReceiveBuffer, offset);

                    byte[] msgBuffer = null;

                    if (msgSize > 4)
                    {
                        msgBuffer = new byte[msgSize - 4];
                        Buffer.BlockCopy(m_ReceiveBuffer, offset + 4, msgBuffer, 0, msgSize - 4);
                    }

                    NetworkRecvMsg msg = server.CreateRecvMsg(msgId);
                    if (msg == null)
                    {
                        WillClose();
                        return;
                    }
                    msg.InitBuffer(msgBuffer);

                    lock (m_RecvMutex)
                    {
                        m_RecvMessage.Add(msg);
                    }

                    offset += msgSize;
                }
            }

            recvSize -= offset;

            if (recvSize > 0)
            {
                Buffer.BlockCopy(m_ReceiveBuffer, offset, m_CacheBuffer, 0, recvSize);
                m_CacheUse = recvSize;
            }
            else
            {
                m_CacheUse = 0;
            }
        }

        public void AddSendMessage(NetworkSendMsg sendMsg)
        {
            sendMsg.Process();

            lock (m_SendMutex)
            {
                m_SendMessage.Add(sendMsg);
            }
        }

        public NetworkSendMsg PopOneSendMsg()
        {
            if (m_SendMessage.Count == 0)
            {
                return null;
            }

            NetworkSendMsg sendMsg = null;
            lock (m_SendMutex)
            {
                sendMsg = m_SendMessage[0];
                m_SendMessage.RemoveAt(0);
            }

            return sendMsg;
        }

        public void SendBuffer()
        {
            if (m_WillClose || ConnectionSocket.Poll(0, SelectMode.SelectWrite) == false)
            {
                return;
            }

            NetworkSendMsg sendMsg = PopOneSendMsg();
            if (sendMsg == null)
            {
                return;
            }

            try
            {
                byte[] byteData = sendMsg.GetBufferData();
                int size = byteData.Length;
                int offset = 0;

                while (size > offset)
                {
                    offset += ConnectionSocket.Send(byteData, offset, size - offset, SocketFlags.None);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(string.Format("Network:SendBuffer Error[{0}]", e.ToString()));
                WillClose();
                return;
            }
        }

        public NetworkRecvMsg PopOneRecvMsg()
        {
            if (m_RecvMessage.Count == 0)
            {
                return null;
            }

            NetworkRecvMsg recvMsg = null;

            lock (m_RecvMutex)
            {
                recvMsg = m_RecvMessage[0];
                m_RecvMessage.RemoveAt(0);
            }

            return recvMsg;
        }

        public void Process()
        {
            NetworkRecvMsg recvMsg = PopOneRecvMsg();
            if (recvMsg != null)
            {
                while (recvMsg != null)
                {
                    if (recvMsg.Parse(this))
                    {
                        recvMsg.Process(this);
                    }

                    recvMsg = PopOneRecvMsg();
                }
            }
        }
    }
}