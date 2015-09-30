using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.SupplierPriceList.Entities
{
   public class Rate
    {
        public long SupplierRateId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public decimal NormalRate { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
        public Status Status { get; set; }

        public Zone Parent { get; set; }
    }
}
