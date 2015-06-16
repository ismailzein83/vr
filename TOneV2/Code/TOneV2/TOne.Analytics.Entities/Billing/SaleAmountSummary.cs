using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SaleAmountSummary
    {
        public double SaleAmount { get; set; }
        public string FormattedSaleAmount { get; set; }
        public string Customer { get; set; }
    }
}
