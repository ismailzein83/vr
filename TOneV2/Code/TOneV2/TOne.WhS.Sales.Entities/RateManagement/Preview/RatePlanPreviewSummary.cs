using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class RatePlanPreviewSummary
    {
        public int NumberOfNewRates { get; set; }

        public int NumberOfIncreasedRates { get; set; }

        public int NumberOfDecreasedRates { get; set; }

        public int NumberOfClosedRates { get; set; }

        public int NumberOfNewSaleZoneRoutingProducts { get; set; }

        public int NumberOfClosedSaleZoneRoutingProducts { get; set; }

        public string NameOfNewDefaultRoutingProduct { get; set; }

        public string NameOfClosedDefaultRoutingProduct { get; set; }
    }
}
