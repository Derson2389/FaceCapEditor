﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace MBR
{
    public class NetworkServer
    {
        private Socket m_Socket = null;
        protected int m_ListenPort = 66666;
        private Thread m_IOThread = null;
        private bool m_bWillExit = false;
        private object m_ConnectMutex = new object();
        private List<Socket> m_ConnectSockets = new List<Socket>();
        private List<Socket> m_NewConnectSockets = new List<Socket>();
        private List<Socket> m_WantAddConnectSockets = new List<Socket>();

        protected Dictionary<int, Type> m_RecvMessageDictionary = new Dictionary<int, Type>();
        protected Dictionary<Socket, NetworkConnectClient> m_Connections = new Dictionary<Socket, NetworkConnectClient>();

        public NetworkServer()
        {

        }

        public bool IsRunning()
        {
            if (m_Socket == null)
            {
                return false;
            }

            return m_Socket.IsBound;
        }

        public NetworkRecvMsg CreateRecvMsg(int msgId)
        {
            Type type;
            if (m_RecvMessageDictionary.TryGetValue(msgId, out type) == false)
            {
                return null;
            }

            return (NetworkRecvMsg)Activator.CreateInstance(type);
        }

        private string GetLocalIp()
        {
            string hostname = Dns.GetHostName();//得到本机名   
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            IPAddress localaddr = null;
            for (int i = localhost.AddressList.Length-1; i >= 0 ; i--)
            {
                //if (localhost.AddressList[i].IsIPv6LinkLocal == false)
                {
                    string addstr = localhost.AddressList[i].ToString();
                    if (addstr.StartsWith("192"))
                    {
                        localaddr = localhost.AddressList[i];
                        break;
                    }
                }
            }
            return localaddr == null ? string.Empty : localaddr.ToString();
        }

        public void BeginListen()
        {
            if (m_Socket != null)
            {
                StopListen();
            }

            EditorApplication.update -= OnNetworkUpdate;
            EditorApplication.update += OnNetworkUpdate;

            string ipStr = GetLocalIp();
            IPAddress ipAdr = IPAddress.Parse(ipStr);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, m_ListenPort);

            try
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.Bind(ipEp);
                m_Socket.Listen(36);

                Debug.Log(string.Format("开始监听[{0}:{1}]", ipStr, m_ListenPort));

                m_IOThread = new Thread(new ThreadStart(OnServerIOThread));
                m_bWillExit = false;
                m_IOThread.Start();
                m_Socket.BeginAccept(OnAccepted, null);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
                StopListen();
            }
        }

        protected virtual void StopCleanup()
        {

        }

        public void StopListen()
        {
            if (m_Socket == null)
            {
                return;
            }

            m_bWillExit = true;

            if (m_IOThread != null)
            {
                while (m_IOThread.IsAlive)
                {
                    Thread.Sleep(1000);
                }
                m_IOThread = null;
            }

            foreach (Socket socket in m_ConnectSockets)
            {
                socket.Close();
            }
            m_ConnectSockets.Clear();
            m_Connections.Clear();

            m_Socket.Close();
            m_Socket = null;

            EditorApplication.update -= OnNetworkUpdate;

            StopCleanup();
        }

        void OnServerIOThread()
        {
            while (!m_bWillExit)
            {
                lock (m_ConnectMutex)
                {
                    foreach (Socket socket in m_WantAddConnectSockets)
                    {
                        m_ConnectSockets.Add(socket);
                    }
                    m_WantAddConnectSockets.Clear();
                }

                for (int i = m_ConnectSockets.Count - 1; i >= 0; --i)
                {
                    if (m_ConnectSockets[i].Connected == false)
                    {
                        m_ConnectSockets.RemoveAt(i);
                    }
                }

                try
                {
                    ProcessNetThread();
                    ProcessRecv();
                    ProcessSend();
                }
                catch (System.Exception e)
                {
                    Debug.LogError(string.Format("OnServerIOThread Error[{0}]", e.ToString()));
                    m_bWillExit = true;
                }

                System.Threading.Thread.Sleep(1);
            }

            Debug.Log("NetworkServer:OnServerIOThread Exit");
        }

        void ProcessRecv()
        {
            NetworkConnectClient connection = null;
            foreach (Socket socket in m_ConnectSockets)
            {
                if (m_Connections.TryGetValue(socket, out connection))
                {
                    connection.RecvBuffer(this);
                }
            }
        }

        void ProcessSend()
        {
            NetworkConnectClient connection = null;
            foreach (Socket socket in m_ConnectSockets)
            {
                if (m_Connections.TryGetValue(socket, out connection))
                {
                    connection.SendBuffer();
                }
            }
        }

        void OnAccepted(IAsyncResult ar)
        {
            Socket clientSocket = m_Socket.EndAccept(ar);
            if (clientSocket != null)
            {
                lock (m_ConnectMutex)
                {
                    m_NewConnectSockets.Add(clientSocket);
                }

                Debug.Log(string.Format("Network:新连接创建[{0}]", clientSocket.RemoteEndPoint.ToString()));
            }

            if (!m_bWillExit)
            {
                System.Threading.Thread.Sleep(1);
                m_Socket.BeginAccept(OnAccepted, null);
            }
        }

        protected virtual NetworkConnectClient CreateClient(Socket socket)
        {
            return new NetworkConnectClient(socket);
        }

        protected virtual void RemoveClient(Socket socket)
        {

        }

        void ProcessConnection()
        {
            lock (m_ConnectMutex)
            {
                foreach (Socket socket in m_NewConnectSockets)
                {
                    m_WantAddConnectSockets.Add(socket);
                    m_Connections.Add(socket, CreateClient(socket));
                }
                m_NewConnectSockets.Clear();
            }

            List<Socket> willDestroyList = new List<Socket>();
            foreach (NetworkConnectClient connection in m_Connections.Values)
            {
                if (connection.IsWillColse())
                {
                    willDestroyList.Add(connection.ConnectionSocket);
                }
            }
            foreach (Socket socket in willDestroyList)
            {
                Debug.Log(string.Format("Network:退出连接[{0}]", socket.RemoteEndPoint.ToString()));
                RemoveClient(socket);
                socket.Close();
                m_Connections.Remove(socket);
            }
        }

        void OnNetworkUpdate()
        {
#if UNITY_EDITOR
            if (EditorApplication.isCompiling)
            {
                return;
            }
#endif

            ProcessConnection();

            OnServerUpdate();

            foreach (NetworkConnectClient connection in m_Connections.Values)
            {
                connection.Process();
            }
        }

        protected virtual void ProcessNetThread()
        {

        }


        protected virtual void OnServerUpdate()
        {

        }
    }

}

