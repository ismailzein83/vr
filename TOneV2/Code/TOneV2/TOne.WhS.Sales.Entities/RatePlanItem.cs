using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class RatePlanItem
    {
        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public long? SaleRateId { get; set; }

        public decimal Rate { get; set; }
    }
}
