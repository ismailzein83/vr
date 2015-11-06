using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class Cost
    {
        static Cost()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(Cost), "RateValue", "TotalNet", "CurrencyId");
        }
        public decimal RateValue { get; set; }

        public decimal TotalNet { get; set; }

        public int CurrencyId { get; set; }
    }
}
