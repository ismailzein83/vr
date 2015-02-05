using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vanrise.Common
{
    public class ProtoBufSerializer
    {
        static ProtoBufSerializer()
        {
            s_SerializeMethod = typeof(ProtoBuf.Serializer).GetMethod("Serialize", BindingFlags.Static);
        }

        static MethodInfo s_SerializeMethod;

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

        public static byte[] Serialize(Object instance)
        {
            byte[] serializedBytes = null;
            using (MemoryStream destination = new MemoryStream())
            {
                s_SerializeMethod.MakeGenericMethod(instance.GetType()).Invoke(null, new Object[] { destination, instance });
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

        public static void AddSerializableType(Type type, params string[] memberNames)
        {
             ProtoBuf.Meta.RuntimeTypeModel.Default.Add(type, false).Add(memberNames);
        }
    }
}
