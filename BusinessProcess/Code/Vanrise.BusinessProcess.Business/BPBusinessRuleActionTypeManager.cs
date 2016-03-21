using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleActionTypeManager
    {
        #region public methods
        public BPBusinessRuleActionType GetBusinessRuleActionType(int bpBusinessRuleActionTypeId)
        {
            return GetCachedBPBusinessRuleActionTypes().GetRecord(bpBusinessRuleActionTypeId);
        }
        #endregion

        #region private methods

        private Dictionary<int, BPBusinessRuleActionType> GetCachedBPBusinessRuleActionTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPBusinessRuleActionTypes",
            () =>
            {
                IBPBusinessRuleActionTypeDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleActionTypeDataManager>();
                var data = dataManager.GetBPBusinessRuleActionTypes();
                return data.ToDictionary(cn => cn.BPBusinessRuleActionTypeId, cn => cn);
            });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPBusinessRuleActionTypeDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleActionTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPBusinessRuleActionTypesUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}