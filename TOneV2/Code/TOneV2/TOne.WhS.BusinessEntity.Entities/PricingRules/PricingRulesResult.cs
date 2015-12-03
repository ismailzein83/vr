using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingRulesResult
    {
        public Decimal Rate { get; set; }

        public Decimal TotalAmount { get; set; }

        public Decimal EffectiveDurationInSeconds { get; set; }

        public Decimal ExtraChargeValue { get; set; }

        public int RateType { get; set; }
    }
}
