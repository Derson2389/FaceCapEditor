using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

namespace DigitalSky.Network
{
    public delegate void OnConnect(string clientId);
    public delegate void OnDisconnect(string clientId);

    public class ClientNetMessage
    {
        private string _clientId;
        public string clientId
        {
            get { return _clientId; }
        }

        private NetMessage _netMessage;
        public NetMessage netMessage
        {
            get { return _netMessage; }
        }

        public ClientNetMessage(string clientId, NetMessage netMessage)
        {
            _clientId = clientId;
            _netMessage = netMessage;
        }
    }

    public class NetServer
    {
        private static NetServer _instance = null;
        public static NetServer instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NetServer();

                return _instance;
            }
        }

        // 服务器监听端口
        private int _listenPort = 5566;
        public int listenPort
        {
            get { return _listenPort; }
        }

        // 所有已连接上的客户端列表
        private Hashtable _clients = new Hashtable();
        public Hashtable clients
        {
            get
            {
                return _clients;
            }
        }

        // 所有刚连接上的连接列表
        private Queue<NetConnection> _acceptConnections = new Queue<NetConnection>();
        private Mutex _connectionMutex = new Mutex();

        public event OnConnect onConnect;
        public event OnDisconnect onDisconnect;

        private TcpListener _listener;
        private Thread _listenerThread;

        // 网络消息管理器
        private NetMessageManager _netMessageManager = new NetMessageManager();

        // 网络事件管理器
        private List<NetEventManager> _netEventManagers = new List<NetEventManager>();

        // 服务器是否开始监听
        private bool _isListen = false;
        public bool isListen
        {
            get { return _isListen; }
        }

        // 收到的消息队列
        private Queue<ClientNetMessage> _receivedMessages = new Queue<ClientNetMessage>();
        private Mutex _messageMutex = new Mutex();

        public NetServer()
        {
            _clients = new Hashtable();
            _netMessageManager = new NetMessageManager();
            _netEventManagers = new List<NetEventManager>();

            _receivedMessages = new Queue<ClientNetMessage>();
            _acceptConnections = new Queue<NetConnection>();
        }

        public static void Destroy()
        {
            if (_instance != null)
                _instance.OnDestroy();

            _instance = null;
        }

        //初始化  
        public void Init()
        {
            _clients = new Hashtable();
            _receivedMessages = new Queue<ClientNetMessage>();
            _acceptConnections = new Queue<NetConnection>();

            _netMessageManager = new NetMessageManager();
            _netMessageManager.RegisterMessage(NetMessageID.HeartBeatMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.ConnectMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.JoinMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.RefuseMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.ARFaceBlendShapeMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.ARCameraMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.ARCameraTextureMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.BytesMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.ScreenCaptureYMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.ScreenCaptureUVMsgId);
            _netMessageManager.RegisterMessage(NetMessageID.StringMsgId);

            _netEventManagers = new List<NetEventManager>();
        }

        public void OnUpdate()
        {
            // 检测是否有新的客户端连接上
            while(_acceptConnections.Count > 0)
            {
                NetConnection connection = DeQueueConnection();
                Connect(connection);
            }

            List<NetConnection> toRemoved = new List<NetConnection>();
            // 检测是否有无效的客户端
            foreach (DictionaryEntry entry in clients)
            {
                NetConnection connection = (NetConnection)entry.Value;
                if (!connection.isValid)
                {
                    toRemoved.Add(connection);
                }
            }

            // 移除无效的客户端
            toRemoved.ForEach((NetConnection c) => { Disconnect(c); });

            // 处理收到的消息队列
            while(_receivedMessages.Count > 0)
            {
                ClientNetMessage message = DeQueueMessage();

                for(int i = 0; i < _netEventManagers.Count; i++)
                {
                    if (_netEventManagers[i].IsRegisterEvent(message.netMessage.netMessageId))
                        _netEventManagers[i].OnUpdateNetMessage(message);
                }

                /*if (_netEvents.ContainsKey((int)message.netMessageId))
                    _netEvents[(int)message.netMessageId].OnReceived();*/
            }
        }

        //销毁
        public void OnDestroy()
        {
            StopListen();

            for (int i = 0; i < _netEventManagers.Count; i++)
                _netEventManagers[i].OnDestroy();
            _netEventManagers.Clear();

            _netMessageManager.OnDestroy();
            _netMessageManager = null;

            onConnect = null;
            onDisconnect = null;
        }

        public void StartListen()
        {
            _clients.Clear();
            _acceptConnections.Clear();
            _receivedMessages.Clear();

            _listenerThread = new Thread(new ThreadStart(DoListen));
            _listenerThread.Start();
            UpdateStatus("Server started");
        }

        public void StopListen()
        {
            UpdateStatus("Server closed");

            foreach(var con in _acceptConnections)
            {
                con.OnDestroy();
            }

            //先关闭客户端
            List<NetConnection> toRemoved = new List<NetConnection>();
            foreach (DictionaryEntry entry in clients)
            {
                NetConnection client = (NetConnection)entry.Value;
                toRemoved.Add(client);
            }
            // 移除无效的客户端
            toRemoved.ForEach((NetConnection c) => { Disconnect(c); });

            //关闭服务器监听
            if (_listener != null)
                _listener.Stop();

            //再关闭线程  
            if (_listenerThread != null)
            {
                _listenerThread.Interrupt();
                _listenerThread.Abort();
                _listenerThread = null;
            }

            _clients.Clear();
            _acceptConnections.Clear();
            _receivedMessages.Clear();

            _isListen = false;
        }

        public NetConnection GetClient(string clientId)
        {
            if (clients.ContainsKey(clientId))
                return (NetConnection)clients[clientId];

            return null;
        }

        public void AddEventManager(NetEventManager eventManager)
        {
            if (eventManager != null && !_netEventManagers.Contains(eventManager))
            {
                eventManager.Init();
                _netEventManagers.Add(eventManager);
            }
        }

        private void EnQueueMessage(ClientNetMessage clientMessage)
        {
            _messageMutex.WaitOne();
            _receivedMessages.Enqueue(clientMessage);
            _messageMutex.ReleaseMutex();
        }

        private ClientNetMessage DeQueueMessage()
        {
            _messageMutex.WaitOne();
            ClientNetMessage clientMessage = _receivedMessages.Dequeue();
            _messageMutex.ReleaseMutex();

            return clientMessage;
        }

        private void EnqueueConnection(NetConnection connection)
        {
            _connectionMutex.WaitOne();
            _acceptConnections.Enqueue(connection);
            _connectionMutex.ReleaseMutex();
        }

        private NetConnection DeQueueConnection()
        {
            _connectionMutex.WaitOne();
            NetConnection connection = _acceptConnections.Dequeue();
            _connectionMutex.ReleaseMutex();

            return connection;
        }

        // This subroutine is used a background listener thread to allow reading incoming
        // messages without lagging the user interface.
        private void DoListen()
        {
            // Listen for new connections.
            _listener = new TcpListener(System.Net.IPAddress.Any, listenPort);
            _listener.Start();
            _isListen = true;

            try
            {
                /*do
                {
                    // Create a new user connection using TcpClient returned by TcpListener.AcceptTcpClient()
                    TcpClient client = _listener.AcceptTcpClient();
                    NetConnection connection = new NetConnection(client, _netMessageManager);                    
                    connection.ReceiveData();
                    EnqueueConnection(connection);
                } while (true);*/

                _listener.BeginAcceptSocket(OnClientConnect,_listener); //异步接受客户端的连接请求, clientConnect为连接的回调函数
            }
            catch (Exception ex)
            {
                Debug.LogError("[NetServer.DoListen] -> " + ex);
                _listener.Stop();
                _isListen = false;
            }
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            try
            {
                //接受客户的连接,得到连接的Socket
                TcpClient client = listener.EndAcceptTcpClient(ar);

                NetConnection connection = new NetConnection(client, _netMessageManager);
                EnqueueConnection(connection);

                listener.BeginAcceptSocket(OnClientConnect, listener); //异步接受客户端的连接请求, clientConnect为连接的回调函数
            }
            catch (Exception ex)
            {
                Debug.LogError("[NetServer.DoListen] -> " + ex);
                _listener.Stop();
                _isListen = false;
            }
        }

        // This is the event handler for the UserConnection when it receives a full netMessage.
        // Parse the cammand and parameters and take appropriate action.
        private void OnReceivedMessage(NetConnection sender, NetMessage netMessage)
        {
            //Debug.Log("[NetServer.OnReceivedMessage] -> " + sender.clientName + " received:" + netMessage.netMessageId.ToString()); 

            // 注意，一定不能在这个回调函数里面发送网络数据，可能会造成死锁!
            switch (netMessage.netMessageId)
            {
                /*case NetMessageID.Join:
                    Join(sender);
                    break;*/

                default:
                    //enter queue
                    EnQueueMessage(new ClientNetMessage(sender.clientName, netMessage));
                    break;
            }
        }

        // This subroutine add a connected client to Hashtable
        private void Connect(NetConnection sender)
        {
            UpdateStatus(sender.clientName + " has connected with the server.");
            clients.Add(sender.clientName, sender);

            // Create an event handler to allow the UserConnection to communicate with the server.
            sender.onReceiveMessage -= OnReceivedMessage;
            sender.onReceiveMessage += OnReceivedMessage;
            sender.ReceiveData();

            // 发送一个connect确认消息
            NetMessage message = new NetMessage(NetMessageID.ConnectMsgId);
            sender.SendData(message);

            // 回调onConnect通知
            if (onConnect != null)
                onConnect(sender.clientName);
        }

        private void Disconnect(NetConnection sender)
        {
            UpdateStatus(sender.clientName + " has left the server.");

            sender.onReceiveMessage -= OnReceivedMessage;
            sender.OnDestroy();
            clients.Remove(sender.clientName);

            // 回调onDisConnect通知
            if (onDisconnect != null)
                onDisconnect(sender.clientName);
        }

        // This subroutine sends a message to all attached clients
        public void Broadcast(NetMessage netMessage)
        {
            // All entries in the clients Hashtable are UserConnection so it is possible
            // to assign it safely.
            foreach (DictionaryEntry entry in clients)
            {
                NetConnection client = (NetConnection)entry.Value;
                client.SendData(netMessage);
            }
        }

        // This subroutine checks to see if username already exists in the clients 
        // Hashtable.  if it does, send a REFUSE message, otherwise confirm with a JOIN.
        private void Join(NetConnection sender)
        {
            if (clients.Contains(sender.clientName))
            {
                NetMessage message = new NetMessage(NetMessageID.RefuseMsgId);
                sender.SendData(message);
            }
            else
            {
                UpdateStatus(sender.clientName + " has joined the server.");

                // Send a JOIN to sender, and notify all other clients that sender joined
                //NetMessage message = new NetMessage(NetMessageID.JoinMsgId);
                //sender.SendData(message);
            }
        }

        // This subroutine adds line to the Status listbox
        private void UpdateStatus(string statusMessage)
        {
            Debug.Log("[NetServer.UpdateStatus] -> " + statusMessage);
        }
    }
}
