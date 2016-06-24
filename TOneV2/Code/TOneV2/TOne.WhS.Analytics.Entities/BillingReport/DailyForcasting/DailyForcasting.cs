using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class DailyForcasting
    {
        public string Day { get; set; }
        public double? SaleNet { get; set; }
        public double? CostNet { get; set; }
    }
}
