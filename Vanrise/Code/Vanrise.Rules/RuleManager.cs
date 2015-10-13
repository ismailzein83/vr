using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Rules.Data;

namespace Vanrise.Rules
{
    public class RuleManager<T> where T : BaseRule
    {
        public Vanrise.Entities.InsertOperationOutput<T> AddRule(T rule)
        {
            int ruleTypeId = GetRuleTypeId();
            Entities.Rule ruleEntity = new Entities.Rule
            {
                TypeId = ruleTypeId,
                RuleDetails = Serializer.Serialize(rule)
            };
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            InsertOperationOutput<T> insertOperationOutput = new InsertOperationOutput<T>();
            int ruleId;
            if (ruleDataManager.AddRule(ruleEntity, out ruleId))
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                rule.RuleId = ruleId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(ruleTypeId);
                insertOperationOutput.InsertedObject = rule;
            }
            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<T> UpdateRule(T rule)
        {
            int ruleTypeId = GetRuleTypeId();
            Entities.Rule ruleEntity = new Entities.Rule
            {
                RuleId = rule.RuleId,
                TypeId = ruleTypeId,
                RuleDetails = Serializer.Serialize(rule)
            };
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            UpdateOperationOutput<T> updateOperationOutput = new UpdateOperationOutput<T>();
            if (ruleDataManager.UpdateRule(ruleEntity))
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(ruleTypeId);
                updateOperationOutput.UpdatedObject = rule;
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<T> DeleteRule(int ruleId)
        {
            DeleteOperationOutput<T> deleteOperationOutput = new DeleteOperationOutput<T>();
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();

            if (ruleDataManager.DeleteRule(ruleId))
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
                int ruleTypeId = GetRuleTypeId();
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(ruleTypeId);
            }

            return deleteOperationOutput;
        }

        public T GetRule(int ruleId)
        {
            var allRules = GetAllRules();
            T rule;
            if (allRules != null && allRules.TryGetValue(ruleId, out rule))
                return rule;
            else
                return null;
        }

        public IEnumerable<T> GetFilteredRules(Func<T, bool> filter)
        {
            var allRules = GetAllRules();
            if (allRules == null)
                return null;
            else
                return allRules.Values.FindAllRecords(filter);
        }

        public Dictionary<int, T> GetAllRules()
        {
            int ruleTypeId = GetRuleTypeId();
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedRules", ruleTypeId,
               () =>
               {
                   
                   IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
                   IEnumerable<Entities.Rule> ruleEntities = ruleDataManager.GetRulesByType(ruleTypeId);
                   Dictionary<int, T> rules = new Dictionary<int, T>();
                   if (ruleEntities != null)
                   {
                       foreach (var ruleEntity in ruleEntities)
                       {
                           T rule = Serializer.Deserialize<T>(ruleEntity.RuleDetails);
                           rule.RuleId = ruleEntity.RuleId;
                           rules.Add(rule.RuleId, rule);
                       }
                   }
                   return rules;
               });
        }

        #region Private Methods
        
        private int GetRuleTypeId()
        {
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            return ruleDataManager.GetRuleTypeId(typeof(T).FullName);
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager<int>
        {
            IRuleDataManager _dataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            ConcurrentDictionary<int, RuleTypeUpdateHandle> _updateHandlesByRuleType = new ConcurrentDictionary<int, RuleTypeUpdateHandle>();

            protected override bool ShouldSetCacheExpired(int parameter)
            {
                RuleTypeUpdateHandle ruleTypeUpdateHandle;
                if(!_updateHandlesByRuleType.TryGetValue(parameter, out ruleTypeUpdateHandle))
                {
                    ruleTypeUpdateHandle = new RuleTypeUpdateHandle ();
                    if (!_updateHandlesByRuleType.TryAdd(parameter, ruleTypeUpdateHandle))
                        ruleTypeUpdateHandle = _updateHandlesByRuleType[parameter];
                }
                object updateHandle = ruleTypeUpdateHandle.UpdateHandle;
                bool isCacheExpired = _dataManager.AreRulesUpdated(parameter, ref updateHandle);
                ruleTypeUpdateHandle.UpdateHandle = updateHandle;
                return isCacheExpired;
            }

            private class RuleTypeUpdateHandle
            {
                public object UpdateHandle { get; set; }
            }
        }

        #endregion
    }
}
