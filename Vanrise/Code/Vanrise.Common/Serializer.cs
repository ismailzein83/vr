using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Vanrise.Common
{
    public static class Serializer
    {
        static JsonSerializerSettings s_Settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };

        static JsonSerializerSettings s_SettingsWithoutType = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None
        };

       
        
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, s_Settings);
        }

        public static string Serialize(object obj, bool withoutType)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, s_SettingsWithoutType);
        }

        public static T Deserialize<T>(string serialized)
        {
            if (serialized == null)
                return default(T);
            return JsonConvert.DeserializeObject<T>(serialized, s_Settings);
        }

        public static object Deserialize(string serialized)
        {
            if (serialized == null)
                return null;
            return JsonConvert.DeserializeObject(serialized, null, s_Settings);
        }

    }
}
