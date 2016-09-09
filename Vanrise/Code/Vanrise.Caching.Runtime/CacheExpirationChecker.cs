using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Common;

namespace Vanrise.Caching.Runtime
{
    internal class CacheExpirationChecker : ICacheExpirationChecker
    {
        static Dictionary<ICacheManagerKey, CacheManagerCheckInfo> s_cacheManagersCheckInfos = new Dictionary<ICacheManagerKey, CacheManagerCheckInfo>();
        static InterRuntimeServiceManager s_interRuntimeServiceManager = new InterRuntimeServiceManager();
        public bool TryCheckExpirationFromRuntimeService<ParamType>(Type cacheManagerType, ParamType parameter, out bool isExpired)
        {
            isExpired = false;
            int? cachingDistributorRuntimeProcessId = SingletonServiceProcessIdGetter.GetRuntimeProcessId(CachingDistributorRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
            if (!cachingDistributorRuntimeProcessId.HasValue)
                return false;
            if (RunningProcessManager.IsCurrentProcessARuntime && RunningProcessManager.CurrentProcess.ProcessId == cachingDistributorRuntimeProcessId.Value)
                return false;

            CacheManagerKey<ParamType> cacheManagerKey = new CacheManagerKey<ParamType>
            {
                CacheManagerType = cacheManagerType,
                ParameterType = parameter
            };
            CacheManagerCheckInfo cacheManagerCheckInfo;
            if (!s_cacheManagersCheckInfos.TryGetValue(cacheManagerKey, out cacheManagerCheckInfo))
            {
                lock (s_cacheManagersCheckInfos)
                {
                    cacheManagerCheckInfo = s_cacheManagersCheckInfos.GetOrCreateItem(cacheManagerKey);
                }
            }
            if ((DateTime.Now - cacheManagerCheckInfo.LastRuntimeServiceCheckTime).TotalSeconds > 4)
            {
                lock (cacheManagerCheckInfo)
                {
                    if ((DateTime.Now - cacheManagerCheckInfo.LastRuntimeServiceCheckTime).TotalSeconds > 4)
                    {
                        try
                        {
                            var expirationResult = s_interRuntimeServiceManager.SendRequest(cachingDistributorRuntimeProcessId.Value,
                                new CheckCacheManagerExpirationRequest<ParamType>
                                {
                                    CacheManagerType = cacheManagerType,
                                    Parameter = parameter,
                                    LastCheckTime = cacheManagerCheckInfo.LastCheckTime
                                });
                            isExpired = expirationResult.IsExpired;
                            cacheManagerCheckInfo.LastCheckTime = expirationResult.LastCheckTime;
                            cacheManagerCheckInfo.LastRuntimeServiceCheckTime = DateTime.Now;
                        }
                        catch
                        {
                            SingletonServiceProcessIdGetter.ResetRuntimeProcessId(CachingDistributorRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private interface ICacheManagerKey
        {

        }
        private struct CacheManagerKey<ParamType> : ICacheManagerKey
        {
            public Type CacheManagerType { get; set; }

            public ParamType ParameterType { get; set; }
        }

        private class CacheManagerCheckInfo
        {
            public DateTime? LastCheckTime { get; set; }

            public DateTime LastRuntimeServiceCheckTime { get; set; }
        }
    }

    internal class CheckCacheManagerExpirationRequest<ParamType> : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<CacheManagerExpirationResult>
    {
        public Type CacheManagerType { get; set; }

        public ParamType Parameter { get; set; }

        public DateTime? LastCheckTime { get; set; }

        public override CacheManagerExpirationResult Execute()
        {
            DateTime? lastCheckTime = this.LastCheckTime;
            bool isExpired = (Vanrise.Caching.CacheManagerFactory.GetCacheManager(this.CacheManagerType) as ICacheManager<ParamType>).IsCacheExpired(this.Parameter, ref lastCheckTime);
            return new CacheManagerExpirationResult
            {
                IsExpired = isExpired,
                LastCheckTime = lastCheckTime
            };
        }
    }

    internal class CacheManagerExpirationResult
    {
        public bool IsExpired { get; set; }

        public DateTime? LastCheckTime { get; set; }
    }
}
