using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace DigitalSky.Network
{
    public class NetClient : MonoBehaviour
    {
        public string ip = "127.0.0.1";
        public int port = 5566;  

        // 客户端端socket  
        IPAddress _ipAddress; //主机ip  
        IPEndPoint _ipEnd;
        public bool autoReconnect = true;
        public int autoReconnectCount = 0;

        public Action onConnect;
        public Action onDisConnect;

        // 网络消息管理器
        private NetMessageManager _netMessageManager = new NetMessageManager();
        private NetConnection _client = null;

        private Queue<NetMessage> _willSendMessages = new Queue<NetMessage>();
        private Mutex _messageMutex = new Mutex();
        private bool _isSending = false;

        //接收线程
        private Thread _recieveThread = null;
        private bool _stopReceiveThreadRequest = false;

        // 发送线程
        private Thread _sendThread = null;
        private bool _stopSendThreadRequest = false;
        

        public bool isConnected
        {
            get
            {
                if (_client != null && _client.isValid)
                    return true;

                return false;
            }
        }

        // Use this for initialization  
        void Start()
        {
            NetConnect();
        }

        // Update is called once per frame  
        void Update()
        {
            // 连接不可用
            if (_client == null || !_client.isValid)
            {
                return;
            }
        }

        //初始化  
        bool NetConnect()
        {
            //定义服务器的IP和端口，端口与服务器对应  
            _ipAddress = IPAddress.Parse(ip); //可以是局域网或互联网ip，此处是本机  
            _ipEnd = new IPEndPoint(_ipAddress, port);

            _netMessageManager = new NetMessageManager();

            // TODO: avoid multi thread spawining.
            try
            {
                //开启一个接收线程连接  
                _recieveThread = new Thread(new ThreadStart(ReceiveSocket));
                _recieveThread.Start();

                // 开启一个发送线程
                _sendThread = new Thread(new ThreadStart(SendSocket));
                _sendThread.Start();
            }
            catch (System.Exception e)
            {
                // TODO: add reconnecting on issue on the socket.
                Debug.LogWarning("[NetClient.NetConnect] -> Error while start connect thread: " + e.Message);
                _recieveThread = null;
                _stopReceiveThreadRequest = false;

                _sendThread = null;
                _stopSendThreadRequest = false;
                return false;
            }

            return true;
        }

        bool NetDisconnect()
        {
            try
            {
                CloseSocket();
            }
            catch (System.Exception e)
            {
                // We don't care of this exception, grabber_disconnect might
                // be called even if socket is not connected.
            }

            _stopReceiveThreadRequest = true;
            if (_recieveThread != null)
            {
                _recieveThread.Interrupt();
                _recieveThread.Join();
            }
            _stopReceiveThreadRequest = false;
            _recieveThread = null;

            _stopSendThreadRequest = true;
            if (_sendThread != null)
            {
                _sendThread.Interrupt();
                _sendThread.Join();
            }
            _stopSendThreadRequest = false;
            _sendThread = null;

            return true;
        }

        void ReceiveSocket()
        {
            int connectCount = 0;
            //Debug.Log("[NetClient.StartSocket] -> StartSocket thread started.");

            do
            {
                if (_client == null || !_client.isValid)
                {
                    if(_client != null)
                    {
                        // 连接丢失回调函数
                        onDisConnect();

                        CloseSocket();
                    }

                    // 重连测试
                    if (autoReconnectCount > 0 && connectCount >= autoReconnectCount)
                        break;

                    connectCount++;
                    StartSocket();

                    if(_client != null)
                    {
                        connectCount = 0;

                        // 连接成功回调函数
                        onConnect();

                        // 调用接收数据接口， 开始自动接收数据
                        _client.ReceiveData();
                    }
                }

            } while (autoReconnect && !_stopReceiveThreadRequest);

            //Debug.Log("[NetClient.StartSocket] -> StartSocket thread finished.");
        }

        private void SendSocket()
        {
            do
            {
                if (_client == null || !_client.isValid)
                    continue;

                if (_willSendMessages.Count > 0 && _isSending == false)
                {
                    NetMessage message = DeQueueMessage();
                    Debug.Log("[NetClient.SendSocket] Will send message, msg id: " + message.netMessageId + ", message queue length: " + _willSendMessages.Count);                  

                    _isSending = true;
                    if (!_client.SendData(message))
                        _isSending = false;
                }

            } while (!_stopSendThreadRequest);
        }

        private void StartSocket()
        {
            try
            {
                TcpClient tcpClient = new TcpClient(ip, port);
                //TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                //tcpClient.Connect(_ipEnd);

                // Create a new user connection using TcpClient
                _client = new NetConnection(tcpClient, _netMessageManager);
                _client.onReceiveMessage -= OnReceivedMessage;
                _client.onReceiveMessage += OnReceivedMessage;

                _client.onSendedMessage -= OnSendedMessage;
                _client.onSendedMessage += OnSendedMessage;
            }
            catch (Exception ex)
            {
                Debug.LogError("[NetClient.StartSocket] -> Connect error: " + ex.ToString());
                _client = null;
            }
        }

        private void CloseSocket()
        {
            //关闭连接
            if (_client != null)
            {
                _client.OnDestroy();
                _client.onReceiveMessage -= OnReceivedMessage;
                _client.onSendedMessage -= OnSendedMessage;
                _client = null;
            }

            //Debug.Log("[NetClient.CloseConnect] -> Client close");
        }

        // This is the event handler for the NetConnection when it receives a full netMessage.
        // Parse the cammand and parameters and take appropriate action.
        private void OnReceivedMessage(NetConnection sender, NetMessage netMessage)
        {
            Debug.Log("[NetClient.OnReceivedMessage] -> " + sender.clientName + " received:" + netMessage.netMessageId.ToString());

            // dataArray(0) is the command.
            switch (netMessage.netMessageId)
            {
                case NetMessageID.ConnectMsgId:
                    Join(sender);
                    break;

                default:
                    // Message is junk do nothing with it.
                    break;
            }
        }

        private void OnSendedMessage(NetConnection sender, NetMessage netMessage)
        {
            _isSending = false;
        }

        public void EnQueueMessage(NetMessage netMessage)
        {
            // 连接不可用
            if (_client == null || !_client.isValid)
            {
                return;
            }

            Debug.Log("[NetClient.EnQueueMessage] EnQueue message, msg id: " + netMessage.netMessageId + " msg size: " + netMessage.size);
            _messageMutex.WaitOne();
            _willSendMessages.Enqueue(netMessage);
            _messageMutex.ReleaseMutex();
        }

        private NetMessage DeQueueMessage()
        {
            _messageMutex.WaitOne();
            NetMessage netMessage = _willSendMessages.Dequeue();
            _messageMutex.ReleaseMutex();

            return netMessage;
        }

        // This subroutine checks to see if username already exists in the clients 
        // Hashtable.  if it does, send a REFUSE message, otherwise confirm with a JOIN.
        private void Join(NetConnection sender)
        {
            NetMessage message = new NetMessage(NetMessageID.JoinMsgId);
            string joinMsg = "Hi, i'm new coming";
            message.SetData(joinMsg.NetSerializeToByteArray());

            EnQueueMessage(message);
        }

        //程序退出则关闭连接  
        void OnApplicationQuit()
        {
            NetDisconnect();
        }
    }
}
