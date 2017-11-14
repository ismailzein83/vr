﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Rules.Data;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules
{
    public abstract class RuleManager<T> : RuleManager<T, T> where T : BaseRule
    {
        public override T MapToDetails(T rule)
        {
            return rule;
        }
    }

    public abstract class RuleManager<T, Q>
        where T : BaseRule
        where Q : class
    {
        #region Public Methods

        public Vanrise.Entities.InsertOperationOutput<Q> AddRule(T rule)
        {
            InsertOperationOutput<Q> insertOperationOutput = new InsertOperationOutput<Q>();

            if (TryAdd(rule))
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                GetCacheManager().SetCacheExpired(GetRuleTypeId());
                TrackAndLogRuleAdded(rule);

                insertOperationOutput.InsertedObject = MapToDetails(rule);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public bool TryAdd(T rule)
        {
            int ruleTypeId = GetRuleTypeId();
            Entities.Rule ruleEntity = new Entities.Rule
            {
                TypeId = ruleTypeId,
                RuleDetails = Serializer.Serialize(rule),
                BED = rule.BeginEffectiveTime,
                EED = rule.EndEffectiveTime,
            };

            if (ValidateBeforeAdd(rule))
            {
                int ruleId;
                IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();

                if (rule.HasAdditionalInformation)
                {
                    if (ruleDataManager.AddRuleAndRuleChanged(ruleEntity, ActionType.AddedRule, out ruleId))
                    {
                        rule.RuleId = ruleId;
                        return true;
                    }
                }
                else if (ruleDataManager.AddRule(ruleEntity, out ruleId))
                {
                    rule.RuleId = ruleId;
                    return true;
                }
            }
            return false;
        }

        public virtual bool ValidateBeforeAdd(T rule)
        {
            return true;
        }

        protected virtual void TrackAndLogRuleAdded(T rule)
        {
            Vanrise.Common.BusinessManagerFactory.GetManager<IVRActionLogger>().TrackAndLogObjectAdded(GetLoggableEntity(rule), rule);
        }

        public Vanrise.Entities.UpdateOperationOutput<Q> UpdateRule(T rule)
        {
            UpdateOperationOutput<Q> updateOperationOutput = new UpdateOperationOutput<Q>();
            if (TryUpdateRule(rule))
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                GetCacheManager().SetCacheExpired(GetRuleTypeId());
                TrackAndLogRuleUpdated(rule);
                updateOperationOutput.UpdatedObject = MapToDetails(rule);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public bool TryUpdateRule(T rule)
        {
            int ruleTypeId = GetRuleTypeId();
            Entities.Rule ruleEntity = new Entities.Rule
            {
                RuleId = rule.RuleId,
                TypeId = ruleTypeId,
                RuleDetails = Serializer.Serialize(rule),
                EED = rule.EndEffectiveTime,
                BED = rule.BeginEffectiveTime
            };

            if (ValidateBeforeUpdate(rule))
            {
                IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();

                if (rule.HasAdditionalInformation)
                {
                    AdditionalInformation additionalInformation = null;
                    string serializedRule = null;

                    T existingRule = this.GetRule(rule.RuleId);
                    RuleChangedData<T> ruleChanged = this.GetRuleChanged(rule.RuleId);

                    bool ruleAdded = false;
                    if (ruleChanged != null)
                    {
                        ruleAdded = ruleChanged.ActionType == ActionType.AddedRule;
                        additionalInformation = ruleChanged.AdditionalInformation;
                    }
                    else
                    {
                        serializedRule = Vanrise.Common.Serializer.Serialize(existingRule);
                    }

                    if (ruleAdded)
                    {
                        if (ruleDataManager.UpdateRule(ruleEntity))
                            return true;
                    }
                    else
                    {
                        rule.UpdateAdditionalInformation(existingRule, ref additionalInformation);
                        string serializedAdditionalInformation = Vanrise.Common.Serializer.Serialize(additionalInformation);

                        if (ruleDataManager.UpdateRuleAndRuleChanged(ruleEntity, ActionType.UpdatedRule, serializedRule, serializedAdditionalInformation))
                            return true;
                    }

                }
                else if (ruleDataManager.UpdateRule(ruleEntity))
                {
                    return true;
                }
            }
            return false;
        }

        public RuleChangedData<T> GetRuleChanged(int ruleId)
        {
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            return RuleChangedDataMapper(ruleDataManager.GetRuleChanged(ruleId, GetRuleTypeId()));
        }

        public List<RuleChangedData<T>> GetRulesChanged()
        {
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            List<RuleChanged> ruleChangedList = ruleDataManager.GetRulesChanged(GetRuleTypeId());
            if (ruleChangedList == null)
                return null;

            List<RuleChangedData<T>> result = new List<RuleChangedData<T>>();
            foreach (RuleChanged ruleChanged in ruleChangedList)
            {
                result.Add(RuleChangedDataMapper(ruleChanged));
            }
            return result;
        }

        public void DeleteRuleChanged(int ruleId)
        {
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            ruleDataManager.DeleteRuleChanged(ruleId, GetRuleTypeId());
        }

        public void DeleteRulesChanged()
        {
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            ruleDataManager.DeleteRulesChanged(GetRuleTypeId());
        }

        private RuleChangedData<T> RuleChangedDataMapper(RuleChanged ruleChanged)
        {
            if (ruleChanged == null)
                return null;

            return new RuleChangedData<T>()
            {
                ActionType = ruleChanged.ActionType,
                AdditionalInformation = !string.IsNullOrEmpty(ruleChanged.AdditionalInformation) ? Vanrise.Common.Serializer.Deserialize<AdditionalInformation>(ruleChanged.AdditionalInformation) : null,
                CreatedTime = ruleChanged.CreatedTime,
                InitialRule = !string.IsNullOrEmpty(ruleChanged.InitialRule) ? Vanrise.Common.Serializer.Deserialize<T>(ruleChanged.InitialRule) : null,
                RuleChangedId = ruleChanged.RuleChangedId,
                RuleId = ruleChanged.RuleId,
                RuleTypeId = ruleChanged.RuleTypeId
            };
        }

        public virtual bool ValidateBeforeUpdate(T rule)
        {
            return true;
        }

        protected virtual void TrackAndLogRuleUpdated(T rule)
        {
            Vanrise.Common.BusinessManagerFactory.GetManager<IVRActionLogger>().TrackAndLogObjectUpdated(GetLoggableEntity(rule), rule);
        }

        public Vanrise.Entities.DeleteOperationOutput<Q> DeleteRule(int ruleId)
        {
            DeleteOperationOutput<Q> deleteOperationOutput = new DeleteOperationOutput<Q>();
            IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
            T rule = GetRule(ruleId);

            if (ruleDataManager.DeleteRule(ruleId))
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
                int ruleTypeId = GetRuleTypeId();
                GetCacheManager().SetCacheExpired(ruleTypeId);
                if (rule != null)
                    TrackAndLogRuleDeleted(rule);

            }

            return deleteOperationOutput;
        }

        protected virtual void TrackAndLogRuleDeleted(T rule)
        {
            Vanrise.Common.BusinessManagerFactory.GetManager<IVRActionLogger>().TrackAndLogObjectDeleted(GetLoggableEntity(rule), rule);
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

        public T GetRule(int ruleId, bool isViewedFromUI)
        {
            var allRules = GetAllRules();
            T rule;
            if (allRules != null && allRules.TryGetValue(ruleId, out rule))
            {
                if (isViewedFromUI)
                    LogRuleViewed(rule);
                return rule;
            }
            else
                return null;
        }

        protected virtual void LogRuleViewed(T rule)
        {
            Vanrise.Common.BusinessManagerFactory.GetManager<IVRActionLogger>().LogObjectViewed(GetLoggableEntity(rule), rule);
        }

        public T GetRule(int ruleId)
        {
            return GetRule(ruleId, false);
        }

        public Q GetRuleDetail(int ruleId)
        {
            var rule = GetRule(ruleId);
            if (rule != null)
                return MapToDetails(rule);
            else
                return null;
        }

        public abstract Q MapToDetails(T rule);

        public abstract RuleLoggableEntity GetLoggableEntity(T rule);

        #endregion

        #region Private Methods

        static VRDictionary<string, int> s_ruleTypesIds = new VRDictionary<string, int>(true);
        static VRDictionary<Type, string> s_ruleTypes = new VRDictionary<Type, string>(true);

        public int GetRuleTypeId()
        {
            string ruleType;
            if (!s_ruleTypes.TryGetValue(typeof(T), out ruleType))
            {
                s_ruleTypes.TryAdd(typeof(T), typeof(T).FullName);
                ruleType = s_ruleTypes[typeof(T)];
            }
            int ruleTypeId;
            if (!s_ruleTypesIds.TryGetValue(ruleType, out ruleTypeId))
            {
                IRuleDataManager ruleDataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
                ruleTypeId = ruleDataManager.GetRuleTypeId(ruleType);
                s_ruleTypesIds.TryAdd(ruleType, ruleTypeId);
            }

            return ruleTypeId;
        }

        #endregion

        #region Caching

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();
        CacheManager GetCacheManager()
        {
            return s_cacheManager;
        }

        protected R GetCachedOrCreate<R>(Object cacheName, Func<R> createObject)
        {
            return GetCacheManager().GetOrCreateObject(cacheName, GetRuleTypeId(), createObject);
        }

        public bool IsCacheExpired(ref DateTime? lastCheckTime)
        {
            return GetCacheManager().IsCacheExpired(GetRuleTypeId(), ref lastCheckTime);
        }

        public void AddRuleCachingExpirationChecker(RuleCachingExpirationChecker ruleCachingExpirationChecker)
        {
            lock (GetCacheManager().ruleCachingExpirationCheckerDict)
            {
                GetCacheManager().ruleCachingExpirationCheckerDict.Add(GetRuleTypeId(), ruleCachingExpirationChecker);
            }
        }

        #endregion
    }

    public abstract class RuleLoggableEntity : VRLoggableEntityBase
    {
        public override string ModuleName
        {
            get { return "Rules"; }
        }

        public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
        {
            BaseRule rule = context.Object.CastWithValidate<BaseRule>("context.Object");
            return rule.RuleId;
        }

        public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
        {
            BaseRule rule = context.Object.CastWithValidate<BaseRule>("context.Object");
            return !String.IsNullOrWhiteSpace(rule.Description) ? rule.Description : rule.RuleId.ToString();
        }
    }

    public class CacheManager : Vanrise.Caching.BaseCacheManager<int>
    {
        public CacheManager()
        {
            Vanrise.Common.LoggerFactory.GetLogger().WriteInformation("CacheManager");
        }

        IRuleDataManager _dataManager = RuleDataManagerFactory.GetDataManager<IRuleDataManager>();
        ConcurrentDictionary<int, Object> _updateHandlesByRuleType = new ConcurrentDictionary<int, Object>();
        public VRDictionary<int, RuleCachingExpirationChecker> ruleCachingExpirationCheckerDict = new VRDictionary<int, RuleCachingExpirationChecker>(true);


        protected override bool ShouldSetCacheExpired(int parameter)
        {
            Object updateHandle;

            _updateHandlesByRuleType.TryGetValue(parameter, out updateHandle);
            bool isCacheExpired = _dataManager.AreRulesUpdated(parameter, ref updateHandle);
            _updateHandlesByRuleType.AddOrUpdate(parameter, updateHandle, (key, existingHandle) => updateHandle);

            RuleCachingExpirationChecker ruleCachingExpirationChecker;
            bool isRuleDependenciesCacheExpired = false;
            if (ruleCachingExpirationCheckerDict.TryGetValue(parameter, out ruleCachingExpirationChecker))
            {
                isRuleDependenciesCacheExpired = ruleCachingExpirationChecker.IsRuleDependenciesCacheExpired();
            }

            return isCacheExpired || isRuleDependenciesCacheExpired;
        }

        private class RuleTypeUpdateHandle
        {
            public object UpdateHandle { get; set; }
        }
    }

    public abstract class RuleCachingExpirationChecker
    {
        public abstract bool IsRuleDependenciesCacheExpired();
    }

    public abstract class AdditionalInformation
    {
    }
}
