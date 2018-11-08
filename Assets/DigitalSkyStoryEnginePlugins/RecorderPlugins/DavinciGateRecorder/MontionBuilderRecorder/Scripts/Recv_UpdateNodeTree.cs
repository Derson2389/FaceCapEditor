using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MBR
{
    public class Recv_UpdateNodeTree : NetworkRecvMsg
    {
        public override bool Parse(BinaryReader br, NetworkConnectClient connection)
        {
            int id = br.ReadInt32();

            MBRootNodeInfo nodeInfo = MontionRecorderServer.Instance.GetNodeInfo(id);
            if (nodeInfo == null)
            {
                return false;
            }

            try
            {
                nodeInfo.DecodeUpdateTree(br);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }

            return true;
        }

        public override void Process(NetworkConnectClient connection)
        {

        }
    }
}

