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
            s_DeserializeMethod = typeof(ProtoBuf.Serializer).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static);
            ProtoBuf.Meta.RuntimeTypeModel.Default.MetadataTimeoutMilliseconds = 300000;
        }

        static MethodInfo s_SerializeMethod;
        static MethodInfo s_DeserializeMethod;

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

        public static dynamic Deserialize(Stream source, Type type)
        {
            return s_DeserializeMethod.MakeGenericMethod(type).Invoke(null, new Object[] { source });
        }

        public static T Deserialize<T>(byte[] serializedBytes)
        {
            return Deserialize<T>(new MemoryStream(serializedBytes));
        }

        public static dynamic Deserialize(byte[] serializedBytes, Type type)
        {
            return s_DeserializeMethod.MakeGenericMethod(type).Invoke(null, new Object[] { new MemoryStream(serializedBytes) });
        }

        public static void AddSerializableType(Type type, params string[] memberNames)
        {
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(type, false).Add(memberNames);
        }
    }
}
