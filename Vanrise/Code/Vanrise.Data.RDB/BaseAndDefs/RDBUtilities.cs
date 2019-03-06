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
            return GetGetReaderValueMethodNameWithValidate(runtimeType, false);
        }

        public static string GetGetReaderValueMethodNameWithValidate(Type runtimeType, bool alwaysReturnNullableMethod)
        {
            Dictionary<Type, string> methodNamesByType = GetRDBReaderMethodNamesByType();

            string functionName;
            if (!methodNamesByType.TryGetValue(runtimeType, out functionName))
                throw new Exception($"No GetReaderValueMethod found for type {runtimeType}");
            if(alwaysReturnNullableMethod)
            {
                if(!functionName.StartsWith("GetNullable"))
                {
                    string newFunctionName = string.Concat("GetNullable", functionName.Substring(3));
                    if (s_methodNamesByType.Values.Contains(newFunctionName))
                        functionName = newFunctionName;
                }
            }
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
                            if (!methodInfo.Name.StartsWith("Get"))
                                continue;
                            string existingMethodName;
                            if (!s_methodNamesByType.TryGetValue(methodInfo.ReturnType, out existingMethodName))
                            {
                                s_methodNamesByType.Add(methodInfo.ReturnType, methodInfo.Name);
                            }
                        }
                    }
                }
            }

            return s_methodNamesByType;
        }
    }
}
