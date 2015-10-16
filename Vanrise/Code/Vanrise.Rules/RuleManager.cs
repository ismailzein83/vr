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
    public abstract class RuleManager<T> : RuleManager<T, T> where T : BaseRule
    {
        protected override T MapRuleTODetails(T rule)
        {
            return rule;
        }
    }
    public abstract class RuleManager<T, Q> where T : BaseRule
    {
        protected abstract Q MapRuleTODetails(T rule);
        public Vanrise.Entities.InsertOperationOutput<Q> AddRule(T rule)
        {
            int ruleTypeId = GetRuleTypeId();
            Entities.Rule ruleEntity = new Entities.Rule
            {
                TypeId = ruleTypeId,
                RuleDetails = Serializer.Serialize(rule)
            };
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            InsertOperationOutput<Q> insertOperationOutput = new InsertOperationOutput<Q>();
            int ruleId;
            if (ruleDataManager.AddRule(ruleEntity, out ruleId))
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                rule.RuleId = ruleId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(ruleTypeId);
                insertOperationOutput.InsertedObject = MapRuleTODetails(rule);
            }
            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<Q> UpdateRule(T rule)
        {
            int ruleTypeId = GetRuleTypeId();
            Entities.Rule ruleEntity = new Entities.Rule
            {
                RuleId = rule.RuleId,
                TypeId = ruleTypeId,
                RuleDetails = Serializer.Serialize(rule)
            };
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            UpdateOperationOutput<Q> updateOperationOutput = new UpdateOperationOutput<Q>();
            if (ruleDataManager.UpdateRule(ruleEntity))
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(ruleTypeId);
                updateOperationOutput.UpdatedObject =  MapRuleTODetails(rule);
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<Q> DeleteRule(int ruleId)
        {
            DeleteOperationOutput<Q> deleteOperationOutput = new DeleteOperationOutput<Q>();
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
            return GetCachedOrCreate("GetCachedRules",
               () =>
               {
                   
                   IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
                   IEnumerable<Entities.Rule> ruleEntities = ruleDataManager.GetRulesByType(GetRuleTypeId());
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

        protected R GetCachedOrCreate<R>(string cacheName, Func<R> createObject)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, GetRuleTypeId(), createObject);
        }

        #region Private Methods

        static ConcurrentDictionary<string, int> s_ruleTypesIds = new ConcurrentDictionary<string, int>();

        private int GetRuleTypeId()
        {
            string ruleType = typeof(T).FullName;
            int ruleTypeId;
            if(!s_ruleTypesIds.TryGetValue(ruleType, out ruleTypeId))
            {
                IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
                ruleTypeId = ruleDataManager.GetRuleTypeId(ruleType);
                s_ruleTypesIds.TryAdd(ruleType, ruleTypeId);
            }

            return ruleTypeId;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager<int>
        {
            IRuleDataManager _dataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            ConcurrentDictionary<int, Object> _updateHandlesByRuleType = new ConcurrentDictionary<int, Object>();

            protected override bool ShouldSetCacheExpired(int parameter)
            {
                Object updateHandle;
                _updateHandlesByRuleType.TryGetValue(parameter, out updateHandle);
                bool isCacheExpired = _dataManager.AreRulesUpdated(parameter, ref updateHandle);
                _updateHandlesByRuleType.AddOrUpdate(parameter, updateHandle, (key, existingHandle) => updateHandle);
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
