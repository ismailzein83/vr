using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    internal class ExecutionFlowRuntimeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        QueueExecutionFlowDefinitionManager.CacheManager _execFlowDefinitionCacheManager = 
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueExecutionFlowDefinitionManager.CacheManager>(Guid.NewGuid());
        QueueExecutionFlowManager.CacheManager _execFlowCacheManager = 
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueExecutionFlowManager.CacheManager>(Guid.NewGuid());
        QueueInstanceManager.CacheManager _queueCacheManager = 
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>(Guid.NewGuid());

        protected override bool ShouldSetCacheExpired()
        {
            return _execFlowDefinitionCacheManager.IsCacheExpired() || _execFlowCacheManager.IsCacheExpired() || _queueCacheManager.IsCacheExpired();
        }
    }
}
