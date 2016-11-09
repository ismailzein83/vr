﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Rules;

namespace Vanrise.Notification.Business
{
    public abstract class VRGenericAlertRuleManager
    {
        #region Public Methods

        public VRAlertRule GetMatchRule(Guid ruleTypeId, GenericRuleTarget target)
        {
            var cachedPreparedData = GetCachedPreparedData(ruleTypeId);
            AlertRuleAsGeneric matchGenericRule = GenericRuleManager<GenericRule>.GetMatchRule<AlertRuleAsGeneric>(cachedPreparedData.RuleTree, cachedPreparedData.CriteriaEvaluationInfos, target);
            return matchGenericRule != null ? matchGenericRule.OriginalAlertRule : null;
        }

        #endregion

        #region Private Methods

        private struct GetCachedPreparedDataCacheName
        {
            public Guid RuleTypeId { get; set; }
        }

        CachedPreparedData GetCachedPreparedData(Guid ruleTypeId)
        {
            var cacheName = new GetCachedPreparedDataCacheName { RuleTypeId = ruleTypeId };
            return GetCachedOrCreate(cacheName, () =>
            {
                VRAlertRuleType alertRuleType = null;
                if (alertRuleType.Settings == null)
                    throw new NullReferenceException("alertRuleType.Settings");
                VRGenericAlertRuleTypeSettings ruleTypeSettingsAsGeneric = alertRuleType.Settings as VRGenericAlertRuleTypeSettings;
                if (ruleTypeSettingsAsGeneric == null)
                    throw new Exception(String.Format("alertRuleType.Settings is not of type VRGenericAlertRuleTypeSettings. it is of type '{0}'", alertRuleType.Settings.GetType()));
                var criteriaDefinition = ruleTypeSettingsAsGeneric.CriteriaDefinition;
                if (criteriaDefinition == null)
                    throw new NullReferenceException(String.Format("criteriaDefinition. ruleTypeId '{0}'", ruleTypeId));
                var objects = ruleTypeSettingsAsGeneric.Objects;
                List<VRAlertRule> alertRules = null;
                var ruleTree = GenericRuleManager<GenericRule>.BuildRuleTree<AlertRuleAsGeneric>(criteriaDefinition, alertRules.Select(r => new AlertRuleAsGeneric { OriginalAlertRule = r }));
                var criteriaEvaluationInfos = GenericRuleManager<GenericRule>.BuildCriteriaEvaluationInfos(criteriaDefinition, objects);
                return new CachedPreparedData
                {
                    RuleTree = ruleTree,
                    CriteriaEvaluationInfos = criteriaEvaluationInfos
                };
            });
        }

        T GetCachedOrCreate<T>(object cacheName, Func<T> createObject)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, createObject);
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                //TODO
                throw new NotImplementedException();
            }
        }

        private class CachedPreparedData
        {
            public RuleTree RuleTree { get; set; }

            public List<CriteriaEvaluationInfo> CriteriaEvaluationInfos { get; set; }
        }


        private class AlertRuleAsGeneric : IVRRule, IGenericRule
        {
            public VRAlertRule OriginalAlertRule { get; set; }

            public GenericRuleCriteria Criteria
            {
                get
                {
                    if (this.OriginalAlertRule == null)
                        throw new NullReferenceException("OriginalAlertRule");
                    if (this.OriginalAlertRule.Settings == null)
                        throw new NullReferenceException("OriginalAlertRule.Settings");
                    if (this.OriginalAlertRule.Settings.ExtendedSettings == null)
                        throw new NullReferenceException("OriginalAlertRule.Settings.ExtendedSettings");
                    VRGenericAlertRuleExtendedSettings alertRuleSettingsAsGeneric = this.OriginalAlertRule.Settings.ExtendedSettings as VRGenericAlertRuleExtendedSettings;
                    if (alertRuleSettingsAsGeneric == null)
                        throw new NullReferenceException(String.Format("OriginalAlertRule.Settings.ExtendedSettings is not of type VRGenericAlertRuleExtendedSettings. it is of type '{0}'", this.OriginalAlertRule.Settings.ExtendedSettings.GetType()));
                    return alertRuleSettingsAsGeneric.Criteria;
                }
            }

            public bool IsAnyCriteriaExcluded(object target)
            {
                return false;
            }

            public DateTime BeginEffectiveTime
            {
                get
                {
                    return DateTime.MinValue;
                }
                set
                {

                }
            }

            public DateTime? EndEffectiveTime
            {
                get
                {
                    return null;
                }
                set
                {

                }
            }

            public DateTime? LastRefreshedTime
            {
                get;
                set;
            }

            public TimeSpan RefreshTimeSpan
            {
                get { return TimeSpan.MaxValue; }
            }

            public void RefreshRuleState(IRefreshRuleStateContext context)
            {

            }
        }

        #endregion
    }
}
