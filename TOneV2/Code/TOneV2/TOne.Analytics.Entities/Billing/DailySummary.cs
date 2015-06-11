using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailySummary
    {
        public string Day { get; set; }
        public int Calls { get; set; }
        public decimal? DurationNet { get; set; }
        public decimal? SaleDuration { get; set; }
        public double? SaleNet { get; set; }
        public double? CostNet { get; set; }
    }
}