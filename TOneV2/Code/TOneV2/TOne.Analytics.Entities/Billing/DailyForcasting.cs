using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailyForcasting
    {
        public string Day { get; set; }
        public double? SaleNet { get; set; }
        public double? CostNet { get; set; }
    }
}
