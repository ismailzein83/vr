using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;
using System;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleActionManager
    {
        #region public methods
        public BPBusinessRuleAction GetBusinessRuleAction(Guid bpBusinessRuleDefinitionId)
        {
            return GetCachedBPBusinessRuleActionsByDefinition().GetRecord(bpBusinessRuleDefinitionId);
        }
        #endregion

        #region private methods

        private Dictionary<Guid, BPBusinessRuleAction> GetCachedBPBusinessRuleActionsByDefinition()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPBusinessRuleActions",
            () =>
            {
                IBPBusinessRuleActionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleActionDataManager>();
                var data = dataManager.GetBPBusinessRuleActions();
                return data.ToDictionary(cn => cn.Details.BPBusinessRuleDefinitionId, cn => cn);
            });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPBusinessRuleActionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleActionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPBusinessRuleActionsUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}