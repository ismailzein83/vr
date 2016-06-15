﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountMappingRuleDefinitionFilter : IGenericRuleDefinitionFilter
    {
        public bool IsMatched(IGenericRuleDefinitionFilterContext context)
        {
            ValidateInput(context);
            return IsAccountIdentificationRuleDefinition(context.RuleDefinition);
        }

        private void ValidateInput(IGenericRuleDefinitionFilterContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (context.RuleDefinition == null)
                throw new ArgumentNullException("context.RuleDefinition");
            if (context.RuleDefinition.SettingsDefinition == null)
                throw new ArgumentNullException(String.Format("context.RuleDefinition.SettingsDefinition. RuleDefinitionId '{0}'", context.RuleDefinition.GenericRuleDefinitionId));
        }

        internal static bool IsAccountIdentificationRuleDefinition(GenericRuleDefinition ruleDefinition)
        {
            var mappingRuleDefinitionSettings = ruleDefinition.SettingsDefinition as MappingRuleDefinitionSettings;
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
