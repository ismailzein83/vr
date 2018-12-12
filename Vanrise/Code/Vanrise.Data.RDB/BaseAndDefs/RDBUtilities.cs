using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public static class RDBUtilities
    {
        static Dictionary<Type, string> s_methodNamesByType = new Dictionary<Type, string>();

        public static string GetGetReaderValueMethodNameWithValidate(Type runtimeType)
        {
            Dictionary<Type, string> methodNamesByType = GetRDBReaderMethodNamesByType();

            string functionName;
            if (!methodNamesByType.TryGetValue(runtimeType, out functionName))
                throw new Exception($"No GetReaderValueMethod found for type {runtimeType}");
            return functionName;
        }

        static Dictionary<Type, string> GetRDBReaderMethodNamesByType()
        {
            if (s_methodNamesByType.Count == 0)
            {
                lock (s_methodNamesByType)
                {
                    if (s_methodNamesByType.Count == 0)
                    {
                        foreach (var methodInfo in typeof(IRDBDataReader).GetMethods())
                        {
                            string existingMethodName;
                            if (!s_methodNamesByType.TryGetValue(methodInfo.ReturnType, out existingMethodName))
                            {
                                s_methodNamesByType.Add(methodInfo.ReturnType, methodInfo.Name);
                            }
                            else
                            {
                                if (methodInfo.Name.Contains("WithNullHandling"))
                                    s_methodNamesByType[methodInfo.ReturnType] = methodInfo.Name;
                            }
                        }
                    }
                }
            }

            return s_methodNamesByType;
        }
    }
}
