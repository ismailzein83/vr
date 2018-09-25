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
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountIdentificationDetail> GetFilteredAccountIdentificationRules(Vanrise.Entities.DataRetrievalInput<AccountIdentificationQuery> input)
        {
            IEnumerable<MappingRule> mappingRules = this.GetAccountMappingRules(input.Query.AccountBEDefinitionId, input.Query.AccountId);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, mappingRules.ToBigResult(input, null, AccountIdentificationMapper));
        }

        public IEnumerable<MappingRule> GetAccountMappingRules(Guid accountBEDefinitionId, long accountId)
        {
            return GetAllAccountsMappingRulesByAccountId(accountBEDefinitionId).GetRecord(accountId);
        }

        #endregion

        #region Private Methods

        private struct GetAllAccountsMappingRulesByAccountIdCacheName
        {
            public Guid AccountBEDefinitionId { get; set; }
        }
        private Dictionary<long, List<MappingRule>> GetAllAccountsMappingRulesByAccountId(Guid accountBEDefinitionId)
        {
            var cacheName = new GetAllAccountsMappingRulesByAccountIdCacheName { AccountBEDefinitionId = accountBEDefinitionId };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    HashSet<Guid> accountMappingRuleDefinitionIds = GetAccountMappingRuleDefinitionsIds(accountBEDefinitionId);
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

        private HashSet<Guid> GetAccountMappingRuleDefinitionsIds(Guid accountBEDefinitionId)
        {
            var accountMappingRuleDefinitions = GetAccountMappingRuleDefinitions(accountBEDefinitionId);
            if (accountMappingRuleDefinitions == null)
                return new HashSet<Guid>();
            return new HashSet<Guid>(accountMappingRuleDefinitions.Select(itm => itm.GenericRuleDefinitionId));
        }

        private static IEnumerable<GenericRuleDefinition> GetAccountMappingRuleDefinitions(Guid accountBEDefinitionId)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            return ruleDefinitionManager.GetGenericRulesDefinitons().FindAllRecords(ruleDefinition => AccountMappingRuleDefinitionFilter.IsAccountIdentificationRuleDefinition(accountBEDefinitionId, ruleDefinition));
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
            List<string> criteriaFieldsValues = new List<string>();

            GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition genericRuleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(mappingRule.DefinitionId);

            var genericRuleDefinitionCriteria = genericRuleDefinition.CriteriaDefinition.CastWithValidate<GenericRuleDefinitionCriteria>("genericRuleDefinition.CriteriaDefinition", genericRuleDefinition.GenericRuleDefinitionId);

            for (int i = 0; i < genericRuleDetail.FieldValueDescriptions.Count(); i++)
            {
                string fieldValueDescription = genericRuleDetail.FieldValueDescriptions[i];
                if (!string.IsNullOrEmpty(fieldValueDescription))
                    criteriaFieldsValues.Add(string.Concat(genericRuleDefinitionCriteria.Fields[i].FieldName, ":", fieldValueDescription));
            }

            criteria = string.Join<string>(" ,", criteriaFieldsValues);

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
