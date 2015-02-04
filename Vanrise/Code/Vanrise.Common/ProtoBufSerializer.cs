using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vanrise.Common
{
    public class ProtoBufSerializer
    {
        public static void Serialize<T>(Stream destination, T instance)
        {
            ProtoBuf.Serializer.Serialize<T>(destination, instance);
        }

        public static byte[] Serialize<T>(T instance)
        {
            byte[] serializedBytes = null;
            using (MemoryStream destination = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(destination, instance);
                serializedBytes = destination.ToArray();
            }
            return serializedBytes;
        }

        public static T Deserialize<T>(Stream source)
        {
            return ProtoBuf.Serializer.Deserialize<T>(source);
        }

        public static T Deserialize<T>(byte[] serializedBytes)
        {
            return Deserialize<T>(new MemoryStream(serializedBytes));
        }
    }
}
