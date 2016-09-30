﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business.BELookupRules.RuleStructureBehaviors;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Vanrise.GenericData.Business
{
    public class BELookupRuleManager
    {
        public dynamic GetMatchBE(Guid beLookupRuleDefinitionId, GenericRuleTarget ruleTarget)
        {
            RuleTree ruleTree = GetRuleTree(beLookupRuleDefinitionId);
            BELookupRule matchRule = ruleTree.GetMatchRule(ruleTarget) as BELookupRule;
            if (matchRule != null)
                return matchRule.BusinessEntityObject;
            else
                return null;
        }

        private RuleTree GetRuleTree(Guid beLookupRuleDefinitionId)
        {
            BELookupRuleDefinition beLookupRuleDefinition = (new BELookupRuleDefinitionManager()).GetBELookupRuleDefinition(beLookupRuleDefinitionId);
            if(beLookupRuleDefinition == null)
                throw new NullReferenceException(String.Format("beLookupRuleDefinition '{0}'", beLookupRuleDefinitionId));
           
            string cacheName = String.Format("GetRuleTree_{0}", beLookupRuleDefinitionId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, beLookupRuleDefinition.BusinessEntityDefinitionId,
                () =>
                {                    
                    List<dynamic> allEntities = (new BusinessEntityManager()).GetAllEntities(beLookupRuleDefinition.BusinessEntityDefinitionId);
                    List<BELookupRule> beLookupRules = new List<BELookupRule>();
                    if (allEntities != null)
                    {
                        foreach (var entity in allEntities)
                        {
                            beLookupRules.Add(new BELookupRule
                            {
                                RuleDefinition = beLookupRuleDefinition,
                                BusinessEntityObject = entity
                            });
                        }
                    }
                    List<BaseRuleStructureBehavior> ruleStructureBehaviors = new List<BaseRuleStructureBehavior>();
                    foreach (var ruleDefinitionCriteriaField in beLookupRuleDefinition.CriteriaFields)
                    {
                        BaseRuleStructureBehavior ruleStructureBehavior = CreateRuleStructureBehavior(ruleDefinitionCriteriaField);
                        ruleStructureBehaviors.Add(ruleStructureBehavior);
                    }
                    return new RuleTree(beLookupRules, ruleStructureBehaviors);
                });           
        }

        private BaseRuleStructureBehavior CreateRuleStructureBehavior(BELookupRuleDefinitionCriteriaField ruleDefinitionCriteriaField)
        {
            IBELookupRuleStructureBehavior behavior = null;
            switch (ruleDefinitionCriteriaField.RuleStructureBehaviorType)
            {
                case MappingRuleStructureBehaviorType.ByKey: behavior = new BELookupRuleStructureBehaviorByKey(); break;
                case MappingRuleStructureBehaviorType.ByPrefix: behavior = new BELookupRuletructureBehaviorByPrefix(); break;
            }
            behavior.FieldPath = ruleDefinitionCriteriaField.FieldPath;
            return behavior as BaseRuleStructureBehavior;
        }

        public static Object GetRuleBEFieldValue(BELookupRule rule, string fieldPath)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");
            GenericBusinessEntity genericBE = rule.BusinessEntityObject as GenericBusinessEntity;
            if (genericBE == null)
                throw new NullReferenceException("genericBE");
            var dataRecord = genericBE.Details;
            if (dataRecord == null)
                throw new NullReferenceException("dataRecord");
            Type beType = dataRecord.GetType();
            string[] propertyParts = fieldPath.Split('.');
            Object value = dataRecord;
            foreach (var propPart in propertyParts)
            {
                value = value.GetType().GetProperty(propPart).GetValue(value);
                if (value == null)
                    break;
            }
            return value;
        }

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            DateTime? _beLookupRuleDefinitionCacheLastCheck;
            DateTime? _businessEntityDefinitionCacheLastCheck;

            ConcurrentDictionary<Guid, BEDefinitionCacheItem> _beDefinitionCacheItems = new ConcurrentDictionary<Guid, BEDefinitionCacheItem>();

            protected override bool ShouldSetCacheExpired(Guid businessEntityDefinitionId)
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<BELookupRuleDefinitionManager.CacheManager>().IsCacheExpired(ref _beLookupRuleDefinitionCacheLastCheck)
                    | Vanrise.Caching.CacheManagerFactory.GetCacheManager<BusinessEntityDefinitionManager.CacheManager>().IsCacheExpired(ref _businessEntityDefinitionCacheLastCheck)
                    | AreBEsChanged(businessEntityDefinitionId);
            }

            private bool AreBEsChanged(Guid businessEntityDefinitionId)
            {
                BEDefinitionCacheItem beDefinitionCacheItem;
                if (!_beDefinitionCacheItems.TryGetValue(businessEntityDefinitionId, out beDefinitionCacheItem))
                {
                    var beManager = (new BusinessEntityDefinitionManager()).GetBusinessEntityManager(businessEntityDefinitionId);
                    if (beManager == null)
                        throw new NullReferenceException(String.Format("beManager. businessEntityDefinitionId '{0}'", businessEntityDefinitionId));
                    _beDefinitionCacheItems.TryAdd(businessEntityDefinitionId, new BEDefinitionCacheItem
                    {
                        BEManager = beManager
                    });
                    _beDefinitionCacheItems.TryGetValue(businessEntityDefinitionId, out beDefinitionCacheItem);
                }
                DateTime? lastCheckTime = beDefinitionCacheItem.LastCheckTime;
                bool isCacheExpired = beDefinitionCacheItem.BEManager.IsCacheExpired(new BusinessEntityIsCacheExpiredContext(businessEntityDefinitionId), ref lastCheckTime);
                beDefinitionCacheItem.LastCheckTime = lastCheckTime;
                return isCacheExpired;
            }

            private class BEDefinitionCacheItem
            {
                public IBusinessEntityManager BEManager { get; set; }

                public DateTime? LastCheckTime { get; set; }
            }
        }

        #endregion
    }    
}
