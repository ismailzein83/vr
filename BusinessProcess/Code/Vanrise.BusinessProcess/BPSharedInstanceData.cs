using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Persistence;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Caching;

namespace Vanrise.BusinessProcess
{
    public class BPSharedInstanceData : PersistenceParticipant
    {
        public BPInstance InstanceInfo { get; set; }

        Dictionary<Type, Guid> cacheManagerIds = new Dictionary<Type, Guid>();
        public T GetCacheManager<T>() where T : class, ICacheManager
        {
            Type t = typeof(T);
            Guid cacheManagerId;
            if (!cacheManagerIds.TryGetValue(t, out cacheManagerId))
            {
                lock (cacheManagerIds)
                {
                    if (!cacheManagerIds.TryGetValue(t, out cacheManagerId))
                    {
                        cacheManagerId = Guid.NewGuid();
                        cacheManagerIds.Add(t, cacheManagerId);
                    }
                }
            }
            return CacheManagerFactory.GetCacheManager<T>(cacheManagerId);
        }

        internal void ClearCacheManagers()
        {
            foreach (var cacheManagerId in cacheManagerIds.Values)
                CacheManagerFactory.RemoveCacheManager(cacheManagerId);
            cacheManagerIds.Clear();
        }
    }
}
