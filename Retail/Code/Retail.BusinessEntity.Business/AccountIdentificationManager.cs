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
            var accountMappingRuleDefinitions = GetAccountMappingRuleDefinitions();
            if (accountMappingRuleDefinitions == null)
                return null;
            HashSet<int> accountMappingRuleDefinitionIds = new HashSet<int>(accountMappingRuleDefinitions.Select(itm => itm.GenericRuleDefinitionId));
            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            return mappingRuleManager.GetAllRules().FindAllRecords(itm => accountMappingRuleDefinitionIds.Contains(itm.DefinitionId));
        }

        IEnumerable<GenericRuleDefinition> GetAccountMappingRuleDefinitions()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            var subscriberAccountBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(Account.BUSINESSENTITY_DEFINITION_NAME);
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            var allMappingRuleDefinitions = ruleDefinitionManager.GetGenericRuleDefinitionsByType(MappingRule.RULE_DEFINITION_TYPE_NAME);
            return allMappingRuleDefinitions.FindAllRecords(itm =>
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
        }
    }
}
