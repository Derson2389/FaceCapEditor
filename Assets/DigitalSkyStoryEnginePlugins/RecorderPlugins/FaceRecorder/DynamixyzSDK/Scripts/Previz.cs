using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using UnityEngine;

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
    public class Previz : MonoBehaviour 
    {
        // Public properties
        public string IPAddress = "localhost";
        public int Port = 5559;
        public bool AutoConnect = true;
        public bool AutoReconnect = true; // Reconnect if the connection to grabber is lost.
        public bool UseGUI = true;

        // 3D parameters
        public enum AxesOrder
        {
            XYZ, XZY,
            ZXY, ZYX,
            YZX, YXZ
        };

        // For bones
        public AxesOrder Axes;
        
        public bool InvertXAxis = false;
        public bool InvertYAxis = false;
        public bool InvertZAxis = false;
        public float Scale = 0.01f;

        // For Headbone
        public AxesOrder HeadAxes;

        public bool InvertHeadXAxis = false;
        public bool InvertHeadYAxis = false;
        public bool InvertHeadZAxis = false;

        // Private properties
        private TcpClient mSocket = null;
        private ArrayList mEntities;

        private UnityEngine.Component[] mRigComponents;
        private UnityEngine.SkinnedMeshRenderer[] mRigSkinnedMeshRenderers;

        private uint mNbCoeff;
        private float[] mCoeffs;
        private bool mNetworkReady;
        private PingPongBuffer mPingPongBuf;
        private uint[] mTimecode = new uint[4];

        // Low level properties
        private Thread mStreamingThread;
        private bool mStopThreadRequest;
        private const uint OPTIMAL_IP_PACKET_SIZE = 1400;

        class ESC_Entity
        {
            public enum Type
            {
                INVALID,
                BONE,
                HEAD_BONE,
                BLENDSHAPE
            }

            public ESC_Entity()
            {
                name = "<INVALID>";
                type = Type.INVALID;
                buf_coeff_idx = -1;
                bshape_mesh_idx = -1;
                bone_transform = null;
            }

            public Type type { get; set; }
            public string name { get; set; }
            public int buf_coeff_idx { get; set; } // Index of the entity value start in the data buffer of packets sent by Grabber.

            // For Bones only.
            public UnityEngine.Transform bone_transform { get; set; }
            public UnityEngine.Vector3 bone_neutral_trans { get; set; }
            public UnityEngine.Quaternion bone_neutral_rot { get; set; }

            // For BShapes only.
            public UnityEngine.SkinnedMeshRenderer bshape_skin_mesh_rend { get; set; }
            public int bshape_mesh_idx { get; set; }
        }


        // Use this for initialization
        void Start()
        {
            gameObject.AddComponent<UnityMainThreadDispatcher>();
            mNetworkReady = false;

            // Retrieve all components attached to the object that own the script.
            mRigComponents = gameObject.GetComponentsInChildren<Component>(true);

            // Retrieve all meshes on the current object.
            // We need to store them in global variable, because network thread is
            // not allowed to access Unity methods.
            mRigSkinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            string vMeshesStr = "";
            foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
            {
                vMeshesStr += (m.sharedMesh.name + " - ");
            }
            Debug.Log("Mesh(es) detected: " + System.Environment.NewLine + mRigSkinnedMeshRenderers.Length + vMeshesStr);


            if (AutoConnect)
            {
                grabber_connect();
            }
        }

        private void OnDisable()
        {
            grabber_disconnect();
        }


        // Update is called once per frame
        void Update()
        {
            if (mStreamingThread == null || !mStreamingThread.IsAlive)
                return;

            if (!mNetworkReady)
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


            Vector3 t = new Vector3();
            Vector3 r = new Vector3();

            foreach (ESC_Entity e in mEntities)
            {
                if (e.type == ESC_Entity.Type.BLENDSHAPE)
                {
                    float value = mCoeffs[e.buf_coeff_idx];

                    e.bshape_skin_mesh_rend.SetBlendShapeWeight(
                        e.bshape_mesh_idx,
                        value * 100
                        );
                }
                else if (e.type == ESC_Entity.Type.BONE || e.type == ESC_Entity.Type.HEAD_BONE)
                {
                    t.x = mCoeffs[e.buf_coeff_idx + 0];
                    t.y = mCoeffs[e.buf_coeff_idx + 1];
                    t.z = mCoeffs[e.buf_coeff_idx + 2];

                    // TODO: old code -> if (mEntityType[e].Equals("head"))
                    r.x = mCoeffs[e.buf_coeff_idx + 3];
                    r.y = mCoeffs[e.buf_coeff_idx + 4];
                    r.z = mCoeffs[e.buf_coeff_idx + 5];

                    AxesOrder vBoneAxe;
                    bool vInvertX;
                    bool vInvertY;
                    bool vInvertZ;
                    if (e.type == ESC_Entity.Type.BONE)
                    {
                        vBoneAxe = Axes;
                        vInvertX = InvertXAxis;
                        vInvertY = InvertYAxis;
                        vInvertZ = InvertZAxis;
                    }
                    else
                    {
                        vBoneAxe = HeadAxes;
                        vInvertX = InvertHeadXAxis;
                        vInvertY = InvertHeadYAxis;
                        vInvertZ = InvertHeadZAxis;
                    }
                    

                    if (vInvertX)
                    {
                        t.x = t.x * -1;
                        r.x = r.x * -1;
                    }

                    if (vInvertY)
                    {
                        t.y = t.y * -1;
                        r.y = r.y * -1;
                    }

                    if (vInvertZ)
                    {
                        t.z = t.z * -1;
                        r.z = r.z * -1;
                    }



                    if (vBoneAxe.Equals(AxesOrder.XZY))
                    {
                        // swap Y-Z on translation.
                        float y = t.y;
                        t.y = t.z;
                        t.z = y;

                        // swap Y-Z on rotation.
                        y = r.y;
                        r.y = r.z;
                        r.z = y;
                    }

                    if (vBoneAxe.Equals(AxesOrder.ZYX))
                    {
                        // swap X-Z on translation.
                        float x = t.x;
                        t.x = t.z;
                        t.z = x;

                        // swap X-Z on rotation.
                        x = r.x;
                        r.x = r.z;
                        r.z = x;
                    }

                    if (vBoneAxe.Equals(AxesOrder.YXZ))
                    {
                        // swap X-Y on translation.
                        float x = t.x;
                        t.x = t.y;
                        t.y = x;

                        // swap X-Y on rotation.
                        x = r.x;
                        r.x = r.y;
                        r.y = x;
                    }

                    if (vBoneAxe.Equals(AxesOrder.ZXY))
                    {
                        float x = t.x;
                        float y = t.y;
                        float z = t.z;

                        t.x = z;
                        t.y = x;
                        t.z = y;

                        // swap Z-X-Y on rotation.
                        x = r.x;
                        r.x = r.z;
                        r.z = r.y;
                        r.y = x;
                    }

                    if (vBoneAxe.Equals(AxesOrder.YZX))
                    {
                        float x = t.x;
                        float y = t.y;
                        float z = t.z;

                        t.x = y;
                        t.y = z;
                        t.z = x;

                        // swap X-Y on rotation.
                        x = r.x;
                        r.x = r.y;
                        r.y = r.z;
                        r.z = x;
                    }

                    if(e.type == ESC_Entity.Type.BONE)
                    {
                        // TODO: move this into grabber3.
                        e.bone_transform.localPosition = t * Scale;

                        Quaternion rQuat = Quaternion.Euler(-r.x, -r.y, -r.z);
                        e.bone_transform.localRotation = e.bone_neutral_rot * rQuat;
                    }
                    else
                    {
                        Quaternion rQuat = Quaternion.Euler(r.x, r.y, r.z);
                        e.bone_transform.localRotation = e.bone_neutral_rot * rQuat;
                    }


                }
            }
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
                ThreadStart vTs = new ThreadStart(_thread_dataStreaming);
                mStreamingThread = new Thread(vTs);

                mStreamingThread.Start();
            }
            catch (System.Exception e)
            {
                // TODO: add reconnecting on issue on the socket.
                Debug.LogWarning("Error while connecting to Grabber: " + e.Message);
                _closeSocket();
                return false;
            }

            return true;
        }

        bool grabber_disconnect()
        {
            try
            {
                if (mSocket != null)
                {
                    mSocket.GetStream().Close();
                }
            }
            catch(System.Exception e)
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


        private bool _grabber_establish_connection()
        {
            _closeSocket();
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
                _readFromStream(vNetStrm, vBuf, sizeof(uint));
                vNbCoeff = System.BitConverter.ToUInt32(vBuf, 0);

                // Read the size of the ESC.
                uint vEscSize = 0;
                _readFromStream(vNetStrm, vBuf, sizeof(uint));
                vEscSize = System.BitConverter.ToUInt32(vBuf, 0);

                // Retrieve the ESC from network.
                byte[] vEscBuf = new byte[vEscSize];
                _readFromStream(vNetStrm, vEscBuf, (int)vEscSize);
                string vEsc = System.Text.Encoding.ASCII.GetString(vEscBuf);

                // Parse the ESC and apply it to the scene. This must be called from
                // the main thread to avoid exception.
                // We use a special object call UnityMainThreadDispatcher that will
                // execute the "job" we give to it.
                UnityMainThreadDispatcher.Instance().EnqueueWait(_parseESCandAllocateBuffers(vEsc, vNbCoeff));

                if (!mNetworkReady)
                {
                    Debug.Log("Failed to parse the ESC.");
                    _closeSocket();
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Can't read initial data from Grabber. " + e.ToString());
                _closeSocket();
                return false;
            }

            

            return true;
        }

        private void _thread_dataStreaming()
        {
            do
            {
                try
                {
                    bool vConnectSuccess = _grabber_establish_connection();
                    if (!vConnectSuccess)
                        continue;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Can't connect to Grabber: " + e.Message);
                    continue;
                }

                try
                {
                    NetworkStream vNetStream = mSocket.GetStream();

                    while (mSocket.Connected)
                    {
                        // Read complete packet.
                        byte[] pPingPongBuf;
                        mPingPongBuf.writeRequest(out pPingPongBuf);

                        _readFromStream(vNetStream, pPingPongBuf, pPingPongBuf.Length);

                        mPingPongBuf.writeRelease();
                    }

                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Error while reading data stream from Grabber: " + e.Message);
                    mPingPongBuf.writeCancelRequest();
                    _closeSocket();
                }
            }
            while (AutoReconnect && !mStopThreadRequest);

            _closeSocket();

            Debug.Log("Disconnected");
        }



        // Try/catch must be used around this function to catch disconnection.
        private bool _readFromStream(NetworkStream iStrm, byte[] iBuf, int iSz)
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

                Debug.Log("Disconneted???");
                // We have to use TcpConnectionInformation to check
                // connection because mSocket.Connected is always true even
                // after disconnection (C# socket implementation issue.)
                TcpConnectionInformation[] tcpConnections = 
                    IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();

                // Iterate through the list of connection opened and try
                // to find our connection to check the status.
                bool bConnectionIsOk = false;
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

                            bConnectionIsOk = true;
                            break;
                        }
                    }
                }

                
                if(!bConnectionIsOk)
                {
                    // The connection has not been found in the list of TCP 
                    // connection.
                    throw new IOException();
                }
            }

            return true;
        }

        // Parse ESC.
        private IEnumerator _parseESCandAllocateBuffers(string iEsc, uint iNbCoeff)
        {
            // Reset all bones positions to initial value (in case of reconnection).
            if(mEntities != null)
            {
                foreach(ESC_Entity e in mEntities)
                {
                    if(e.type == ESC_Entity.Type.BONE)
                    {
                        e.bone_transform.localPosition = e.bone_neutral_trans;
                        e.bone_transform.localRotation = e.bone_neutral_rot;
                    }
                }

                mEntities.Clear();
                mEntities = null;
            }

            mEntities = new ArrayList();

            // All of this code MUST be executed in the main thread. All variable
            // modified here can't be in race condition with update thus.
            XmlReader vRdr = XmlReader.Create(new StringReader(iEsc));

            ESC_Entity vCurrEntity = null;
            int vCurrentBufIdx = 0;
            while (vRdr.Read())
            {
                if (!vRdr.IsStartElement())
                    continue;

                if (vRdr.Name == "entity")
                {
                    vCurrEntity = new ESC_Entity();
                    vCurrEntity.name = vRdr.GetAttribute("name");
                    vCurrEntity.buf_coeff_idx = vCurrentBufIdx;




                    // Retrieve the Transform from the scene if it is either a bone
                    // or a blendshape.
                    if (vRdr.GetAttribute("class") == "component")
                    {
                        // B O N E S


                        // Head bone is handled differently.
                        if (vRdr.GetAttribute("solver").Equals("head"))
                            vCurrEntity.type = ESC_Entity.Type.HEAD_BONE;
                        else
                            vCurrEntity.type = ESC_Entity.Type.BONE;

                        vCurrentBufIdx += 9;

                        // Search the ESC bone into the current script's attached 
                        // GameObject.
                        foreach (Component cp in mRigComponents)
                        {
                            if (cp.name == vCurrEntity.name)
                            {
                                vCurrEntity.bone_transform = cp.gameObject.transform;
                                break;
                            }
                        }

                        if (vCurrEntity.bone_transform != null)
                        {
                            // Save the neutral translation and the neutral rotation of the bone.
                            // This will be used while reading streaming for calculation and also
                            // used for restoring default pose when server is disconnected.
                            vCurrEntity.bone_neutral_trans = vCurrEntity.bone_transform.localPosition;
                            vCurrEntity.bone_neutral_rot = vCurrEntity.bone_transform.localRotation;

                        }
                        else
                        {
                            vCurrEntity.type = ESC_Entity.Type.INVALID;
                            Debug.LogWarning("Error: can't find the bone " + vCurrEntity.name + " in the object " + this.gameObject.name);
                        }
                        
                    }
                    else if (vRdr.GetAttribute("class") == "blendshapetarget")
                    {

                        vCurrentBufIdx += 1;

                        vCurrEntity.type = ESC_Entity.Type.BLENDSHAPE;
                        // Try to parse the blendshape name. ESC style is
                        // BSHAPE_SUBNAME[BSHAP_NAME]
                        // e.g: level3_Layers_38_Blendshape[grandyeuxD]
                        // The @ describe a C# verbatim string.
                        Match result =
                            Regex.Match(vCurrEntity.name, @"([^\[]*)\[([^\]]+)");

                        if (result.Success)
                        {
                            // Generate the shape name according to Unity 
                            // syntax.
                            // Groups[0] contain the original pattern without
                            // groups. Groups are starting from 1.
                            string vGroupName = result.Groups[1].Value;
							string vBshapeName = result.Groups[2].Value.Substring(0, result.Groups[2].Value.Length - 1);
                            string vUnityBShapeName = vGroupName + "." + vBshapeName;


                            // We do proceed with 2 search algorithm for the blendshapes.
                            // The second algorithm is for some bugs with maya export.

                            // Iterate through the meshes and search for the
                            // blendshape.

                            // First search algorithm: exact name.
                            foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
                            {
                                int vBshapeIdx =
                                    m.sharedMesh.GetBlendShapeIndex(vUnityBShapeName);

                                if (vBshapeIdx == -1)
                                    continue;

                                // We have found the corresponding blendshape in the mesh. 
                                // Store the reference to constraint the blendshape and its index.
                                vCurrEntity.bshape_skin_mesh_rend = m;
                                vCurrEntity.bshape_mesh_idx = vBshapeIdx;

                                break;
                            }

                            // 2nd algorithm
                            // If first search algorithm didn't work. Try to find
                            // the blendshape without the group name.
                            if (vCurrEntity.bshape_skin_mesh_rend == null)
                            {
                                foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
                                {
                                    int vBshapeIdx =
										m.sharedMesh.GetBlendShapeIndex(vBshapeName);

                                    if (vBshapeIdx == -1)
                                        continue;

                                    // We have found the corresponding blendshape in the mesh. 
                                    // Store the reference to constraint the blendshape and its index.
                                    vCurrEntity.bshape_skin_mesh_rend = m;
                                    vCurrEntity.bshape_mesh_idx = vBshapeIdx;

                                    break;
                                }
                            }

                            // 3rd algorithm: search any blendshape with name.
                            // Warning! It can create animate the wrong blendshape.
                            if (vCurrEntity.bshape_skin_mesh_rend == null)
                            {
                                foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
                                {
                                    for(int i = 0; i < m.sharedMesh.blendShapeCount; i++)
                                    {
                                        string vBlendshape = m.sharedMesh.GetBlendShapeName(i);

                                        if (!vBlendshape.Contains(vBshapeName))
                                            continue;
                                        
                                        vCurrEntity.bshape_skin_mesh_rend = m;
                                        vCurrEntity.bshape_mesh_idx = i;

                                        break;
                                    }
                                }
                            }


                            if (vCurrEntity.bshape_skin_mesh_rend == null)
                            {
                                //Debug.LogWarning("Can't find blendshape \"" + vUnityBShapeName);
								Debug.LogWarning("Can't find blendshape \"" + vBshapeName);
                                vCurrEntity.type = ESC_Entity.Type.INVALID;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Can't parse blendshape name \"" + vCurrEntity.name);
                            vCurrEntity.type = ESC_Entity.Type.INVALID;
                        }

                    }
                    else if(vRdr.GetAttribute("class") == "customproperty")
                    {
                        vCurrentBufIdx += 1;
                        break;
                    }
                    else
                    {
                        Debug.LogError("Error: bad entity type detected.");
                        // TODO: Abort connection? State might be corrupted 
                        // because vCurrentBufIdx can't be incremented correctly.
                    }

                    // TODO: Initialized all offset based on the PRL file? (cf old script code).
                    if (vCurrEntity.type != ESC_Entity.Type.INVALID)
                    {
                        mEntities.Add(vCurrEntity);
                    }
                }
                else if(vRdr.Name == "default" && vCurrEntity.type == ESC_Entity.Type.BONE) 
                {
                    // NOTA: this code is not used currently, but might be in
                    // future.
                    // Parsing default position for bones.
					/*
                    XmlReader vSubTree = vRdr.ReadSubtree();
                    Vector3 t = new Vector3();
                    Vector3 r = new Vector3();
                    Vector3 s = new Vector3();
                    while (vSubTree.Read())
                    {
                        if (!vSubTree.IsStartElement())
                            continue;

                        if(vSubTree.Name == "translation")
                        {
                            t = _readXMLPoint(vSubTree.ReadSubtree());
                        }
                        else if(vSubTree.Name == "rotation")
                        {
                            r = _readXMLPoint(vSubTree.ReadSubtree());
                        }
                        else if(vSubTree.Name == "scale")
                        {
                            s = _readXMLPoint(vSubTree.ReadSubtree());
                        }
                    }
					*/
                }
            }


            // Allocate buffering stuff.
            mNbCoeff = iNbCoeff;

            // Allocate the number of floats
            // Compute the size of the network buffer.
            // size of all coeff + frame tag (unsigned int) + 4 unsigned for TC : HH MM SS FF
            uint bufSize = 5 * sizeof(uint) + sizeof(float) * iNbCoeff;

            // We are padding packet with dummy data, on the server side, to reach the 
            // packet MTU size to force the packet to be sent on the network.
            if (bufSize < OPTIMAL_IP_PACKET_SIZE)
                bufSize = OPTIMAL_IP_PACKET_SIZE;

            mPingPongBuf = new PingPongBuffer(bufSize);
            mCoeffs = new float[mNbCoeff];

            mNetworkReady = true;

            //TODO: save neutral rig pose.
            yield return null;
        }


        private void OnGUI()
        {
            if (!UseGUI)
                return;

            //TODO: CLEAN THAT.

            float msec = 1000.0f;
            string text = string.Format("Reception FPS: {0:0.0} ms ({1:0.} fps)", msec, 30.0);

            int AnchorX = Screen.width - 235;
            int AnchorY = 30;

            GUI.Box(new Rect(AnchorX, AnchorY, 220, 260), "Face Animation");

            if (mStreamingThread == null || !mStreamingThread.IsAlive)
            {
                GUI.Label(new Rect(AnchorX + 5, AnchorY + 35, 100, 20), "IP Address:");
                IPAddress = GUI.TextField(new Rect(AnchorX + 100, AnchorY + 35, 100, 20), IPAddress);
                GUI.Label(new Rect(AnchorX + 5, AnchorY + 65, 100, 20), "Port:");
                Port = int.Parse(GUI.TextField(new Rect(AnchorX + 100, AnchorY + 65, 100, 20), Port.ToString()));
                GUI.Label(new Rect(AnchorX + 5, AnchorY + 130, 210, 20), text);

                GUI.Label(new Rect(AnchorX + 5, AnchorY + 160, 200, 100), "");
                if (GUI.Button(new Rect(AnchorX + 5, AnchorY + 95, 200, 30), "Connect Face"))
                {
                    grabber_connect();
                }
            }
            else
            {
                GUI.Label(new Rect(AnchorX + 5, AnchorY + 70, 210, 20), text);

                string textTC = string.Format("Timecode: {0:00}:{1:00}:{2:00}:{3:00}", mTimecode[0], mTimecode[1], mTimecode[2], mTimecode[3]);
                GUI.Label(new Rect(AnchorX + 5, AnchorY + 125, 200, 20), textTC);


                GUI.Label(new Rect(AnchorX + 5, AnchorY + 155, 200, 100), "");
                GUI.Label(new Rect(AnchorX + 5, AnchorY + 100, 250, 20), "Server:" + IPAddress + ":" + Port);
                if (GUI.Button(new Rect(AnchorX + 5, AnchorY + 35, 200, 30), "Disconnect Face"))
                {
                    grabber_disconnect();
                }
            }
        }

        // Utilities functions
        private void _closeSocket()
        {
            if(mSocket != null)
            {
                try
                {
                    NetworkStream vStrm = mSocket.GetStream();
                    if (vStrm != null)
                        vStrm.Dispose();

                    mSocket.Close();
                    mSocket = null;
                }
                catch(System.Exception ex)
                {
                    // InvalidOperationException: The TcpClient is not connected to a remote host.
                    // ObjectDisposedException: The TcpClient has been closed.
                }
            }

            mNetworkReady = false;
        }

        private Vector3 _readXMLPoint(XmlReader iXml)
        {
            Vector3 vec = new Vector3();
            while(iXml.Read())
            {
                if (!iXml.IsStartElement())
                    continue;

                switch(iXml.Name)
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