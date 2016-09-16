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
        public Guid ConfigId { get; set; }
        public string PartTitle { get; set; }
    }

    public abstract class BaseChargingPolicyPartRuleDefinition : ChargingPolicyPartDefinitionSettings
    {
        public GenericRuleDefinitionCriteria RuleCriteriaDefinition { get; set; }

    }
}
