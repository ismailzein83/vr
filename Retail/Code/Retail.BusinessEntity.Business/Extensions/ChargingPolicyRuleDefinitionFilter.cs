using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ChargingPolicyRuleDefinitionFilter : IGenericRuleDefinitionFilter
    {
        public bool IsMatched(IGenericRuleDefinitionFilterContext context)
        {
            ValidateInput(context);
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            var serviceTypeBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(ServiceType.BUSINESSENTITY_DEFINITION_NAME);
            var chargingPolicyBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(ChargingPolicy.BUSINESSENTITY_DEFINITION_NAME);
            var packageBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(Package.BUSINESSENTITY_DEFINITION_NAME);

            List<Guid> neededCriteriaBEIds = new List<Guid> { serviceTypeBEDefinitionId, chargingPolicyBEDefinitionId, packageBEDefinitionId };
            foreach (var criteriaField in context.RuleDefinition.CriteriaDefinition.Fields)
            {
                var businessEntityFieldType = criteriaField.FieldType as Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType;
                if (businessEntityFieldType != null)
                {
                    if(neededCriteriaBEIds.Contains(businessEntityFieldType.BusinessEntityDefinitionId))
                    {
                        neededCriteriaBEIds.Remove(businessEntityFieldType.BusinessEntityDefinitionId);
                        if (neededCriteriaBEIds.Count == 0)
                            return true;
                    }
                }
            }
            return false;
        }

        private void ValidateInput(IGenericRuleDefinitionFilterContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (context.RuleDefinition == null)
                throw new ArgumentNullException("context.RuleDefinition");
            if (context.RuleDefinition.CriteriaDefinition == null)
                throw new ArgumentNullException(String.Format("context.RuleDefinition.CriteriaDefinition. RuleDefinitionId '{0}'", context.RuleDefinition.GenericRuleDefinitionId));
            if (context.RuleDefinition.CriteriaDefinition.Fields == null)
                throw new ArgumentNullException(String.Format("context.RuleDefinition.CriteriaDefinition.Fields. RuleDefinitionId '{0}'", context.RuleDefinition.GenericRuleDefinitionId));
        }
    }
}
