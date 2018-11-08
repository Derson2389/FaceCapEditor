using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using UnityEngine;
using DigitalSky.Tracker;

/******************************************************************************
* [2010] - [2017] Dynamixyz
* All Rights Reserved.
*
* NOTICE:  All information contained herein is, and remains the property
* of Dynamixyz and its suppliers,if any. The intellectual and technical
* concepts contained herein are proprietary to Dynamixyz and its suppliers
* and may be covered by U.S. and Foreign Patents, patents in process, and
* are protected by trade secret or copyright law.
* Dissemination of this information or reproduction of this material
* is strictly forbidden unless prior written permission is obtained
* from Dynamixyz.
*
******************************************************************************/

/* CHANGELOG:
 * v 1.0.6
 * + Fix difference of rotation between generic & specific tracking.
 * + Separate axe swapping & inverting between head & bones.
 * + Only work since build a67f641 of Grabber3.
 * 
 * v 1.0.5
 * + Fix issue while building and exporting project in Unity Editor.
 *
 * v 1.0.4
 * + Fix issue with head rotation (generic retargeting).
 * + Add additional search algorithm for blendshapes.
 * 
 * v 1.0.3
 * + Fix crash when bone was not found in the rig.
 * 
 * v 1.0.2
 * + Fix blenshape search issue with some exported maya rigs.
 * 
 * v 1.0.1:
 * + Fix unrestored bones rotation & position on reconnect.
 * + Fix bad rotation's Quaternion computation.
 * + Fix error logging that was causing game pause.
 * 
 * v 1.0.0:
 * + Adding double buffering protection to avoid bad coefficients.
 * + Add some protection around variable that might be modified concurrently
 *  */

namespace dxyz
{
    public class PrevizTracker : ITracker
    {
		private static PrevizTracker mInstance = null;

		public static PrevizTracker instance
		{
			get
			{
				if (mInstance == null)
                {
                    mInstance = new PrevizTracker();
                    mInstance.LoadConfig();
                }

				return mInstance;
			}
		}

        // Public properties
        private string mIPAddress = "localhost";
        public string IPAddress
        {
            get { return mIPAddress; }
            set
            {
                mIPAddress = value;
                SaveConfig();
            }
        }

        private int mPort = 5559;
        public int Port
        {
            get { return mPort; }
            set
            {
                mPort = value;
                SaveConfig();
            }
        }

        public bool autoReconnect = true; // Reconnect if the connection to grabber is lost.
        public int autoReconnectCount = 3;

        // Private properties
        private TcpClient mSocket = null;
        private bool mNetworkReady;
        private PingPongBuffer mPingPongBuf;
        private uint mNbCoeff;

        private float[] mCoeffs;
        public float[] coeffs
        {
            get { return mCoeffs; }
        }

        private uint[] mTimecode = new uint[4];
        private const uint OPTIMAL_IP_PACKET_SIZE = 1400;
        private PrevizBindingData mBindingData = null;

        // Low level properties
        private Thread mStreamingThread;
        private bool mStopThreadRequest;

        private bool mIsInit;
        public bool isInit
        {
            get { return mIsInit; }
        }

        public string trackerName
        {
            get { return "Previs"; }
        }

        private bool mIsTracking = false;
        public bool isTracking
        {
            get { return mIsTracking; }
        }

        public bool trackerActive
        {
            get { return mNetworkReady; }
        }

        private List<TrackRetargeter> mRetargeters;

        private PrevizTracker()
        {
            mRetargeters = new List<TrackRetargeter>();
        }

        public void LoadConfig()
        {
            mIPAddress = PlayerPrefs.GetString("PrevizTracker_ip", "localhost");
            mPort = PlayerPrefs.GetInt("PrevizTracker_port", 5559);
        }

        public void SaveConfig()
        {
            PlayerPrefs.SetString("PrevizTracker_ip", mIPAddress);
            PlayerPrefs.SetInt("PrevizTracker_port", mPort);
        }

