using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;
using System;

namespace Vanrise.BusinessProcess.Business
{
    public class BPTaskTypeManager
    {
        #region public methods

        public BPTaskType GetBPTaskType(Guid taskTypeId)
        {
            return GetCachedBPTaskTypesById().GetRecord(taskTypeId);
        }

        public BPTaskType GetBPTaskType(string taskTypeName)
        {
            return GetCachedBPTaskTypesByName().GetRecord(taskTypeName);
        }

        public BPTaskType GetBPTaskTypeByTaskId(long taskId)
        {
            BPTaskManager bpTaskManager = new BPTaskManager();
            var bpTask = bpTaskManager.GetTask(taskId);
            return GetBPTaskType(bpTask.TypeId);
        }
        #endregion

        #region private methods
        private Dictionary<Guid, BPTaskType> GetCachedBPTaskTypesById()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBPTaskTypesById",
               () =>
               {
                   return GetBPTaskTypes().ToDictionary(cn => cn.BPTaskTypeId, cn => cn);
               });
        }

        private Dictionary<string, BPTaskType> GetCachedBPTaskTypesByName()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBPTaskTypesByName",
               () =>
               {
                   return GetBPTaskTypes().ToDictionary(cn => cn.Name, cn => cn);
               });
        }

        private IEnumerable<BPTaskType> GetBPTaskTypes()
        {
            IBPTaskTypeDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskTypeDataManager>();
            return dataManager.GetBPTaskTypes();
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPTaskTypeDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPTaskTypesUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}