using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities.GenericRules;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountMappingRuleDefinitionFilter : IGenericRuleDefinitionFilter
    {
        public bool IsMatched(IGenericRuleDefinitionFilterContext context)
        {
            var mappingRuleDefinitionSettings = context.RuleDefinition.SettingsDefinition as MappingRuleDefinitionSettings;
            if (mappingRuleDefinitionSettings != null)
            {
                var businessEntityFieldType = mappingRuleDefinitionSettings.FieldType as Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType;
                if (businessEntityFieldType != null)
                {
                    BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
                    var subscriberAccountBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(Account.BUSINESSENTITY_DEFINITION_NAME);
                    return businessEntityFieldType.BusinessEntityDefinitionId == subscriberAccountBEDefinitionId;
                }
            }
            return false;
        }
    }
}
