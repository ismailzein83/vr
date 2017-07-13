using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Cost.Entities
{
    public class CDRCost
    {
        public long CDRCostId { get; set; }

        public string SourceId { get; set; }

        public DateTime? AttemptDateTime { get; set; }

        public string CGPN { get; set; }

        public string CDPN { get; set; }

        public decimal? DurationInSeconds { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Amount { get; set; }

        public string SupplierName { get; set; }

        public int? CurrencyId { get; set; }
    }
}
