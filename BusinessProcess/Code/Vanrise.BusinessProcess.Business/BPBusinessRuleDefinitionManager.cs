using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;
using System;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleDefinitionManager
    {
        #region public methods
        public List<BPBusinessRuleDefinition> GetBPBusinessRuleDefinitions(string key, Guid bpDefinitionId)
        {
            List<BPBusinessRuleDefinition> result = GetCachedBPBusinessRules();
            if (result == null || result.Count == 0)
                return null;

            return result.FindAll(itm => itm.BPDefintionId == bpDefinitionId && itm.Name == key);
        }
        public BPBusinessRuleSettings GetRuleDefinitionSettings(Guid ruleDefinitionId)
        {
            var allRulesDefinitions = GetCachedBPBusinessRulesById();
            var ruleDefinition = allRulesDefinitions.GetRecord(ruleDefinitionId);
            ruleDefinition.ThrowIfNull("ruleDefinition");
            ruleDefinition.Settings.ThrowIfNull("ruleDefinition.Settings");
            return ruleDefinition.Settings;
        }
        public BPBusinessRuleDefinition GetBusinessRuleDefinitionById(Guid ruleDefinitionId)
        {
            var allRulesDefinitions = GetCachedBPBusinessRulesById();
            return allRulesDefinitions.GetRecord(ruleDefinitionId);
        }
        public List<BPBusinessRuleDefinition>GetAllBusinessRuleDefinitions ()
        {
            var allBusinessRuleDefinitions = GetCachedBPBusinessRules();
            return allBusinessRuleDefinitions;
        }
        public List<BPBusinessRuleDefinitionDetail> GetBusinessRuleDefintionsByBPDefinitionID(Guid bpDefinitionId)
        {
            List<BPBusinessRuleDefinition> result = GetCachedBPBusinessRules();
            if (result == null || result.Count == 0)
                return null;

            var matchedResult = result.FindAll(itm => itm.BPDefintionId == bpDefinitionId);
            if (matchedResult == null || matchedResult.Count == 0)
                return null;

            List<BPBusinessRuleDefinitionDetail> bpBusinessRuleDefinitionDetails = matchedResult.Select(item => new BPBusinessRuleDefinitionDetail() { Entity = item }).ToList();
            BPBusinessRuleActionTypeManager bpBusinessRuleActionTypeManager = new BPBusinessRuleActionTypeManager();
            
            foreach (BPBusinessRuleDefinitionDetail bpBusinessRuleDefinitionDetail in bpBusinessRuleDefinitionDetails)
            {
                bpBusinessRuleDefinitionDetail.ActionTypes = new List<BPBusinessRuleActionType>();
                foreach (Guid actionTypeId in bpBusinessRuleDefinitionDetail.Entity.Settings.ActionTypes)
                {
                    bpBusinessRuleDefinitionDetail.ActionTypes.Add(bpBusinessRuleActionTypeManager.GetBusinessRuleActionType(actionTypeId));
                }
            }

            return bpBusinessRuleDefinitionDetails;
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

        private Dictionary<Guid, BPBusinessRuleDefinition> GetCachedBPBusinessRulesById()
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