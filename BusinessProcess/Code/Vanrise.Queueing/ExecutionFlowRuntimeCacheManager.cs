using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    internal class ExecutionFlowRuntimeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        QueueExecutionFlowDefinitionManager.CacheManager _execFlowDefinitionCacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueExecutionFlowDefinitionManager.CacheManager>();
        QueueExecutionFlowManager.CacheManager _execFlowCacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueExecutionFlowManager.CacheManager>();
        QueueInstanceManager.CacheManager _queueCacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>();

        protected override bool ShouldSetCacheExpired()
        {
            return _execFlowDefinitionCacheManager.IsCacheExpired() || _execFlowCacheManager.IsCacheExpired() || _queueCacheManager.IsCacheExpired();
        }
    }
}
