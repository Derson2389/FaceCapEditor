using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Network
{
    [Serializable]
    public class SerializableARFaceBlendShapes
    {
        public byte[] data;

        public SerializableARFaceBlendShapes(byte[] inputDatas)
        {
            data = inputDatas;
        }

        public static implicit operator SerializableARFaceBlendShapes(Dictionary<string, float> blendshapeData)
        {
            if (blendshapeData != null)
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(stream);

                bw.Write(blendshapeData.Count);
                foreach (KeyValuePair<string, float> kvp in blendshapeData)
                {
                    bw.Write(kvp.Key);
                    bw.Write(kvp.Value);
                }
                bw.Close();

                return new SerializableARFaceBlendShapes(stream.GetBuffer());
            }
            else
            {
                return new SerializableARFaceBlendShapes(null);
            }
        }

        public static implicit operator Dictionary<string, float>(SerializableARFaceBlendShapes spc)
        {
            if (spc.data != null)
            {
                MemoryStream stream = new MemoryStream(spc.data);
                BinaryReader br = new BinaryReader(stream);

                Dictionary<string, float> blendshapeData = new Dictionary<string, float>();

                int count = 0;
                count = br.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string bindName = br.ReadString();
                    float blendshapeValue = br.ReadSingle();

                    blendshapeData.Add(bindName, blendshapeValue);
                }
                return blendshapeData;
            }
            else
            {
                return null;
            }
        }
    };

    [Serializable]
    public class SerializableByteData
    {
        public byte[] data;
    }

    [Serializable]
    public class SerializableTextureData
    {
        public byte[] data;
        public int width;
        public int height;
    }
}