        public static void Destroy()
        {
            if (mInstance != null)
                mInstance.OnDestroy();

            mInstance = null;
        }

        /// <summary>
        /// 实现ITracker的Init接口
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            grabber_disconnect();

            mRetargeters = new List<TrackRetargeter>();
            mIsTracking = false;
            mIsInit = true;
            return true;
        }

        /// <summary>
        /// 实现ITracker的Open接口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (!grabber_connect())
                return false;

            return true;
        }

        /// <summary>
        /// 实现ITracker的Close接口
        /// </summary>
        public void Close()
        {
            grabber_disconnect();
        }

        /// <summary>
        /// 实现ITracker的OnUpdate接口
        /// </summary>
        public void OnUpdate()
        {
            if (mStreamingThread == null || !mStreamingThread.IsAlive)
                return;

            if (!trackerActive || !isTracking)
                return;

            byte[] pBuf;
            if (!mPingPongBuf.readRequest(out pBuf))
            {
                // No data available.
                return;
            }

            // Parse data from the buffer.
            // +-------------------------------------+
            // | ID | Coeffs | TimeCode | <opt.pad.> |
            // +-------------------------------------+
            //  uint|float*x | 4*uint   | char* -> MTU
            int vBufPos = 0;
            //uint frameID = System.BitConverter.ToUInt32(pBuf, vBufPos);
            vBufPos += sizeof(uint); // Array starts at 0.

            // Copy coefficients.
            System.Buffer.BlockCopy(pBuf, vBufPos, mCoeffs, 0, (int)(mNbCoeff * sizeof(float)));
            vBufPos += (int)mNbCoeff * sizeof(float);

            // Copy Timecode.
            System.Buffer.BlockCopy(pBuf, vBufPos, mTimecode, 0, 4 * sizeof(uint));

            mPingPongBuf.readRelease();

            // 为所有retargeter更新重定向track数据
            for (int i = 0; i < mRetargeters.Count; i++)
            {
                if(mRetargeters[i].isBinding)
                    mRetargeters[i].OnUpdateTrackDatas(this);
            }
        }

        /// <summary>
        /// 实现ITracker的OnDestroy接口
        /// </summary>
        public void OnDestroy()
        {
            grabber_disconnect();

            mRetargeters.Clear();
            mNetworkReady = false;
            mIsTracking = false;

            mIsInit = false;
        }

        /// <summary>
        /// 实现ITracker的EnableTracking接口
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableTracking(bool enabled)
        {
            mIsTracking = enabled;
        }

        /// <summary>
        /// 实现ITracker的AddListener接口
        /// </summary>
        /// <param name="listener"></param>
        public bool AddListener(TrackRetargeter listener)
        {
            if (listener == null)
                return false;

            if(mRetargeters.Contains(listener))
            {
                Debug.LogWarning("[PrevizTracker.AddListener] -> listener already exists.");
                return false;
            }

            if (!listener.Init() || !listener.CreateTrackBinding(mBindingData))
                return false;

            mRetargeters.Add(listener);
            return true;
        }

        /// <summary>
        /// 实现ITracker的RemoveListener接口
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(TrackRetargeter listener)
        {
            mRetargeters.Remove(listener);
        }

        /// <summary>
        /// 实现ITracker的HasListener接口
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool HasListener(TrackRetargeter listener)
        {
            if (mRetargeters.Contains(listener))
            {
                return true;
            }

            return false;
        }

        // Grabber connection methods.
        bool grabber_connect()
        {
            if (mStreamingThread != null && mStreamingThread.IsAlive)
            {
                grabber_disconnect();
            }

            // TODO: avoid multi thread spawining.
            try
            {
                // Start the listening thread. Associate the thread main loop function.
                ThreadStart vTs = new ThreadStart(thread_dataStreaming);
                mStreamingThread = new Thread(vTs);

                mStreamingThread.Start();
            }
            catch (System.Exception e)
            {
                // TODO: add reconnecting on issue on the socket.
                Debug.LogWarning("[PrevizTracker.grabber_connect] -> Error while connecting to Grabber: " + e.Message);
                closeSocket();
                return false;
            }

            return true;
        }

        bool grabber_disconnect()
        {
            try
            {
                closeSocket();
            }
            catch (System.Exception e)
            {
                // We don't care of this exception, grabber_disconnect might
                // be called even if socket is not connected.
            }

            mStopThreadRequest = true;
            if (mStreamingThread != null)
            {
                mStreamingThread.Interrupt();
                mStreamingThread.Join();
            }

            mStopThreadRequest = false;
            mStreamingThread = null;

            return true;
        }

        bool grabber_reconnect()
        {
            return true;
        }

        private bool grabber_establish_connection()
        {
            closeSocket();
            mNetworkReady = false;
            mSocket = new TcpClient();

            mSocket.Client.Blocking = true;
            mSocket.Connect(IPAddress, Port);

            NetworkStream vNetStrm = mSocket.GetStream();
            try
            {
                // Read the number of coefficients.
                byte[] vBuf = new byte[sizeof(uint)];
                uint vNbCoeff = 0;
                readFromStream(vNetStrm, vBuf, sizeof(uint));
                vNbCoeff = System.BitConverter.ToUInt32(vBuf, 0);

                // Read the size of the ESC.
                uint vEscSize = 0;
                readFromStream(vNetStrm, vBuf, sizeof(uint));
                vEscSize = System.BitConverter.ToUInt32(vBuf, 0);

                // Retrieve the ESC from network.
                byte[] vEscBuf = new byte[vEscSize];
                readFromStream(vNetStrm, vEscBuf, (int)vEscSize);
                string vEsc = System.Text.Encoding.ASCII.GetString(vEscBuf);

                // 正确获取ESC信息
                if (vEsc == null || vEsc == "")
                {
                    Debug.Log("[PrevizTracker.grabber_establish_connection] -> Failed to get the ESC.");
                    closeSocket();
                    return false;
                }

                mNbCoeff = vNbCoeff;
                mCoeffs = new float[mNbCoeff];

                // Allocate the number of floats
                // Compute the size of the network buffer.
                // size of all coeff + frame tag (unsigned int) + 4 unsigned for TC : HH MM SS FF
                uint bufSize = 5 * sizeof(uint) + sizeof(float) * mNbCoeff;

                // We are padding packet with dummy data, on the server side, to reach the 
                // packet MTU size to force the packet to be sent on the network.
                if (bufSize < OPTIMAL_IP_PACKET_SIZE)
                    bufSize = OPTIMAL_IP_PACKET_SIZE;

                mPingPongBuf = new PingPongBuffer(bufSize);

                // Parse the ESC and apply it to the scene. This must be called from
                // the main thread to avoid exception.
                // We use a special object call UnityMainThreadDispatcher that will
                // execute the "job" we give to it.
                //UnityMainThreadDispatcher.Instance().EnqueueWait(_parseESCandAllocateBuffers(vEsc, vNbCoeff));
                
                PrevizBindingData bindingData = new PrevizBindingData();
                bindingData.data = vEsc;
                bindingData.size = vNbCoeff;

                mBindingData = bindingData;
                /*for (int i = 0; i < mRetargeters.Count; i++)
                {
                    UnityMainThreadDispatcher.Instance().EnqueueWait(mRetargeters[i].CreateTrackBinding(bindingData));
					//mRetargeters[i].bindingData = bindingData;
					//mRetargeters[i].CreateTrackBinding();
                }*/

                mNetworkReady = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("[PrevizTracker.grabber_establish_connection] -> Can't read initial data from Grabber. " + e.ToString());
                closeSocket();
                return false;
            }

            return true;
        }

        private void thread_dataStreaming()
        {
            int connectCount = 0;
            do
            {
                try
                {
                    // 重连测试
                    if (connectCount >= autoReconnectCount)
                        break;

                    connectCount++;

                    bool connectSuccess = grabber_establish_connection();
                    if (!connectSuccess)
                    {
                        continue;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("[PrevizTracker.thread_dataStreaming] -> Can't connect to Grabber: " + e.Message);
                    continue;
                }

                connectCount = 0;

                try
                {
                    NetworkStream vNetStream = mSocket.GetStream();

                    while (mSocket.Connected)
                    {
                        // Read complete packet.
                        byte[] pPingPongBuf;
                        mPingPongBuf.writeRequest(out pPingPongBuf);

                        readFromStream(vNetStream, pPingPongBuf, pPingPongBuf.Length);

                        mPingPongBuf.writeRelease();
                    }

                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("[PrevizTracker.thread_dataStreaming] -> Error while reading data stream from Grabber: " + e.Message);
                    mPingPongBuf.writeCancelRequest();
                    closeSocket();
                }
            }
            while (autoReconnect && !mStopThreadRequest);

            closeSocket();
            Debug.Log("[PrevizTracker.thread_dataStreaming] -> Disconnected");
        }


        // Try/catch must be used around this function to catch disconnection.
        private bool readFromStream(NetworkStream iStrm, byte[] iBuf, int iSz)
        {
            int totalRead = 0;

            while (totalRead < iSz)
            {
                int byteRead = iStrm.Read(iBuf, totalRead, iSz - totalRead);
                totalRead += byteRead;

                // Check connection if no exception has been thrown 
                // (if the grabber was just closed) and data is null
                if (byteRead != 0)
                    continue;

                Debug.Log("[PrevizTracker.readFromStream] -> Disconneted?");
                // We have to use TcpConnectionInformation to check
                // connection because mSocket.Connected is always true even
                // after disconnection (C# socket implementation issue.)
                TcpConnectionInformation[] tcpConnections =
                    IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();

                // Iterate through the list of connection opened and try
                // to find our connection to check the status.
                bool connectionIsOk = false;
                foreach (TcpConnectionInformation t in tcpConnections)
                {
                    if (t.LocalEndPoint.Equals(mSocket.Client.LocalEndPoint)
                        && t.RemoteEndPoint.Equals(mSocket.Client.RemoteEndPoint))
                    {
                        if (t.State != TcpState.Established)
                        {
                            throw new IOException();
                        }
                        else
                        {
                            // We need to check if the client dropped the 
                            // connection. Try to write some dummy data on the
                            // socket.
                            iStrm.WriteByte(133);

                            connectionIsOk = true;
                            break;
                        }
                    }
                }


                if (!connectionIsOk)
                {
                    // The connection has not been found in the list of TCP 
                    // connection.
                    mNetworkReady = false;
                    throw new IOException();
                }
            }

            return true;
        }

        // Utilities functions
        private void closeSocket()
        {
            if (mSocket != null)
            {
                try
                {
                    NetworkStream vStrm = mSocket.GetStream();
                    if (vStrm != null)
                        vStrm.Dispose();

                    mSocket.Close();
                    mSocket = null;
                }
                catch (System.Exception ex)
                {
                    // InvalidOperationException: The TcpClient is not connected to a remote host.
                    // ObjectDisposedException: The TcpClient has been closed.
                }
            }

            mNetworkReady = false;
        }

        private Vector3 readXMLPoint(XmlReader iXml)
        {
            Vector3 vec = new Vector3();
            while (iXml.Read())
            {
                if (!iXml.IsStartElement())
                    continue;

                switch (iXml.Name)
                {
                    case "x":
                        vec.x = iXml.ReadElementContentAsFloat();
                        break;

                    case "y":
                        vec.y = iXml.ReadElementContentAsFloat();
                        break;

                    case "z":
                        vec.z = iXml.ReadElementContentAsFloat();
                        break;
                }
            }

            return vec;
        }
    }

}
