using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class ZoneRate
    {
        public long RateId { get; set; }

        public int ZoneId { get; set; }

        public int PriceListId { get; set; }

        public string CarrierAccountId { get; set; }

        public decimal Rate { get; set; }

        public short ServicesFlag { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
