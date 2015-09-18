using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierRate
    {
        public long SupplierRateId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public decimal NormalRate { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
    }
}
