using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DigitalSky.Network
{
    //Extension class to provide serialize / deserialize methods to object.
    //src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    //NOTE: You need add [Serializable] attribute in your class to enable serialization
    public static class NetSerializableExtension
    {
        public static byte[] NetSerializeToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T NetDeserialize<T>(this byte[] byteArray)
        {
            if (byteArray == null)
            {
                return default(T);
            }
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
