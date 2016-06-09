using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyRateValue : ChargingPolicyPart
    {
        public int ConfigId { get; set; }

        public abstract void Execute(IChargingPolicyRateValueContext context);

        public override ChargingPolicyPartType PartType
        {
            get { return ChargingPolicyPartType.RateValue; }
        }
    }

    public interface IChargingPolicyRateValueContext
    {
        PricingEntity PricingEntity { get; }

        long PricingEntityId { get; }

        int ServiceTypeId { get; }

        Vanrise.GenericData.Entities.GenericRuleTarget RuleTarget { get; }

        decimal NormalRate { set; }

        Dictionary<int, decimal> RatesByRateType { set; }
    }
}
