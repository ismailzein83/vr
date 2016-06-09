using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyRateType : ChargingPolicyPart
    {
        public int ConfigId { get; set; }

        public abstract void Execute(IChargingPolicyRateTypeContext context);

        public override ChargingPolicyPartType PartType
        {
            get { return ChargingPolicyPartType.RateType; }
        }
    }

    public interface IChargingPolicyRateTypeContext
    {
        PricingEntity PricingEntity { get; }

        long PricingEntityId { get; }

        int ServiceTypeId { get; }

        Vanrise.GenericData.Entities.GenericRuleTarget RuleTarget { get; }

        Decimal NormalRate { get; }

        Dictionary<int, Decimal> RatesByRateType { get; }

        DateTime? TargetTime { get; }

        Decimal EffectiveRate { set; }

        int? RateTypeId { set; }
    }
}
