using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class Sale
    {
        static Sale()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(Sale), "RateValue", "TotalNet", "CurrencyId", "EffectiveDurationInSeconds", "ExtraChargeValue", "RateType");
        }
        public decimal RateValue { get; set; }

        public decimal TotalNet { get; set; }

        public int CurrencyId { get; set; }
        public Decimal EffectiveDurationInSeconds { get; set; }

        public Decimal ExtraChargeValue { get; set; }
        public int RateType { get; set; }
    }
}
