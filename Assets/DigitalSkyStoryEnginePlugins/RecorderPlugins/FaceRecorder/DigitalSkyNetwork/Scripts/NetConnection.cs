using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine;

namespace DigitalSky.Network
{
    public delegate void OnReceiveMessage(NetConnection netConnection, NetMessage netMessage);
    public delegate void OnSendedMessage(NetConnection netConnection, NetMessage netMessage);

    public class NetConnection
    {
        const int READ_BUFFER_SIZE = 1024 * 1024 * 8;
        public event OnReceiveMessage onReceiveMessage;
        public event OnSendedMessage onSendedMessage;

        private TcpClient _client;
        // The Name property uniquely identifies the user connection.
        private string _clientName;
        public string clientName
        {
            get
            {
                return _clientName;
            }
            set
            {
                _clientName = value;
            }
        }

        private NetMessageManager _netMessageManager;

        // 接受网络数据的Buffer
        private byte[] _readBuffer = new byte[READ_BUFFER_SIZE];

        // 缓存池Buffer, 用于一次消息包数据未接收完全的
        private byte[] _cacheBuffer = new byte[READ_BUFFER_SIZE];
        private int _cacheUse = 0;

        public bool isValid
        {
            get { return _client != null; }
        }

        // Overload the new operator to set up a read thread.
        public NetConnection(TcpClient client, NetMessageManager netMessageManager)
        {
            this._client = client;
            this._clientName = client.Client.RemoteEndPoint.ToString();

            this._netMessageManager = netMessageManager;
        }

        // This subroutine uses a StreamWriter to send a message to the user.
        public bool SendData(NetMessage netMessage)
        {
            if (!netMessage.isValid)
            {
                Debug.LogError("[NetConnection.SendData] -> Send failed, netMessage is invalid.");
                return false;
            }

            if (!isValid || !_client.Client.Connected)
            {
                return false;
            }

            try
            {
                //lock ensure that no other threads try to use the stream at the same time.
                lock (_client.GetStream())
                {
                    if (_client.GetStream() == null || !_client.GetStream().CanWrite)
                        return false;

                    byte[] pack = netMessage.Packet();
                    //Debug.Log("[NetConnection.SendData] -> start send message: " + netMessage.netMessageId.ToString() + ", data size: " + pack.Length + ", client: " + clientName);

                    //_client.GetStream().Write(pack, 0, pack.Length);
                    // Make sure all data is sent now.
                    //_client.GetStream().Flush();

                    _client.GetStream().BeginWrite(pack, 0, pack.Length, OnSendData, netMessage);
                    _client.GetStream().Flush();
                }
            }
            catch (Exception ex)
            {
                // InvalidOperationException: The TcpClient is not connected to a remote host.
                // ObjectDisposedException: The TcpClient has been closed.
                Debug.LogError("[NetConnection.SendData] -> " + ex);
                CloseSocket();
            }

            return true;
        }

        private void OnSendData(IAsyncResult ar)
        {
            // Ensure that no other threads try to use the stream at the same time.
            lock (_client.GetStream())
            {
                // Finish asynchronous write.
                _client.GetStream().EndWrite(ar);
            }

            NetMessage netMessage = (NetMessage)ar.AsyncState;
            //Debug.Log("[NetConnection.OnSendData] -> send message success: " + netMessage.netMessageId.ToString() + ", client: " + clientName + ", size: " + netMessage.size);
            
            onSendedMessage(this, netMessage);
        }

        public void ReceiveData()
        {
            /*if (this._client.Available <= 0)
                return;*/

            // Ensure that no other threads try to use the stream at the same time.
            lock (_client.GetStream())
            {
                // Start a new asynchronous read into readBuffer. The data will be saved into readBuffer.
                _client.GetStream().BeginRead(_readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(OnReceiveData), null);
            }
        }

