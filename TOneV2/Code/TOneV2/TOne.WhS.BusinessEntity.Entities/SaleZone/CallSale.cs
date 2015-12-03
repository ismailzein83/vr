using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CallSale
    {
        public decimal RateValue { get; set; }

        public decimal TotalNet { get; set; }

        public int CurrencyId { get; set; }
        public Decimal ExtraChargeValue { get; set; }
        public Decimal EffectiveDurationInSeconds { get; set; }
        public int RateType { get; set; }
    }
}
