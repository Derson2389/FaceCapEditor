using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MBR
{
    public class Send_CreateNodeTree : NetworkSendMsg
    {
        private int m_MarkId;
        private int m_Index;

        public Send_CreateNodeTree(int markId, int index)
            : base((int)MontionRecorderServerPacketId.Id_CreateNodeTree)
        {
            m_MarkId = markId;
            m_Index = index;
        }

        protected override void Process(BinaryWriter bw)
        {
            bw.Write(m_MarkId);
            bw.Write(m_Index);
        }
    }

}

