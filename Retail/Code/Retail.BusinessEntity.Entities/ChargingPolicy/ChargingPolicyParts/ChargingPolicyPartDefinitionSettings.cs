using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyPartDefinitionSettings
    {
        public int ConfigId { get; set; }
    }

    public abstract class BaseChargingPolicyPartRuleDefinition : ChargingPolicyPartDefinitionSettings
    {
        public GenericRuleDefinitionCriteria RuleCriteriaDefinition { get; set; }

    }
}
