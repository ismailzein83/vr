using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.GenericData.Transformation;

namespace Retail.BusinessEntity.Business
{
    public class AccountIdentificationManager
    {
        public IEnumerable<MappingRule> GetAccountMappingRules(long accountId)
        {
            return GetAllAccountsMappingRulesByAccountId().GetRecord(accountId);
        }

        #region PrivateMethods

        private Dictionary<long, List<MappingRule>> GetAllAccountsMappingRulesByAccountId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllAccountsMappingRulesByAccountId",
                () =>
                {
                    HashSet<int> accountMappingRuleDefinitionIds = GetAccountMappingRuleDefinitionsIds();
                    MappingRuleManager mappingRuleManager = new MappingRuleManager();
                    var allRules = mappingRuleManager.GetAllRules().FindAllRecords(itm => accountMappingRuleDefinitionIds.Contains(itm.DefinitionId));
                    if (allRules != null)
                    {
                        Dictionary<long, List<MappingRule>> rulesByAccountIds = new Dictionary<long, List<MappingRule>>();
                        foreach (var rule in allRules)
                        {
                            if (rule.Settings == null)
                                throw new NullReferenceException(string.Format("rule.Settings. ruleId '{0}'", rule.RuleId));
                            List<MappingRule> matchAccountRules = rulesByAccountIds.GetOrCreateItem(Convert.ToInt64(rule.Settings.Value));
                            matchAccountRules.Add(rule);
                        }
                        return rulesByAccountIds;
                    }
                    else
                        return null;
                });           
        }        

        HashSet<int> GetAccountMappingRuleDefinitionsIds()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            var subscriberAccountBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(Account.BUSINESSENTITY_DEFINITION_NAME);
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            var allMappingRuleDefinitions = ruleDefinitionManager.GetGenericRuleDefinitionsByType(MappingRule.RULE_DEFINITION_TYPE_NAME);
            var accountMappingRuleDefinitions = allMappingRuleDefinitions.FindAllRecords(itm =>
            {
                var mappingRuleDefinitionSettings = itm.SettingsDefinition as MappingRuleDefinitionSettings;
                if (mappingRuleDefinitionSettings != null)
                {
                    var businessEntityFieldType = mappingRuleDefinitionSettings.FieldType as Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType;
                    if (businessEntityFieldType != null)
                        return businessEntityFieldType.BusinessEntityDefinitionId == subscriberAccountBEDefinitionId;
                }
                return false;
            });
            if (accountMappingRuleDefinitions == null)
                return new HashSet<int>();
            return new HashSet<int>(accountMappingRuleDefinitions.Select(itm => itm.GenericRuleDefinitionId));
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _ruleDefinitionCacheLastCheck;
            DateTime? _mappingRuleCacheLastCheck;
            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<GenericRuleDefinitionManager.CacheManager>().IsCacheExpired(ref _ruleDefinitionCacheLastCheck)
                    |
                    mappingRuleManager.IsCacheExpired(ref _mappingRuleCacheLastCheck);
            }
        }

        #endregion
    }
}
