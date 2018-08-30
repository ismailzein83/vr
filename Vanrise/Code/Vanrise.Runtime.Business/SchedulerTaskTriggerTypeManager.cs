using System;
using System.Collections.Generic;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using System.Linq;
using Vanrise.Common;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskTriggerTypeManager
    {
        public List<SchedulerTaskTriggerType> GetSchedulerTaskTriggerTypes()
        {
            var cachedSchedulerTaskTriggerTypes = GetCachedSchedulerTaskTriggerTypes();
            if (cachedSchedulerTaskTriggerTypes == null)
                return null;

            return cachedSchedulerTaskTriggerTypes.Values.ToList();
        }

        public SchedulerTaskTriggerType GetSchedulerTaskTriggerType(Guid triggerTypeId)
        {
            var cachedSchedulerTaskTriggerTypes = GetCachedSchedulerTaskTriggerTypes();
            if (cachedSchedulerTaskTriggerTypes == null)
                return null;

            return cachedSchedulerTaskTriggerTypes.GetRecord(triggerTypeId);
        }

        private Dictionary<Guid, SchedulerTaskTriggerType> GetCachedSchedulerTaskTriggerTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSchedulerTaskTriggerTypes",
               () =>
               {
                   ISchedulerTaskTriggerTypeDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskTriggerTypeDataManager>();
                   return dataManager.GetAll().ToDictionary(itm => itm.TriggerTypeId, itm => itm);
               });
        }

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISchedulerTaskTriggerTypeDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskTriggerTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreSchedulerTaskTriggerTypesUpdated(ref _updateHandle);
            }
        }
    }
}