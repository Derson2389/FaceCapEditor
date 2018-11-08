using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace MBR
{
    public class Recv_CreateNodeTree : NetworkRecvMsg
    {
        private int m_MarkId = 0;
        private MBRootNodeInfo m_NewNode = null;
        public override bool Parse(BinaryReader br, NetworkConnectClient connection)
        {
            m_MarkId = br.ReadInt32();
            int index = MontionRecorderServer.Instance.GetNodeIndex();

            m_NewNode = new MBRootNodeInfo(index, connection.ConnectionSocket);

            try
            {
                m_NewNode.DecodeTree(br);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                m_NewNode = null;
            }

            return m_NewNode != null;
        }

        public override void Process(NetworkConnectClient connection)
        {
            MontionRecorderServer.Instance.RegisterNodeInfo(m_NewNode);
            connection.AddSendMessage(new Send_CreateNodeTree(m_MarkId, m_NewNode.GetId()));

#if UNITY_EDITOR
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
        }
    }

}

