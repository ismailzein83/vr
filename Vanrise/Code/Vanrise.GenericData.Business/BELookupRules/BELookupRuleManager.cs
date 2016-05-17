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
        public dynamic GetMatchBE(int beLookupRuleDefinitionId, GenericRuleTarget ruleTarget)
        {
            RuleTree ruleTree = GetRuleTree(beLookupRuleDefinitionId);
            BELookupRule matchRule = ruleTree.GetMatchRule(ruleTarget) as BELookupRule;
            if (matchRule != null)
                return matchRule.BusinessEntityObject;
            else
                return null;
        }

        private RuleTree GetRuleTree(int beLookupRuleDefinitionId)
        {
            BELookupRuleDefinition beLookupRuleDefinition = GetRuleDefinition(beLookupRuleDefinitionId);
            if(beLookupRuleDefinition == null)
                throw new NullReferenceException(String.Format("beLookupRuleDefinition '{0}'", beLookupRuleDefinitionId));
            string cacheName = String.Format("GetRuleTree_{0}", beLookupRuleDefinitionId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, beLookupRuleDefinition.BusinessEntityDefinitionId,
                () =>
                {
                    var beManager = (new BusinessEntityDefinitionManager()).GetBusinessEntityManager(beLookupRuleDefinition.BusinessEntityDefinitionId);
                    if (beManager == null)
                        throw new NullReferenceException(String.Format("beManager. BusinessEntityDefinitionId '{0}'", beLookupRuleDefinition.BusinessEntityDefinitionId));

                    var getAllEntitiesContext = new BusinessEntityGetAllContext(beLookupRuleDefinition.BusinessEntityDefinitionId);
                    List<dynamic> allEntities = beManager.GetAllEntities(getAllEntitiesContext);
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

        private BELookupRuleDefinition GetRuleDefinition(int beLookupRuleDefinitionId)
        {
            var ruleDefinition = new BELookupRuleDefinition
            {
                 BusinessEntityDefinitionId = 1,
                  CriteriaFields = new List<BELookupRuleDefinitionCriteriaField>()
            };
            //ruleDefinition.CriteriaFields.Add(new BELookupRuleDefinitionCriteriaField
            //    {
            //        FieldPath = "Settings.Company",
            //        RuleStructureBehaviorType = MappingRuleStructureBehaviorType.ByPrefix
            //    });

            //ruleDefinition.CriteriaFields.Add(new BELookupRuleDefinitionCriteriaField
            //{
            //    FieldPath = "Settings.CountryId",
            //    RuleStructureBehaviorType = MappingRuleStructureBehaviorType.ByKey
            //});


            ruleDefinition.CriteriaFields.Add(new BELookupRuleDefinitionCriteriaField
            {
                FieldPath = "Settings.PhoneNumbers",
                RuleStructureBehaviorType = MappingRuleStructureBehaviorType.ByPrefix
            });
            return ruleDefinition;
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
            Type beType = rule.BusinessEntityObject.GetType();
            string[] propertyParts = fieldPath.Split('.');
            Object value = rule.BusinessEntityObject;
            foreach (var propPart in propertyParts)
            {
                value = value.GetType().GetProperty(propPart).GetValue(value);
                if (value == null)
                    break;
            }
            return value;
        }

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager<int>
        {
            DateTime? _beLookupRuleDefinitionCacheLastCheck;
            DateTime? _businessEntityDefinitionCacheLastCheck;

            ConcurrentDictionary<int, BEDefinitionCacheItem> _beDefinitionCacheItems = new ConcurrentDictionary<int, BEDefinitionCacheItem>();

            protected override bool ShouldSetCacheExpired(int businessEntityDefinitionId)
            {
                if (Vanrise.Caching.CacheManagerFactory.GetCacheManager<BELookupRuleDefinitionManager.CacheManager>().IsCacheExpired(ref _beLookupRuleDefinitionCacheLastCheck)
                    || Vanrise.Caching.CacheManagerFactory.GetCacheManager<BusinessEntityDefinitionManager.CacheManager>().IsCacheExpired(ref _businessEntityDefinitionCacheLastCheck))
                    return true;

                BEDefinitionCacheItem beDefinitionCacheItem;
                if(!_beDefinitionCacheItems.TryGetValue(businessEntityDefinitionId, out beDefinitionCacheItem))
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
