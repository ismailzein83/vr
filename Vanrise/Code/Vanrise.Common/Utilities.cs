using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class Utilities
    {
        public static Dictionary<T,Q> GetEnumAttributes<T,Q>() where T : struct
            where Q : Attribute
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception(String.Format("{0} is not an Enum type", enumType));

            Type attributeType = typeof(Q);
            Dictionary<T, Q> enumAttributes = new Dictionary<T, Q>();
            foreach (var member in enumType.GetFields())
            {
                Q mbrAttribute = member.GetCustomAttributes(attributeType, true).FirstOrDefault() as Q;
                if (mbrAttribute != null)
                    enumAttributes.Add((T)Enum.Parse(enumType, member.Name), mbrAttribute);
            }
            return enumAttributes;
        }

        public static Q GetEnumAttribute<T, Q>(T enumItem)
            where T : struct
            where Q : Attribute
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception(String.Format("{0} is not an Enum type", enumType));

            Type attributeType = typeof(Q);
            Dictionary<T, Q> enumAttributes = new Dictionary<T, Q>();
            foreach (var member in enumType.GetFields())
            {
                T memberAsEnum;
                if (Enum.TryParse<T>(member.Name, true, out memberAsEnum) && memberAsEnum.Equals(enumItem))
                {
                    return member.GetCustomAttributes(attributeType, true).FirstOrDefault() as Q;
                }

            }
            return default(Q);
        }

        public static IEnumerable<Type> GetAllImplementations(Type baseType)
        {
            List<Type> lst = new List<Type>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    {
                        lst.Add(t);
                    }
                }
            }
            return lst;
        }

        public static IEnumerable<Type> GetAllImplementations<T>()
        {
            return GetAllImplementations(typeof(T));
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1 > date2 ? date1 : date2;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return date1 < date2 ? date1 : date2;
        }

    }
}
