using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SupplierCostDetailsFormatted
    {
        public string Carrier { get; set; }
        public string Customer { get; set; }
        public decimal? Duration { get; set; }
        public string DurationFormatted { get; set; }
        public double? Amount { get; set; }
        public string AmountFormatted { get; set; }
        public string CustomerGroupName { get; set; }

    }
}
