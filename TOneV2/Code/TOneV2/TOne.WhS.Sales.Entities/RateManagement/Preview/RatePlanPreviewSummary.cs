using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

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

        public IEnumerable<ZoneService> NewDefaultServices { get; set; }

        public DateTime? ClosedDefaultServiceEffectiveOn { get; set; }

        public int NumberOfNewSaleZoneServices { get; set; }

        public int NumberOfClosedSaleZoneServices { get; set; }

		public int NumberOfChangedCountries { get; set; }

        public int NumberOfNewCountries { get; set; }

        public int NumberOfNewOtherRates { get; set; }

        public int NumberOfIncreasedOtherRates { get; set; }

        public int NumberOfDecreasedOtherRates { get; set; }
    }
}
