using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CustomerSummary
    {
        public string Carrier { get; set; }
        public decimal? SaleDuration { get; set; }
        public double? SaleNet { get; set; }
        public decimal? CostDuration { get; set; }
        public double? CostNet { get; set; }
    }
}
