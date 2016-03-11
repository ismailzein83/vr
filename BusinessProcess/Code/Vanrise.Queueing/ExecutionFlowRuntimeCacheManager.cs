using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    internal class ExecutionFlowRuntimeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        DateTime? _execFlowDefinitionCacheLastCheck;
        DateTime? _execFlowCacheLastCheck;
        DateTime? _queueCacheLastCheck;

        protected override bool ShouldSetCacheExpired()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueExecutionFlowDefinitionManager.CacheManager>().IsCacheExpired(ref _execFlowDefinitionCacheLastCheck)
                |
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueExecutionFlowManager.CacheManager>().IsCacheExpired(ref _execFlowCacheLastCheck)
                |
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>().IsCacheExpired(ref _queueCacheLastCheck); ;
        }
    }
}
