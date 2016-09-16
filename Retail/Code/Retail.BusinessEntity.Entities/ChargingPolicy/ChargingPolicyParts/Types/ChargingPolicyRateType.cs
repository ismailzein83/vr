using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyRateType : ChargingPolicyPart
    {
        public Guid ConfigId { get; set; }

        public abstract void Execute(IChargingPolicyRateTypeContext context);

        public override string PartTypeExtensionName
        {
            get { return "Retail_BE_ChargingPolicyPart_RateType"; }
        }
    }

    public interface IChargingPolicyRateTypeContext : IChargingPolicyPartExecutionContext
    {
        Decimal NormalRate { get; }

        Dictionary<int, Decimal> RatesByRateType { get; }

        DateTime? TargetTime { get; }

        Decimal EffectiveRate { set; }

        int? RateTypeId { set; }
    }
}