        // This is the callback function for TcpClient.GetStream.Begin. It begins an asynchronous read from a stream.
        private void OnReceiveData(IAsyncResult ar)
        {
            int bytesRead = 0;

            if (!isValid || _client.GetStream() == null || !_client.GetStream().CanRead)
                return;

            try
            {
                // Ensure that no other threads try to use the stream at the same time.
                lock (_client.GetStream())
                {
                    // Finish asynchronous read into readBuffer and get number of bytes read.
                    bytesRead = _client.GetStream().EndRead(ar);
                }

                // 未收到任何数据
                if (bytesRead <= 0)
                {
                    Debug.LogWarning("[NetConnection.OnReceiveData] -> received data size is 0, will close this connection.");
                    CloseSocket();
                    return;
                }

                //Debug.Log("[NetConnection.OnReceiveData] -> recieved data size: " + bytesRead);
            }
            catch (Exception ex)
            {
                // InvalidOperationException: The TcpClient is not connected to a remote host.
                // ObjectDisposedException: The TcpClient has been closed.
                Debug.LogError("[NetConnection.OnReceiveData] -> " + ex);
                CloseSocket();
                return;
            }

            // 默认收取消息Buffer为网络读取Buffer
            byte[] receiveBuffer = _readBuffer;
            int receiveRead = bytesRead;
            if (_cacheUse > 0)
            {
                // 如果缓存Buffer里有未收取完的消息，则将网络读取Buffer里消息包所有数据都复制到缓存Buffer里

                if (_cacheUse + bytesRead < READ_BUFFER_SIZE)
                {
                    Buffer.BlockCopy(_readBuffer, 0, _cacheBuffer, _cacheUse, bytesRead);

                    // 并且设置缓存Buffer为收取消息Buffer
                    receiveBuffer = _cacheBuffer;
                    receiveRead = bytesRead + _cacheUse;

                    _cacheUse = 0;
                }else
                {
                    Debug.LogError("[NetConnection.OnReceiveData] -> 消息大小超过READ_BUFFER_SIZE");
                }
            }

            int offset = 0;
            int resetOffset = 0;

            while ((receiveRead - offset) >= 8)
            {
                resetOffset = offset;

                // 读取包消息大小
                int msgSize = BitConverter.ToInt32(receiveBuffer, offset);
                if (msgSize > READ_BUFFER_SIZE)
                {
                    // 如果包消息大小大于最大的数值， 则判定为异常链接， 立即关闭
                    Debug.LogError("[NetConnection.OnReceiveData] -> 消息太大， size: " + msgSize);
                    CloseSocket();
                    return;
                }
                if (msgSize < 4)
                {
                    // 如果包消息大小小于4, 也是一个异常链接， 立即关闭
                    Debug.LogError("[NetConnection.OnReceiveData] -> 消息太小， size: " + msgSize);
                    CloseSocket();
                    return;
                }

                // 读取完消息大小字节，偏移+4
                offset += 4;

                if (msgSize > receiveRead - offset)
                {
                    // 如果当前一个消息包的数据没有收完，则继续收
                    offset = resetOffset;
                    break;
                }
                else
                {
                    // 读取包消息体
                    NetMessage message = new NetMessage();

                    // 解析时将之前的消息大小一起拷贝进去
                    bool isParse = message.Parse(_netMessageManager, receiveBuffer, offset, msgSize);

                    if (!isParse)
                    {
                        Debug.LogError("[NetConnection.OnReceiveData] -> 消息解析错误");
                    }
                    else
                    { 
                        //Debug.Log("[NetConnection.OnReceiveData] -> received message success: " + message.netMessageId.ToString() + ", client: " + clientName + ", size: " + message.size);
                        onReceiveMessage(this, message);
                    }

                    offset += msgSize;
                }
            }

            // 读取解析了offset之前所有收到数据
            int bytesWillCached = receiveRead - offset;

            // 如果还有未解析完的数据，则缓存起来
            if (bytesWillCached > 0)
            {
                Buffer.BlockCopy(receiveBuffer, offset, _cacheBuffer, 0, bytesWillCached);
                _cacheUse = bytesWillCached;
            }
            else
            {
                _cacheUse = 0;
            }

            try
            {
                // Ensure that no other threads try to use the stream at the same time.
                ReceiveData();
            }
            catch (Exception ex)
            {
                // InvalidOperationException: The TcpClient is not connected to a remote host.
                // ObjectDisposedException: The TcpClient has been closed.
                Debug.LogError("[NetConnection.OnReceiveData] -> " + ex);
                CloseSocket();
            }
        }

        private void CloseSocket()
        {
            if (_client != null)
            {
                try
                {
                    lock (_client.GetStream())
                    {
                        NetworkStream stream = _client.GetStream();
                        if (stream != null)
                            stream.Dispose();
                    }

                    _client.Close();
                    _client = null;
                }
                catch (System.Exception ex)
                {
                    // InvalidOperationException: The TcpClient is not connected to a remote host.
                    // ObjectDisposedException: The TcpClient has been closed.
                    Debug.LogError("[NetConnection.CloseSocket] -> " + ex);
                }
            }
        }

        public void OnDestroy()
        {
            //先关闭客户端
            CloseSocket();

            _netMessageManager = null;
        }
    }
}
