using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IChargingPolicyPartExecutionContext
    {
        PricingEntity PricingEntity { get; }

        long PricingEntityId { get; }

        int ServiceTypeId { get; }

        ChargingPolicyPartSettings ChargingPolicyPartSettings { get; }

        Vanrise.GenericData.Entities.GenericRuleTarget RuleTarget { get; }
    }
}
