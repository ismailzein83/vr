using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Runtime
{
    public static class SingletonServiceProcessIdGetter
    {
        static Dictionary<string, ServiceTypeInfo> s_serviceTypeInfo = new Dictionary<string, ServiceTypeInfo>();

        public static int? GetRuntimeProcessId(string serviceTypeUniqueName)
        {
            ServiceTypeInfo serviceTypeInfo;
            if (!s_serviceTypeInfo.TryGetValue(serviceTypeUniqueName, out serviceTypeInfo))
            {
                lock (s_serviceTypeInfo)
                {
                    serviceTypeInfo = s_serviceTypeInfo.GetOrCreateItem(serviceTypeUniqueName);
                }
            }
            if (!serviceTypeInfo.RuntimeProcessId.HasValue && (DateTime.Now - serviceTypeInfo.LastRetrievedTime).TotalSeconds > 10)
            {
                lock (serviceTypeInfo)
                {
                    if (!serviceTypeInfo.RuntimeProcessId.HasValue && (DateTime.Now - serviceTypeInfo.LastRetrievedTime).TotalSeconds > 10)
                    {
                        try
                        {
                            int runtimeProcessId;
                            if (new RunningProcessManager().TryGetRuntimeServiceProcessId(serviceTypeUniqueName, out runtimeProcessId))
                                serviceTypeInfo.RuntimeProcessId = runtimeProcessId;
                        }
                        catch (Exception ex)
                        {
                            serviceTypeInfo.RuntimeProcessId = null;
                            Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                        finally
                        {
                            serviceTypeInfo.LastRetrievedTime = DateTime.Now;
                        }
                    }
                }
            }
            return serviceTypeInfo.RuntimeProcessId;
        }

        public static void ResetRuntimeProcessId(string serviceTypeUniqueName)
        {
            ServiceTypeInfo serviceTypeInfo;
            if (!s_serviceTypeInfo.TryGetValue(serviceTypeUniqueName, out serviceTypeInfo))
            {
                lock (s_serviceTypeInfo)
                {
                    serviceTypeInfo = s_serviceTypeInfo.GetOrCreateItem(serviceTypeUniqueName);
                }
            }
            lock (serviceTypeInfo)
            {
                serviceTypeInfo.RuntimeProcessId = null;
            }
        }

        private class ServiceTypeInfo
        {
            public DateTime LastRetrievedTime { get; set; }

            public int? RuntimeProcessId { get; set; }
        }
    }
}
