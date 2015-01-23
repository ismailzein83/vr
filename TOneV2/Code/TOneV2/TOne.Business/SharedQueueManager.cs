using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Caching;
using TOne.Entities;
using Vanrise.Caching;

namespace TOne.Business
{
    public class SharedQueueManager
    {
        protected SharedQueueManager()
        {
        }

        static ConcurrentDictionary<Type, Object> s_Managers = new ConcurrentDictionary<Type, object>();
        public static T GetManager<T>() where T : SharedQueueManager
        {
            Object manager;
            if(!s_Managers.TryGetValue(typeof(T), out manager))
            {
                s_Managers.TryAdd(typeof(T), Activator.CreateInstance<T>());
                manager = s_Managers[typeof(T)];
            }
            return manager as T;
        }

        protected TOneQueue<T> GetQueue<T>(string queueName)
        {
            queueName = String.Format("{0}_{1}", this.GetType().FullName, queueName);
            return CacheManagerFactory.GetCacheManager<TOneCacheManager>().GetOrCreateObject(queueName, Entities.CacheObjectType.SharedMemoryQueue,
                () =>
                {
                    return new TOneQueue<T>();
                });
        }
    }
}
