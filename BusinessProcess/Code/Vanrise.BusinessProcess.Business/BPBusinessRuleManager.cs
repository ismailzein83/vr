using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleManager
    {
        #region public methods
        public List<BPBusinessRuleDefinition> GetBPBusinessRuleDefinitions(string key, int bpDefinitionId)
        {
            List<BPBusinessRuleDefinition> result = GetCachedBPBusinessRules();
            if (result == null || result.Count == 0)
                return null;

            return result.FindAll(itm => itm.BPDefintionId == bpDefinitionId && itm.Name == key);
        }
        #endregion

        #region private methods
        private List<BPBusinessRuleDefinition> GetCachedBPBusinessRules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPBusinessRuleDefinitions",
            () =>
            {
                IBPBusinessRuleDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleDefinitionDataManager>();
                return dataManager.GetBPBusinessRuleDefinitions();
            });
        }

        private Dictionary<int, BPBusinessRuleDefinition> GetCachedBPBusinessRulesById()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPBusinessRuleDefinitionsById",
            () =>
            {
                IBPBusinessRuleDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleDefinitionDataManager>();
                var data = dataManager.GetBPBusinessRuleDefinitions();
                return data.ToDictionary(cn => cn.BPBusinessRuleDefinitionId, cn => cn);
            });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPBusinessRuleDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPBusinessRuleDefinitionsUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}