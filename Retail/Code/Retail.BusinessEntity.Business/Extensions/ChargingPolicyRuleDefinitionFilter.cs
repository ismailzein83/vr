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
            GenericRuleDefinitionCriteria genericRuleDefinitionCriteria = context.RuleDefinition.CriteriaDefinition as GenericRuleDefinitionCriteria;

            if (genericRuleDefinitionCriteria == null || genericRuleDefinitionCriteria.Fields == null)
                return false;

            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            var serviceTypeBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(ServiceType.BUSINESSENTITY_DEFINITION_NAME);
            var chargingPolicyBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(ChargingPolicy.BUSINESSENTITY_DEFINITION_NAME);

            List<Guid> neededCriteriaBEIds = new List<Guid> { serviceTypeBEDefinitionId, chargingPolicyBEDefinitionId };
            foreach (var criteriaField in genericRuleDefinitionCriteria.Fields)
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
        }
    }
}
