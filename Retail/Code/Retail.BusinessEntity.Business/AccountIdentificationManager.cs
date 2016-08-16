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


        public Vanrise.Entities.IDataRetrievalResult<AccountIdentificationDetail> GetFilteredAccountIdentificationRules(Vanrise.Entities.DataRetrievalInput<AccountIdentificationQuery> input)
        {
            IEnumerable<MappingRule> mappingRules = this.GetAccountMappingRules(input.Query.AccountId);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, mappingRules.ToBigResult(input, null, AccountIdentificationMapper));
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
            var accountMappingRuleDefinitions = GetAccountMappingRuleDefinitions();
            if (accountMappingRuleDefinitions == null)
                return new HashSet<int>();
            return new HashSet<int>(accountMappingRuleDefinitions.Select(itm => itm.GenericRuleDefinitionId));
        }

        private static IEnumerable<GenericRuleDefinition> GetAccountMappingRuleDefinitions()
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            return ruleDefinitionManager.GetGenericRulesDefinitons().FindAllRecords(ruleDefinition => AccountMappingRuleDefinitionFilter.IsAccountIdentificationRuleDefinition(ruleDefinition));
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

        #region Private Mappers

        AccountIdentificationDetail AccountIdentificationMapper(MappingRule mappingRule)
        {
            GenericRuleManager<MappingRule> genericRuleManager = new GenericRuleManager<MappingRule>();
            GenericRuleDetail genericRuleDetail = genericRuleManager.MapToDetails(mappingRule);
            string criteria = "";
            List<string> criteriaFieldsValues= new List<string>();

            GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition genericRuleDefiniton = genericRuleDefinitionManager.GetGenericRuleDefinition(mappingRule.DefinitionId);

            GenericRuleDefinitionCriteria criteriaFields = genericRuleDefiniton.CriteriaDefinition;


            for (int i = 0; i < genericRuleDetail.FieldValueDescriptions.Count(); i++)
            {
                string fieldValueDescription = genericRuleDetail.FieldValueDescriptions[i];
                if (!string.IsNullOrEmpty(fieldValueDescription))
                {
                    criteriaFieldsValues.Add(string.Concat(criteriaFields.Fields[i].FieldName, ":", fieldValueDescription));
                }
            }

           criteria =string.Join<string>(" ,", criteriaFieldsValues);

            return new AccountIdentificationDetail()
            {
                GenericRuleId = mappingRule.RuleId,
                GenericRuleDefinitionId = mappingRule.DefinitionId,
                BED = mappingRule.BeginEffectiveTime,
                EED = mappingRule.EndEffectiveTime,
                Criteria = criteria,
                Description = mappingRule.Description
            };
        }


        #endregion


    }
}
