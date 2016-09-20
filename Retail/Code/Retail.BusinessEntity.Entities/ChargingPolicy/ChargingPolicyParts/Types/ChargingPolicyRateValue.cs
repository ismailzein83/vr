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
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IChargingPolicyRateValueContext context);
        
        public override string PartTypeExtensionName
        {
            get { return "Retail_BE_ChargingPolicyPart_RateValue"; }
        }
    }

    public interface IChargingPolicyRateValueContext : IChargingPolicyPartExecutionContext
    {   
        decimal NormalRate { set; }

        Dictionary<int, decimal> RatesByRateType { set; }
    }
}
