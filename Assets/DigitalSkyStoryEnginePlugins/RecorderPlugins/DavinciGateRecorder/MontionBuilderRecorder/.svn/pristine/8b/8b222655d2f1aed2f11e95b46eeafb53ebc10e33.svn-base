using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace MBR
{
    public static class NetworkUtils
    {

        public static string ReadServerString(this BinaryReader br)
        {
            int len = br.ReadInt32();
            if (len == 0)
            {
                return string.Empty;
            }
            byte[] bytes = br.ReadBytes(len);
            return Encoding.UTF8.GetString(bytes);
        }
    }

}
