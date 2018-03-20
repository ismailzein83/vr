using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class SaleCodeMatch
    {
        public string SaleCode { get; set; }

        public long SaleZoneId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public string SaleCodeSourceId { get; set; }
    }

    public class SaleCodeMatchWithMaster
    {
        public int? CustomerSellingNumberPlanId { get; set; }

        public SaleCodeMatch SaleCodeMatch { get; set; }

        public SaleCodeMatch MasterPlanCodeMatch { get; set; }
    }

    public class CustomerSaleCodeMatchWithMaster
    {
        public int? CustomerId { get; set; }

        public SaleCodeMatchWithMaster SaleCodeMatchWithMaster { get; set; }
    }

    public struct SaleZoneKey
    {
        public int SellingNumberPlanId { get; set; }

        public long SaleZoneId { get; set; }
    }
}
