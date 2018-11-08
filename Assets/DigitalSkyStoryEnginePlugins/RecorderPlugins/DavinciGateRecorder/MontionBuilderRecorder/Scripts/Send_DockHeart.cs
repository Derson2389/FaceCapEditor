using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBR
{
    public class Send_DockHeart : NetworkSendMsg
    {
        public Send_DockHeart()
            : base((int)MontionRecorderServerPacketId.Id_DockHeart)
        {
        }
    }

}
