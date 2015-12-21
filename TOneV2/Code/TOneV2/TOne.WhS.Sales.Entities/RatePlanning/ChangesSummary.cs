using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ChangesSummary
    {
        public int TotalNewRates { get; set; }
        
        public int TotalRateIncreases { get; set; }
        
        public int TotalRateDecreases { get; set; }
        
        public int TotalRateChanges { get; set; }

        public int TotalNewZoneRoutingProducts { get; set; }

        public int TotalZoneRoutingProductChanges { get; set; }

        public string NewDefaultRoutingProductName { get; set; }

        public string ChangedToDefaultRoutingProductName { get; set; } // Do something with this
    }
}
