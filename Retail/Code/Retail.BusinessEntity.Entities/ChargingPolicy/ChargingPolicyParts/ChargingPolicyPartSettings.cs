using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyPartSettings
    {

    }

    public abstract class BaseChargingPolicyPartRuleSettings : ChargingPolicyPartSettings
    {
        public GenericRuleDefinitionCriteria RuleCriteriaDefinition { get; set; }

    }
}
